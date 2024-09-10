using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Xml;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Linq;

namespace SPImporter
{
    class ConvertXMLtoText
    {
        //LN-SR2019-002 HRP System Interface Changes
        public static void Convert_HRMSXML_To_OICA()
        {
            Dictionary<String, MT_OICA_AP_DST> OICA_APs = new Dictionary<String, MT_OICA_AP_DST>();
            Dictionary<String, MT_OICA_MOE_DST> OICA_MOEs = new Dictionary<String, MT_OICA_MOE_DST>();
            Dictionary<String, String> MOE_Map = new Dictionary<String, String>();

            List<string> files = new List<string>();
            string filePath = ConfigurationManager.AppSettings["OICA_Source"];
            int fileCount = 0; //reset file count
            string[] fileEntries;

            XmlSerializer serializer_AP = new XmlSerializer(typeof(MT_OICA_AP_DST));
            fileEntries = Directory.GetFiles(filePath, "*_OICA_AP_*.XML");
            foreach (string fullFileName in fileEntries)
            {
                if (fullFileName.IndexOf("_LOG_") == -1)
                {
                    fileCount++;
                    using (FileStream fileStream = new FileStream(fullFileName, FileMode.Open))
                    {
                        OICA_APs.Add((new System.IO.FileInfo(fullFileName)).Name, (MT_OICA_AP_DST)serializer_AP.Deserialize(fileStream));
                    }
                    files.Add(fullFileName);
                }
            }

            //MT_OICA_MOE_DST
            XmlSerializer serializer_MOE = new XmlSerializer(typeof(MT_OICA_MOE_DST));
            fileEntries = Directory.GetFiles(filePath, "*_OICA_MOE_*.XML");
            foreach (string fullFileName in fileEntries)
            {
                Console.WriteLine("Reading - " + fullFileName);
                if (fullFileName.IndexOf("_LOG_") == -1)
                {
                    fileCount++;
                    using (FileStream fileStream = new FileStream(fullFileName, FileMode.Open))
                    {
                        OICA_MOEs.Add((new System.IO.FileInfo(fullFileName)).Name, (MT_OICA_MOE_DST)serializer_MOE.Deserialize(fileStream));
                    }
                    files.Add(fullFileName);
                }
            }

            foreach (var data in OICA_MOEs)
            {                
                String filename = data.Key;
                MT_OICA_MOE_DST MOE = data.Value;
                
                foreach (var employee in MOE.Employees)
                {
                    var NRIC = formatText(employee.NRIC, 9);

                    if (!MOE_Map.ContainsKey(NRIC))
                    {
                        MOE_Map.Add(NRIC, Return_SHORT(employee) + "|" + Return_ORG_LVL(employee));
                        CopySPGroup.WriteToLog("New NRIC added " + NRIC + " - " + Return_SHORT(employee));
                    }
                    else
                    {
                        //remove existing entry and add the latest entry
                        MOE_Map.Remove(NRIC);
                        MOE_Map.Add(NRIC, Return_SHORT(employee) + "|" + Return_ORG_LVL(employee));
                        CopySPGroup.WriteToLog("Existing NRIC added " + NRIC + " - " + Return_SHORT(employee));
                    }
                }
            }
             
            foreach (var data in OICA_APs)
            {
                String filename = data.Key;
                MT_OICA_AP_DST AP = data.Value;

                foreach (var employee in AP.Employees)
                {

                    try
                    {
                        Generate_newOICAFile(employee.NRIC, MOE_Map[employee.NRIC], Return_ORGEH(employee), Return_STELL(employee), Return_DIVIS(employee), Return_SCSER(employee), Return_PR_UNIT_HIER(employee));
                        CopySPGroup.WriteToLog("Try " + employee.NRIC + " - " + MOE_Map[employee.NRIC]);
                    }
                    catch (Exception e)
                    {
                        CopySPGroup.WriteToLog("error catch = " + employee.NRIC);
                        CopySPGroup.WriteToLog(e.Message);
                        Generate_newOICAFile(employee.NRIC, "", Return_ORGEH(employee), Return_STELL(employee), Return_DIVIS(employee), Return_SCSER(employee), Return_PR_UNIT_HIER(employee));
                    }


                }
            }
        }

