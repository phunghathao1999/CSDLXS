using PRDB_Sqlite.Domain.Interface;
using PRDB_Sqlite.Domain.Interface.Service;
using PRDB_Sqlite.Sevice.SysService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRDB_Sqlite.Sevice.Factory
{
    public class DbFactory
    {
        public static IDbService GetDatabaseService()
        {
            //switch (ConfigurationSettings.AppSettings.Get("defaultDbService").ToString())
            switch (ConfigurationManager.AppSettings["defaultDbService"].ToString())
            {
                case "EF":
                case "Raw": return RawDatabaseService.Instance();
                default:
                   return RawDatabaseService.Instance(); 
            }
        }
    }
}
