using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MOE.Collaborate.UserCleanUp.DTO
{
    public class SiteCollectionDTO
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
    }
}
