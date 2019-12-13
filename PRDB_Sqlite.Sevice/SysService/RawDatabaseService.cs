using PRDB_Sqlite.Data.Connector;
using PRDB_Sqlite.Domain.Interface;
using PRDB_Sqlite.Domain.Interface.Service;
using PRDB_Sqlite.Domain.Interface.Service.dbComponent;
using PRDB_Sqlite.Domain.Model;
using PRDB_Sqlite.Domain.Unit;
using PRDB_Sqlite.Infractructure.Common;
using PRDB_Sqlite.Infractructure.Constant;
using PRDB_Sqlite.Sevice.CommonService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Windows.Forms;

namespace PRDB_Sqlite.Sevice.SysService
{
    public class RawDatabaseService : IService, IRawDatabaseService
    {
        private static object syncLock = new object();
        private static RawDatabaseService instance;
        public static RawDatabaseService Instance()
        {

            if (instance == null) instance = new RawDatabaseService();
            {
                lock (syncLock)
                {
                    instance = new RawDatabaseService();
                }
            }
            return instance;
        }

        protected RawDatabaseService()
        {

        }

        public PDatabase CreateNewDatabase(ref PDatabase dbInfo)
        {

            //reset Concrete
            ConcreteDb.Instance.resetConnection();
            try
            {
                SQLiteConnection.CreateFile(dbInfo.DBPath);

                Parameter.connectionString = dbInfo.ConnectString; //chac chan param da mang value
                ConcreteDb db = ConcreteDb.Instance;


                string strSQL = "";

                // Record set of schemes to the database system
                strSQL += "CREATE TABLE SystemScheme ( ID INT, SchemeName NVARCHAR(255) );";
                if (!db.CreateTable(strSQL))
                    throw new Exception(db.errorMessage);

                // Record set of relations to the database system
                strSQL = "";
                strSQL += "CREATE TABLE SystemRelation (ID INT,RelationName NVARCHAR(255), SchemeID INT );";
                if (!db.CreateTable(strSQL))
                    throw new Exception(db.errorMessage);

                // Record set of attributes to the database system  
                strSQL = "";
                strSQL += "CREATE TABLE SystemAttribute (ID INT,PrimaryKey NVARCHAR(10),AttributeName NVARCHAR(255),DataType NVARCHAR(255),Domain TEXT,Description TEXT,SchemeID INT ); ";
                if (!db.CreateTable(strSQL))
                    throw new Exception(db.errorMessage);

                // Record set of queries to the database system
                strSQL = "";
                strSQL += "CREATE TABLE SystemQuery (ID INT,QueryName NVARCHAR(255),QueryString TEXT );";
                if (!db.CreateTable(strSQL))
                {
                    MessageBox.Show("Error : " + db.errorMessage + " please try again!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
                Parameter.connectionString = String.Empty;
                return null;
            }
            //get full info before
            return dbInfo;
        }

        public void Dispose()
        {
            this.Dispose();
        }

        public bool SaveDatabase()
        {
            throw new NotImplementedException();
        }

        public PDatabase OpenExistingDatabase(ref PDatabase pDatabase)
        {
            //reset Concrete
            ConcreteDb.Instance.resetConnection();
            try
            {
                //IList<PSchema> Schemas = new List<PSchema>();
                var Schemas = SchemaService.Instance().getAllScheme();
                pDatabase.Schemas = Schemas;

                //IList<PRelation> relations = new List<PRelation>();
                var relations = RelationService.Instance().getAllRelation();
                pDatabase.Relations = relations;

                //var querys = new List<PQuery>();
                var querys = QueryService.Instance().getAllQuery();
                pDatabase.Queries = querys;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Notification");

                return null;
            }
            return pDatabase;
        }
        #region schema
        public int getNextIdSch()
        {
            return SchemaService.Instance().getNextIdSch();
        }

        public PSchema getSchemeById(int id)
        {
            return SchemaService.Instance().getSchemeById(id);
        }

        public PSchema Insert(PSchema pSchema)
        {
            return SchemaService.Instance().Insert(pSchema);
        }

        public PSchema Update(PSchema pSchema)
        {
            return SchemaService.Instance().Update(pSchema);
        }

        public PSchema Delete(PSchema pSchema)
        {
            return SchemaService.Instance().Delete(pSchema);
        }

        public int getNextIdAttr()
        {
            return AttributeService.Instance().getNextIdAttr();
        }

        public PAttribute Insert(PAttribute pAttribute)
        {
            return AttributeService.Instance().Insert(pAttribute);
        }

        public PAttribute Update(PAttribute pAttribute)
        {
            return AttributeService.Instance().Update(pAttribute);
        }

        public PAttribute Delete(PAttribute pAttribute)
        {
            return AttributeService.Instance().Delete(pAttribute);
        }

        public int getNextIdRl()
        {
            return RelationService.Instance().getNextIdRel();
        }

        public PRelation Insert(PRelation pRelation)
        {

            if (pRelation.id == -1)
            {
                pRelation.id = RelationService.Instance().getNextIdRel();
            }
            using (var conn = ConcreteDb.Instance.BeginTransaction())
            {
                try
                {
                    string SQL = "";
                    SQL = "";
                    SQL += "INSERT INTO SystemRelation VALUES ( ";
                    SQL += pRelation.id + ",";
                    SQL += "'" + pRelation.relationName + "'" + ",";
                    SQL += pRelation.schema.id;
                    SQL += " );";
                    if (ConcreteDb.Instance.Update(SQL, false) < 0)
                        throw new Exception(ConcreteDb.Instance.errorMessage);
                    //insert Relation Data Table

                    if (!RelationService.Instance().InsertTupleDataTable(pRelation))
                        throw new Exception("The schema have no Attribute!");
                    conn.Commit();
                }
                catch (Exception ex)
                {
                    conn.Rollback();
                    throw new Exception(ex.Message);
                }
                ConcreteDb.Instance.closeConnection();
            }

            return pRelation;
        }

        public PRelation Update(PRelation pRelation)
        {
            throw new NotImplementedException();
        }

        public PRelation Delete(PRelation pRelation)
        {
            return RelationService.Instance().Delete(pRelation);
        }

        public IList<PRelation> getRelByIdSch(PSchema pSchema)
        {
            return RelationService.Instance().getAllRelation().Where(p => p.schema.id == pSchema.id).ToList();
        }

        public bool InsertTupleData(PRelation pRelation)
        {
            throw new NotImplementedException();
        }
        public bool InsertTupleIntoTableRelation(PRelation pRelation)
        {
            return RelationService.Instance().InsertTupleIntoTableRelation(pRelation);
        }
        public PTuple GetTuplebyId(ref PRelation rel, string tupId)
        {
            var relid = rel.id;
            PTuple reVal = null;
            {
                var relation = RelationService.Instance().getAllRelation().Where(r => r.id.Equals(relid)).First();

                tupId = tupId.Replace("{", "");
                tupId = tupId.Replace("}", "");

                if (!(relation is null))
                {
                    var pri = relation.schema.Attributes.Where(a => a.primaryKey).First();
                    var atr = String.Format("{0}.{1}", relation.relationName, pri.AttributeName);
                    if (!(pri is null))
                    {
                        try
                        {
                            reVal = relation.tupes.Where(t => SelectCondition.EQUAL(t.valueSet[atr].First(), tupId.Trim(), pri.Type.TypeName)).First();
                        }
                        catch //ko tim thay (insert khi id is Empty)
                        {
                            if (String.IsNullOrEmpty(tupId))
                                reVal = new PTuple(relation);
                        }
                    }
                    rel = relation;
                }
            }
            return reVal;
        }
      
        public bool DeleteTupleById(PRelation pRelation, PTuple pTuple)
        {
            return PTupleService.Instance().DeleteTupleById(pRelation, pTuple);
        }

        public PTuple Insert(PTuple pTuple, PRelation pRelation)
        {
            return PTupleService.Instance().Insert(pTuple, pRelation);
        }
        public PTuple Update(PTuple pTuple, PRelation pRelation, String key)
        {
            return PTupleService.Instance().Update(pTuple, pRelation, key);
        }

        public PTuple insertEmptyTuple(PRelation pRelation,PAttribute pri, String idTuple)
        {
            return PTupleService.Instance().insertEmptyTuple(pRelation,pri,idTuple);
        }


        #endregion

        protected class SchemaService : ISchemaService
        {
            private static Object synclock = new Object();
            private static SchemaService instance;
            protected SchemaService()
            {

            }
            public static SchemaService Instance()
            {
                lock (synclock)
                {
                    if (instance == null) instance = new SchemaService();
                }
                return instance;
            }

            public PSchema Delete(PSchema pSchema)
            {
                if (!checkDelSch(pSchema))
                    throw new Exception("The Schema have some relation!");

                using (var conn = ConcreteDb.Instance.BeginTransaction())
                {
                    try
                    {
                        //del attr 
                        AttributeService.Instance().DeleteAllAttributeByScheme(pSchema, false);

                        if (ConcreteDb.Instance.Update("DELETE FROM SystemScheme Where ID = " + pSchema.id, false) < 0)
                            throw new Exception(ConcreteDb.Instance.errorMessage);

                        conn.Commit();
                    }
                    catch (Exception ex)
                    {
                        conn.Rollback();
                        throw new Exception(String.Format("Cannot delete Schema {0} : {1}", pSchema.SchemaName, ex.Message));
                    }
                    ConcreteDb.Instance.closeConnection();
                }

                return pSchema;
            }

            private bool checkDelSch(PSchema pSchema)
            {
                var listRel = RawDatabaseService.Instance().getRelByIdSch(pSchema);
                if (listRel.Count == 0 || listRel is null) return true;
                return false;
            }
            public bool DeleteAllScheme()
            {
                throw new NotImplementedException();
            }

            public void Dispose()
            {
                this.Dispose();
            }

            public IList<PSchema> getAllScheme()
            {
                var newSchemas = new List<PSchema>();
                var dts = new DataSet();
                try
                {
                    dts.Tables.Add(ConcreteDb.Instance.getDataTable("SELECT * FROM SystemScheme", "system_scheme"));
                    foreach (DataRow row in dts.Tables["system_scheme"].Rows)
                    {
                        IList<PAttribute> attributes = AttributeService.Instance().getListAttributeByScheme(new PSchema(Convert.ToInt16(row[0])));
                        newSchemas.Add(new PSchema(Convert.ToInt32(row[0]), row[1].ToString(), attributes));
                    }
                }
                catch
                {

                }
               
                return newSchemas;
            }

            public int getNextIdSch()
            {
                var i = 1;
                while (true)
                {
                    var count = ConcreteDb.Instance.ExecuteScalar(String.Format("select count(*) from SystemScheme sch where sch.Id = '{0}'", i));
                    if (count == 0d) break;
                    i++;
                }
                return i;
            }

            public PSchema getSchemeById(int id)
            {
                PSchema newSchema = new PSchema();
                var dts = new DataSet();
                dts.Tables.Add(ConcreteDb.Instance.getDataTable("SELECT * FROM SystemScheme where ID = " + id, "system_scheme"));

                foreach (DataRow row in dts.Tables["system_scheme"].Rows)
                {
                    IList<PAttribute> attributes = AttributeService.Instance().getListAttributeByScheme(new PSchema(Convert.ToInt16(row[0])));
                    newSchema = new PSchema(Convert.ToInt16(row[0]), row[1].ToString(), attributes);
                }
                return newSchema;
            }

            public PSchema Insert(PSchema pSchema) // chi add tren pDatabase
            {
                try
                {
                    string SQL = "";
                    SQL += "INSERT INTO SystemScheme VALUES (";
                    SQL += pSchema.id + ",";
                    SQL += "'" + pSchema.SchemaName + "'";
                    SQL += " );";
                    if (ConcreteDb.Instance.Update(SQL) == 0)
                        throw new Exception(ConcreteDb.Instance.errorMessage);
                    return pSchema;
                }
                catch
                {
                    return null;
                }

            }

            public PSchema Update(PSchema pSchema)
            {
                string SQL = "";
                SQL += "Update SystemScheme  SET ";
                SQL += " SchemeName  = '" + pSchema.SchemaName+"'";
                SQL += " Where  ID = '" + pSchema.id+"'";
                if (ConcreteDb.Instance.Update(SQL) < 0)
                    throw new Exception("Failed to Update Schema, plz try again!");
                return pSchema;
            }
        }
        protected class AttributeService : IAttributeSetvice
        {
            private static Object synclock = new Object();
            private static AttributeService instance;
            protected AttributeService()
            {

            }
            public static AttributeService Instance()
            {
                lock (synclock)
                {
                    if (instance == null) instance = new AttributeService();
                }
                return instance;
            }

            public PAttribute Delete(PAttribute pAttribute)
            {
                if (ConcreteDb.Instance.Update(String.Format("DELETE FROM SystemAttribute Where ID = {0}", pAttribute.id)) < 0)
                    throw new Exception(ConcreteDb.Instance.errorMessage);
                return pAttribute;
            }

            public bool DeleteAllAttribute()
            {
                throw new NotImplementedException();
            }

            public bool DeleteAllAttributeByScheme(PSchema pSchema, bool con)
            {
                if (ConcreteDb.Instance.Update(String.Format("DELETE FROM SystemAttribute Where SchemeID = {0}", pSchema.id), con) < 0)
                    throw new Exception(ConcreteDb.Instance.errorMessage);
                return true;
            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public IList<PAttribute> getListAttributeByScheme(PSchema pSchema)
            {
                IList<PAttribute> probAttributes = new List<PAttribute>();
                DataTable dtb = ConcreteDb.Instance.getDataTable("SELECT * FROM SystemAttribute Where SchemeID = " + pSchema.id);

                if (dtb != null)
                    foreach (DataRow attrRow in dtb.Rows)
                    {
                        var NewAttr = new PAttribute();
                        NewAttr.id = Convert.ToInt32(attrRow[0]);
                        NewAttr.primaryKey = Convert.ToBoolean(attrRow[1]);
                        //NewAttr.AttributeName = String.Format("{0}.{1}", pSchema.SchemaName, Convert.ToString(attrRow[2]).ToLower());
                        NewAttr.AttributeName = Convert.ToString(attrRow[2]).ToLower();
                        NewAttr.Type.TypeName = Convert.ToString(attrRow[3]);
                        NewAttr.Type.GetDomain(Convert.ToString(attrRow[4]));
                        NewAttr.Type.GetDataType();
                        NewAttr.Description = Convert.ToString(attrRow[5]);
                        NewAttr.Schema.id = (int)attrRow[6];
                        probAttributes.Add(NewAttr);
                    }
                return probAttributes;
            }

            public int getNextIdAttr()
            {
                var i = 1;
                while (true)
                {
                    var count = ConcreteDb.Instance.ExecuteScalar(String.Format("select count(*) from SystemAttribute att where att.Id = '{0}'", i));
                    if (count == 0d) break;
                    i++;
                }
                return i;
            }

            public PAttribute Insert(PAttribute pAttribute)
            {
                //add attr o dau => CSDL
                string SQL = "";
                SQL += "INSERT INTO SystemAttribute VALUES ( ";
                SQL += pAttribute.id + ",";
                SQL += "'" + pAttribute.primaryKey + "'" + ",";
                SQL += "'" + pAttribute.AttributeName + "'" + ",";
                SQL += "'" + pAttribute.Type.TypeName + "'" + ",";
                SQL += "'" + pAttribute.Type.DomainString + "'" + ",";
                SQL += "'" + pAttribute.Description + "'" + ",";
                SQL += pAttribute.Schema.id; // notice
                SQL += " );";
                try
                {
                    ConcreteDb.Instance.Update(SQL);
                    return pAttribute;
                }
                catch
                {
                    return null;
                }
            }

            public PAttribute Update(PAttribute pAttribute)
            {
                if (pAttribute.AttributeName.Equals(ContantCls.emlementProb,StringComparison.CurrentCultureIgnoreCase)) return null;
                string sql = String.Format("UPDATE SystemAttribute SET PrimaryKey = '{0}',AttributeName='{1}', DataType='{2}', Domain='{3}', Description='{4}', SchemeID='{5}' WHERE ID= '{6}' ",
                    pAttribute.primaryKey ? "True" : "False",
                    pAttribute.AttributeName,
                    pAttribute.Type.DataType,
                    pAttribute.Type.DomainString,
                    pAttribute.Description,
                    pAttribute.Schema.id,
                    pAttribute.id);
                if (ConcreteDb.Instance.Update(sql) < 0)
                    throw new Exception(String.Format("Failed to Update Attribute {0}, Plz try again!", pAttribute.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error));
                return pAttribute;
            }
        }
        protected class RelationService : IRelationService
        {
            private static Object synclock = new Object();
            private static RelationService instance;
            protected RelationService()
            {

            }
            public static RelationService Instance()
            {
                lock (synclock)
                {
                    if (instance == null) instance = new RelationService();
                }
                return instance;
            }

            public bool DeleteAllRelation()
            {
                throw new NotImplementedException();
            }

            public PRelation Delete(PRelation pRelation)
            {
                using (var conn = ConcreteDb.Instance.BeginTransaction())
                {
                    try
                    {
                        //delete SystemRelation
                        if (ConcreteDb.Instance.Update("DELETE FROM SystemRelation where ID = " + pRelation.id, false) < 0)
                            throw new Exception(ConcreteDb.Instance.errorMessage);
                        //delete data tuple
                        if (!ConcreteDb.Instance.DropTable(pRelation.relationName, false))
                            throw new Exception(ConcreteDb.Instance.errorMessage);
                        conn.Commit();
                    }
                    catch (Exception ex)
                    {
                        conn.Rollback();
                        throw new Exception(ex.Message);
                    }
                    ConcreteDb.Instance.closeConnection();
                }
                return pRelation;
            }

            public void Dispose()
            {
                this.Dispose();
            }

            public bool DropTableByTableName(PRelation pRelation)
            {
                throw new NotImplementedException();
            }

            public IList<PRelation> getAllRelation()
            {
                //"SELECT * FROM SystemRelation", "system_relation")

                var relations = new List<PRelation>();
                DataSet dts = new DataSet();
                try
                {
                    dts.Tables.Add(ConcreteDb.Instance.getDataTable("SELECT * FROM SystemRelation", "system_relation"));
                    foreach (DataRow row in dts.Tables["system_relation"].Rows)
                    {
                        string relationname = row[1].ToString();
                        PSchema schema = SchemaService.Instance().getSchemeById(Convert.ToInt16(row[2]));
                        var tuple = PTupleService.Instance().getAllTupleByRelationName(relationname, schema.Attributes);
                        if (tuple is null) tuple = new List<PTuple>();
                        PRelation relation = new PRelation(Convert.ToInt16(row[0]), schema, relationname, tuple);
                        relations.Add(relation);
                    }
                }
                catch
                {

                }
             
                return relations;
            }

            public int getNextIdRel()
            {
                var i = 1;
                while (true)
                {
                    var count = ConcreteDb.Instance.ExecuteScalar(String.Format("select count(*) from SystemRelation att where att.Id = '{0}'", i));
                    if (count == 0d) break;
                    i++;
                }
                return i;
            }

            public PRelation InsertSystemRelation(PRelation pRelation)
            {
                throw new NotImplementedException();
            }

            public bool InsertTupleDataTable(PRelation pRelation)
            {
                if (pRelation.schema.Attributes.Count > 0)
                {
                    string SQL = "";
                    SQL += "CREATE TABLE " + pRelation.relationName + " ( ";
                    foreach (var attribute in pRelation.schema.Attributes)
                    {
                        SQL += attribute.AttributeName + " " + "TEXT" + ", ";
                    }
                    SQL = SQL.Remove(SQL.LastIndexOf(','), 1);
                    SQL += " ); ";

                    if (!ConcreteDb.Instance.CreateTable(SQL, false))
                        throw new Exception(ConcreteDb.Instance.errorMessage);
                    return true;
                }
                else
                    return false;
            }

            public bool InsertTupleIntoTableRelation(PRelation pRelation)
            {
                if (pRelation.schema.Attributes.Count > 0)
                {
                    string SQL = "";
                    SQL += "INSERT INTO " + pRelation.relationName + " VALUES ( ";
                    foreach (var attribute in pRelation.schema.Attributes)
                    {
                        SQL +=  " " +  ", ";
                    }
                    SQL = SQL.Remove(SQL.LastIndexOf(','), 1);
                    SQL += " ); ";

                    if (!ConcreteDb.Instance.CreateTable(SQL, false))
                        throw new Exception(ConcreteDb.Instance.errorMessage);
                    return true;
                }
                else
                    return false;
            }

            public PRelation UpdateSystemRelation(PRelation pRelation)
            {
                throw new NotImplementedException();
            }
        }
        protected class QueryService : IQueryService
        {
            private static Object synclock = new Object();
            private static QueryService instance;
            protected QueryService()
            {

            }
            public static QueryService Instance()
            {
                lock (synclock)
                {
                    if (instance == null) instance = new QueryService();
                }
                return instance;
            }

            public bool DeleteAllQuery()
            {
                try
                {
                    if (!(ConcreteDb.Instance.Update("DELETE FROM SystemQuery") > 0))
                        throw new Exception(ConcreteDb.Instance.errorMessage);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ContantCls.lblNotice);
                    return false;
                }
                return true;
            }

            public bool DeleteById(PQuery pQuery)
            {
                try
                {
                    if (!(ConcreteDb.Instance.Update("DELETE FROM SystemQuery where ID = " + pQuery.IDQuery) > 0))
                        throw new Exception(ConcreteDb.Instance.errorMessage);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ContantCls.lblNotice, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                return true;
            }

            public void Dispose()
            {
                this.Dispose();
            }

            public IList<PQuery> getAllQuery()
            {
                var probQueries = new List<PQuery>();
                DataSet dts = dts = new DataSet();
                try
                {
                    dts.Tables.Add(ConcreteDb.Instance.getDataTable("SELECT * FROM SystemQuery", "system_query"));

                    foreach (DataRow queryRow in dts.Tables["system_query"].Rows)
                    {
                        var NewQuery = new PQuery();
                        NewQuery.IDQuery = Convert.ToInt16(queryRow[0].ToString());
                        NewQuery.QueryName = queryRow[1].ToString();
                        if (queryRow[2].ToString() != "Empty")
                            NewQuery.QueryString = queryRow[2].ToString();
                        else
                            NewQuery.QueryString = "";
                        probQueries.Add(NewQuery);
                    }
                }
                catch
                {

                }

                return probQueries;
            }

            public IList<PQuery> getQueryByName(PQuery pQuery)
            {
                var dts = new DataSet();
                dts.Tables.Add(ConcreteDb.Instance.getDataTable("SELECT * FROM SystemQuery where QueryName = '" + pQuery.QueryName + "'", "system_query"));

                var queryList = new List<PQuery>();
                foreach (DataRow queryRow in dts.Tables["system_query"].Rows)
                {
                    var newQuery = new PQuery();
                    newQuery.IDQuery = Convert.ToInt16(queryRow[0].ToString());
                    newQuery.QueryName = queryRow[1].ToString();
                    newQuery.QueryString = queryRow[2].ToString();
                    queryList.Add(newQuery);
                }
                return queryList;
            }

            public bool Insert(PQuery pQuery)
            {
                try
                {


                    if (pQuery.IDQuery == -1)
                    {
                        pQuery.IDQuery = ConcreteDb.Instance.getNextSysId("SystemQuery");
                    }

                    string SQL = "";

                    if (pQuery.QueryString == "")
                        pQuery.QueryString = "Empty";

                    SQL += "INSERT INTO SystemQuery VALUES (";
                    SQL += pQuery.IDQuery + ",";
                    SQL += "'" + pQuery.QueryName + "'" + ",";
                    SQL += "'" + pQuery.QueryString + "'";
                    SQL += " );";


                    if (!(ConcreteDb.Instance.Update(SQL) > 0))
                        throw new Exception(ConcreteDb.Instance.errorMessage);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Notification");
                    return false;
                }
                return true;
            }

            public int Update(PQuery pQuery)
            {
                string SQL = "";
                SQL += "Update SystemQuery  SET ";
                SQL += " QueryName  = '" + pQuery.QueryName + "'";
                SQL += " , QueryString = '" + pQuery.QueryString.Replace("'", "''") + "'";
                SQL += " Where  ID = " + pQuery.IDQuery;

                return ConcreteDb.Instance.Update(SQL);
            }
        }
        protected class PTupleService : ITupleService
        {
            private static PTupleService instance;
            private PTupleService()
            {

            }
            public static PTupleService Instance()
            {
                if (instance == null) instance = new PTupleService();
                return instance;
            }
            public bool DeleteTupleById(PRelation pRelation, PTuple pTuple)
            {
 
                    try
                    {
                        var strPriAtt = pRelation.schema.Attributes.Where(att => att.primaryKey).First().ToString().Trim();
                        var pri = String.Format("{0}.{1}", pRelation.relationName, strPriAtt);
                        var tupID = pTuple.valueSet[pri].FirstOrDefault().Trim();
                        //del in db
                        var sql = "";
                        sql += String.Format("DELETE FROM {0} ", pRelation.relationName.ToUpper());
                        sql += String.Format("WHERE {0} = '{1}'", strPriAtt, "{ " + tupID + " }");
                        //del in ram
                        SystemParam.StaticParams.currentDb.Relations.Where(p => p.id == pRelation.id).First().tupes.Remove(pTuple);
                        if (ConcreteDb.Instance.Update(sql) < 0)
                            throw new Exception(ConcreteDb.Instance.errorMessage);

                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                    return true;
            }

            public void Dispose()
            {
                this.Dispose();
            }

            public IList<PTuple> getAllTupleByRelationName(string relationName, IList<PAttribute> pAttributes)
            {
                relationName = relationName.Trim().ToLower();
                var result = new List<PTuple>();
                var dtb = ConcreteDb.Instance.getDataTable("Select * From " + relationName);
                if (dtb is null) return null;

                foreach (DataRow row in dtb.Rows)
                {
                    var newRecord = new PTuple();

                    //id Attr is null
                    var i = 0;
                    foreach (var item in pAttributes)
                    {
                        if (item.AttributeName.Equals(ContantCls.emlementProb))
                            newRecord.Ps = new ElemProb(row[i++].ToString());
                        else
                            newRecord.valueSet.Add(String.Format("{0}.{1}", relationName, item.AttributeName), getData(row[i++].ToString(), item.Type, item.primaryKey));
                    }
                    result.Add(newRecord);

                }
                return result.Count > 0 ? result : null;
            }
            public IList<String> getData(string rawValue, PDataType type, bool priKey)
            {
                //checkFormat data
                try
                {
                    if (!checkDataFormat(rawValue)) throw new Exception("Wrong Format's Data!");
                    //fomat handle {x,y,z}
                    rawValue = rawValue.Trim();
                    rawValue = rawValue.Replace("{", "");
                    rawValue = rawValue.Replace("}", "");
                    rawValue = rawValue.Trim();
                    if (priKey && rawValue.Contains(",")) return new List<string>() { rawValue.Split(',').ToList().FirstOrDefault().Trim() };
                    return rawValue.Contains(",") ? rawValue.Split(',').ToList().Select(p => p.Trim()).ToList() : new List<string>() { rawValue.Trim() };
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ContantCls.lblNotice, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return null;
                }
            }

            public bool checkDataFormat(string rawValue)
            {
                return true;
            }

            public object getValuebyType(string rawValue, PDataType type)
            {

                var result = new Object();
                try
                {
                    switch (type.TypeName)
                    {
                        case "Int16": result = Convert.ToInt16(rawValue); break;
                        case "Int64": result = Convert.ToInt64(rawValue); break;
                        case "Int32": result = Convert.ToInt32(rawValue); break;
                        case "Byte": result = Convert.ToByte(rawValue); break;
                        case "Decimal": result = Convert.ToDecimal(rawValue); break;

                        case "String": result = Convert.ToString(rawValue); break;
                        case "DateTime": result = Convert.ToDateTime(rawValue); break;

                        case "Single": result = Convert.ToSingle(rawValue); break;
                        case "Double": result = Convert.ToDouble(rawValue); break;
                        case "Boolean": result = Convert.ToBoolean(rawValue); break;
                        case "Currency":; break;
                        case "Binary":; break;
                        default: break;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Notification", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                finally
                {
                    result = type.CheckDataTypeOfVariables(rawValue) ? result : "Unknown Value";

                }
                return result;
            }

            public PTuple Insert(PTuple pTuple, PRelation pRelation)
            {
                var sql = "";
                sql += String.Format("INSERT INTO {0} ", pRelation.relationName.ToLower().Trim());
                var atrrs = String.Join(",", pRelation.schema.Attributes.Select(a => a.AttributeName.Trim().ToLower()).ToArray());
                sql += String.Format("({0}) VALUES", atrrs);
                //value list
                var vallist = new List<String>();
                foreach (var key in pTuple.valueSet.Keys)
                {
                    if (!key.Substring(key.IndexOf(".") + 1).Equals(ContantCls.emlementProb, StringComparison.CurrentCultureIgnoreCase))
                    {
                        var vals = String.Join(",", pTuple.valueSet[key].ToArray());
                        vallist.Add("'" + vals + "'");
                    }
                }
                vallist.Add("'"+pTuple.Ps.ToString()+"'");
                String strVal = String.Join(",", vallist.ToArray());
                sql += $"({strVal})";
                try
                {
                    //in db
                    ConcreteDb.Instance.Update(sql);
                    //in ram
                    SystemParam.StaticParams.currentDb.Relations.Where(p => p.id == pRelation.id).First().tupes.Add(pTuple) ;
                }
                catch (Exception ex)
                {
                    return null;
                }
                return pTuple;
            }

            public PTuple Update(PTuple pTuple, PRelation pRelation, String key)
            {
                try
                {
                    if (key.Substring(key.IndexOf(".") + 1).Equals(ContantCls.emlementProb, StringComparison.CurrentCultureIgnoreCase))
                {
                    var priName = pRelation.schema.Attributes.Where(a => a.primaryKey).Select(p => p.AttributeName).Single().Trim().ToLower();
                    var standard = key.IndexOf(".") == -1 ? key : key.Substring(key.IndexOf(".") + 1).Trim();
                    var sql = "";
                    sql += String.Format("UPDATE {0} SET ", pRelation.relationName.ToLower().Trim());
                    sql += String.Format("{0} = ", standard);
                    //note
                    sql += "' " + pTuple.Ps.ToString() + " '";
                    priName = String.Format("{0}.{1}", pRelation.relationName, priName);
                    var priVal = pTuple.valueSet[priName].First().Trim();
                    sql += String.Format(" WHERE {0} = ", priName);
                    //note
                    sql += "'{ " + priVal + " }'";
                        //in db
                        ConcreteDb.Instance.Update(sql);
                        //in ram
                        var relation = SystemParam.StaticParams.currentDb.Relations.Where(p => p.id == pRelation.id).First();
                        var tuple = relation.tupes.Where(t => t.valueSet[priName].First().Equals(pTuple.valueSet[priName].First(), StringComparison.CurrentCultureIgnoreCase)).First();
                        tuple = pTuple;
                    }
                else
                {
                    var priName = pRelation.schema.Attributes.Where(a => a.primaryKey).Select(p => p.AttributeName).Single().Trim().ToLower();
                    var standard = key.IndexOf(".") == -1 ? key : key.Substring(key.IndexOf(".") + 1).Trim();
                    var sql = "";
                    sql += String.Format("UPDATE {0} SET ", pRelation.relationName.ToLower().Trim());
                    var vals = String.Join(" , ", pTuple.valueSet[key].ToArray());
                    sql += String.Format("{0} = ", standard);
                    //note
                    sql += "'{ " + vals + " }'";
                    priName = String.Format("{0}.{1}", pRelation.relationName, priName);
                    var priVal = pTuple.valueSet[priName].First().Trim();
                    sql += String.Format(" WHERE {0} = ", priName);
                    //note
                    sql += "'{ " + priVal + " }'";
                        //in db
                        ConcreteDb.Instance.Update(sql);
                        //in ram
                        var relation = SystemParam.StaticParams.currentDb.Relations.Where(p => p.id == pRelation.id).First();
                        var tuple = relation.tupes.Where(t => t.valueSet[priName].First().Equals(pTuple.valueSet[priName].First(), StringComparison.CurrentCultureIgnoreCase)).First();
                        tuple = pTuple;
                    }
               
                   
                }
                catch (Exception ex)
                {
                    return null;
                }
                return pTuple;
            }

            public PTuple insertEmptyTuple(PRelation pRelation,PAttribute pri,String IdTuple)
            {
                var relation = new PRelation() { id = pRelation.id };
                    
                var tupID = String.Empty;
                //insert empty tuple
                try
                {
                    var tuple = RawDatabaseService.Instance().GetTuplebyId(ref relation, tupID);
                    tuple.valueSet[$"{relation.relationName.ToLower()}.{pri.AttributeName.ToLower()}"] = new List<String>() { "{ "+ IdTuple+" }" };

                    foreach (var key in tuple.valueSet.Keys.ToList())
                    {
                        var atr = key.Substring(key.IndexOf(".") + 1);
                        var Pri = $"{relation.relationName.ToLower()}.{pri.AttributeName.ToLower()}";
                        if (!atr.Equals(ContantCls.emlementProb, StringComparison.CurrentCultureIgnoreCase) &&
                            key != Pri)
                        {
                            tuple.valueSet[key] = new List<String>() { "{ Insert Data }" };
                        }
                    }
                    PTupleService.Instance().Insert(tuple, relation);
                    return tuple;
                }
                catch(Exception Ex)
                {
                    return null;
                }

            }

            public string getStrTupleId(PRelation pRelation)
            {
                var pri = pRelation.schema.Attributes.Where(p => p.primaryKey).FirstOrDefault();
                var sql = String.Empty;
                int i = 0;
                //if(pri != null)
                //{

                //    while (true)
                //    {
                //        var count = ConcreteDb.Instance.ExecuteScalar($"SELECT * FROM {pRelation.relationName.ToLower()} WHERE {pri.AttributeName.ToLower()} = '{}'"));
                //        if (count == 0d) break;
                //        i++;
                //    }
                //}
                return "";
            }
        }
    }
}

