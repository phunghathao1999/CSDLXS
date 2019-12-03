using PRDB_Sqlite.Domain.Model;
using PRDB_Sqlite.Infractructure.Common;
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
    /// Interaction logic for opn_del_Schema.xaml
    /// </summary>
    public partial class opn_del_Schema : Window
    {
        string mode;
        public opn_del_Schema(string which)
        {
            InitializeComponent();
            this.mode = which;
        }

        public Border getBorderButton()
        {
             Border borderbtn = new Border();
            borderbtn.Width = 60;
            borderbtn.CornerRadius = new CornerRadius(5);
            borderbtn.Background = Brushes.LightBlue;
            borderbtn.BorderBrush = Brushes.Black;
            borderbtn.BorderThickness = new Thickness(1,1,1,1);
            return borderbtn;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Style roundButton_Style = FindResource("RoundButton_Style") as Style;
            this.btnMain.Width = 60;
            this.btnMain.Style = roundButton_Style;
            if ("opnsch".Equals(this.mode, StringComparison.CurrentCultureIgnoreCase))
            {
                this.Title = "Open Schema";
                this.grpMain.Header = "Select Schema";
                this.btnMain.Content = "Open";


                this.cbx.ItemsSource = StaticParams.currentDb.Schemas.ToList();
                this.cbx.DisplayMemberPath = "SchemaName";

            }

            if ("opnrel".Equals(this.mode, StringComparison.CurrentCultureIgnoreCase))
            {
                this.Title = "Open Relation";
                this.grpMain.Header = "Select Relation";
                this.btnMain.Content = "Open";

                this.cbx.ItemsSource = StaticParams.currentDb.Relations.ToList();
                this.cbx.DisplayMemberPath = "relationName";
            }

            if ("delsch".Equals(this.mode, StringComparison.CurrentCultureIgnoreCase))
            {
                this.Title = "Delte Schema";
                this.grpMain.Header = "Select Schema";
                this.btnMain.Content = "Delete";

                this.cbx.ItemsSource = StaticParams.currentDb.Schemas.ToList();
                this.cbx.DisplayMemberPath = "SchemaName";
            }

            if ("delrel".Equals(this.mode, StringComparison.CurrentCultureIgnoreCase))
            {
                this.Title = "Delete Relation";
                this.grpMain.Header = "Select Relation";
                this.btnMain.Content = "Delete";

                this.cbx.ItemsSource = StaticParams.currentDb.Relations.ToList();
                this.cbx.DisplayMemberPath = "relationName";
            }

            
            this.lblHeader.Content = this.Title;
        }

        private void btnMain_Click(object sender, RoutedEventArgs e)
        {

            if ("opnsch".Equals(this.mode, StringComparison.CurrentCultureIgnoreCase))
            {
                Parameter.activeTabIdx = 0;
                Parameter.SchemaIndex = this.cbx.SelectedIndex;

            }

            if ("opnrel".Equals(this.mode, StringComparison.CurrentCultureIgnoreCase))
            {
                Parameter.activeTabIdx = 1;
                Parameter.RelationIndex = this.cbx.SelectedIndex;
            }

            if ("delsch".Equals(this.mode, StringComparison.CurrentCultureIgnoreCase))
            {
                try {
                    //delete schema
                    var schema = StaticParams.currentDb.Schemas.Where(p => p == this.cbx.SelectedItem as PSchema).FirstOrDefault();
                    //in Db
                    RawDatabaseService.Instance().Delete(schema);
                    //in Ram
                    StaticParams.currentDb.Schemas.Remove(schema);

                    //delete attr of schema
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            if ("delrel".Equals(this.mode, StringComparison.CurrentCultureIgnoreCase))
            {
                try
                {
                    var relation = StaticParams.currentDb.Relations.Where(p => p == this.cbx.SelectedItem as PRelation).FirstOrDefault();
                    //in DB
                    RawDatabaseService.Instance().Delete(relation);
                    //in Ram
                    StaticParams.currentDb.Relations.Remove(relation);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, "Infomation", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }

            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
