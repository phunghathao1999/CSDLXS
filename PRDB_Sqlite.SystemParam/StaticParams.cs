using PRDB_Sqlite.Domain.Model;
using System.Windows.Forms;

namespace PRDB_Sqlite.SystemParam
{
    public class StaticParams
    {
        public static PDatabase currentDb { get; set; }
        public static TreeView currentTreView { get; set; }
    }
}
