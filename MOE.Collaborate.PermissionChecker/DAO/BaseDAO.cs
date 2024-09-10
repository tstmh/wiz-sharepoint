using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using MOE.Collaborate.PermissionChecker.DTO;
using System.Collections.ObjectModel;
using System.IO.Compression;


namespace MOE.Collaborate.PermissionChecker.DAO
{
    public class BaseDAO
    {
        List<SiteURLDTO> SitCollectionURListDTO = new List<SiteURLDTO>();

        string PermissionLibrary;
        string LogsFolder;
        string iCollaborateWebSiteURL;
        string SubSite;

        public void LoadConfiguration()
        {
            try
            {
                Console.WriteLine("Start LoadConfiguration");
                WriteLog("Start LoadConfiguration");
                iCollaborateWebSiteURL = ConfigurationSettings.AppSettings["iCollaborateWebSiteURL"];
                SubSite = ConfigurationSettings.AppSettings["SubSite"];

                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SPSite site = new SPSite(iCollaborateWebSiteURL + "/" + SubSite))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            foreach (SPListItem items in web.Lists["Configurations"].Items)
                            {
                                Console.WriteLine("Loading Configurations");
                                WriteLog("Loading Configurations");
                                LogsFolder = items["LogsFolder"].ToString();
                                PermissionLibrary = items["PermissionLibrary"].ToString();
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                WriteErrorLog("LoadConfiguration: " + ex.Message + " " + ex.StackTrace);
            }
            WriteLog("End LoadConfiguration");
            Console.WriteLine("End LoadConfiguration");




        }

