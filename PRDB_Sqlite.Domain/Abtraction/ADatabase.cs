using PRDB_Sqlite.Domain.Interface;
using PRDB_Sqlite.Infractructure.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRDB_Sqlite.Domain.Abtraction
{
    public abstract class ADatabase : IDatabaseUtil
    {
        #region declare Properties

        public SQLiteConnection connection { get; set; }
        public SQLiteCommand command { get; set; }
        public SQLiteDataAdapter adapter { get; set; }
        public string errorMessage { get; set; }
        public Object[] valueCollection { get; set; }
        #endregion
        public ADatabase()
        {
            this.connection = new SQLiteConnection(Parameter.connectionString);
            this.command = new SQLiteCommand();
            this.adapter = new SQLiteDataAdapter();
        }


        public ADatabase(string connectionString)
        {
            this.connection = new SQLiteConnection(connectionString);
            this.command = new SQLiteCommand();
            this.adapter = new SQLiteDataAdapter();
        }

        public abstract bool closeConnection();
       

        public abstract bool CreateTable(string sqlCreateTable,bool con);


        public void Dispose() { }
        
        public abstract bool DropTable(string tblName,bool con);


        public abstract DataTable ExecuteQuery(string query,bool con);


        public abstract double? ExecuteScalar(string query,bool con);

        public abstract bool openConnection();

        public abstract DataSet getDataSet(string query, string tblName);

        public abstract DataSet getDataSet(string query);

        public abstract bool TableExist(string TableName);

        public abstract DataTable getDataTable(string QueryString);

        public abstract DataTable getDataTable(string QueryString, string tablename);

        public abstract int Update(string stringUpdate,bool clscon);

        public abstract int? getNextSysId(string systblname);

        public abstract SQLiteTransaction BeginTransaction();

        public abstract void resetConnection();
      
    }
}
