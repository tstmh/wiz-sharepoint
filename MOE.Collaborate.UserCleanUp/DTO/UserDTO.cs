using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace MOE.Collaborate.UserCleanUp.DTO
{
    public class UserDTO
    {
        private int _ID;
        public int ID
        {
            get
            {
                return this._ID;
            }
            set
            {
                this._ID = value;
            }
        }

        private string _Name;
        public string Name
        {
            get
            {
                return this._Name;
            }
            set
            {
                this._Name = value;
            }
        }


        private string _Email;
        public string Email
        {
            get
            {
                return this._Email;
            }
            set
            {
                this._Email = value;
            }
        }


        private string _ldapPath;
        public string LdapPath
        {
            get
            {
                return this._ldapPath;
            }
            set
            {
                this._ldapPath = value;
            }
        }


        private string _Account;
        public string Account
        {
            get
            {
                return this._Account;
            }
            set
            {
                this._Account = value;
            }
        }



        private SPUser _SPAccount;
        public SPUser SPAccount
        {
            get
            {
                return this._SPAccount;
            }
            set
            {
                this._SPAccount = value;
            }
        }


        private string _URL;
        public string URL
        {
            get
            {
                return this._URL;
            }
            set
            {
                this._URL = value;
            }
        }

        private string _Role;
        public string Role
        {
            get
            {
                return this._Role;
            }
            set
            {
                this._Role = value;
            }
        }


        private string _SharePointGroup;
        public string SharePointGroup
        {
            get
            {
                return this._SharePointGroup;
            }
            set
            {
                this._SharePointGroup = value;
            }
        }


        private bool _UserEnabled;
        public bool UserEnabled
        {
            get
            {
                return this._UserEnabled;
            }
            set
            {
                this._UserEnabled = value;
            }
        }


        private string _AccountStatus;
        public string AccountStatus
        {
            get
            {
                return this._AccountStatus;
            }
            set
            {
                this._AccountStatus = value;
            }
        }


        private string _Domain;
        public string Domain
        {
            get
            {
                return this._Domain;
            }
            set
            {
                this._Domain = value;
            }
        }

    }
}
