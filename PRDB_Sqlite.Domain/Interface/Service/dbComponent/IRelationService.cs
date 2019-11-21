using PRDB_Sqlite.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRDB_Sqlite.Domain.Interface.Service.dbComponent
{
    public interface IRelationService : IService
    {
        IList<PRelation> getAllRelation();
        bool DropTableByTableName(PRelation pRelation);
        bool DeleteAllRelation();
        PRelation InsertSystemRelation(PRelation pRelation);
        PRelation UpdateSystemRelation(PRelation pRelation);
        PRelation Delete(PRelation pRelation);
        bool InsertTupleDataTable(PRelation pRelation);
        bool InsertTupleIntoTableRelation(PRelation pRelation);
        int getNextIdRel();
    }
}
