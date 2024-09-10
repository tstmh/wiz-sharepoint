using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Microsoft.SharePoint;
using MOE.Collaborate.UserCleanUp.DTO;

namespace MOE.Collaborate.UserCleanUp.DAO
{
    public class BaseDAO
    {
        List<SiteCollectionDTO> URListDTO = new List<SiteCollectionDTO>();

        public string iCollaborateWebSiteURL;
        public string IntranetWebSiteURL;
        public string SubSite;
        public string DeleteUserListName;
        public string LogsFolder;
        public string Domain;

        string Log = ConfigurationSettings.AppSettings["Log"] + DateTime.Now.ToString("yyyyMMdd") + "_Deleted_User.txt";
        string ErrorLog = ConfigurationSettings.AppSettings["ErrorLog"] + DateTime.Now.ToString("yyyyMMdd") + "_Deleted_User_Error.txt";
        string SPImporterURL = "spimporter";
        public void DeleteUserFromSiteCollection(string SiteURL, string WebApplicationName)
        {
            Logger.WriteLog("Web Application: " + SiteURL);

            GetAllSiteCollection(SiteURL);
            foreach (SiteCollectionDTO SiteCollectionItemDTO in URListDTO)
            {
                if (SiteCollectionItemDTO.URL.Trim().ToLower() != (SiteURL + @"/" + SPImporterURL).Trim().ToLower() &&
                    SiteCollectionItemDTO.URL.Trim().ToLower() != (SiteURL + @"/" + SubSite).Trim().ToLower())
                {
                    List<UserDTO> UserList = GetAllSiteCollectionUser(SiteCollectionItemDTO.URL);
                    Logger.WriteLog("Number UserList: " + UserList.Count.ToString());

                    List<UserDTO> DeletedUserList = DeleteUserFromSiteCollection(SiteCollectionItemDTO.URL, UserList);
                    Logger.WriteLog("Number DeletedUserList: " + DeletedUserList.Count.ToString());

                    GenerateCSVFile(DeletedUserList, SiteCollectionItemDTO.Name);
                    UploadFileIntoSharePoint(SiteCollectionItemDTO.Name);
                }
                else
                {
                    Logger.WriteLog("Ignore SP Importer site");
                }
            }
            string fileLocation = Log;
            string URL = iCollaborateWebSiteURL + "/" + SubSite + "/" + LogsFolder + "/" + DateTime.Now.ToString("yyyyMMdd") + "_Deleted_User.txt";
            UploadFileIntoSharePoint(URL, fileLocation, LogsFolder);

            fileLocation = ErrorLog;
            URL = iCollaborateWebSiteURL + "/" + SubSite + "/" + LogsFolder + "/" + DateTime.Now.ToString("yyyyMMdd") + "_Deleted_User_Error.txt";
            UploadFileIntoSharePoint(URL, fileLocation, LogsFolder);
        }
        private void UploadFileIntoSharePoint(string SiteCollectionName)
        {
            string fileLocation = ConfigurationSettings.AppSettings["Log"] + DateTime.Now.ToString("yyyyMMdd") + SiteCollectionName + "_DeletedUsers.csv";
            string URL = "";
            string Folder = "";
            if (SiteCollectionName == "")
            {
                Folder = iCollaborateWebSiteURL + "/" + SubSite + "/" + DeleteUserListName + "/MainSite/";
                URL = iCollaborateWebSiteURL + "/" + SubSite + "/" + DeleteUserListName + "/MainSite/" + DateTime.Now.ToString("yyyyMMdd") + "_DeletedUsers.csv";
            }
            else
            {
                Folder = iCollaborateWebSiteURL + "/" + SubSite + "/" + DeleteUserListName + "/" + SiteCollectionName + "/";
                URL = iCollaborateWebSiteURL + "/" + SubSite + "/" + DeleteUserListName + "/" + SiteCollectionName + "/" + DateTime.Now.ToString("yyyyMMdd") + "_DeletedUsers.csv";
            }
            UploadFileIntoSharePoint(URL, fileLocation, Folder);
            Logger.WriteLog("UploadFileIntoSharePoint for: " + SiteCollectionName);
        }

