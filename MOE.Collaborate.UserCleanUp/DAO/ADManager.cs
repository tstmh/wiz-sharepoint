using System;
using System.DirectoryServices;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Configuration;
using MOE.Collaborate.UserCleanUp.DTO;
using System.DirectoryServices.ActiveDirectory;

namespace MOE.Collaborate.UserCleanUp.DAO
{
    public class ADManager
    {
        #region private variables
        //private static readonly ADManager instance = new ADManager();
        #endregion private variables
        #region constructor
        // Explicit static constructor to tell C# compiler

        //private ADManager()
        //{
        //}
        #endregion constructor
        #region public methods

        //public bool UpdateEmail(XDBSImportRecord record)
        //{
        //    bool result = true;
        //    Logger.Logger.WriteLog("Processing user '{0}'.", record.UserName);
        //    string domainName = FindUser(record);
        //    if (!string.IsNullOrEmpty(domainName))
        //    {
        //        Logger.Logger.WriteLog("Found user '{0}' in domain '{1}'.", record.UserName, domainName);
        //        try
        //        {
        //            ADUpdateEmail(record);
        //        }
        //        catch (System.UnauthorizedAccessException uae)
        //        {

        //            Logger.Logger.WriteLog(uae.ToString());
        //            Logger.WriteLog("Authorization Exception when updating user '{0}' into AD. Please check log file!.", record.UserName);
        //            result = false;
        //        }
        //        catch (COMException comEx)
        //        {
        //            Logger.Logger.WriteLog(comEx.ToString());
        //            Logger.WriteLog("Exception when updating user '{0}' into AD. Please check log file!.", record.UserName);
        //            result = false;
        //        }
        //        catch (TargetInvocationException targetInEx)
        //        {
        //            Logger.Logger.WriteLog(targetInEx.ToString());
        //            Logger.WriteLog("Exception when updating user '{0}' into AD. Please check log file!.", record.UserName);
        //            result = false;
        //        }
        //    }
        //    else
        //    {
        //        Logger.WriteLog("Cannot find user '{0}' in any domain. Record skipped!.", record.UserName);
        //        result = false;
        //    }
        //    return result;
        //}
        //public bool ProcessRecord(ImportBaseRecord record, List<string> allGroups)
        //{
        //    bool result = true;
        //    Logger.WriteLog(String.Format("Processing user '{0}'.", record.UserName));
        //    string domainName = FindUser(record);
        //    if (!string.IsNullOrEmpty(domainName))
        //    {
        //        Logger.WriteLog(String.Format("Found user '{0}' in domain '{1}'.", record.UserName, domainName));

