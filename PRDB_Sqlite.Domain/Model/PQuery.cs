using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRDB_Sqlite.Domain.Model
{
    public class PQuery
    {

        #region Properties

        public int? IDQuery { get; set; }
        public string QueryName { get; set; }

        public string QueryString { get; set; }

        #endregion
        #region Methods

        public PQuery()
        {
            this.IDQuery = -1;
            this.QueryString = "";
            this.QueryName = "";
        }

        public PQuery(string p)
        {
            // TODO: Complete member initialization
            this.QueryName = p;
            this.IDQuery = -1;
            this.QueryString = "";
        }

        public PQuery(string p, int Id, string QueryString)
        {
            // TODO: Complete member initialization
            this.QueryName = p;
            this.IDQuery = Id;
            this.QueryString = QueryString;

        }

        public override string ToString()
        {
            return this.QueryName;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion Method

    }
}
