using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using PRDB_Sqlite.GUI;

namespace PRDB_Sqlite
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            DevExpress.UserSkins.OfficeSkins.Register();
            DevExpress.UserSkins.BonusSkins.Register();
            DevExpress.Skins.SkinManager.Default.RegisterAssembly(typeof(DevExpress.UserSkins.OfficeSkins).Assembly);

            Application.SetCompatibleTextRenderingDefault(false);
            //var a = new Dictionary<string, dynamic>();
            //a.Add("string", "isString");
            //a.Add("number", 123);
            //a.Add("float", 1232.4);
            //MessageBox.Show(a.ElementAt("string");
            Application.Run(new Form_Main());
        }
    }
}
