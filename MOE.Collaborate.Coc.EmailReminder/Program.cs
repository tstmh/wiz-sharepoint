using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using MOE.Collaborate.Coc.EmailReminder.DAO;

namespace MOE.Collaborate.Coc.EmailReminder
{
    class Program
    {
        public static string SiteURL;
        public static string LogPath;
        static void Main(string[] args)
        {
            SiteURL = ConfigurationSettings.AppSettings["COCSite"] + "";
            LogPath = ConfigurationSettings.AppSettings["LogPath"] + "";
            BaseDAO BaseDao = new BaseDAO();

            Console.WriteLine("Starting Email Reminder.");
            Logger.WriteLog("--------------------------------------------------------------------");
            Logger.WriteLog("Starting Email.......................");

            BaseDao.SendEmailNotification();
            
            Console.WriteLine("Completed Email Reminder job.");
            Logger.WriteLog("Completed Email Reminder job.");
            Logger.WriteLog("End Email Reminder");
            Logger.WriteLog("--------------------------------------------------------------------");
        }


        
    }
}
