using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using System.Configuration;

namespace SPImporter
{
    class Program
    {
        static void Main(string[] args)
        {
            //CreateSPGroup.CreateSharePointGroup("Admin(Secretary)");
            CreateSPGroup.testing();
        }
        static void Main2(string[] args)
        {
            //Phase 1, translate HRPS data to text
            ConvertXMLtoText.Convert_HRMSXML_To_OICA();

            //Phase 2, Assign users in translated text to SPGroup, in reference site collection
            //Get Converted Text File
            string ArchiveDestination = ConfigurationManager.AppSettings["Archive_Destination"] + DateTime.Now.ToString("yyyyMMdd") + "\\";
            HashSet<string> oListofModifiedGroups = new HashSet<string>();

            try
            {
                using (System.IO.StreamReader oReader = new System.IO.StreamReader(ArchiveDestination + "HRPStoSPImporter_" + DateTime.Now.ToString("yyyyMMdd") + ".txt"))
                {
                    string line = "";
                    while ((line = oReader.ReadLine()) != null)
                    {
                        try
                        {
                            string[] TextArray = line.Split('|');
                            string userNRIC = "", OrganisationUnit = "", JobID = "", DivisionStatus = "", PersonalGroup = "", Hierarchy = "", SchoolCode = "", LevelofOrgUnit = "";
                            //SPImporter Format - NRIC, ORGEH, STELL, DIVIS, SCSER, PR_UNIT_HIER, SHORT, ORG_LVL                            
                            try
                            {
                                userNRIC = ConfigurationManager.AppSettings["EnvironmentDomain"] + TextArray[0];
                            }
                            catch (Exception NullStringArray)
                            {
                                userNRIC = "";
                            }

                            try
                            {
                                OrganisationUnit = TextArray[1];
                            }
                            catch (Exception NullStringArray)
                            {
                                OrganisationUnit = "";
                            }

                            try
                            {
                                JobID = TextArray[2];
                            }
                            catch (Exception NullStringArray)
                            {
                                JobID = "";
                            }

                            try
                            {
                                DivisionStatus = TextArray[3];
                            }
                            catch (Exception NullStringArray)
                            {
                                DivisionStatus = "";
                            }

                            try
                            {
                                PersonalGroup = TextArray[4];
                            }
                            catch (Exception NullStringArray)
                            {
                                PersonalGroup = "";
                            }

                            try
                            {
                                Hierarchy = TextArray[5];
                            }
                            catch (Exception NullStringArray)
                            {
                                Hierarchy = "";
                            }

                            try
                            {
                                SchoolCode = TextArray[6];
                            }
                            catch (Exception NullStringArray)
                            {
                                SchoolCode = "";
                            }

                            try
                            {
                                LevelofOrgUnit = TextArray[7];
                            }
                            catch (Exception NullStringArray)
                            {
                                LevelofOrgUnit = "";
                            }


                            //Dummy Entries
                            //string userNRIC = ConfigurationManager.AppSettings["EnvironmentDomain"] + "user4";
                            //int OrganisationUnit = 10000712;
                            //string JobID = "10021337";
                            //string DivisionStatus = "01";
                            //string PersonalGroup = "002";
                            //string Hierarchy = @"\10000658\10000659";
                            //int SchoolCode = 7400;
                            //string LevelofOrgUnit = "600";
                            //------------------------------------------------------------------

                            //Business Rule Validation Section
                            if (FirstLevelCheckStep1(OrganisationUnit) || FirstLevelCheckStep2(SchoolCode) || FirstLevelCheckStep3(JobID, Hierarchy, SchoolCode, OrganisationUnit))
                            {
                                CopySPGroup.WriteToLog("Processing " + userNRIC + " passed first level checks");

                                //If True, passed First Level Verification, proceed to Second Level Verification
                                SPSecurity.RunWithElevatedPrivileges(delegate ()
                                {
                                    using (SPSite oSite = new SPSite(ConfigurationManager.AppSettings["ReferenceSiteURL"]))
                                    {
                                        using (SPWeb oWeb = oSite.OpenWeb())
                                        {
                                            //Get all SPListItem in Rules List, where the input match the builtin rule(s)
                                            SPList oRuleList = oWeb.Lists[ConfigurationManager.AppSettings["RuleListTitle"]];

                                            //Business Rule Set 1
                                            SPListItemCollection oRuleCollection1 = oRuleList.GetItems(new SPQuery()
                                            {
                                                Query = @"<Where><And><Or><Or><Or><Or><Eq>" +
                                                                "<FieldRef Name='Organisation_x0020_Unit'/>" +
                                                                "<Value Type='Text'>" + OrganisationUnit + "</Value>" +
                                                          "</Eq><Eq>" +
                                                                "<FieldRef Name='Job_x0020_ID'/>" +
                                                                "<Value Type='Text'>" + JobID + "</Value>" +
                                                          "</Eq></Or><Eq>" +
                                                                "<FieldRef Name='Division_x0020_Status'/>" +
                                                                "<Value Type='Text'>" + DivisionStatus + "</Value>" +
                                                          "</Eq></Or><Eq>" +
                                                                "<FieldRef Name='Personal_x0020_Group'/>" +
                                                                "<Value Type='Text'>" + PersonalGroup + "</Value>" +
                                                          "</Eq></Or><Eq>" +
                                                                "<FieldRef Name='Level_x0020_of_x0020_Org_x0020_U'/>" +
                                                                "<Value Type='Text'>" + LevelofOrgUnit + "</Value>" +
                                                          "</Eq></Or><Eq>" +
                                                                "<FieldRef Name='Single_x0020_Condition'/>" +
                                                                "<Value Type='Text'>Yes</Value>" +
                                                          "</Eq></And></Where>"
                                            });

                                            foreach (SPListItem oItem in oRuleCollection1)
                                            {
                                                try
                                                {
                                                    CopySPGroup.WriteToLog("Matchd Rules Set 1, adding " + userNRIC + " into " + oItem["Group"].ToString());
                                                    oListofModifiedGroups.Add(AddUserIntoReferenceSite(oWeb, userNRIC, oItem["Group"].ToString()));
                                                }
                                                catch (Exception eRule1)
                                                {
                                                    CopySPGroup.WriteToErrorLog("Matchd Rules Set 1, failed to add " + userNRIC + " into " + oItem["Group"].ToString() + "|" + eRule1.Message + "|" + line);
                                                }
                                            }

                                            //Business Rule Set 2
                                            SPListItemCollection oRuleCollection2 = oRuleList.GetItems(new SPQuery()
                                            {
                                                Query = @"<Where><And><And><Eq>" +
                                                            "<FieldRef Name='Organisation_x0020_Unit'/>" +
                                                            "<Value Type='Text'>" + OrganisationUnit + "</Value>" +
                                                          "</Eq><Eq>" +
                                                            "<FieldRef Name='Division_x0020_Status'/>" +
                                                            "<Value Type='Text'>" + DivisionStatus + "</Value>" +
                                                          "</Eq></And><Eq>" +
                                                            "<FieldRef Name='Single_x0020_Condition'/>" +
                                                            "<Value Type='Text'>No</Value>" +
                                                          "</Eq></And></Where>"
                                            });

                                            foreach (SPListItem oItem in oRuleCollection2)
                                            {
                                                try
                                                {
                                                    CopySPGroup.WriteToLog("Matched Rules Set 2, adding " + userNRIC + " into " + oItem["Group"].ToString());
                                                    oListofModifiedGroups.Add(AddUserIntoReferenceSite(oWeb, userNRIC, oItem["Group"].ToString()));
                                                }
                                                catch (Exception eRule2)
                                                {
                                                    CopySPGroup.WriteToErrorLog("Matchd Rules Set 2, failed to add " + userNRIC + " into " + oItem["Group"].ToString() + "|" + eRule2.Message + "|" + line);
                                                }
                                            }

                                            //Business Rule Set 3
                                            SPListItemCollection oRuleCollection3 = oRuleList.GetItems(new SPQuery()
                                            {
                                                Query = @"<Where><And><And><And><Eq>" +
                                                              "<FieldRef Name='Job_x0020_ID'/>" +
                                                              "<Value Type='Text'>" + JobID + "</Value>" +
                                                          "</Eq><Leq>" +
                                                              "<FieldRef Name='School_x0020_Code_x0020_From'/>" +
                                                              "<Value Type='Text'>" + SchoolCode + "</Value>" +
                                                          "</Leq></And><Geq>" +
                                                              "<FieldRef Name='School_x0020_Code_x0020_To'/>" +
                                                              "<Value Type='Text'>" + SchoolCode + "</Value>" +
                                                          "</Geq></And><Eq>" +
                                                              "<FieldRef Name='Single_x0020_Condition'/>" +
                                                              "<Value Type='Text'>No</Value>" +
                                                          "</Eq></And></Where>"
                                            });

                                            foreach (SPListItem oItem in oRuleCollection3)
                                            {
                                                try
                                                {
                                                    CopySPGroup.WriteToLog("Matchd Rules Set 3, adding " + userNRIC + " into " + oItem["Group"].ToString());
                                                    oListofModifiedGroups.Add(AddUserIntoReferenceSite(oWeb, userNRIC, oItem["Group"].ToString()));
                                                }
                                                catch (Exception eRule3)
                                                {
                                                    CopySPGroup.WriteToErrorLog("Matchd Rules Set 3, failed to add " + userNRIC + " into " + oItem["Group"].ToString() + "|" + eRule3.Message + "|" + line);
                                                }
                                            }
                                        }
                                    }
                                });
                            }
                            else
                            {
                                Console.WriteLine("Processing user does not pass first level checks");
                            }

                        }
                        catch (Exception ex)
                        {
                            CopySPGroup.WriteToErrorLog("Error when processing entry|" + ex.Message + "|" + line);
                        }
                    }

                }
            }
            catch (Exception e)
            {
                CopySPGroup.WriteToErrorLog("Unexceptional Error 01 - " + e.Message);
            }

            //Phase 3, Read Master List, populate reference site group members to other site collections
            CopySPGroup.CheckLastModifiedinMasterList();

            //Phase 4, Read all Groups, check if there is member change
            //If yes, read Master List, get the list of affected site collection and push down the member changes
            CopySPGroup.CheckLastModifiedinSPGroups(oListofModifiedGroups);

            //Phase 5, Upload this run's generated logs into SharePoint
            //CopySPGroup.UploadLogsIntoSPImporterSiteCollection();
        }