        private static void Generate_newOICAFile(string PERNO, string School_and_ORGLvl, string ORGEH, string STELL, string DIVIS, string SCSER, string PR_UNIT_HIER)
        {
            string SPImporterFile = "";

            if (string.IsNullOrEmpty(School_and_ORGLvl))
            {
                SPImporterFile = PERNO + "|" + ORGEH + "|" + STELL + "|" + DIVIS + "|" + SCSER + "|" + PR_UNIT_HIER + "|" + "" + "|" + "";
            }
            else
            {
                string[] sch_and_orglvl = School_and_ORGLvl.Split('|');
                //SPImporter Format - NRIC, ORGEH, STELL, DIVIS, SCSER, PR_UNIT_HIER, SHORT, ORG_LVL
                SPImporterFile = PERNO + "|" + ORGEH + "|" + STELL + "|" + DIVIS + "|" + SCSER + "|" + PR_UNIT_HIER + "|" + sch_and_orglvl[0] + "|" + sch_and_orglvl[1];
            }

            string ArchiveDestination = ConfigurationManager.AppSettings["Archive_Destination"] + DateTime.Now.ToString("yyyyMMdd") + "\\";
            System.IO.Directory.CreateDirectory(ArchiveDestination);

            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(ArchiveDestination + "HRPStoSPImporter_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", true))
                {
                    file.WriteLine(SPImporterFile);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Unable to write to SPImporter file - " + SPImporterFile);
            }
        }

        private static string space(int length)
        {
            return "".PadRight(length, ' ');
        }

        private static string formatText(string source, int length)
        {
            if (source == null)
            {
                return "".PadRight(length, ' ');
            }
            else
            {
                return source.Trim().PadRight(length, ' ').Substring(0, length);
            }
        }

        private static string Return_ORGEH(EMPLOYEE_AP employee)
        {
            string XML_ENDDA_NULL = "99991231";
            if (employee.IT0001 != null)
            {
                var t1 = employee.IT0001.LastOrDefault(x => x.ENDDA.Trim().Equals(XML_ENDDA_NULL, StringComparison.OrdinalIgnoreCase));
                if (t1 != null)
                {
                    return formatText(t1.ORGEH, 8);
                }
            }
            return "";
        }

        private static string Return_STELL(EMPLOYEE_AP employee)
        {
            string XML_ENDDA_NULL = "99991231";
            if (employee.IT0001 != null)
            {
                var t1 = employee.IT0001.LastOrDefault(x => x.ENDDA.Trim().Equals(XML_ENDDA_NULL, StringComparison.OrdinalIgnoreCase));
                if (t1 != null)
                {
                    return formatText(t1.STELL, 8);
                }
            }
            return "";
        }

        private static string Return_PR_UNIT_HIER(EMPLOYEE_AP employee)
        {
            string XML_ENDDA_NULL = "99991231";
            if (employee.IT0001 != null)
            {
                var t1 = employee.IT0001.LastOrDefault(x => x.ENDDA.Trim().Equals(XML_ENDDA_NULL, StringComparison.OrdinalIgnoreCase));
                if (t1 != null)
                {
                    return formatText(t1.PR_UNIT_HIER, 63);
                }
            }
            return "";
        }

        private static string Return_DIVIS(EMPLOYEE_AP employee)
        {
            string XML_ENDDA_NULL = "99991231";
            if (employee.IT9040 != null)
            {
                var t1 = employee.IT9040.LastOrDefault(x => x.ENDDA.Trim().Equals(XML_ENDDA_NULL, StringComparison.OrdinalIgnoreCase));
                if (t1 != null)
                {
                    return formatText(t1.DIVIS, 2);
                }
            }
            return "";
        }

        private static string Return_SCSER(EMPLOYEE_AP employee)
        {
            string XML_ENDDA_NULL = "99991231";
            if (employee.IT9040 != null)
            {
                var t1 = employee.IT9040.LastOrDefault(x => x.ENDDA.Trim().Equals(XML_ENDDA_NULL, StringComparison.OrdinalIgnoreCase));
                if (t1 != null)
                {
                    return formatText(t1.SCSER, 3);
                }
            }
            return "";
        }

        private static string Return_SHORT(EMPLOYEE_MOE employee)
        {
            var t1 = employee.DEPT_CODEs.LastOrDefault();
            if (t1 != null)
            {
                return formatText(t1.SHORT, 4);                
            }

            return "";
        }

        private static string Return_ORG_LVL(EMPLOYEE_MOE employee)
        {
            var t1 = employee.DEPT_CODEs.LastOrDefault();
            if (t1 != null)
            {
                return formatText(t1.ORG_LVL, 3);
            }

            return "";
        }
    }
}
