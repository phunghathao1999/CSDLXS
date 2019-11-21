using PRDB_Sqlite.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRDB_Sqlite.Domain.ModelView
{
    public class SchemaModelView
    {
        public string attrName { get; set; }
        public bool isPri { get; set; }
        public string datatype { get; set; }
        public string typeName { get; set; }
        public string domain { get; set; }
        public string descs { get; set; }

        public SchemaModelView(string attrName, bool isPri, string datatype, string typeName, string domain, string descs)
        {
            this.attrName = attrName ?? throw new ArgumentNullException(nameof(attrName));
            this.isPri = isPri;
            this.datatype = datatype ?? throw new ArgumentNullException(nameof(datatype));
            this.typeName = typeName ?? throw new ArgumentNullException(nameof(typeName));
            this.domain = domain ?? throw new ArgumentNullException(nameof(domain));
            this.descs = descs ?? throw new ArgumentNullException(nameof(descs));
        }

        public SchemaModelView()
        {
            this.attrName = "";
            this.isPri = false ;
            this.datatype = "";
            this.typeName = "";
            this.domain = "";
            this.descs = "";
        }
    }
}
