using System;
using System.Collections.Generic;
using System.Configuration;

namespace PRDB_Sqlite.Infractructure.Common
{
    public class Parameter
    {
        #region Tra_Prop
        private static string _connectionString;
        private static string _curStrategy;
        private static string _curStrategy_case;

        #endregion
        #region Tra_get_seter
        public static string connectionString
        {
            get => string.IsNullOrEmpty(_connectionString) ? ConfigurationManager.AppSettings["conectionString"].ToString() : _connectionString;
            set => _connectionString = value;
        }
    
        public static string curStrategy
        {
            set
            {
                _curStrategy = value;
            }
            get
            {
                if (string.IsNullOrEmpty(_curStrategy))
                    return "⊗_in";
                return _curStrategy;
            }
        }
        public static string curStrategy_case
        {
            set
            {
                _curStrategy_case = value;
            }
            get
            {
                if (string.IsNullOrEmpty(_curStrategy_case))
                    return "in";
                return _curStrategy_case;
            }
        }

        #endregion
        #region param presentation
        public static bool indexSchChange { get; set; }
        public static bool indexRelChange { get; set; }
        public static int SchemaIndex { get; set; }
        public static int RelationIndex { get; set; }
        public static int activeTabIdx { get; set; }
        public static bool resetMainF { get; set; }
        public static int idLength { get => 3; }
        public static int currentColumn { get; set; }
        public static int currentRow { get; set; }

        public static IList<string> datatype
        {
            get
            {
                return new List<string>()
                {
                    "Int16",
                    "Int32",
                    "Int64",
                    "Byte",
                    "String",
                    "DateTime",
                    "Decimal",
                    "Single",
                    "Double",
                    "Boolean",
                    "Binary",
                    "Currency",
                    "UserDefined"
                };
            }
        }
        public static IList<string> strategies
        {
            get
            {
                return new List<string> {
                    "⊗_in", "⊗_ig", "⊗_me","⊕_in", "⊕_ig", "⊕_me", "⊖_ig", "⊖_in", "⊖_pc"
                };
            }
        }
        public static IList<string> strategies_case
        {
            get
            {
                return new List<string> {
                    "in", "ig", "me"
                };
            }
        }
        #endregion
    }
}
