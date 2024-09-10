using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Xml.Serialization;

namespace SPImporter
{
    #region "MT_OICA_AP_DST"

    [XmlRoot(Namespace = "http://hrp.gov.sg/OICA_AP")]
    public class MT_OICA_AP_DST
    {
        [XmlElement(Namespace = "", ElementName = "EMPLOYEE")]
        public HashSet<EMPLOYEE_AP> Employees { get; set; }
    }

    public class EMPLOYEE_AP
    {
        [XmlElement(Namespace = "", ElementName = "NRIC")]
        public string NRIC { get; set; }

        [XmlElement(Namespace = "", ElementName = "PERNR")]
        public string PERNR { get; set; }

        [XmlElement(Namespace = "", ElementName = "IT0001")]
        public HashSet<InforType_AP> IT0001 { get; set; }

        [XmlElement(Namespace = "", ElementName = "IT9040")]
        public HashSet<InforType_AP> IT9040 { get; set; }
    }


    public class InforType_AP
    {
        [XmlElement(Namespace = "", ElementName = "BEGDA")]
        public string BEGDA { get; set; }

        [XmlElement(Namespace = "", ElementName = "ENDDA")]
        public string ENDDA { get; set; }

        //IT0001
        [XmlElement(Namespace = "", ElementName = "ORGEH")]
        public string ORGEH { get; set; }

        //IT0001
        [XmlElement(Namespace = "", ElementName = "STELL")]
        public string STELL { get; set; }

        //IT0001
        [XmlElement(Namespace = "", ElementName = "PR_UNIT_HIER")]
        public string PR_UNIT_HIER { get; set; }


        //IT9040
        [XmlElement(Namespace = "", ElementName = "DIVIS")]
        public string DIVIS { get; set; }

        //IT9040
        [XmlElement(Namespace = "", ElementName = "SCSER")]
        public string SCSER { get; set; }

        /// <summary>
        /// U=Add/Update, X=Deleted, Empty=Unchanged
        /// </summary>
        [XmlElement("DEL_IND")]
        public string DEL_IND { get; set; }

    }
    #endregion

    #region "MT_OICA_MOE_DST"

    [XmlRoot(Namespace = "http://hrp.gov.sg/OICA_MOE")]
    public class MT_OICA_MOE_DST
    {
        [XmlElement(Namespace = "", ElementName = "EMPLOYEE")]
        public HashSet<EMPLOYEE_MOE> Employees { get; set; }
    }

    public class EMPLOYEE_MOE
    {
        [XmlElement(Namespace = "", ElementName = "NRIC")]
        public string NRIC { get; set; }

        [XmlElement(Namespace = "", ElementName = "PERNR")]
        public string PERNR { get; set; }

        [XmlElement(Namespace = "", ElementName = "DEPT_CODE")]
        public HashSet<InforType_MOE> DEPT_CODEs { get; set; }
    }

    public class InforType_MOE
    {
        [XmlElement("SHORT")]
        public string SHORT { get; set; }

        [XmlElement("ORG_LVL")]
        public string ORG_LVL { get; set; }

        [XmlElement("ENDDA")]
        public string ENDDA { get; set; }

        /// <summary>
        /// U=Add/Update, X=Deleted, Empty=Unchanged
        /// </summary>
        [XmlElement("DEL_IND")]
        public string DEL_IND { get; set; }

    }


    #endregion
}