        //Add User into Group
        public static string AddUserIntoReferenceSite(SPWeb oWeb, string LoginName, string GroupName)
        {
            try
            {
                //add user to the group in the web
                SPGroup oGroup = oWeb.Groups[GroupName];
                SPUser oUser = oWeb.EnsureUser(LoginName);
                oGroup.AddUser(oUser);
                oWeb.Update();
                CopySPGroup.WriteToLog("Added " + LoginName + " into Refereence Site's " + GroupName);

                return oGroup.ID.ToString();
            }
            catch (Exception e)
            {
                CopySPGroup.WriteToErrorLog("Unable to add " + LoginName + " into Refereence Site's " + GroupName + "|" + e.Message);

                return "";
            }
        }

        //Remove User into Group
        public static string RemoveUserIntoReferenceSite(SPWeb oWeb, string LoginName, string GroupName)
        {
            try
            {
                SPUserCollection SPUserCollection = oWeb.SiteUsers;
                SPUser oUser = oWeb.EnsureUser(LoginName);
                SPUserCollection.Remove(oUser.LoginName);

                CopySPGroup.WriteToLog("Removed " + LoginName + " from site collection");
                return "";
            }
            catch (Exception e)
            {
                CopySPGroup.WriteToErrorLog("Unable to remove " + LoginName + " from site collection|" + e.Message);

                return "";
            }
        }