        public void GetAllSiteCollection(string RootSite)
        {
            URListDTO = new List<SiteCollectionDTO>();
            string WebApplication = "";
            try
            {
                Logger.WriteLog("Start GetAllSiteCollection:" + RootSite);
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SPSite site = new SPSite(RootSite))
                    {
                        foreach (SPSite webcollection in site.WebApplication.Sites)
                        {
                            WebApplication = webcollection.WebApplication.Name;

                            SiteCollectionDTO SiteCollectionItemDTO = new SiteCollectionDTO();
                            SiteCollectionItemDTO.Name = webcollection.ServerRelativeUrl.Replace("/", "");
                            SiteCollectionItemDTO.URL = webcollection.Url;
                            URListDTO.Add(SiteCollectionItemDTO);
                            Logger.WriteLog("Site Collection list: " + WebApplication + " : " + webcollection.Url);
                        }
                    }
                });
                Logger.WriteLog("End GetAllSiteCollection:" + RootSite);
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("GetAllSiteCollection: " + ex.Message + " " + ex.StackTrace);
            }
        }

        public List<UserDTO> GetAllSiteCollectionUser(string SiteURL)
        {
            List<UserDTO> UserListDTO = new List<UserDTO>();
            try
            {
                string RoleLevel = "";

                bool isUser = false;
                Logger.WriteLog("Start GetAllSiteCollectionUser for " + SiteURL);
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SPSite SPsite = new SPSite(SiteURL))
                    {
                        SPWebCollection webs = SPsite.AllWebs;
                        foreach (SPWeb web in webs)
                        {

                            foreach (SPUser spUser in web.SiteAdministrators)
                            {
                                UserDTO UserItemDTO = new UserDTO();
                                UserItemDTO.Name = spUser.Name;

                                String str = spUser.LoginName.Trim();
                                int index = str.LastIndexOf(@"\");
                                UserItemDTO.Account = str.Substring(index + 1).Trim().ToLower();

                                UserItemDTO.Email = spUser.Email;
                                UserItemDTO.SPAccount = spUser;
                                UserItemDTO.SharePointGroup = "Site Collection Administator";
                                UserItemDTO.Role = "Site Collection Administator";
                                UserItemDTO.URL = web.Url;
                                UserListDTO.Add(UserItemDTO);
                            }


                            foreach (SPRoleAssignment SProleAssignment in web.RoleAssignments)
                            {
                                RoleLevel = "";
                                isUser = false;
                                SPUser spUser = null;
                                try
                                {
                                    spUser = SProleAssignment.Member as SPUser;
                                    if (spUser == null)
                                    {
                                        isUser = false;
                                    }
                                    else
                                    {
                                        isUser = true;
                                    }
                                }
                                catch
                                {
                                    isUser = false;
                                }

                                if (isUser)
                                {
                                    foreach (SPRoleDefinition RoleDefinition in SProleAssignment.RoleDefinitionBindings)
                                    {
                                        RoleLevel += RoleDefinition.Name + ";";
                                    }

                                    UserDTO UserItemDTO = new UserDTO();
                                    UserItemDTO.ID = spUser.ID;

                                    UserItemDTO.Name = spUser.Name;
                                    //UserItemDTO.Account = spUser.LoginName.Replace("i:0#.w|", "");

                                    String str = spUser.LoginName.Trim();
                                    int index = str.LastIndexOf(@"\");
                                    UserItemDTO.Account = str.Substring(index + 1).Trim().ToLower();

                                    UserItemDTO.Email = spUser.Email;
                                    UserItemDTO.SPAccount = spUser;
                                    UserItemDTO.SharePointGroup = "";
                                    UserItemDTO.Role = RoleLevel.Replace(",",";");
                                    UserItemDTO.URL = web.Url;
                                    //UserListDTO.Add(UserItemDTO);


                                    UserDTO UserDTOtmp = UserListDTO.FirstOrDefault(x => x.SPAccount.ID == UserItemDTO.SPAccount.ID && x.URL == UserItemDTO.URL);
                                    if (UserDTOtmp == null)
                                    {
                                        //UserDTOtmp.SharePointGroup = UserDTOtmp.SharePointGroup + "; " + UserItemDTO.SharePointGroup;
                                        //UserDTOtmp.Role = UserDTOtmp.Role + UserItemDTO.Role;
                                        UserListDTO.Add(UserItemDTO);
                                    }
                                    //else
                                    //{
                                    //    UserListDTO.Add(UserItemDTO);
                                    //}
                                }
                                else
                                {
                                    // Retrieve user groups having permissions on the list
                                    SPPrincipal oPrincipal = SProleAssignment.Member;
                                    SPGroup oRoleGroup = (SPGroup)oPrincipal;

                                    if (oRoleGroup.Users.Count > 0)
                                    {
                                        string strGroupName = oRoleGroup.Name;
                                        //string strGroupNames = oPrincipal.Roles[0].Name;
                                        foreach (SPRoleDefinition RoleDefinition in SProleAssignment.RoleDefinitionBindings)
                                        {
                                            RoleLevel += RoleDefinition.Name + ";";
                                        }
                                        foreach (SPUser SPuser in oRoleGroup.Users)
                                        {
                                            UserDTO UserItemDTO = new UserDTO();
                                            UserItemDTO.Name = SPuser.Name;

                                            String str = SPuser.LoginName.Trim();
                                            int index = str.LastIndexOf(@"\");
                                            UserItemDTO.Account = str.Substring(index + 1).Trim().ToLower();

                                            //UserItemDTO.Account = SPuser.LoginName;
                                            UserItemDTO.Email = SPuser.Email;
                                            UserItemDTO.SPAccount = SPuser;
                                            UserItemDTO.SharePointGroup = strGroupName;
                                            UserItemDTO.Role = RoleLevel.Replace(",", ";");

                                            UserItemDTO.URL = web.Url;
                                            //UserListDTO.Add(UserItemDTO);

                                            UserDTO UserDTOtmp = UserListDTO.FirstOrDefault(x => x.SPAccount.ID == UserItemDTO.SPAccount.ID && x.URL == UserItemDTO.URL);
                                            if (UserDTOtmp == null)
                                            {
                                                //UserDTOtmp.SharePointGroup = UserDTOtmp.SharePointGroup + "; " + UserItemDTO.SharePointGroup;
                                                //UserDTOtmp.Role = UserDTOtmp.Role + UserItemDTO.Role;
                                                UserListDTO.Add(UserItemDTO);
                                            }
                                            //else
                                            //{
                                            //    UserListDTO.Add(UserItemDTO);
                                            //}

                                        }
                                    }
                                }
                            }
                        }
                    }
                });
                Logger.WriteLog("End GetAllSiteCollectionUser for " + SiteURL);
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("GetAllSiteCollectionUser: " + ex.Message + " " + ex.StackTrace);
            }
            return UserListDTO;
        }


        
        public List<UserDTO> DeleteUserFromSiteCollection(string SiteURL, List<UserDTO> UserListDTO)
        {
            List<UserDTO> DeleteUserList = new List<UserDTO>();
            UserDTO DeleteUserItem = new UserDTO();
            bool userexist = false;
            try
            {
                Logger.WriteLog("Start DeleteUserFromSiteCollection for " + SiteURL);

                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SPSite site = new SPSite(SiteURL))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            foreach (UserDTO userItemDTO in UserListDTO)
                            {
                                if (!userItemDTO.SPAccount.LoginName.ToLower().Contains("nt authority"))
                                {
                                    //if (!userItemDTO.SPAccount.LoginName.ToLower().Contains("spfarm"))
                                    // {
                                    if (!userItemDTO.SPAccount.IsDomainGroup)
                                    {
                                        try
                                        {
                                            //if (!userItemDTO.SPAccount.IsSiteAdmin && userItemDTO.SPAccount.LoginName.ToLower() != @"sharepoint\system")
                                            if (userItemDTO.SPAccount.LoginName.ToLower() != @"sharepoint\system")
                                            {
                                                DeleteUserItem = new UserDTO();


                                                try
                                                {
                                                    //add user to the group in the web
                                                    SPUser oUser = web.EnsureUser(Domain + userItemDTO.Account);
                                                    //SPUser oUser = oWeb.EnsureUser(Domain + LoginName);
                                                    Logger.WriteLog("User Exist: " + Domain + userItemDTO.Account);
                                                    userexist = true; 
                                                }
                                                catch (Exception e)
                                                {
                                                    userexist = false;
                                                    Logger.WriteErrorLog("Unable to find: " + userItemDTO.Account + " in " + web.Url + " : " + e.Message);
                                                }

                                                //if (CheckUserExist(web, Domain + userItemDTO.Account) == false)
                                                if (userexist == false)
                                                {
                                                    Logger.WriteLog("Account do not exist: " + userItemDTO.Account);

                                                    DeleteUserItem.Name = userItemDTO.Name;
                                                    DeleteUserItem.Account = userItemDTO.Account;
                                                    DeleteUserItem.URL = userItemDTO.URL;
                                                    DeleteUserItem.SharePointGroup = userItemDTO.SharePointGroup;
                                                    DeleteUserItem.Role = userItemDTO.Role;
                                                    DeleteUserItem.AccountStatus = "Disabled/Account do not exist";

                                                    DeleteUserList.Add(DeleteUserItem);
                                                }
                                                else
                                                {
                                                    Logger.WriteLog("Account exist: " + userItemDTO.Account);

                                                    DeleteUserItem.Name = userItemDTO.Name;
                                                    DeleteUserItem.Account = userItemDTO.Account;
                                                    DeleteUserItem.URL = userItemDTO.URL;
                                                    DeleteUserItem.SharePointGroup = userItemDTO.SharePointGroup;
                                                    DeleteUserItem.Role = userItemDTO.Role;
                                                    DeleteUserItem.AccountStatus = "Active";

                                                    DeleteUserList.Add(DeleteUserItem);
                                                }
                                            }
                                            
                                        }
                                        catch (Exception ex)
                                        {
                                            Logger.WriteErrorLog("Error removing: " + userItemDTO.SPAccount.LoginName + ". Error: " + ex.Message + " " + ex.StackTrace);
                                        }
                                    }
                                    //}
                                }
                                else
                                {

                                }
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("DeleteUserFromSiteCollection: " + ex.Message + " " + ex.StackTrace);
            }
            Logger.WriteLog("End DeleteUserFromSiteCollection for " + SiteURL);
            return DeleteUserList;
        }

        public void LoadConfiguration()
        {
            try
            {
                Logger.WriteLog("Start LoadConfiguration");
                iCollaborateWebSiteURL = ConfigurationSettings.AppSettings["iCollaborateWebSiteURL"];
                IntranetWebSiteURL = ConfigurationSettings.AppSettings["IntranetWebSiteURL"];
                SubSite = ConfigurationSettings.AppSettings["SubSite"];
                Domain = ConfigurationSettings.AppSettings["Domain"];
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SPSite site = new SPSite(iCollaborateWebSiteURL + "/" + SubSite))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            foreach (SPListItem items in web.Lists["Configurations"].Items)
                            {
                                Logger.WriteLog("Loading Configurations");
                                DeleteUserListName = items["DeleteUserListName"].ToString();
                                LogsFolder = items["LogsFolder"].ToString();
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("LoadConfiguration: " + ex.Message + " " + ex.StackTrace);
            }
            Logger.WriteLog("End LoadConfiguration");
        }

        public void FirstCheck()
        {
            try
            {
                Logger.WriteLog("Start FirstCheck");
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SPSite site = new SPSite(iCollaborateWebSiteURL))
                    {
                        using (SPWeb oWeb = site.OpenWeb())
                        {
                            oWeb.AllowUnsafeUpdates = true;

                            try
                            {
                                Logger.WriteLog("Check: " + Domain + "slnu_spfarm");
                                //add user to the group in the web
                                SPUser oUser = oWeb.EnsureUser(Domain + "slnu_spfarm");
                                Logger.WriteLog("User Exist: " + "slnu_spfarm");
                            }
                            catch (Exception e)
                            {
                                Logger.WriteLog("Unable to find: slnu_spfarm in " + oWeb.Url + " : " + e.Message);
                            }

                            string useraccount = "";
                            try
                            {

                                useraccount = ConfigurationSettings.AppSettings["TestUserAccount"];

                                Logger.WriteLog("Check TestUserAccont: " + useraccount);
                                SPUser oUser = oWeb.EnsureUser(useraccount);
                                Logger.WriteLog("User Exist: " + useraccount);
                            }
                            catch (Exception e)
                            {
                                Logger.WriteLog("Unable to find: "+ useraccount + " in " + oWeb.Url + " : " + e.Message);
                            }
                            oWeb.AllowUnsafeUpdates = false;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("FirstCheck: " + ex.Message + " " + ex.StackTrace);
            }
            Logger.WriteLog("End FirstCheck");
        }


        public bool CheckUserExist(SPWeb oWeb, string LoginName)
        {
            try
            {
                //add user to the group in the web
                SPUser oUser = oWeb.EnsureUser(LoginName);
                //SPUser oUser = oWeb.EnsureUser(Domain + LoginName);
                Logger.WriteLog("User Exist: " + LoginName);
                return true ;
            }
            catch (Exception e)
            {
                Logger.WriteErrorLog("Unable to find: " + LoginName + " in " + oWeb.Url + " : " + e.Message);
                return false;
            }
        }


        public void GenerateCSVFile(List<UserDTO> UserListDTO, string CSVFileName)
        {
            //Header
            WriteCSVLog("Name,Account ,URL ,SharePoint Group ,Role ,Status", CSVFileName);

            foreach (UserDTO UserDTOItem in UserListDTO)
            {
                WriteCSVLog(UserDTOItem.Name + "," + UserDTOItem.Account + "," + UserDTOItem.URL + "," + UserDTOItem.SharePointGroup + "," + UserDTOItem.Role + "," + UserDTOItem.AccountStatus, CSVFileName);
            }
            Logger.WriteLog("GenerateCSVFile for: " + CSVFileName);
        }

        public void UploadFileIntoSharePoint(string URL, string FileLocation, string Folder)
        {
            try
            {
                Logger.WriteLog("Start UploadCVSFileIntoSharePoint: " + FileLocation);
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SPSite site = new SPSite(iCollaborateWebSiteURL + "/" + SubSite))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            if (File.Exists(@FileLocation))
                            {
                                Stream fs = File.OpenRead(@FileLocation);
                                BinaryReader br = new BinaryReader(fs);
                                byte[] Logobytes = br.ReadBytes((Int32)fs.Length);
                                web.Folders.Add(Folder);
                                web.Update();
                                SPFile spfile = web.Folders[DeleteUserListName].Files.Add(URL, Logobytes, true);
                                //try
                                //{
                                //    spfile.CheckIn("");
                                //}
                                //catch (Exception A)
                                //{
                                //    WriteErrorLog("UploadCVSFileIntoSharePoint: " + A.Message + " " + A.StackTrace);
                                //}
                                //try
                                //{
                                //    spfile.Publish("");
                                //}
                                //catch (Exception B)
                                //{
                                //    WriteErrorLog("UploadCVSFileIntoSharePoint: " + B.Message + " " + B.StackTrace);
                                //}
                                //try
                                //{
                                //    spfile.Approve("");
                                //}
                                //catch (Exception C)
                                //{
                                //    WriteErrorLog("UploadCVSFileIntoSharePoint: " + C.Message + " " + C.StackTrace);
                                //}
                                spfile.Update();
                            }
                        }

                    }
                });
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("UploadCVSFileIntoSharePoint: " + ex.Message + " " + ex.StackTrace);
            }
            Logger.WriteLog("End UploadCVSFileIntoSharePoint");
        }

        public void WriteCSVLog(string LineRecord, string SiteCollectionName)
        {
            try
            {
                Console.WriteLine("UploadCVSFileIntoSharePoint");
                TextWriter tw = new StreamWriter(ConfigurationSettings.AppSettings["Log"] + DateTime.Now.ToString("yyyyMMdd") + SiteCollectionName + "_DeletedUsers.csv", true);
                //12-Jan-2021
                //TextWriter tw = new StreamWriter(ConfigurationSettings.AppSettings["Log"] + CSVFileName + DateTime.Now.ToString("yyyyMMdd") + ".csv", true);
                tw.WriteLine(LineRecord);
                tw.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error creating log file:" + ex.Message + " " + ex.StackTrace);
                Logger.WriteErrorLog("Error creating log file:" + ex.Message + " " + ex.StackTrace);
            }
        }



















    }
}
