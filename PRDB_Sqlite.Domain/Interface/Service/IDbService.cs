using PRDB_Sqlite.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRDB_Sqlite.Domain.Interface.Service
{
    public interface IDbService : IService
    {
        PDatabase CreateNewDatabase(ref PDatabase pDatabase); //dbpath require
        PDatabase OpenExistingDatabase(ref PDatabase pDatabase); //dbpath require
        bool SaveDatabase();
    }
}