        //First level verification
        public static bool FirstLevelCheckStep1(string input_OrganisationUnit)
        {
            int OrganisationUnit = 0;
            try
            {
                OrganisationUnit = Convert.ToInt32(input_OrganisationUnit);
            }
            catch
            {
                OrganisationUnit = 0;
            }

            if (OrganisationUnit == 20000027 || OrganisationUnit == 10000712 || OrganisationUnit == 10000799 || OrganisationUnit == 10000735 || OrganisationUnit == 10000839 || OrganisationUnit == 10000674 || OrganisationUnit == 10000670 || OrganisationUnit == 10000661 || OrganisationUnit == 10000677 || OrganisationUnit == 10000664 || OrganisationUnit == 10000686 || OrganisationUnit == 10000787 || OrganisationUnit == 10000699 || OrganisationUnit == 10004901 || OrganisationUnit == 10000843 || OrganisationUnit == 20013127 || OrganisationUnit == 20009736 || OrganisationUnit == 20009926 || OrganisationUnit == 20006040 || OrganisationUnit == 20007854 || OrganisationUnit == 20007876 || OrganisationUnit == 20007900 || OrganisationUnit == 20016253 || OrganisationUnit == 10000680 || OrganisationUnit == 10000708 || OrganisationUnit == 10000783 || OrganisationUnit == 10000813 || OrganisationUnit == 20015975)
                return true;
            else
                return false;
        }

