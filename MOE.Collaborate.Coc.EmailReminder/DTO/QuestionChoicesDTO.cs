using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MOE.Collaborate.Coc.DTO
{
    public class QuestionChoicesDTO
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

        private string _ChoiceLetter;
        public string ChoiceLetter
        {
            get
            {
                return this._ChoiceLetter;
            }
            set
            {
                this._ChoiceLetter = value;
            }
        }


        private string _ChoiceAnswer;
        public string ChoiceAnswer
        {
            get
            {
                return this._ChoiceAnswer;
            }
            set
            {
                this._ChoiceAnswer = value;
            }
        }
    }
}
