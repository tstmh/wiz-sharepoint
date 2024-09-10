using System;

namespace MOE.Collaborate.UserCleanUp.DTO
{
    public class IAMSDTO
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


        private string _nRIC;
      
        public string NRIC
        {
            get
            {
                return this._nRIC;
            }
            set
            {
                this._nRIC = value;
            }
        }

        private string _full_Name;
      
        public string Full_Name
        {
            get
            {
                return this._full_Name;
            }
            set
            {
                this._full_Name = value;
            }
        }

        private string _account_Id;
      
        public string Account_Id
        {
            get
            {
                return this._account_Id;
            }
            set
            {
                this._account_Id = value;
            }
        }

        private string _location_Code;
      
        public string Location_Code
        {
            get
            {
                return this._location_Code;
            }
            set
            {
                this._location_Code = value;
            }
        }
       
        public string StandardLocationName { get; set; }

        private string _mOE_Applications;
      
        public string MOE_Applications
        {
            get
            {
                return this._mOE_Applications;
            }
            set
            {
                this._mOE_Applications = value;
            }
        }

        private string _designation;
      
        public string Designation
        {
            get
            {
                return this._designation;
            }
            set
            {
                this._designation = value;
            }
        }

        private string _email;
      
        public string Email
        {
            get
            {
                return this._email;
            }
            set
            {
                this._email = value;
            }
        }

        private string _division_Name;
      
        public string Division_Name
        {
            get
            {
                return this._division_Name;
            }
            set
            {
                this._division_Name = value;
            }
        }

        private string _branch_Name;
      
        public string Branch_Name
        {
            get
            {
                return this._branch_Name;
            }
            set
            {
                this._branch_Name = value;
            }
        }

        private string _school_Name;
      
        public string School_Name
        {
            get
            {
                return this._school_Name;
            }
            set
            {
                this._school_Name = value;
            }
        }

        private string _last_Changed_NRIC;
      
        public string Last_Changed_NRIC
        {
            get
            {
                return this._last_Changed_NRIC;
            }
            set
            {
                this._last_Changed_NRIC = value;
            }
        }

        private DateTime? _nRIC_Date_Changed;
      
        public DateTime? NRIC_Date_Changed
        {
            get
            {
                return this._nRIC_Date_Changed;
            }
            set
            {
                this._nRIC_Date_Changed = value;
            }
        }

        private DateTime _modified;
      
        public DateTime Modified
        {
            get
            {
                return this._modified;
            }
            set
            {
                this._modified = value;
            }
        }

        private string _modifiedBy;
      
        public string ModifiedBy
        {
            get
            {
                return this._modifiedBy;
            }
            set
            {
                this._modifiedBy = value;
            }
        }

        private DateTime _created;
      
        public DateTime Created
        {
            get
            {
                return this._created;
            }
            set
            {
                this._created = value;
            }
        }

        private string _createdBy;
      
        public string CreatedBy
        {
            get
            {
                return this._createdBy;
            }
            set
            {
                this._createdBy = value;
            }
        }

    }
}
