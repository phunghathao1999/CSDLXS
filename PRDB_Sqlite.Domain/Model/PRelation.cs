using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRDB_Sqlite.Domain.Model
{
    public class PRelation
    {
        public int? id { get; set; }
        public PSchema schema { get; set; }
        public string relationName { get; set; } 
        public IList<PTuple> tupes { get; set; }
        public override string ToString()
        {
            return this.relationName;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public PRelation(int id, PSchema schema, string relationName, IList<PTuple> tupes)
        {
            this.id = id;
            this.schema = schema;
            this.relationName = relationName;
            this.tupes = tupes;
        }

        public PRelation()
        {
            this.id = -1;
            this.schema = new PSchema();
            this.tupes = new List<PTuple>();
            this.relationName = "No Name";
           // this.RenameRelationName = string.Empty;
            //this.ListRenameRelation = new List<string>();
        }
    }
}
