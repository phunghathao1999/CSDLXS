using PRDB_Sqlite.Domain.Unit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRDB_Sqlite.Domain.Model
{
    public class PAttribute: ICloneable
    {
        public bool primaryKey { get; set; }
        public int? id { get; set; }
        public string AttributeName { get; set; }
        public PDataType Type { get; set; }
        public string Description { get; set; }
        public PSchema Schema { get; set; }

        public override string ToString()
        {
            return this.AttributeName;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public PAttribute(bool primaryKey, int id, string attributeName, PDataType type, string description, PSchema pSchema)
        {
            this.primaryKey = primaryKey;
            this.id = id;
            AttributeName = attributeName;
            Type = type;
            Description = description;
            Schema = pSchema;
        }

        public PAttribute()
        {
            this.Type = new PDataType();
            this.Schema = new PSchema();
        }
        public PAttribute(PAttribute pAttribute)
        {
            this.AttributeName = pAttribute.AttributeName;
            this.Description = pAttribute.Description;
            this.id = pAttribute.id;
            this.primaryKey = pAttribute.primaryKey;
            this.Schema = pAttribute.Schema;
            this.Type = pAttribute.Type;
        }

    }
}
