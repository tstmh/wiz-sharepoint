using System;

namespace MOE.Collaborate.Coc.EmailReminder.DAO
{
	public class CocSurveyDTO
	{
		private string _QuestionNumber;

		private string _Answer;

		private DateTime _SubmittionDate;

		private string _CreatedByLoginName;

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

		public string CreatedByLoginName
		{
			get
			{
				return this._CreatedByLoginName;
			}
			set
			{
				this._CreatedByLoginName = value;
			}
		}

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

		public DateTime SubmittionDate
		{
			get
			{
				return this._SubmittionDate;
			}
			set
			{
				this._SubmittionDate = value;
			}
		}

		public CocSurveyDTO()
		{
		}
	}
}