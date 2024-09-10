using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOE.Collaborate.UserCleanUp
{
   
    public class Logger
    {
        public static void WriteErrorLog(string log)
        {// create a writer and open the file
            try
            {
                Console.WriteLine(log);
                string ErrorLog = ConfigurationSettings.AppSettings["ErrorLog"] + DateTime.Now.ToString("yyyyMMdd") + "_Deleted_User_Error.txt";
                TextWriter tw = new StreamWriter(ErrorLog, true);
                // write a line of text to the file
                tw.WriteLine(DateTime.Now + "    :   " + log);
                // close the stream
                tw.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error creating log file:" + ex.Message);
                //throw;
            }

        }

        public static void WriteLog(string log)
        {// create a writer and open the file
            try
            {
                Console.WriteLine(log);

                string Logs = ConfigurationSettings.AppSettings["Log"] + DateTime.Now.ToString("yyyyMMdd") + "_Deleted_User.txt";
                TextWriter tw = new StreamWriter(Logs, true);
                // write a line of text to the file
                tw.WriteLine(DateTime.Now + "    :   " + log);
                // close the stream
                tw.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error creating log file:" + ex.Message);
                //throw;
            }
        }
    }
}
