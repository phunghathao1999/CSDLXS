using PRDB_Sqlite.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRDB_Sqlite.Domain.Interface.Service
{
    public interface IRawDatabaseService : IDbService
    {
        //schemaSv
        int getNextIdSch();
        PSchema getSchemeById(int id);
        PSchema Insert(PSchema pSchema);
        PSchema Update(PSchema pSchema);
        PSchema Delete(PSchema pSchema);
        //AttrSv
        int getNextIdAttr();

        PAttribute Insert(PAttribute pAttribute);
        PAttribute Update(PAttribute pAttribute);
        PAttribute Delete(PAttribute pAttribute);
        //RelationSv
        int getNextIdRl();
        IList<PRelation> getRelByIdSch(PSchema pSchema);
        PRelation Insert(PRelation pRelation);
        bool InsertTupleData(PRelation pRelation);
        PRelation Update(PRelation pRelation);
        PRelation Delete(PRelation pRelation);

        //QuerySv

    }
}