        public static bool FirstLevelCheckStep2(string input_SchoolCode)
        {
            int SchoolCode = 0;

            try
            {
                SchoolCode = Convert.ToInt32(input_SchoolCode);
            }
            catch
            {
                SchoolCode = 0;
            }

            //School Code ID	School Code Description
            //If 1001<= School Code <= 1999	Range of Primary Government
            //Or 5001<=School Code <= 5990	Range of Primary Aid
            //Or 5991<=School Code <= 6000	Range of Primary Reserve 
            //Or School Code = 7301	Range of Marymount Convert
            //Or 0450<=School Code <= 0499	Range of Special Program Center
            //Or 3001<=School Code <= 5000	Range of Secondary Government
            //Or 7001<=School Code <= 7401	Range of Full School Aid
            //Or  7951<=School Code <= 8000	Range of Full School
            //Or 0701<=School Code <= 0800	Range of Junior College Government
            //Or 0801<=School Code <= 0900	Range of Junior College Aid
            //Or  0901<=School Code <= 1000	Range of Centralised Institute

            if (SchoolCode >= 1001 && SchoolCode <= 1999)
                return true;
            else if (SchoolCode >= 5001 && SchoolCode <= 5990)
                return true;
            else if (SchoolCode >= 5991 && SchoolCode <= 6000)
                return true;
            else if (SchoolCode == 7301)
                return true;
            else if (SchoolCode >= 0450 && SchoolCode <= 0499)
                return true;
            else if (SchoolCode >= 3001 && SchoolCode <= 5000)
                return true;
            else if (SchoolCode >= 7001 && SchoolCode <= 7401)
                return true;
            else if (SchoolCode >= 7951 && SchoolCode <= 8000)
                return true;
            else if (SchoolCode >= 0701 && SchoolCode <= 0800)
                return true;
            else if (SchoolCode >= 0801 && SchoolCode <= 0900)
                return true;
            else if (SchoolCode >= 0901 && SchoolCode <= 1000)
                return true;
            else
                return false;
        }

        public static bool FirstLevelCheckStep3(string JobID, string Hierarchy, string input_SchoolCode, string input_OrganisationUnit)
        {
            int SchoolCode = 0;
            int OrganisationUnit = 0;

            try
            {
                SchoolCode = Convert.ToInt32(input_SchoolCode);
            }
            catch
            {
                SchoolCode = 0;
            }

            try
            {
                OrganisationUnit = Convert.ToInt32(input_OrganisationUnit);
            }
            catch
            {
                OrganisationUnit = 0;
            }

            //Condition in Code
            //If Job Id  is blank
            //Else if Hierarchy = “\10000658\10000659”
            //Else if 
            //(
            //School Code = 0385
            //And 20000000 <= Organisation Unit <= 20000999
            //And Organisation Unit <> 20000027
            //)

            if (string.IsNullOrEmpty(JobID))
                return true;
            else if (Hierarchy.Equals(@"\10000658\10000659"))
                return true;
            else if ((OrganisationUnit >= 20000000 && OrganisationUnit <= 20000999) && OrganisationUnit != 20000027 && SchoolCode == 0385)
                return true;

            return false;
        }

    }
}
