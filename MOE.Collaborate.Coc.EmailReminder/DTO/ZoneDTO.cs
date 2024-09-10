using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MOE.Collaborate.Coc.DTO
{
    public class ZoneDTO
    {
        private string _ZoneName;
        public string ZoneName
        {
            get
            {
                return this._ZoneName;
            }
            set
            {
                this._ZoneName = value;
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
        
