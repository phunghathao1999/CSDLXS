using PRDB_Sqlite.Domain.Abtraction;
using PRDB_Sqlite.Infractructure.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PRDB_Sqlite.Data.Connector
{
    public class ConcreteDb : ADatabase
    {
        private static Object syncLock = new object();
        private static ConcreteDb _instance;
        public static ConcreteDb Instance
        {
            
            get
            {
                if (_instance == null)
                {
                    lock (syncLock)
                    {
                        var connectionString = Parameter.connectionString.ToString().Trim();
                        if (connectionString != null)
                            _instance = new ConcreteDb(Parameter.connectionString);
                        else
                        {
                            _instance = new ConcreteDb(ConfigurationManager.AppSettings["defaultConectionString"]);
                            MessageBox.Show("Force to use default ConnectionString!", "Application's Alert", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }
                return _instance;
            }
        }
        protected ConcreteDb()
        {

        }

        protected ConcreteDb(string connectionString) : base(connectionString)
        {
        }

        public override bool closeConnection()
        {
            try
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
            catch (SQLiteException sqliteEx)
            {
                errorMessage = sqliteEx.Message;
                return false;
            }
            return true;

        }

        public override bool CreateTable(string sqlCreateTable,bool con = true)
        {
            int rows = 0;

            try
            {
                if(con)
                openConnection();
                command.CommandText = sqlCreateTable;
                command.Connection = connection;
                rows = command.ExecuteNonQuery();

                if (rows < 0)
                    return false;
            }
            catch (SQLiteException sqliteEx)
            {
                errorMessage = sqliteEx.Message;
                return false;
            }
            finally
            {
                if(con)
                closeConnection();
            }
            return true;
        }

        public override bool DropTable(string tblName,bool con = true)
        {
            try
            {
                if(con)
                openConnection();
                command.Connection = connection;
                command.CommandText = "DROP TABLE IF EXISTS " + tblName;
                command.CommandType = CommandType.Text;

                int result = command.ExecuteNonQuery();
                if (result < 0) return false;
            }
            catch (SQLiteException sqliteEx)
            {
                errorMessage = sqliteEx.Message;
                return false;
            }
            finally
            {
                if(con)
                closeConnection();
            }
            return true;
        }

        public override DataTable ExecuteQuery(string query,bool con = true)
        {
            DataTable dtb = new DataTable();
            if(con)
            openConnection();
            try
            {
                command.CommandText = query;
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                adapter = new SQLiteDataAdapter(command);
                adapter.Fill(dtb);
            }
            catch (SQLiteException sqliteEx)
            {
                errorMessage = sqliteEx.Message;
                return null;
            }
            finally
            {
                if(con)
                closeConnection();
            }
            return (dtb.Rows.Count > 0)?dtb:null;
        }

        public override double? ExecuteScalar(string query,bool con = true)
        {
            double rs;
            if(con)
            openConnection();
            try
            {
                command.CommandText = query;
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                if (!Double.TryParse(command.ExecuteScalar().ToString(), out rs)) throw new SQLiteException("Can not Convert QueryScalar to Number");
            }
            catch (SQLiteException sqliteEx)
            {
                errorMessage = sqliteEx.Message;
                return null;
            }
            finally
            {
                if(con)
                closeConnection();
            }
            return rs;
        }

        public override bool openConnection()
        {
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
            }
            catch (SQLiteException sqliteEx)
            {
                errorMessage = sqliteEx.Message;
                return false;
            }
            return true;

        }

        public override DataSet getDataSet(string query, string tblName)
        {
            DataSet dts = new DataSet();
            openConnection();
            try
            {
                command.CommandText = query;
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                adapter = new SQLiteDataAdapter(command);
                adapter.Fill(dts, tblName);
            }
            catch (SQLiteException sqliteEx)
            {
                errorMessage = sqliteEx.Message;
                return null;
            }
            finally
            {
                closeConnection();
            }
            return (dts.Tables.Count >0)?dts:null;
        }

        public override DataSet getDataSet(string query)
        {
            DataSet dts = new DataSet();
            openConnection();
            try
            {
                command.CommandText = query;
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                adapter = new SQLiteDataAdapter(command);
                adapter.Fill(dts);
            }
            catch (SQLiteException sqliteEx)
            {
                errorMessage = sqliteEx.Message;
                return null;
            }
            finally
            {
                closeConnection();
            }
            return (dts.Tables.Count > 0)?dts:null;
        }

        public override bool TableExist(string TableName)
        {
            openConnection();
            try
            {
                command.CommandText = "SELECT name FROM sqlite_master WHERE name='" + TableName + "'";
                command.Connection = connection;
                SQLiteDataReader dtreader = command.ExecuteReader();
                if (dtreader.HasRows)
                    return true;
                return false;
            }
            catch (SQLiteException sqliteEx)
            {
                errorMessage = sqliteEx.Message;
            }
            finally
            {
                closeConnection();
            }

            return false;
        }

        public override DataTable getDataTable(string QueryString)
        {
            var dtb = new DataTable();
            openConnection();
            try
            {
                command.CommandText = QueryString;
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                adapter = new SQLiteDataAdapter(command);
                adapter.Fill(dtb);
            }
            catch (SQLiteException sqliteEx)
            {
                errorMessage = sqliteEx.Message;
                return null;
            }
            finally
            {
                closeConnection();
            }
            return (dtb.Rows.Count >0) ? dtb:null;
        }

        public override DataTable getDataTable(string QueryString, string tablename)
        {
            var dtb = new DataTable(tablename);
            openConnection();
            try
            {
                command.CommandText = QueryString;
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                adapter = new SQLiteDataAdapter(command);
                adapter.Fill(dtb);
            }
            catch (SQLiteException sqliteEx)
            {
                errorMessage = sqliteEx.Message;
                return null;
            }
            finally
            {
                closeConnection();
            }
            return (dtb.Rows.Count > 0) ? dtb:null;
        }

        public override int Update(string stringUpdate,bool con = true)
        {
            int result = -1;
            try
            {
                if(con)
                openConnection();
                command.CommandText = stringUpdate;
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                result = command.ExecuteNonQuery();
            }
            catch (SQLiteException sqliteEx)
            {
                errorMessage = sqliteEx.Message;
            }
            finally
            {
                if(con)
                closeConnection();
            }
            return result;
        }

        public override int? getNextSysId(string systblname)
        {
            openConnection();
            var idListFetch = new List<string>();
            try
            {
                idListFetch.Clear();
                string query = "SELECT ID FROM " + systblname;
                var idList = this.getDataTable(query);
                foreach(DataRow row in idList.Rows)
                {
                    idListFetch.Add(Convert.ToString(row[0]));
                }
                for(int i = 0; ; i++)
                {
                    if (idListFetch.Exists(p => p.Equals(i.ToString()))) i++;
                    else return i;
                }
            }
            catch (SQLiteException sqliteEx)
            {
                errorMessage = sqliteEx.Message;
                return null;
            }
            finally
            {
                closeConnection();
            }
        }

        public override SQLiteTransaction BeginTransaction()
        {
                openConnection();
                return this.connection.BeginTransaction();
        }

        public override void resetConnection()
        {
            _instance = null;
        }
    }
}
