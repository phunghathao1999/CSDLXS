using PRDB_Sqlite.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRDB_Sqlite.Domain.Interface.Service.dbComponent
{
    public interface IQueryService:IService
    {
        IList<PQuery> getAllQuery();
        bool Insert(PQuery pQuery);
        bool DeleteAllQuery();
        int Update(PQuery pQuery);
        IList<PQuery> getQueryByName(PQuery pQuery);
        bool DeleteById(PQuery pQuery);
    }
}
