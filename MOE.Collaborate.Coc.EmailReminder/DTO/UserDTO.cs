using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MOE.Collaborate.Coc.DTO
{
    public class UserDTO
    {
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


        private string _NRIC;
        public string NRIC
        {
            get
            {
                return this._NRIC;
            }
            set
            {
                this._NRIC = value;
            }
        }

        private string _Schools;
        public string Schools
        {
            get
            {
                return this._Schools;
            }
            set
            {
                this._Schools = value;
            }
        }


        private bool _FirstReminderFlag;
        public bool FirstReminderFlag
        {
            get
            {
                return this._FirstReminderFlag;
            }
            set
            {
                this._FirstReminderFlag = value;
            }
        }

        private string _Zone;
        public string Zone
        {
            get
            {
                return this._Zone;
            }
            set
            {
                this._Zone = value;
            }
        }

        private string _ZoneID;
        public string ZoneID
        {
            get
            {
                return this._ZoneID;
            }
            set
            {
                this._ZoneID = value;
            }
        }

        private bool _Completed_Survey;
        public bool Completed_Survey
        {
            get
            {
                return this._Completed_Survey;
            }
            set
            {
                this._Completed_Survey = value;
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


        private string _Year;
        public string Year
        {
            get
            {
                return this._Year;
            }
            set
            {
                this._Year = value;
            }
        }
    }
}