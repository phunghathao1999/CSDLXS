using PRDB_Sqlite.Domain.Model;
using PRDB_Sqlite.Sevice.SysService;
using PRDB_Sqlite.SystemParam;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for AddingNewRelation.xaml
    /// </summary>
    public partial class AddingNewRelation : Window
    {
        public AddingNewRelation()
        {
            InitializeComponent();
        }

      

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                if (this.txtRelName.Text.Trim().Length <= 0)
                {
                   MessageBox.Show( "You must enter a relation name, please try again !","Error",MessageBoxButton.OK,MessageBoxImage.Error);
                    return;
                }


                if (this.txtRelName.Text.ToLower() == "select" || this.txtRelName.Text.ToLower() == "from" || this.txtRelName.Text.ToLower() == "where")
                {
                    MessageBox.Show( "Relation name is not valid ( not match with keyword 'select', 'from', 'where')","Error",MessageBoxButton.OK,MessageBoxImage.Error);
                    return;
                }

                foreach (var item in StaticParams.currentDb.Relations.ToList())
                {
                    if (item.relationName.Equals(this.txtRelName.Text.ToLower(), StringComparison.OrdinalIgnoreCase))
                    {
                        MessageBox.Show( "This relation name has already existed in the database, please try again !","Error",MessageBoxButton.OK,MessageBoxImage.Error);
                        return;
                    }
                }

                PSchema schema = StaticParams.currentDb.Schemas.SingleOrDefault(c => c == this.cbxSch.SelectedItem);
                var relation = new PRelation();
                relation.relationName = this.txtRelName.Text.ToLower();
                relation.id = RawDatabaseService.Instance().getNextIdRl();
                relation.schema = schema;

                try
                {
               
                    //in db
                    RawDatabaseService.Instance().Insert(relation);
                    //in ram 
                    StaticParams.currentDb.Relations.Add(relation);

                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                if (MessageBox.Show("Add successfully.Do you want add a new relation name ?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    this.txtRelName.Focus();
                    this.txtRelName.Text = null;
                    this.Window_Loaded(sender, e);
                }
                else
                    this.Close();
            }
            catch (Exception EX)
            {
                MessageBox.Show(EX.Message);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.txtRelName.Focus();
            this.cbxSch.ItemsSource = StaticParams.currentDb.Schemas.ToList();
            this.cbxSch.DisplayMemberPath= "SchemaName";
            this.cbxSch.SelectedValuePath = "id";
        }
    }
}
