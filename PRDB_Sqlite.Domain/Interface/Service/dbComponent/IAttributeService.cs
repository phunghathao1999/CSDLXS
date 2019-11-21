using PRDB_Sqlite.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRDB_Sqlite.Domain.Interface.Service.dbComponent
{
    public interface IAttributeSetvice : IService
    {
        IList<PAttribute> getListAttributeByScheme(PSchema pSchema);
        bool DeleteAllAttribute();
        bool DeleteAllAttributeByScheme(PSchema pSchema,bool con);
        PAttribute Insert(PAttribute pAttribute);
        PAttribute Update(PAttribute pAttribute);
        PAttribute Delete(PAttribute pAttribute);
        int getNextIdAttr();
    }
}
