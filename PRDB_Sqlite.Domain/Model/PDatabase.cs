using PRDB_Sqlite.Infractructure.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRDB_Sqlite.Domain.Model
{
    public class PDatabase
    {
        #region Properties

        // Tên cơ sở dữ liệu
        public string DbName { get; set; }
        //Chuổi kết nối
        public string ConnectString { get; set; }
        // Đường dẫn đến CSDL
        public string DBPath { get; set; }
        // Tập các lược đồ cơ sở dữ liệu
        public IList<PSchema> Schemas { get; set; }

        // Tập các quan hệ cơ sở dữ liệu
        public IList<PRelation> Relations { get; set; }
        // Tập các truy vấn cơ sở dữ liệu
          public IList<PQuery> Queries { get; set; }
        //DataSet
        public DataSet DataSet { get; set; }


        #endregion
        public PDatabase(string dbName, string connectString, string dBPath, IList<PSchema> schemas, IList<PRelation> relations,IList<PQuery> query, DataSet dataSet)
        {
            DbName = dbName;
            ConnectString = connectString;
            DBPath = dBPath;
            Schemas = schemas;
            Relations = relations;
            Queries = query;
            DataSet = dataSet;
        }

        public PDatabase(string dBPath)
        {
            this.DBPath = dBPath;
            // Lấy đường dẫn cho CSDL 
            this.DbName = "";

            for (int i = dBPath.Length - 1; i >= 0; i--)
            {
                if (dBPath[i] == '\\') break;
                else this.DbName = dBPath[i] + DbName;
            }
            // Đặt chuỗi kết nối
            this.ConnectString = "Data Source=" + DBPath + ";Version=3;";

            this.DbName = Utility.Instance().CutExtension(DbName);
            this.Relations = new List<PRelation>();
            this.Queries = new List<PQuery>();
            this.Schemas = new List<PSchema>();
        }

        public override string ToString()
        {
            return this.ConnectString + "\n" + this.Relations.ToString() + "\n" + this.Schemas.ToString();
        }
    }
}
