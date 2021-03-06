using PRDB_Sqlite.Domain.Model;
using PRDB_Sqlite.Domain.Unit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRDB_Sqlite.Domain.Interface.Service.dbComponent
{
    public interface ITupleService : IService
    {
        IList<PTuple> getAllTupleByRelationName(string relationName, IList<PAttribute> pAttributes);
        bool DeleteTupleById(PRelation pRelation, PTuple pTuple);
        Object getValuebyType(string rawValue, PDataType type);
        IList<String> getData(string rawValue, PDataType type,bool priKey);
        
        bool checkDataFormat(string rawValue);
        PTuple Insert(PTuple pTuple, PRelation pRelation); //by id
        PTuple Update(PTuple pTuple, PRelation pRelation,String Key); //by id

        PTuple insertEmptyTuple(PRelation pRelation, PAttribute pri, String IdTuple);

        String getStrTupleId(PRelation pRelation);

    }
}
