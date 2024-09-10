using MOE.Collaborate.PermissionChecker.DAO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace MOE.Collaborate.PermissionChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            BaseDAO BaseDao = new BaseDAO();

            Console.WriteLine("Starting Job.");
            BaseDao.WriteLog("--------------------------------------------------------------------");
            BaseDao.WriteLog("Starting Job.......................");
            string iCollaborateWebSiteURL = ConfigurationSettings.AppSettings["iCollaborateWebSiteURL"];

            BaseDao.LoadConfiguration();


            if (!string.IsNullOrEmpty(iCollaborateWebSiteURL))
            {
                BaseDao.RetrieveWebApplicationInfo(iCollaborateWebSiteURL);
            }
            else
            {
                BaseDao.WriteLog("URL is blank for iCollaborateWebSiteURL...");
            }


            Console.WriteLine("Completed Job.");
            //BaseDao.WriteLog("Completed Job.");
            //BaseDao.WriteLog("--------------------------------------------------------------------");
            //Console.ReadKey();


        }
    }
}
