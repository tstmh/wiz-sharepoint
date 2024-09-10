using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace MOE.Collaborate.PermissionChecker.DTO
{
    public class UserDTO
    {
        private long _id;

        public long Id
        {
            get
            {
                return this._id;
            }
            set
            {
                this._id = value;
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

        private string _Permission;
        public string Permission
        {
            get
            {
                return this._Permission;
            }
            set
            {
                this._Permission = value;
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
    }
}
