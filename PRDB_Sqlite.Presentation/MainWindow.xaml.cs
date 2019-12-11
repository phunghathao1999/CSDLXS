using PRDB_Sqlite.Infractructure.Common;
using PRDB_Sqlite.Presentation.Module;
using PRDB_Sqlite.Presentation.Screen;
using PRDB_Sqlite.Sevice.SysService;
using PRDB_Sqlite.SystemParam;
using System;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
                if (Parameter.indexRelChange)
                {
                    Refresh();
                }
                if (Parameter.indexSchChange)
                {
                    Refresh();
                }
        }
        private void reloadDb()
        {
            StaticParams.currentDb = MdlFileDialog.Instance().OpenDialogGetPDb(true);
            Refresh();
        }

        private void Refresh(bool loadLeft = true)
        {
            //load Main interface 
            if (StaticParams.currentDb is null)
            {
                //load Header
                loadHeader(false);

                //load leftContent
                if (loadLeft)
                {
                    tvLeftNode.Items.Clear();
                    tvLeftNode.Items.Add(new TreeViewItem() { Header = "There is no DataBase", FontSize = 14f });
                }
                //load rightContent
            }
            else
            {
                //load Header
                loadHeader(true);

                //load leftContent
                if (loadLeft)
                {
                    tvLeftNode.Items.Clear();
                    tvLeftNode.Items.Add(MdlTreeView.Instance(StaticParams.currentDb).getTreeViewItemFromDb());
                }
                //load rightContent
                this.tbMainTab.SelectedIndex = Parameter.activeTabIdx;
                MdlRContent.Instance(StaticParams.currentDb).getTabByUid(ref this.tbiSch);
                MdlRContent.Instance(StaticParams.currentDb).getTabByUid(ref this.tbiRel);
                MdlRContent.Instance(StaticParams.currentDb).getTabByUid(ref this.tbiQry);
            }

            this.cbxStrategy.ItemsSource = Parameter.strategies;
            this.NumberTextBox.Text = Parameter.eulerThreshold.ToString();
        }
        //false == none Db
        private void loadHeader(bool v)
        {
            
            if (v)
            {
                //config options
                //database
                //this.rgClsDb.Visibility = Visibility.Visible;
                //this.rgSaveDb.Visibility = Visibility.Visible;
                //schema
                this.rgNewSch.Visibility = Visibility.Visible;
                this.rgOpnSch.Visibility = Visibility.Visible;
                this.rgEdtSch.Visibility = Visibility.Visible;
                this.rgDelSch.Visibility = Visibility.Visible;
                //relation
                this.rgNewRel.Visibility = Visibility.Visible;
                this.rgOpnRel.Visibility = Visibility.Visible;
                this.rgDelRel.Visibility = Visibility.Visible;
                this.rgTup.Visibility = Visibility.Visible;
                //query

            }
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
                //case "sch": MdlRContent.Instance(currentDb).getTabByUid(ref this.tbiSch); break;
                //case "rel": MdlRContent.Instance(currentDb).getTabByUid(ref this.tbiRel); break;
                default: break;
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

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            var oldVal = (sender as TextBox).Text;
            //Regex regex = new Regex("[+]?([0-1]*[.])?[0-9]+");
            e.Handled = (regex.IsMatch(e.Text));
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnReload_Click(object sender, RoutedEventArgs e)
        {
            this.reloadDb();
        }

        private float? checkEuler(string str)
        {
            float num;
            if (float.TryParse(str, out num))
                if (num >= 0 && num <= 1)
                    return num;
            return null;
        }

        private void loadCurE_Click(object sender, RoutedEventArgs e)
        {
            this.NumberTextBox.Text = Parameter.eulerThreshold.ToString();
        }

        private void Set_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(this.NumberTextBox.Text))
                loadCurE_Click(sender, e);
            var num = checkEuler(this.NumberTextBox.Text);
            if (num != null)
                Parameter.eulerThreshold = (float)num;
            else
                MessageBox.Show("Invalid Euler Threshold", "NOtification", MessageBoxButton.OK, MessageBoxImage.Information);
            Parameter.curStrategy = this.cbxStrategy.SelectedItem.ToString();
        }

        private void tvLeftNode_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.Refresh(false);
        }

        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            var showAbout = new About();
            showAbout.ShowDialog();
            this.Refresh();
        }
        


        private void tbiQry_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Left)
            {
                if (Parameter.resetMainF)
                {
                    this.reloadDb();
                    Parameter.resetMainF = false;
                }
            }
        }

        private void btnIns_Click(object sender, RoutedEventArgs e)
        {
            Parameter.activeTabIdx = 1;
            addingNewTuple();
            this.reloadDb();
        }

        private void addingNewTuple()
        {

            if (StaticParams.currentRelation != null)
            {
                var addIdform = new addingTuples();
                addIdform.ShowDialog();
                var idList = addIdform.idTupleList;
                var pri = StaticParams.currentRelation.schema.Attributes.Where(a => a.primaryKey).FirstOrDefault();
                var priAtr = $"{StaticParams.currentRelation.relationName.ToLower()}.{pri.AttributeName.ToLower()}";
                foreach (var id in idList)
                {
                    var tup = RawDatabaseService.Instance().insertEmptyTuple(StaticParams.currentRelation,pri,id.IDtuple);
                }
                Parameter.activeTabIdx = 1;
                this.reloadDb();
            }

        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            Parameter.activeTabIdx = 1;
            this.reloadDb();
        }
    }
}
