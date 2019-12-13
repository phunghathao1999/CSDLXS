using PRDB_Sqlite.Domain.Model;
using PRDB_Sqlite.Domain.ModelView;
using PRDB_Sqlite.Domain.Unit;
using PRDB_Sqlite.Sevice.SysService;
using PRDB_Sqlite.SystemParam;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PRDB_Sqlite.Presentation.Screen
{
    /// <summary>
    /// Interaction logic for AddingSchema.xaml
    /// </summary>
    public partial class AddingSchema : Window
    {
        public PDatabase pDatabase;
        ObservableCollection<SchemaModelView> attrs = new ObservableCollection<SchemaModelView>();

        public AddingSchema()
        {
            this.pDatabase = StaticParams.currentDb;
            InitializeComponent();
            setUpDtg();

        }

        private void setUpDtg()
        {
            this.btnDelRow.Visibility = Visibility.Collapsed;
            this.dtg.AutoGenerateColumns = false;
            this.dtg.CanUserAddRows = false;
            this.dtg.CanUserDeleteRows = true;
            this.dtg.IsReadOnly = false;

            this.dtg.CellEditEnding += cellEditEnding;
        }

        private void cellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                //var column = e.Column as DataGridBoundColumn;
                //var val = e.EditingElement;
                //var row = this.dtg.Items.IndexOf(e.Row.DataContext);
                //var send = sender as SchemaModelView;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            var attLs = new List<PAttribute>();
            if (String.IsNullOrEmpty(this.txtSchName.Text.ToString()))
            {
                MessageBox.Show("You must enter a schema name, please try again!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if ("select".Equals(this.txtSchName.Text.ToLower(), StringComparison.CurrentCultureIgnoreCase) ||
                "from".Equals(this.txtSchName.Text.ToLower(), StringComparison.CurrentCultureIgnoreCase) ||
                "where".Equals(this.txtSchName.Text.ToLower(), StringComparison.CurrentCultureIgnoreCase))
            {
                MessageBox.Show("Schema name is not valid ( not match with keyword 'select', 'from', 'where')", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            foreach (var item in this.pDatabase.Schemas.ToList())
            {
                if (item.SchemaName.ToLower().Equals(this.txtSchName.Text.ToLower(), StringComparison.CurrentCultureIgnoreCase))
                {
                    MessageBox.Show("This schema name has already existed in the database, please try again !", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            if (!CheckValidatedDataGridView(attrs)) return;

            //create schema
            PSchema schema = new PSchema();
            schema.id = RawDatabaseService.Instance().getNextIdSch(); //important
            schema.SchemaName = this.txtSchName.Text.Trim();

            PAttribute attribute;

            //insert Schema and relative component in Db
            try
            {
                #region addAtrr
                foreach (SchemaModelView attr in attrs.ToList())
                {

                    attribute = new PAttribute();
                    attribute.id = RawDatabaseService.Instance().getNextIdAttr();
                    attribute.AttributeName = attr.attrName;
                    attribute.primaryKey = attr.isPri;
                    attribute.Type = new Domain.Unit.PDataType() { DomainString = attr.domain, DataType = attr.datatype, TypeName = attr.typeName };
                    attribute.Description = attr.descs;
                    attribute.Schema = schema;

                    RawDatabaseService.Instance().Insert(attribute); //=>csdl need id schema
                    //update full info schema
                    schema.Attributes.Add(attribute);
                }
                //Ps
                var datatype = new PDataType();
                    datatype.TypeName = "String";
                    datatype.GetDomain("[0  ...  32767] characters");
                    datatype.GetDataType();
                var attrPs = new PAttribute()
                {
                    AttributeName = "Ps",
                    id = RawDatabaseService.Instance().getNextIdAttr(),
                    Schema = schema,
                    primaryKey = false,
                    Description = "Prob",
                    Type = datatype
                };
                RawDatabaseService.Instance().Insert(attrPs);
                schema.Attributes.Add(attrPs);
                #endregion addAtrr

                
                //insert scheme in db
                RawDatabaseService.Instance().Insert(schema);

                //insert scheme in ram
                StaticParams.currentDb.Schemas.Add(schema);
                //commit

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (MessageBox.Show("Add successfully. Do you want add new schema ?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {

                this.txtSchName.Text = "";
                this.attrs.Clear();
                this.dtg.ItemsSource = attrs;
                this.dtg.Items.Refresh();
                this.txtSchName.Focus();
            }
            else
                this.Close();

        }


        private bool CheckValidatedDataGridView(ObservableCollection<SchemaModelView> attrs)
        {
            if (attrs.Count == 0)
            {
                MessageBox.Show("Error: Unable to create Schema. Schema attribute is required !", "Notification", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            var flagPri = false;

            foreach (SchemaModelView item in attrs)
            {
                if (String.IsNullOrWhiteSpace(item.attrName) ||
                    String.IsNullOrEmpty(item.attrName) ||
                    String.IsNullOrEmpty(item.datatype) ||
                    String.IsNullOrEmpty(item.domain))
                {
                    MessageBox.Show(String.Format("The content in row {0} is require!", dtg.Items.IndexOf(item)), "error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                if ("select".Equals(item.attrName.ToLower(), StringComparison.CurrentCultureIgnoreCase) ||
                    "from".Equals(item.attrName.ToLower(), StringComparison.CurrentCultureIgnoreCase) ||
                    "where".Equals(item.attrName.ToLower(), StringComparison.CurrentCultureIgnoreCase))
                {
                    MessageBox.Show("Attribute name is not valid ( not match with keyword 'select', 'from', 'where')", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                if (item.isPri) flagPri = true;
                //duplicat atribute
                foreach (SchemaModelView item_ch in attrs)
                {
                    if (item == item_ch) continue;
                    if (item_ch.attrName.Equals(item.attrName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        MessageBox.Show("There is already an attribute with the same name!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                }
            }
            if (!flagPri) { 
                MessageBox.Show("Schema must have at least 1 Primary Key!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnAddRow_Click(object sender, RoutedEventArgs e)
        {
            this.dtg.CommitEdit();
            attrs.Add(new SchemaModelView());
            this.dtg.ItemsSource = attrs;
            this.dtg.CommitEdit();
            this.dtg.Items.Refresh();
        }
        private void btnDelRow_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnGetDataType_Click(object sender, RoutedEventArgs e)
        {

            var inp_Dtt = new Inp_DataType(null, null);
            inp_Dtt.ShowDialog();
            var dataType = inp_Dtt.dataType;
            var indexRow = this.dtg.Items.IndexOf(this.dtg.CurrentItem);
            var indexCol = this.dtg.CurrentCell.Column.DisplayIndex;
            var row = (DataGridRow)this.dtg.ItemContainerGenerator.ContainerFromIndex(indexRow);

            var cell = (SchemaModelView)row.Item;
            cell.datatype = dataType.DataType;

            attrs[indexRow] = cell;
            this.dtg.ItemsSource = attrs;

        }

        private void DataGridCell_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var inp_Dtt = new Inp_DataType(null, null);
            inp_Dtt.ShowDialog();
            var dataType = inp_Dtt.dataType;

            var x = this.dtg.SelectedItem;
            var indexRow = this.dtg.Items.IndexOf(this.dtg.SelectedItem);

            var obj = (SchemaModelView)this.dtg.SelectedItem;

            obj.datatype = dataType.DataType;
            obj.domain = dataType.DomainString;
            obj.typeName = dataType.TypeName;

            attrs[indexRow] = obj;
            this.dtg.ItemsSource = attrs;
            this.dtg.CommitEdit();

            this.dtg.Items.Refresh();

        }

        private void DataGridCell_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {


        }
    }
}