        //        try
        //        {
        //            //List<string> groups = GetUserGroupMembership(record.UserName, domainName, record.LdapPath);
        //            //result &= ProcessDelete(domainName, record.UserName, record.LdapPath, record.GroupToBeAdded, groups, allGroups);
        //            //result &= ProcessAdd(domainName, record.UserName, record.LdapPath, record.GroupToBeAdded, groups, allGroups);
        //        }
        //        catch (System.UnauthorizedAccessException uae)
        //        {
        //            Logger.WriteErrorLog(uae.ToString());
        //            Logger.WriteLog(String.Format("Authorization Exception when updating user '{0}' into AD. Please check log file!.", record.UserName));
        //            result = false;
        //        }
        //        catch (COMException comEx)
        //        {
        //            Logger.WriteErrorLog(comEx.ToString());
        //            Logger.WriteLog(String.Format("Exception when updating user '{0}' into AD. Please check log file!.", record.UserName));
        //            result = false;
        //        }
        //        catch (TargetInvocationException targetInEx)
        //        {
        //            Logger.WriteErrorLog(targetInEx.ToString());
        //            Logger.WriteLog(String.Format("Exception when updating user '{0}' into AD. Please check log file!.", record.UserName));
        //            result = false;
        //        }
        //    }
        //    else
        //    {
        //        Logger.WriteLog(String.Format("Cannot find user '{0}' in any domain. Record skipped!.", record.UserName));
        //        result = false;
        //    }
        //    return result;
        //}
        #endregion
        #region private method
        //private bool ProcessDelete(string domainName, string userName, string ldapPath, List<string> groupToBeAdded, List<string> existingGroups, List<string> allGroups)
        //{
        //    bool result = true;
        //    List<string> upperToBeAdded = new List<string>();
        //    foreach (string group in groupToBeAdded)
        //    {
        //        upperToBeAdded.Add(group.ToUpper());
        //    }
        //    foreach (string currentGroup in existingGroups)
        //    {
        //        if (!upperToBeAdded.Contains(currentGroup))
        //        {
        //            if (allGroups.Contains(currentGroup))
        //            {
        //                //if a group is deleted in AD, all users will be removed from the membership
        //                //therefore there is no need to check group exist here.
        //                string groupPath = FindGroup(currentGroup, domainName, ldapPath);
        //                if (!string.IsNullOrEmpty(groupPath))
        //                {
        //                    Logger.Logger.WriteLog("Group '{0}' found '{1}'.", currentGroup, groupPath);
        //                    RemoveUserFromGroup(userName, domainName, ldapPath, groupPath);
        //                    Logger.Logger.WriteLog("User '{0}' has been removed from group '{1}'.", userName, currentGroup);
        //                }
        //                else
        //                {
        //                    Logger.WriteLog("Cannot find group '{0}' in domain '{1}'. Cannot remove membership from user '{2}'!.", currentGroup, domainName, userName);
        //                    result = false;
        //                }
        //            }
        //        }
        //    }
        //    return result;
        //}
        //private bool ProcessAdd(string domainName, string userName, string ldapPath, List<string> groupToBeAdded, List<string> existingGroups, List<string> allGroups)
        //{
        //    bool result = true;
        //    foreach (string toBeAdded in groupToBeAdded)
        //    {
        //        if (!existingGroups.Contains(toBeAdded.ToUpper()))
        //        {
        //            if (allGroups.Contains(toBeAdded.ToUpper()))
        //            {
        //                string groupPath = FindGroup(toBeAdded, domainName, ldapPath);
        //                if (!string.IsNullOrEmpty(groupPath))
        //                {
        //                    Logger.Logger.WriteLog("Group '{0}' found '{1}'.", toBeAdded, groupPath);
        //                    AddUserToGroup(userName, domainName, ldapPath, groupPath);
        //                    Logger.Logger.WriteLog("User '{0}' has been added to group '{1}'.", userName, toBeAdded);
        //                }
        //                else
        //                {
        //                    Logger.WriteLog("Cannot find group '{0}' in domain '{1}'. Cannot add membership to user '{2}'!.", toBeAdded, domainName, userName);
        //                    result = false;
        //                }
        //            }
        //            else
        //            {
        //                Logger.WriteLog("Cannot find group '{0}' in list of acceptable groups. Cannot add membership to user '{1}'!.", toBeAdded, userName);
        //            }
        //        }
        //        else
        //        {
        //            Logger.Logger.WriteLog("User '{0}' already belongs to group '{1}'. Cannot add membership!", userName, toBeAdded);
        //        }
        //    }
        //    return result;
        //}
        private static DirectoryEntry GetUser(string userName, string domainName, string ldapPath)
        {
            String userPath = CreateUserPath(userName, domainName, ldapPath);
            DirectoryEntry result = new DirectoryEntry(userPath);
            return result;
        }
        //private string CreateLocalGroupPath(string groupName, string domainName)
        //{
        //    return String.Format("WinNT://{0}/{1},group", domainName, groupName);

