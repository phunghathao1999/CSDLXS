using System.Collections.Generic;

namespace PRDB_Sqlite.Domain.Model
{
    public class PSchema
    {
        public int id { get; set; }
        public string SchemaName { get; set; }
        public IList<PAttribute> Attributes { get; set; }

        public override string ToString()
        {
            return this.SchemaName;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public PSchema(int id, string schemaName = "", IList<PAttribute> attributes = null)
        {
            this.id = id;
            SchemaName = schemaName;
            Attributes = attributes;
        }

        public PSchema()
        {
            this.id = -1;
            this.SchemaName = "No Name";
            this.Attributes = new List<PAttribute>();
        }


    }
}