        public void RetrieveWebApplicationInfo(string SiteURL)
        {
            Console.WriteLine("Web Application: " + SiteURL);
            GetAllSiteCollection(SiteURL);
            List<UserDTO> UserListDTO = new List<UserDTO>();
            foreach (SiteURLDTO SiteCollectionItemDTO in SitCollectionURListDTO)
            {
                UserListDTO = new List<UserDTO>();
                //UserListDTO.AddRange(GetAllSiteCollectionUser(SiteCollectionItemDTO.URL));
                GenerateCSVFile(GetAllSiteCollectionUser(SiteCollectionItemDTO.URL), SiteCollectionItemDTO.Name);
                UploadFileIntoSharePoint(SiteCollectionItemDTO.Name);
            }

            string fileLocation = ConfigurationSettings.AppSettings["Log"] + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            string URL = iCollaborateWebSiteURL + "/" + SubSite + "/" + LogsFolder + "/" + DateTime.Now.ToString("yyyyMMdd") + " - PermissionCheckerLog.zip";
            UploadFileIntoSharePoint(URL, fileLocation, LogsFolder);

            fileLocation = ConfigurationSettings.AppSettings["ErrorLog"] + DateTime.Now.ToString("yyyyMMdd") + " - Error.txt";
            URL = iCollaborateWebSiteURL + "/" + SubSite + "/" + LogsFolder + "/" + DateTime.Now.ToString("yyyyMMdd") + " - PermissionChecker - Error.zip";
            UploadFileIntoSharePoint(URL, fileLocation, LogsFolder);
            WriteLog("Completed Job.");
            WriteLog("--------------------------------------------------------------------");


            bool isbeingused = true;
            System.Threading.Thread.Sleep(10000);
            while (isbeingused)
            {
                try
                {
                    if (!File.Exists(ConfigurationSettings.AppSettings["archivelocation"] + DateTime.Now.ToString("yyyyMMdd") + " - Archive.zip"))
                    {
                        ZipFile.CreateFromDirectory(ConfigurationSettings.AppSettings["rootfolder"], ConfigurationSettings.AppSettings["archivelocation"] + DateTime.Now.ToString("yyyyMMdd") + " - Archive.zip");
                        isbeingused = false;
                    }
                    else
                    {
                        isbeingused = false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error Archiving the file : " + ConfigurationSettings.AppSettings["archivelocation"], ConfigurationSettings.AppSettings["archivelocation"] + DateTime.Now.ToString("yyyyMMdd") + ".zip || " + ex.Message);
                    System.Threading.Thread.Sleep(5000);
                }

            }
            System.Threading.Thread.Sleep(10000);

            isbeingused = true;
            while (isbeingused)
            {
                try
                {
                    if (Directory.Exists(ConfigurationSettings.AppSettings["Log"]))
                    {
                        Directory.Delete(ConfigurationSettings.AppSettings["Log"], true);
                        isbeingused = false;
                    }
                    else
                    {
                        isbeingused = false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error deleting:" + ex.Message);
                    System.Threading.Thread.Sleep(1000);
                }

            }
            System.Threading.Thread.Sleep(10000);

            isbeingused = true;
            while (isbeingused)
            {
                try
                {
                    if (Directory.Exists(ConfigurationSettings.AppSettings["ErrorLog"]))
                    {
                        Directory.Delete(ConfigurationSettings.AppSettings["ErrorLog"], true);
                        isbeingused = false;
                    }
                    else
                    {
                        isbeingused = false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error deleting:" + ex.Message);
                    System.Threading.Thread.Sleep(1000);
                }

            }
            System.Threading.Thread.Sleep(20000);

            isbeingused = true;
            while (isbeingused)
            {
                try
                {
                    if (Directory.Exists(ConfigurationSettings.AppSettings["ziplocation"]))
                    {
                        Directory.Delete(ConfigurationSettings.AppSettings["ziplocation"], true);
                        isbeingused = false;
                    }
                    else
                    {
                        isbeingused = false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error deleting:" + ex.Message);
                    System.Threading.Thread.Sleep(1000);
                }

            }

        }

        public void GetAllSiteCollection(string SiteURL)
        {
            SitCollectionURListDTO = new List<SiteURLDTO>();
            string WebApplication = "";
            try
            {
                Console.WriteLine("Start GetAllSiteCollection");
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SPSite site = new SPSite(SiteURL))
                    {
                        foreach (SPSite webcollection in site.WebApplication.Sites)
                        {
                            WebApplication = webcollection.WebApplication.Name;

                            SiteURLDTO SiteCollectionItemDTO = new SiteURLDTO();
                            //SiteCollectionItemDTO.Name = WebApplication;
                            //SiteCollectionItemDTO.Name = webcollection.RootWeb.ToString();
                            SiteCollectionItemDTO.Name = webcollection.ServerRelativeUrl.Replace("/", "");
                            SiteCollectionItemDTO.URL = webcollection.Url;
                            SitCollectionURListDTO.Add(SiteCollectionItemDTO);
                            WriteLog("Site Collection list: " + WebApplication + " : " + webcollection.Url);
                            Console.WriteLine(WebApplication + " : " + webcollection.Url);
                        }
                    }
                });
                Console.WriteLine("End GetAllSiteCollection");
            }
            catch (Exception ex)
            {
                WriteErrorLog("GetAllSiteCollection: " + ex.Message + " " + ex.StackTrace);
            }
        }

        public List<UserDTO> GetAllSiteCollectionUser(string SiteURL)
        {
            List<UserDTO> UserListDTO = new List<UserDTO>();
            try
            {
                string RoleLevel = "";

                bool isUser = false;
                Console.WriteLine("Start GetAllSiteCollectionUser for " + SiteURL);
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SPSite SPsite = new SPSite(SiteURL))
                    {
                        SPWebCollection webs = SPsite.AllWebs;
                        foreach (SPWeb web in webs)
                        {
                            #region Site Permission
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
                                    UserItemDTO.Name = spUser.Name;
                                    UserItemDTO.Account = spUser.LoginName;
                                    UserItemDTO.Email = spUser.Email;
                                    UserItemDTO.SPAccount = spUser;
                                    UserItemDTO.SharePointGroup = "-";
                                    UserItemDTO.Role = RoleLevel;
                                    UserItemDTO.URL = web.Url;
                                    UserListDTO.Add(UserItemDTO);
                                }
                                else
                                {
                                    // Retrieve user groups having permissions on the list
                                    SPPrincipal oPrincipal = SProleAssignment.Member;
                                    SPGroup oRoleGroup = (SPGroup)oPrincipal;

                                    if (oRoleGroup.Users.Count > 0)
                                    {
                                        string strGroupName = oRoleGroup.Name;
                                        string strGroupNames = oPrincipal.Roles[0].Name;
                                        foreach (SPRoleDefinition RoleDefinition in SProleAssignment.RoleDefinitionBindings)
                                        {
                                            RoleLevel += RoleDefinition.Name + ";";
                                        }
                                        foreach (SPUser SPuser in oRoleGroup.Users)
                                        {
                                            UserDTO UserItemDTO = new UserDTO();
                                            UserItemDTO.Name = SPuser.Name;
                                            UserItemDTO.Account = SPuser.LoginName;
                                            UserItemDTO.Email = SPuser.Email;
                                            UserItemDTO.SPAccount = SPuser;
                                            UserItemDTO.SharePointGroup = strGroupName;
                                            UserItemDTO.Role = RoleLevel;
                                            UserItemDTO.URL = web.Url;
                                            UserListDTO.Add(UserItemDTO);

                                        }
                                    }
                                }
                            }

                            #endregion


                            foreach (SPList SPListcollection in web.Lists)
                            {
                                #region Library Permision
                                if (SPListcollection.HasUniqueRoleAssignments && !SPListcollection.Hidden)
                                {
                                    foreach (SPRoleAssignment SProleAssignment in SPListcollection.RoleAssignments)
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
                                            UserItemDTO.Name = spUser.Name;
                                            UserItemDTO.Account = spUser.LoginName;
                                            UserItemDTO.Email = spUser.Email;
                                            UserItemDTO.SPAccount = spUser;
                                            UserItemDTO.SharePointGroup = "-";
                                            UserItemDTO.Role = RoleLevel;
                                            UserItemDTO.URL = web.Url + "/" + SPListcollection.RootFolder.Url;
                                            UserListDTO.Add(UserItemDTO);
                                        }
                                        else
                                        {
                                            // Retrieve user groups having permissions on the list
                                            SPPrincipal oPrincipal = SProleAssignment.Member;
                                            SPGroup oRoleGroup = (SPGroup)oPrincipal;

                                            if (oRoleGroup.Users.Count > 0)
                                            {
                                                string strGroupName = oRoleGroup.Name;
                                                string strGroupNames = oPrincipal.Roles[0].Name;
                                                foreach (SPRoleDefinition RoleDefinition in SProleAssignment.RoleDefinitionBindings)
                                                {
                                                    RoleLevel += RoleDefinition.Name + ";";
                                                }
                                                foreach (SPUser SPuser in oRoleGroup.Users)
                                                {
                                                    UserDTO UserItemDTO = new UserDTO();
                                                    UserItemDTO.Name = SPuser.Name;
                                                    UserItemDTO.Account = SPuser.LoginName;
                                                    UserItemDTO.Email = SPuser.Email;
                                                    UserItemDTO.SPAccount = SPuser;
                                                    UserItemDTO.SharePointGroup = strGroupName;
                                                    UserItemDTO.Role = RoleLevel;
                                                    UserItemDTO.URL = web.Url;
                                                    UserListDTO.Add(UserItemDTO);

                                                }
                                            }
                                        }
                                    }
                                }

                                #endregion

                                #region Document Permision

                                foreach (SPListItem oLI in SPListcollection.Folders)
                                {
                                    if (oLI.HasUniqueRoleAssignments && !SPListcollection.Hidden)
                                    {
                                        foreach (SPRoleAssignment SProleAssignment in oLI.RoleAssignments)
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
                                                UserItemDTO.Name = spUser.Name;
                                                UserItemDTO.Account = spUser.LoginName;
                                                UserItemDTO.Email = spUser.Email;
                                                UserItemDTO.SPAccount = spUser;
                                                UserItemDTO.SharePointGroup = "-";
                                                UserItemDTO.Role = RoleLevel;
                                                //UserItemDTO.URL = web.Url + "/" + SPListcollection.RootFolder.Url;
                                                UserItemDTO.URL = web.Url + "/" + oLI.Folder.Url;
                                                UserListDTO.Add(UserItemDTO);
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
                                                        UserItemDTO.Account = SPuser.LoginName;
                                                        UserItemDTO.Email = SPuser.Email;
                                                        UserItemDTO.SPAccount = SPuser;
                                                        UserItemDTO.SharePointGroup = strGroupName;
                                                        UserItemDTO.Role = RoleLevel;
                                                        UserItemDTO.URL = web.Url + "/" + oLI.Folder.Url;
                                                        //UserItemDTO.URL = web.Url;
                                                        UserListDTO.Add(UserItemDTO);

                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                    }
                });
                Console.WriteLine("End GetAllSiteCollectionUser for " + SiteURL);
            }
            catch (Exception ex)
            {
                WriteErrorLog("GetAllSiteCollectionUser: " + ex.Message + " " + ex.StackTrace);
            }
            return UserListDTO;
        }

        public void GenerateCSVFile(List<UserDTO> UserListDTO, string SiteCollectionName)
        {
            //Header
            WriteCSVLog("Name,Account ,URL ,SharePoint Group ,Role", SiteCollectionName);
            foreach (UserDTO UserDTOItem in UserListDTO)
            {
                WriteCSVLog(UserDTOItem.Name + "," + UserDTOItem.Account + "," + UserDTOItem.URL + "," + UserDTOItem.SharePointGroup + "," + UserDTOItem.Role, SiteCollectionName);
            }
        }

        #region Upload
        private void UploadFileIntoSharePoint(string SiteCollectionName)
        {
            string fileLocation = ConfigurationSettings.AppSettings["csvlocation"] + DateTime.Now.ToString("yyyyMMdd") + SiteCollectionName + "_Permissions.csv";
            string URL = "";
            string Folder = "";
            if (SiteCollectionName == "")
            {
                Folder = iCollaborateWebSiteURL + "/" + SubSite + "/" + PermissionLibrary + "/MainSite/";
                URL = iCollaborateWebSiteURL + "/" + SubSite + "/" + PermissionLibrary + "/MainSite/" + DateTime.Now.ToString("yyyyMMdd") + "_Permissions.zip";
            }
            else
            {
                Folder = iCollaborateWebSiteURL + "/" + SubSite + "/" + PermissionLibrary + "/" + SiteCollectionName + "/";
                URL = iCollaborateWebSiteURL + "/" + SubSite + "/" + PermissionLibrary + "/" + SiteCollectionName + "/" + DateTime.Now.ToString("yyyyMMdd") + "_Permissions.zip";
            }
            UploadFileIntoSharePoint(URL, fileLocation, Folder);
        }

        public void UploadFileIntoSharePoint(string URL, string FileLocation, string Folder)
        {
            try
            {
                WriteLog("Start UploadCVSFileIntoSharePoint: " + FileLocation);
                Console.WriteLine("Start UploadFileIntoSharePoint.");
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SPSite site = new SPSite(iCollaborateWebSiteURL + "/" + SubSite))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            if (File.Exists(@FileLocation))
                            {
                                //12-July-2024 LN-2024 SR
                                if (!Directory.Exists(ConfigurationSettings.AppSettings["ziplocation"]))
                                {
                                    Directory.CreateDirectory(ConfigurationSettings.AppSettings["ziplocation"]);
                                }

                                string startPath = Path.GetDirectoryName(FileLocation);
                                string zipPath = ConfigurationSettings.AppSettings["ziplocation"] + Path.GetFileName(FileLocation).Replace(".csv", ".zip").Replace(".txt",".zip");
                                ZipFile.CreateFromDirectory(startPath, zipPath);

                                //12-July-2024 LN-2024 SR


                                //Stream fs = File.OpenRead(@FileLocation);
                                Stream fs = File.OpenRead(@zipPath);
                                BinaryReader br = new BinaryReader(fs);
                                byte[] Logobytes = br.ReadBytes((Int32)fs.Length);

                                
                                web.Folders.Add(Folder);
                                web.Update();
                                SPFile spfile = web.Folders[PermissionLibrary].Files.Add(URL, Logobytes, true);
                               
                                try
                                {
                                    spfile.CheckIn("");
                                }
                                catch (Exception A)
                                {
                                    WriteErrorLog("UploadCVSFileIntoSharePoint: " + A.Message + " " + A.StackTrace);
                                }
                                try
                                {
                                    spfile.Publish("");
                                }
                                catch (Exception B)
                                {
                                    WriteErrorLog("UploadCVSFileIntoSharePoint: " + B.Message + " " + B.StackTrace);
                                }
                                try
                                {
                                    spfile.Approve("");
                                }
                                catch (Exception C)
                                {
                                    WriteErrorLog("UploadCVSFileIntoSharePoint: " + C.Message + " " + C.StackTrace);
                                }
                                spfile.Update();
                                fs.Close();
                            }
                        }

                    }
                });
                System.Threading.Thread.Sleep(5000);
                bool isbeingused = true;
                while(isbeingused)
                {
                    try
                    {
                        if (Directory.Exists(ConfigurationSettings.AppSettings["csvlocation"]))
                        {
                            Directory.Delete(ConfigurationSettings.AppSettings["csvlocation"], true);
                            isbeingused = false;
                        }
                        else
                        {
                            isbeingused = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error deleting:" + ex.Message);
                        System.Threading.Thread.Sleep(1000);
                    }
                    System.Threading.Thread.Sleep(5000);
                }

            }
            catch (Exception ex)
            {
                WriteErrorLog("UploadCVSFileIntoSharePoint: " + ex.Message + " " + ex.StackTrace);
            }
            Console.WriteLine("End UploadCVSFileIntoSharePoint");
            WriteLog("End UploadCVSFileIntoSharePoint");
        }

        
        #endregion

        #region Logging
        public void WriteCSVLog(string LineRecord, string SiteCollectionName)
        {
            if (!Directory.Exists(ConfigurationSettings.AppSettings["csvlocation"]))
            {
                Directory.CreateDirectory(ConfigurationSettings.AppSettings["csvlocation"]);
            }

            try
            {
                TextWriter tw = new StreamWriter(ConfigurationSettings.AppSettings["csvlocation"] + DateTime.Now.ToString("yyyyMMdd") + SiteCollectionName + "_Permissions.csv", true);
                tw.WriteLine(LineRecord);
                tw.Close();
                //tw.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error creating log file:" + ex.Message);
                //throw;
            }
        }

        public void WriteErrorLog(string log)
        {// create a writer and open the file

            if (!Directory.Exists(ConfigurationSettings.AppSettings["ErrorLog"]))
            {
                Directory.CreateDirectory(ConfigurationSettings.AppSettings["ErrorLog"]);
            }

            try
            {
                TextWriter tw = new StreamWriter(ConfigurationSettings.AppSettings["ErrorLog"] + DateTime.Now.ToString("yyyyMMdd") + " - Error.txt", true);
                // write a line of text to the file
                tw.WriteLine(DateTime.Now + "    :   " + log);
                // close the stream
                tw.Close();
                //tw.Dispose();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error creating log file:" + ex.Message);
                //throw;
            }

        }

        public void WriteLog(string log)
        {// create a writer and open the file

            if (!Directory.Exists(ConfigurationSettings.AppSettings["Log"]))
            {
                Directory.CreateDirectory(ConfigurationSettings.AppSettings["Log"]);
            }

            
            try
            {
                TextWriter tw = new StreamWriter(ConfigurationSettings.AppSettings["Log"] + DateTime.Now.ToString("yyyyMMdd") + ".txt", true);
                // write a line of text to the file
                tw.WriteLine(DateTime.Now + "    :   " + log);
                // close the stream
                tw.Close();
                //tw.Dispose();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error creating log file:" + ex.Message);
                //throw;
            }

        }
        #endregion
    }
}
