using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MOE.Collaborate.Coc.DTO
{
    public class EmailTemplateDTO
    {
        private string _Title;
        public string Title
        {
            get
            {
                return this._Title;
            }
            set
            {
                this._Title = value;
            }
        }


        private string _Subject;
        public string Subject
        {
            get
            {
                return this._Subject;
            }
            set
            {
                this._Subject = value;
            }
        }


        private string _Body;
        public string Body
        {
            get
            {
                return this._Body;
            }
            set
            {
                this._Body = value;
            }
        }

        private DateTime _Reminder_Date_and_Time;
        public DateTime Reminder_Date_and_Time
        {
            get
            {
                return this._Reminder_Date_and_Time;
            }
            set
            {
                this._Reminder_Date_and_Time = value;
            }
        }

        private bool _Active;
        public bool Active
        {
            get
            {
                return this._Active;
            }
            set
            {
                this._Active = value;
            }
        }
    }
}
