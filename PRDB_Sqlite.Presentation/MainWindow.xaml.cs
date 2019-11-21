using PRDB_Sqlite.Domain.Model;
using PRDB_Sqlite.Infractructure.Common;
using PRDB_Sqlite.Presentation.Module;
using PRDB_Sqlite.Presentation.Screen;
using PRDB_Sqlite.SystemParam;
using System;
using System.Configuration;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace PRDB_Sqlite.Presentation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        //Data Source = D:\uni\2018-2019\KHOALUANTOTNGHIEP\Project PRDB\PRDB_Sqlite\PRDB_Sqlite.Presentation\CLINIC_DATABASE.pdb;Version=3;
        // public static PDatabase currentDb = null;
        //public static PDatabase currentDb;
        public Thread bindingTimer;
        public static TabItem tab_tmp = new TabItem();
        public MainWindow()
        {

            InitializeComponent();
            #region init
            Parameter.SchemaIndex = 0;
            Parameter.RelationIndex = 0;
            #endregion
            if (ConfigurationManager.AppSettings["devmode"].ToString().Contains("1"))
                StaticParams.currentDb = MdlFileDialog.Instance().OpenDialogGetPDb();
            else
                StaticParams.currentDb = null;
                
            Refresh();
            //setAutoBinding();
        }
        private void setAutoBinding()
        {

            {
                if (Parameter.indexRelChange)
                {   
                    Refresh();
                }
                if (Parameter.indexSchChange)
                {
                    Refresh();
                }
            }

        }
        private void reloadDb()
        {
           StaticParams.currentDb = MdlFileDialog.Instance().OpenDialogGetPDb(true);
           Refresh();
        }

        private void Refresh()
        {
            //load Main interface 
            if (StaticParams.currentDb is null)
            {
                //load Header
                loadHeader(false);

                //load leftContent
                tvLeftNode.Items.Clear();
                tvLeftNode.Items.Add(new TreeViewItem() { Header = "There is no DataBase", FontSize = 14f });
                //load rightContent
            }
            else
            {
                //load Header
                loadHeader(true);

                //load leftContent
                tvLeftNode.Items.Clear();
                tvLeftNode.Items.Add(MdlTreeView.Instance(StaticParams.currentDb).getTreeViewItemFromDb());

                //load rightContent
                this.tbMainTab.SelectedIndex = Parameter.activeTabIdx;
                MdlRContent.Instance(StaticParams.currentDb).getTabByUid(ref this.tbiSch);
                MdlRContent.Instance(StaticParams.currentDb).getTabByUid(ref this.tbiRel);
                MdlRContent.Instance(StaticParams.currentDb).getTabByUid(ref this.tbiQry);
            }
        }
        //false == none Db
        private void loadHeader(bool v)
        {
           

            if (v)
            {
                //config options

                //database
                this.rgClsDb.Visibility = Visibility.Visible;
                this.rgSaveDb.Visibility = Visibility.Visible;
                //schema
                this.rgNewSch.Visibility = Visibility.Visible;
                this.rgOpnSch.Visibility = Visibility.Visible;
                this.rgEdtSch.Visibility = Visibility.Visible;
                this.rgDelSch.Visibility = Visibility.Visible;
                //relation
                this.rgNewRel.Visibility = Visibility.Visible;
                this.rgOpnRel.Visibility = Visibility.Visible;
                this.rgDelRel.Visibility = Visibility.Visible;
                //query

            }
            //this.tbtHeader.ToolBars.Add(tbDb);

        }

        private void btnNewDb_Click(object sender, EventArgs e)
        {
            SystemParam.StaticParams.currentDb = MdlFileDialog.Instance().CreateNewDb();
            //currentDb = SystemParam.StaticParams.currentDb;
            Refresh();
        }
        private void btnOpenDb_Click(object sender, EventArgs e)
        {
            SystemParam.StaticParams.currentDb = MdlFileDialog.Instance().OpenDialogGetPDb();
            //currentDb = SystemParam.StaticParams.currentDb;
            Refresh();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //this.bindingTimer.Dispose();
        }
        public static void resetTab(string stp)
        {
            switch (stp.ToLower())
            {
                //case "sch": MdlRContent.Instance(currentDb).getTabByUid(ref this.tbiSch);break;
                //case "rel": MdlRContent.Instance(currentDb).getTabByUid(ref this.tbiRel);break;
                default:break;
            }
        }

        private void btnNewSch_Click(object sender, RoutedEventArgs e)
        {
            var newSchForm = new AddingSchema();
            newSchForm.ShowDialog();
            this.Refresh();

        }

        private void btnNewRel_Click(object sender, RoutedEventArgs e)
        {
            var newRelForm = new AddingNewRelation();
            newRelForm.ShowDialog();
            this.Refresh();
        }

        private void btnOpnSch_Click(object sender, RoutedEventArgs e)
        {
            var newOpnSch = new opn_del_Schema("opnsch");
            newOpnSch.ShowDialog();
            this.Refresh();

        }

        private void btnEdtSch_Click(object sender, RoutedEventArgs e)
        {
            var newEditSch = new EditingSchema();
            newEditSch.ShowDialog();
            this.Refresh();
        }
        private void btnDelSch_Click(object sender, RoutedEventArgs e)
        {
            var newOpnSch = new opn_del_Schema("delsch");
            newOpnSch.ShowDialog();
            this.Refresh();
        }

        private void btnOpnRel_Click(object sender, RoutedEventArgs e)
        {
            var newOpnSch = new opn_del_Schema("opnrel");
            newOpnSch.ShowDialog();
            this.Refresh();
        }

        private void btnDelRel_Click(object sender, RoutedEventArgs e)
        {
            var newOpnSch = new opn_del_Schema("delrel");
            newOpnSch.ShowDialog();
            this.Refresh();
        }

   

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnReload_Click(object sender, RoutedEventArgs e)
        {
            this.reloadDb();
            
        }

    }
}
