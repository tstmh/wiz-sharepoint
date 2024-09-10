using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MOE.Collaborate.UserCleanUp.DAO;

namespace MOE.Collaborate.UserCleanUp
{
    class Program
    {
        static void Main(string[] args)
        {
            BaseDAO BaseDao = new BaseDAO();

            Logger.WriteLog("--------------------------------------------------------------------");
            Logger.WriteLog("Starting Job.......................");

            BaseDao.LoadConfiguration();
            BaseDao.FirstCheck();
            if (!string.IsNullOrEmpty(BaseDao.iCollaborateWebSiteURL))
            {
                BaseDao.DeleteUserFromSiteCollection(BaseDao.iCollaborateWebSiteURL, "iCollaborate");
            }
            else
            {
                Logger.WriteLog("URL is blank for iCollaborateWebSiteURL...");
            }

            Logger.WriteLog("Completed Job.");
            Logger.WriteLog("--------------------------------------------------------------------");
        }

    }
}
