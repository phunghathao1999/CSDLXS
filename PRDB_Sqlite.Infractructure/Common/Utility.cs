using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PRDB_Sqlite.Infractructure.Common
{
    public class Utility
    {
        MessageBox mesbox;
        // Lock synchronization object

        private static object syncLock = new object();
        //singleton
        private static Utility instance;
        public static Utility Instance()
        {
            lock (syncLock)
            {
                if (instance == null) instance = new Utility();
            }
            return instance; 
        }

        protected Utility()
        {
            this.mesbox = null;
        }
        public string CutExtension(string name)
        {
            for (int i = name.Length - 1; i >= 0; i--)
                if (name[i] == '.')
                {
                    name = name.Remove(i);
                    break;
                }
            return name;
        }
        public string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
        public string getPathDialog(string path)
        {
            var temp = path;
            var lst = temp.Split('\\').ToList();
            lst.Remove(lst.LastOrDefault());
            lst.Remove(lst.LastOrDefault());
            lst.Remove(lst.LastOrDefault());
            temp = String.Empty ;
            foreach (var item in lst)
            {
                temp += (item.ToString()) + '\\';
            }      
            return temp;
        }
        public string GetRootPath(string path)
        {
            
            string root = "";
            try
            {

                for (int i = 0; i < path.Length; i++)
                    if (path[i] == '\\')
                    {
                        root = path.Substring(0, i + 1);
                        break;
                    }
                return root;

            }
            catch (Exception)
            {


            }
            return root;

        }
        public static dynamic dictionaryToObject(IDictionary<string, object> dict)
        {
            IDictionary<string, object> eo = (IDictionary<string, object>)new ExpandoObject();
            foreach (KeyValuePair<string, object> kvp in dict)
            {
                eo.Add(kvp);
            }
            return eo;
        }
    }
}
