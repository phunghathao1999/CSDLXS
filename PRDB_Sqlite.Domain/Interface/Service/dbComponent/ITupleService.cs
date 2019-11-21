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
        bool DeleteTypeById(PTuple probTuple);
        Object getValuebyType(string rawValue, PDataType type);
        IList<String> getData(string rawValue, PDataType type,bool priKey);

        bool checkDataFormat(string rawValue);


    }
}
