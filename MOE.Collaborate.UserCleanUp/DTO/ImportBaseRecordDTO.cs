using System;
using System.Collections.Generic;
using System.Text;

namespace MOE.Collaborate.UserCleanUp.DTO
{
    public class ImportBaseRecord
    {
        private string userName = string.Empty;

        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        private string currentEmail = string.Empty;

        public string CurrentEmail
        {
            get { return currentEmail; }
            set { currentEmail = value; }
        }
        private string ldapPath = string.Empty;

        public string LdapPath
        {
            get { return ldapPath; }
            set { ldapPath = value; }
        }

        private string domain = string.Empty;

        public string Domain
        {
            get { return domain; }
            set { domain = value; }
        }

        private bool isActive = false;

        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }


    }
}
