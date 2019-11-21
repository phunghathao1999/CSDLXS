using PRDB_Sqlite.Domain.Interface.Service;
using PRDB_Sqlite.Domain.Model;
using PRDB_Sqlite.Infractructure.Common;
using PRDB_Sqlite.Sevice.Factory;
using System;
using System.Configuration;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;
using Utility = PRDB_Sqlite.Infractructure.Common.Utility;

namespace PRDB_Sqlite.Presentation.Module
{
    public class MdlFileDialog
    {
        private static object syncLock = new object();
        private static MdlFileDialog instance;
        private System.Windows.Forms.FileDialog dialog;
        private PDatabase pDatabase;
        private IDbService dbService;
        public static MdlFileDialog Instance()
        {
            if (instance == null)
            {
                lock (syncLock)
                {
                    instance = new MdlFileDialog();
                }
            }
            return instance;
        }

        protected MdlFileDialog()
        {
            dbService = DbFactory.GetDatabaseService();
            this.pDatabase = null;
            this.dialog = null;
        }
        protected void setupDialog(bool isNewDb = false)
        {
            try
            {
                if (this.dialog != null)
                    this.dialog.Dispose();

                if (!isNewDb)
                {
                    this.dialog = new System.Windows.Forms.OpenFileDialog();
                    dialog.DefaultExt = "pdb";
                    dialog.Filter = "Database file (*.pdb)|*.pdb";
                    dialog.AddExtension = true;
                    dialog.RestoreDirectory = true;
                    //co the se loi
                    dialog.InitialDirectory = Utility.Instance().getPathDialog(AppDomain.CurrentDomain.BaseDirectory.ToString());
                    dialog.SupportMultiDottedExtensions = true;
                }
                else
                {
                    this.dialog = new System.Windows.Forms.SaveFileDialog();                                   // Save dialog
                    this.dialog.DefaultExt = "pdb";                                                      // Default extension
                    this.dialog.Filter = "Database file (*.pdb)|*.pdb";                                  // add extension to dialog
                                                                                                         //DialogSave.Filter = "Database file (*.pdb)|*.pdb|All files (*.*)|*.*";               
                    this.dialog.AddExtension = true;                                                     // enable adding extension
                    this.dialog.RestoreDirectory = true;                                                 // Tu dong phuc hoi duong dan cho lan sau
                    this.dialog.Title = "Create new database...";
                    this.dialog.InitialDirectory = Utility.Instance().getPathDialog(AppDomain.CurrentDomain.BaseDirectory.ToString());
                    this.dialog.SupportMultiDottedExtensions = true;

                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        public PDatabase OpenDialogGetPDb(bool reload = false)
        {
            this.setupDialog();

            if (reload)
            {
                if (String.IsNullOrEmpty(this.pDatabase.ConnectString))
                    MessageBox.Show("Can not Reload Db", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Parameter.connectionString = this.pDatabase.ConnectString;
                if (this.dbService.OpenExistingDatabase(ref this.pDatabase) == null)
                {
                    MessageBox.Show("Error : Cannot connect to the database, please try again!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            #region forDebug

            if (ConfigurationManager.AppSettings["devmode"].ToString().Contains("1"))
            {
                //this.pDatabase = new PDatabase(@"D:\uni\2018-2019\KHOALUANTOTNGHIEP\Project PRDB\PRDB_Sqlite\PRDB_Sqlite.Presentation\CLINIC_DATABASE.pdb");
                var constr = AppDomain.CurrentDomain.BaseDirectory + @"CLINIC_DATABASE.pdb";
                this.pDatabase = new PDatabase(constr);
                Parameter.connectionString = this.pDatabase.ConnectString;
                if (this.dbService.OpenExistingDatabase(ref this.pDatabase) == null)
                {
                    MessageBox.Show("Error : Cannot connect to the database, please try again!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            #endregion

            else
            {
                if (this.dialog.ShowDialog() == DialogResult.OK)
                {

                    this.pDatabase = new PDatabase(this.dialog.FileName);
                    Parameter.connectionString = this.pDatabase.ConnectString;
                    if (this.dbService.OpenExistingDatabase(ref this.pDatabase) == null)
                    {
                        MessageBox.Show("Error : Cannot connect to the database, please try again!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                this.dialog.Dispose();
            }
            return (this.pDatabase != null) ? this.pDatabase : null;
        }
        public PDatabase CreateNewDb()
        {
            this.setupDialog(isNewDb: true);

            if (this.dialog.ShowDialog() == DialogResult.OK)
            {
                this.pDatabase = new PDatabase(dialog.FileName);
                Parameter.connectionString = pDatabase.ConnectString;
                if (this.dbService.CreateNewDatabase(ref this.pDatabase) == null)
                {
                    MessageBox.Show("Error : Cannot create new database, please try again!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            this.dialog.Dispose();
            return (this.pDatabase != null) ? this.pDatabase : null;

        }
    }
}
