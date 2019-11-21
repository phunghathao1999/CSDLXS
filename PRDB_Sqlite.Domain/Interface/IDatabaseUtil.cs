using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRDB_Sqlite.Domain.Interface
{
    public interface IDatabaseUtil : IDisposable
    {
        bool openConnection();
        bool closeConnection();
        double? ExecuteScalar(string query,bool con);
        DataTable ExecuteQuery(string query,bool con);
        bool DropTable(string tblName,bool con);
        bool CreateTable(string sqlCreateTable,bool con);
        DataSet getDataSet(string query, string tblName);
        DataSet getDataSet(string query);
        bool TableExist(string TableName); // Check if the table had been created
        DataTable getDataTable(string QueryString);
        DataTable getDataTable(string QueryString, string tablename);
        int Update(string stringUpdate,bool con);
        int? getNextSysId(string systblname);

        SQLiteTransaction BeginTransaction();

    }
}
