using Caliburn.Micro;
using PRDB_Sqlite.Infractructure.Common;
using PRDB_Sqlite.Presentation.Module;
using PRDB_Sqlite.Presentation.Screen;
using PRDB_Sqlite.Sevice.SysService;
using PRDB_Sqlite.SystemParam;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
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
                    tvLeftNode.Items.Add(MdlTreeView.Instance().getTreeViewItemFromDb());
                }
                //load rightContent
                this.tbMainTab.SelectedIndex = Parameter.activeTabIdx;
                MdlRContent.Instance().getTabByUid(ref this.tbiSch);
                MdlRContent.Instance().getTabByUid(ref this.tbiRel);
                MdlRContent.Instance().getTabByUid(ref this.tbiQry);
            }

            //this.cbxStrategy.ItemsSource = Parameter.strategies;
            //this.cbxStrategy.Items = true;
            //this.NumberTextBox.Text = Parameter.eulerThreshold.ToString();
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
                this.btnNewSch.Visibility = Visibility.Visible;
                this.btnEdtSch.Visibility = Visibility.Visible;
                this.btnOpnSch.Visibility = Visibility.Visible;
                this.btnDelSch.Visibility = Visibility.Visible;
                //relation
                this.btnNewRel.Visibility = Visibility.Visible;
                this.btnOpnRel.Visibility = Visibility.Visible;
                this.btnDelRel.Visibility = Visibility.Visible;
                this.btnIns.Visibility = Visibility.Visible;
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
            Regex regex = new Regex("[^0-9.]+");
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

        private void Set_Click(object sender, RoutedEventArgs e)
        {
            
            MenuItem item = e.Source as MenuItem;
            Parameter.curStrategy = item.Header.ToString();
            Console.WriteLine("item.Items.CurrentItem.ToString(): " + item.Header.ToString());
            Console.WriteLine("Main: " + Parameter.curStrategy);
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
                if (idList != null)
                    foreach (var id in idList)
                        RawDatabaseService.Instance().insertEmptyTuple(StaticParams.currentRelation, pri, id.IDtuple);
                Parameter.activeTabIdx = 1;
                this.reloadDb();
            }

        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            Parameter.activeTabIdx = 1;
            this.reloadDb();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            //this.cbxSetOpr.SelectionChanged += (s, evt) =>
            //{
            //    Parameter.curStrategy_case = this.cbxSetOpr.SelectedItem.ToString();
            //};

        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.RightAlt:
                    reloadDb();
                    break;
                default: break;
            }
        }

        private void File_DpiChanged(object sender, DpiChangedEventArgs e)
        {

        }

        public class MenuItemViewModel
        {
            private readonly ICommand _command;

            public MenuItemViewModel()
            {
                _command = new CommandViewModel(Execute);
            }

            public string Header { get; set; }

            public ObservableCollection<MenuItemViewModel> MenuItems { get; set; }

            public ICommand Command
            {
                get
                {
                    return _command;
                }
            }

            private void Execute()
            {
                // (NOTE: In a view model, you normally should not use MessageBox.Show()).
                MessageBox.Show("Clicked at " + Header);
            }
        }

        public class CommandViewModel : ICommand
        {
            private readonly Action _action;

            public CommandViewModel(Action action)
            {
                _action = action;
            }

            public void Execute(object o)
            {
                _action();
            }

            public bool CanExecute(object o)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged
            {
                add { }
                remove { }
            }
        }
    }
}
