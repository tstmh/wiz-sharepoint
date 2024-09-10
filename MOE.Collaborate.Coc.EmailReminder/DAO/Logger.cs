using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOE.Collaborate.Coc.EmailReminder.DAO
{
    public class Logger
    {
        public static void WriteLog(string log)
        {
            string filePath = Program.LogPath + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".txt";
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine(DateTime.Now + "    :   " + log);
            }
        }
    }
}
