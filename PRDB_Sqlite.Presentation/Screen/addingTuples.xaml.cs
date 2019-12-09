using PRDB_Sqlite.Domain.ModelView;
using PRDB_Sqlite.Infractructure.Common;
using PRDB_Sqlite.Sevice.SysService;
using PRDB_Sqlite.SystemParam;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PRDB_Sqlite.Presentation.Screen
{
    /// <summary>
    /// Interaction logic for addingTuples.xaml
    /// </summary>
    public partial class addingTuples : Window
    {
        public ObservableCollection<IDTupMView> idTupleList = new ObservableCollection<IDTupMView>();

        public addingTuples()
        {
            InitializeComponent();
            buildDtg();
            this.lblRelation.Content = $"INSERT INTO {StaticParams.currentRelation.relationName.ToUpper()}";
        }


        private void buildDtg()
        {
            this.dtg.AutoGenerateColumns = false;
            this.dtg.CanUserAddRows = false;
            this.dtg.CanUserDeleteRows = true;
            this.dtg.IsReadOnly = false;
            this.dtg.ItemsSource = this.idTupleList;
        }

        private void btnCancle_Click(object sender, RoutedEventArgs e)
        {
            this.idTupleList = null;
            this.Close();
        }

        private void btnCommit_Click(object sender, RoutedEventArgs e)
        {
            this.dtg.CommitEdit();

            foreach (var item in this.idTupleList.ToList())
            {
                if(String.IsNullOrEmpty(item.IDtuple))
                {
                    MessageBox.Show("ID Tuple must not be NULL", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (!checkDataType(item.IDtuple))
                {
                    MessageBox.Show("DataType is invalid", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if(!(item.IDtuple.Length >= Parameter.idLength))
                {
                    MessageBox.Show("Length's ID is too Short!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (checkExitID(item.IDtuple))
                {
                    MessageBox.Show($"ID: {item.IDtuple} has already existed in {StaticParams.currentRelation.relationName.ToUpper()}!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            //duplicate id
            foreach (var item1 in this.idTupleList.ToList())
            {
                foreach (var item2 in this.idTupleList.ToList())
                {
                    if (item1 == item2) continue;
                    if(item1.IDtuple == item2.IDtuple)
                    {
                        MessageBox.Show("There is the duplicate ID!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    
                 }
            }
            this.Close();
        }

        private bool checkExitID(string dtuple) //false: dosenot exist | true: existed
        {
            var rel = new Domain.Model.PRelation() { id = StaticParams.currentRelation.id };
            try
            {
               var tuple = RawDatabaseService.Instance().GetTuplebyId(ref rel , dtuple);
                foreach (var item in tuple.valueSet)
                    if (item.Value.Count != 0) return true;
            }
            catch {
                return false;
            }
            return false;
        }

        private bool checkDataType(string idtuple) =>
             StaticParams.currentRelation.schema.Attributes.Where(a => a.primaryKey).FirstOrDefault().Type.CheckDataTypeOfVariables(idtuple);

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            this.dtg.CommitEdit();
            idTupleList.Add(new IDTupMView());
            this.dtg.ItemsSource = idTupleList;
            this.dtg.CommitEdit();
            this.dtg.Items.Refresh();
        }
    }
}
