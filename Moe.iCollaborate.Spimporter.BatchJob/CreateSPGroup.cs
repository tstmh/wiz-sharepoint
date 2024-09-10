using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPImporter
{
    class CreateSPGroup
    {
        public static void CreateSharePointGroup(string groupname)
        {
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SPSite oSite = new SPSite(ConfigurationManager.AppSettings["ReferenceSiteURL"]))
                    {
                        using (SPWeb oWeb = oSite.OpenWeb())
                        {
                            //SPList oMasterList = oWeb.Lists[ConfigurationManager.AppSettings["MasterListTitle"]];

                            SPGroup group = null;

                            // Check if the group exists
                            try
                            {
                                group = oWeb.SiteGroups[groupname];
                            }
                            catch { }

                            // If it doesn't, add it
                            if (group == null)
                            {
                                oWeb.SiteGroups.Add(groupname, oWeb.Author, oWeb.Author, groupname);
                                group = oWeb.SiteGroups[groupname];

                                // Add the group's permissions
                                SPRoleDefinition roleDefinition = oWeb.RoleDefinitions.GetByType(SPRoleType.Reader);
                                SPRoleAssignment roleAssignment = new SPRoleAssignment(group);
                                roleAssignment.RoleDefinitionBindings.Add(roleDefinition);
                                oWeb.RoleAssignments.Add(roleAssignment);
                                oWeb.Update();
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

        public static void testing()
        {
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SPSite oSite = new SPSite(ConfigurationManager.AppSettings["ReferenceSiteURL"]))
                    {
                        using (SPWeb oWeb = oSite.OpenWeb())
                        {
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
                CopySPGroup.WriteToErrorLog("Unexception error at GetAllSPGroups()|" + e.Message);
            }


        }
    }
}