        //}
        //private void AddUserToGroup(String userName, string domainName, string ldapPath, string groupPath)
        //{
        //    using (DirectoryEntry theGroup = new DirectoryEntry(groupPath))
        //    {
        //        String userPath = CreateUserPath(userName, domainName, ldapPath);
        //        theGroup.Invoke("Add", new object[] { userPath });
        //        theGroup.CommitChanges();
        //        theGroup.Close();
        //    }
        //}
        private static string CreateUserPath(string userName, string domainName, string ldapPath)
        {
            if (!string.IsNullOrEmpty(ldapPath))
            {
                return ldapPath;
            }
            else
            {
                return String.Format("WinNT://{0}/{1},user", domainName, userName);
            }
        }
        //private void RemoveUserFromGroup(String userName, string domainName, string ldapPath, string groupPath)
        //{
        //    using (DirectoryEntry theGroup = new DirectoryEntry(groupPath))
        //    {
        //        String userPath = CreateUserPath(userName, domainName, ldapPath);
        //        theGroup.Invoke("Remove", new object[] { userPath });
        //        theGroup.CommitChanges();
        //        theGroup.Close();
        //    }
        //}
        //private List<string> GetUserGroupMembership(string userName, string domainName, string ldapPath)
        //{
        //    List<string> groups = new List<string>();
        //    using (DirectoryEntry theUser = GetUser(userName, domainName, ldapPath))
        //    {
        //        object allGroups = theUser.Invoke("Groups");
        //        if (allGroups is IEnumerable)
        //        {
        //            //http://forums.microsoft.com/MSDN/ShowPost.aspx?PostID=139981&SiteID=1
        //            //foreach bug error in VSTS unit testing code coverage causing two blocks error
        //            //foreach (object group in (IEnumerable)allGroups)
        //            IEnumerable enumerable = allGroups as IEnumerable;
        //            IEnumerator enumerator = enumerable.GetEnumerator();
        //            while (enumerator.MoveNext())
        //            {
        //                using (DirectoryEntry theGroup = new DirectoryEntry(enumerator.Current))
        //                {
        //                    string[] tempSplit = theGroup.Name.ToUpper().Split('=');
        //                    string actualName = string.Empty;
        //                    if (tempSplit.Length == 1)
        //                    {
        //                        actualName = tempSplit[0];
        //                    }
        //                    else
        //                    {
        //                        actualName = tempSplit[1];
        //                    }
        //                    groups.Add(actualName);
        //                    Logger.Logger.WriteLog("Group found '{0}'", actualName);
        //                    theGroup.Close();
        //                }
        //                theUser.Close();
        //            }
        //        }
        //    }
        //    return groups;
        //}
        public static List<UserDTO> FindAllUsers()
        {
            List<UserDTO> ADrecords = new List<UserDTO>();
            UserDTO record = new UserDTO();

            string domain = string.Empty;
            // Get the directoryentry of the Global Catalog root
            try
            {
                using (DirectoryEntry globalCatalog = new DirectoryEntry("GC:"))
                {
                    if (globalCatalog != null)
                    {
                        // This node has exactly one child, which can be used to search the entire forest
                        IEnumerator enumerator = globalCatalog.Children.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            using (DirectorySearcher searcher = new DirectorySearcher(((DirectoryEntry)enumerator.Current), string.Format("(&(objectCategory=person)(objectClass=user)(sAMAccountName={0}))", "*"), new string[] { "distinguishedName" }, SearchScope.Subtree))
                            {
                                SearchResultCollection result = searcher.FindAll();
                                if ((result != null) && (result.Count > 0))
                                {
                                    for (int i = 0; i < result.Count; i++)
                                    {
                                        record = new UserDTO();
                                        record.LdapPath = result[i].Path.Replace("GC:", "LDAP:");
                                        if (IsAccountActive(record))
                                        {
                                            string currentDomain = ExtractDomain(result[i].Path);

                                            if (currentDomain != "")
                                            {
                                                record.UserEnabled = true;
                                                record.Domain = currentDomain;
                                                ADrecords.Add(record);
                                            }
                                            else
                                            {
                                                Logger.WriteLog(String.Format("Cannot find active user '{0}' in any domains. Record skipped!.", record.Account));
                                                domain = string.Empty;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        globalCatalog.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("Error FindAllUsers: " + ex.Message + ex.StackTrace);
            }
            
            return ADrecords;
        }

        
        //private string FindGroup(string groupName, string domainName, string userLdapPath)
        //{

        //    string groupPath = string.Empty;

        //    if ((string.IsNullOrEmpty(userLdapPath)) && GroupExistsInLocalMachine(groupName))
        //    {
        //        return CreateLocalGroupPath(domainName, groupName);
        //    }
        //    // Get the directoryentry of the Global Catalog root
        //    using (DirectoryEntry globalCatalog = new DirectoryEntry("GC:"))
        //    {
        //        if (globalCatalog != null)
        //        {
        //            // This node has exactly one child, which can be used to search the entire forest
        //            IEnumerator enumerator = globalCatalog.Children.GetEnumerator();
        //            while (enumerator.MoveNext())
        //            {
        //                using (DirectorySearcher searcher = new DirectorySearcher(((DirectoryEntry)enumerator.Current), string.Format("(&(objectCategory=group)(objectClass=group)(CN={0}))", groupName), new string[] { "distinguishedName" }, SearchScope.Subtree))
        //                {
        //                    SearchResultCollection result = searcher.FindAll();
        //                    if ((result != null) && (result.Count > 0))
        //                    {

        //                        for (int i = 0; i < result.Count; i++)
        //                        {
        //                            string tempLdapPath = result[i].Path.Replace("GC:", "LDAP:");
        //                            string currentDomain = ExtractDomain(tempLdapPath);

        //                            if (currentDomain.Trim().ToUpper().Equals(domainName.ToUpper()))
        //                            {
        //                                groupPath = tempLdapPath;
        //                                break;
        //                            }
        //                        }

        //                    }
        //                    else
        //                    {
        //                        ////create AD group
        //                        //try
        //                        //{
        //                        //    var rootDC = new DirectoryEntry(((DirectoryEntry)enumerator.Current).Path.Replace("GC:", "LDAP:"));
        //                        //    GrpType gt = GrpType.GlobalGrp | GrpType.SecurityGrp;
        //                        //    int typeNum = (int)gt;
        //                        //    DirectoryEntry group = rootDC.Children.Add("cn=" + groupName + ",cn=Users", "group");
        //                        //    group.Properties["sAMAccountName"].Add(groupName);
        //                        //    group.Properties["groupType"].Add(typeNum);
        //                        //    group.CommitChanges();
        //                        //    string tempLdapPath = group.Path.Replace("GC:", "LDAP:");
        //                        //    string currentDomain = ExtractDomain(tempLdapPath);

        //                        //    if (currentDomain.Trim().ToUpper().Equals(domainName.ToUpper()))
        //                        //    {
        //                        //        groupPath = tempLdapPath;
        //                        //        Logger.Logger.WriteLog("Group '{0}' are created successfully.", groupName);
        //                        //        break;
        //                        //    }
        //                        //}
        //                        //catch (Exception e)
        //                        //{
        //                        //    Logger.Logger.WriteLog("Group '{0}' cannot be created. The error is: '{1}'.", groupName, e.ToString());
        //                        //    Console.WriteLine(e.Message.ToString());
        //                        //}

        //                        groupPath = string.Empty;
        //                    }
        //                }
        //            }
        //            globalCatalog.Close();
        //        }
        //    }
        //    return groupPath;
        //}

        // Grouptype-Definition 
        enum GrpType : uint
        {
            UnivGrp = 0x08,
            DomLocalGrp = 0x04,
            GlobalGrp = 0x02,
            SecurityGrp = 0x80000000
        }

        private static string ExtractDomain(string ldapPath)
        {
            string domainName = string.Empty;
            string[] saTemp = ldapPath.Split(',');
            foreach (string temp in saTemp)
            {
                if (temp.StartsWith("DC="))
                {
                    string[] saDomain = temp.Split('=');
                    domainName = saDomain[1];
                    break;
                }
            }
            return (domainName);
        }

        //private bool GroupExistsInLocalMachine(string groupName)
        //{
        //    try
        //    {
        //        using (DirectoryEntry result = new DirectoryEntry(CreateLocalGroupPath(groupName, Environment.MachineName)))
        //        {
        //            string test = result.SchemaClassName;
        //            result.Close();
        //            return true;
        //        }
        //    }
        //    catch (COMException comEx)
        //    {
        //        Logger.Logger.WriteLog(comEx.ToString());
        //        return false;
        //    }
        //}
        private static bool UserExistsInLocalMachine(string userName)
        {
            try
            {
                using (DirectoryEntry result = GetUser(userName, Environment.MachineName, null))
                {
                    string test = result.SchemaClassName;
                    result.Close();
                    return true;
                }
            }
            catch (COMException comEx)
            {
                Logger.WriteErrorLog(comEx.ToString());
                return false;
            }
        }
        private static bool IsAccountActive(UserDTO record)
        {
            using (DirectoryEntry entry = new DirectoryEntry(record.LdapPath))
            {
                if (entry.Properties.Contains("samaccountname"))
                {
                    record.Account = entry.Properties["samaccountname"][0].ToString();
                }

                if (entry.Properties.Contains("mail"))
                {
                    record.Email = entry.Properties["mail"][0].ToString();
                }
                int userAccountControl = Convert.ToInt32(entry.Properties["userAccountControl"][0].ToString());
                int flagExists = userAccountControl & 2;
                return (!(flagExists > 0));
            }
        }
        //private void ADUpdateEmail(XDBSImportRecord record)
        //{
        //    if (!record.Email.Equals(record.CurrentEmail))
        //    {
        //        using (DirectoryEntry theUser = new DirectoryEntry(record.LdapPath))
        //        {
        //            if (theUser.Properties.Contains("mail"))
        //            {
        //                Logger.Logger.WriteLog("Updating User '{0}' '{1}' email from '{2}' to '{3}'", record.UserName, record.LdapPath, record.CurrentEmail, record.Email);
        //                theUser.Properties["mail"][0] = record.Email;
        //            }
        //            else
        //            {
        //                Logger.Logger.WriteLog("User '{0}' '{1}' does not has any email. Adding '{2}'.", record.UserName, record.LdapPath, record.Email);
        //                theUser.Properties["mail"].Add(record.Email);
        //            }
        //            theUser.CommitChanges();
        //            theUser.Close();
        //        }
        //        Logger.Logger.WriteLog("User '{0}' email has been updated from '{1}' to '{2}'", record.UserName, record.CurrentEmail, record.Email);
        //    }
        //}
        #endregion private method


    }
}
