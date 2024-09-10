using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MOE.Collaborate.Coc.DTO
{
    public class CocSurveyFormDTO
    {
        private string _QuestionNumber;
        public string QuestionNumber
        {
            get
            {
                return this._QuestionNumber;
            }
            set
            {
                this._QuestionNumber = value;
            }
        }


        private string _Question;
        public string Question 
        {
            get
            {
                return this._Question;
            }
            set
            {
                this._Question = value;
            }
        }

        private string _Topic;
        public string Topic
        {
            get
            {
                return this._Topic;
            }
            set
            {
                this._Topic = value;
            }
        }


        private string _Answer;
        public string Answer
        {
            get
            {
                return this._Answer;
            }
            set
            {
                this._Answer = value;
            }
        }

        private string _Choice_A ;
        public string Choice_A
        {
            get
            {
                return this._Choice_A;
            }
            set
            {
                this._Choice_A = value;
            }
        }

        private string _Choice_B;
        public string Choice_B
        {
            get
            {
                return this._Choice_B;
            }
            set
            {
                this._Choice_B = value;
            }
        }
        private string _Choice_C;
        public string Choice_C
        {
            get
            {
                return this._Choice_C;
            }
            set
            {
                this._Choice_C = value;
            }
        }
        private string _Choice_D;
        public string Choice_D
        {
            get
            {
                return this._Choice_D;
            }
            set
            {
                this._Choice_D = value;
            }
        }
        private string _IMRulesExplanation;
        public string IMRulesExplanation
        {
            get
            {
                return this._IMRulesExplanation;
            }
            set
            {
                this._IMRulesExplanation = value;
            }
        }
       
       
    }
}
