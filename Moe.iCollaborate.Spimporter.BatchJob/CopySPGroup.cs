using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using System.Configuration;
using System.IO;

//26 Aug, completed replication logic. To refine the code with try-catch and loggings

namespace SPImporter
{
    class CopySPGroup
    {
        public static void CheckLastModifiedinMasterList()
        {
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (SPSite oSite = new SPSite(ConfigurationManager.AppSettings["ReferenceSiteURL"]))
                    {
                        using (SPWeb oWeb = oSite.OpenWeb())
                        {
                            //DateTime LastRunDateTime = DateTime.Today.AddDays(-1);

                            //Get all SPListItem in Master List, where SPListItem's Last Modified Date is later than last run
                            SPList oMasterList = oWeb.Lists[ConfigurationManager.AppSettings["MasterListTitle"]];
                            SPListItemCollection oItemCollection = oMasterList.GetItems(new SPQuery()
                            {
                                //Query = @"<Where><Geq><FieldRef Name='Modified'/><Value Type='DateTime'><Today OffsetDays='-1'/></Value></Geq></Where>"
                                Query = "<Where><Eq><FieldRef Name='Modified'/><Value Type='DateTime'><Today/></Value></Eq></Where>"
                            });

                            foreach (SPListItem oItem in oItemCollection)
                            {
                                try
                                {
                                    //Process for each item, based on the input
                                    //1. Read Title Value, go to Site Collection
                                    //2. Read Group Value, access Target Site Collection's Group and identify the group
                                    //Create SPGroup and Add members if the group does not exist
                                    //Delete all members and re-add based on Reference Site Collection

                                    SPGroup oGroup = oWeb.SiteGroups[oItem["Group"].ToString()];
                                    CopySPGroup.WriteToLog("Getting Item from Master List - " + oItem.Title + " - " + oItem["Group"].ToString() + " - " + oItem["Permission"].ToString());

                                    AccessTargetSPSite(oItem.Title, oGroup, oItem["Permission"].ToString());
                                }
                                catch (Exception ex)
                                {
                                    CopySPGroup.WriteToErrorLog("Unable to read MasterList Item due to " + ex.Message + "|" + oItem.Title + "|" + oItem.ID);
                                }
                            }
                        }
                    }
                });
            }
            catch (Exception e)
            {
                CopySPGroup.WriteToErrorLog("Unexception error at GetAllSPGroups()|" + e.Message );
            }
        }

        public static void CheckLastModifiedinSPGroups(HashSet<string> oListofModifiedGroups)
        {
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (SPSite oSite = new SPSite(ConfigurationManager.AppSettings["ReferenceSiteURL"]))
                    {
                        using (SPWeb oWeb = oSite.OpenWeb())
                        {
                            SPList oMasterList = oWeb.Lists[ConfigurationManager.AppSettings["MasterListTitle"]];

                            foreach (string GroupID in oListofModifiedGroups)
                            {
                                try
                                {
                                    SPGroup oGroup = oWeb.SiteGroups.GetByID(Convert.ToInt16(GroupID));

                                    SPListItemCollection oItemCollection = oMasterList.GetItems(new SPQuery()
                                    {
                                        Query = "<Where><Eq><FieldRef Name='Group'/><Value Type='Text'>" + oGroup.Name + "</Value></Eq></Where>"
                                    });

                                    foreach (SPListItem oItem in oItemCollection)
                                    {
                                        try
                                        {
                                            AccessTargetSPSite(oItem.Title, oGroup, oItem["Permission"].ToString());
                                        }
                                        catch (Exception FailedToPopulateGroupToOtherSiteCollections)
                                        {
                                            WriteToErrorLog("Failed to populate group, " + FailedToPopulateGroupToOtherSiteCollections.Message + "|" + oItem.ID.ToString() + " - " + oItem.Title + " - " + oItem["Group"].ToString() + " - " + oItem["Permission"].ToString());
                                        }
                                    }
                                }
                                catch (Exception UnableToRetrieveGroup)
                                {
                                    WriteToErrorLog("Failed to retrieve group, " + UnableToRetrieveGroup.Message + "|" + "Group ID: " + GroupID);
                                }
                            }
                        }
                    }
                });
            }
            catch (Exception e)
            {
                CopySPGroup.WriteToErrorLog("Unexception error at GetAllSPGroups()|" + e.Message);
            }
        }

        public static void UploadLogsIntoSPImporterSiteCollection()
        {
            string ProcessLogs = ConfigurationManager.AppSettings["LogsLocation"] + "\\" + DateTime.Now.ToString("yyyyMMdd") + "_ProcessLogs.txt";
            string ErrorLogs = ConfigurationManager.AppSettings["LogsLocation"] + "\\" + DateTime.Now.ToString("yyyyMMdd") + "_ErrorLogs.txt";

            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite oSite = new SPSite(ConfigurationManager.AppSettings["ReferenceSiteURL"]))
                {
                    using (SPWeb oWeb = oSite.OpenWeb())
                    {
                        SPFolder myLibrary = oWeb.Folders["Logs"];

                        try
                        {
                            System.Threading.Thread.Sleep(1000);
                            // Prepare to upload
                            Boolean replaceExistingFiles = true;
                            String fileName = System.IO.Path.GetFileName(ProcessLogs);
                            FileStream fileStream = File.OpenRead(ProcessLogs);

                            // Upload document
                            SPFile spfile = myLibrary.Files.Add(fileName, fileStream, replaceExistingFiles);

                            // Commit 
                            myLibrary.Update();
                            WriteToLog("Uploaded process logs into Logs Library");
                        }
                        catch(Exception FailtoUploadProcessLogs)
                        {
                            WriteToErrorLog("Fail to upload process logs, " + ProcessLogs + "|" + FailtoUploadProcessLogs.Message);
                        }

                        try
                        {
                            System.Threading.Thread.Sleep(1000);
                            // Prepare to upload
                            Boolean replaceExistingFiles = true;
                            String fileName = System.IO.Path.GetFileName(ErrorLogs);
                            FileStream fileStream = File.OpenRead(ErrorLogs);

                            // Upload document
                            SPFile spfile = myLibrary.Files.Add(fileName, fileStream, replaceExistingFiles);

                            // Commit 
                            myLibrary.Update();
                            WriteToLog("Uploaded error logs into Logs Library");
                        }
                        catch (Exception FailtoUploadErrorLogs)
                        {
                            WriteToErrorLog("Fail to upload error logs, " + ErrorLogs + "|" + FailtoUploadErrorLogs.Message);
                        }

                    }
                }
            });
        }

        public static void AccessTargetSPSite(string TargetSiteCollectionURL, SPGroup AddingGroup, string PermissionLevel)
        {
            //Self Note - To check if Site Collection exist. If No, error log it
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (SPSite tSite = new SPSite(TargetSiteCollectionURL))
                    {
                        using (SPWeb tWeb = tSite.OpenWeb())
                        {
                            CopySPGroup.WriteToLog("Accssing " + tWeb.Url);

                            if (GroupExistsInSiteCollection(tWeb, AddingGroup.Name)) //Check if Group Exist
                            {
                                //if True, delete all members and re-add all members
                                CopySPGroup.WriteToLog("Group exist in - " + tWeb.Url);

                                SPGroup tGroup = tWeb.SiteGroups[AddingGroup.Name];
                                foreach (SPUser tempUser in tGroup.Users)
                                {
                                    try
                                    {
                                        tGroup.RemoveUser(tempUser);
                                        CopySPGroup.WriteToLog("Refreshing group memberes, removed " + tempUser.LoginName);
                                    }
                                    catch (Exception FailedToRemoveUser)
                                    {
                                        CopySPGroup.WriteToErrorLog("Failed to remove " + tempUser.LoginName + " at AccessTargetSPSite() " + "|" + FailedToRemoveUser.Message);
                                    }
                                }

                                foreach (SPUser oUser in AddingGroup.Users)
                                {
                                    try
                                    {
                                        tGroup.AddUser(tWeb.EnsureUser(oUser.LoginName));
                                        CopySPGroup.WriteToLog("Refreshing group memberes, Added " + oUser.LoginName);
                                    }
                                    catch (Exception FailedToAddUser)
                                    {
                                        CopySPGroup.WriteToErrorLog("Failed to add " + oUser.LoginName + " at AccessTargetSPSite() " + "|" + FailedToAddUser.Message);
                                    }
                                }
                            }
                            else
                            {
                                //Else, group does not exist. Create new SPGroup and add all members into the new created group
                                CopySPGroup.WriteToLog("Group does not exist in - " + tWeb.Url);
                                CopySPGroup.WriteToLog("Creating new SPGroup - " + AddingGroup.Name + ", with Permission level of '" + PermissionLevel + "'");

                                tWeb.SiteGroups.Add(AddingGroup.Name, tWeb.Site.Owner, tWeb.Site.Owner, "Automate Managed Group from SPImporter");
                                SPGroup tGroup = tWeb.SiteGroups[AddingGroup.Name];

                                SPRoleDefinition role = tWeb.RoleDefinitions[PermissionLevel];
                                SPRoleAssignment roleAssignment = new SPRoleAssignment(tGroup);
                                roleAssignment.RoleDefinitionBindings.Add(role);
                                tWeb.RoleAssignments.Add(roleAssignment);
                                tWeb.Update();
                                CopySPGroup.WriteToLog("Created new SPGroup - " + AddingGroup.Name + ", with Permission level of '" + PermissionLevel + "'");

                                foreach (SPUser oUser in AddingGroup.Users)
                                {
                                    try
                                    {
                                        tGroup.AddUser(tWeb.EnsureUser(oUser.LoginName));
                                        CopySPGroup.WriteToLog("Added user " + oUser.LoginName + " into " + AddingGroup.Name);
                                    }
                                    catch (Exception ex)
                                    {
                                        CopySPGroup.WriteToErrorLog("Failed to add user " + oUser.LoginName + " into " + AddingGroup.Name + " in " + tWeb.Url + "|" + ex.Message);
                                    }
                                }
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                CopySPGroup.WriteToErrorLog("Unexception error at AccessTargetSPSite(), " + ex.Message);
            }
        }

        public static bool GroupExistsInSiteCollection(SPWeb web, string name)
        {
            try
            {
                return web.SiteGroups.OfType<SPGroup>().Count(g => g.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)) > 0;
            }
            catch
            {
                return false;
            }
        }

        public static void WriteToLog(string text)
        {
            Console.WriteLine(text);
            using (StreamWriter oWriter = new StreamWriter(ConfigurationManager.AppSettings["LogsLocation"] + "\\" + DateTime.Now.ToString("yyyyMMdd") + "_ProcessLogs.txt", true))
            {
                oWriter.WriteLine(DateTime.Now.ToString() + "|" + text);
            }
        }

        public static void WriteToErrorLog(string text)
        {
            Console.WriteLine(text);
            using (StreamWriter oWriter = new StreamWriter(ConfigurationManager.AppSettings["LogsLocation"] + "\\" + DateTime.Now.ToString("yyyyMMdd") + "_ErrorLogs.txt", true))
            {
                oWriter.WriteLine(DateTime.Now.ToString() + "|" + text);
            }
        }
    }
}
