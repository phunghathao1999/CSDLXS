using PRDB_Sqlite.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRDB_Sqlite.Domain.Interface.Service.dbComponent
{
    public interface ISchemaService : IService
    {
        IList<PSchema> getAllScheme();
        bool DeleteAllScheme();
        PSchema getSchemeById(int id);
        PSchema Insert(PSchema pSchema);
        PSchema Update(PSchema pSchema);
        PSchema Delete(PSchema pSchema);
        int getNextIdSch();
    }
}
