using PRDB_Sqlite.Domain.Model;
using PRDB_Sqlite.Domain.ModelView;
using PRDB_Sqlite.Infractructure.Constant;
using PRDB_Sqlite.Sevice.SysService;
using PRDB_Sqlite.SystemParam;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PRDB_Sqlite.Presentation.Screen
{
    /// <summary>
    /// Interaction logic for EditingSchema.xaml
    /// </summary>
    public partial class EditingSchema : Window
    {
        ObservableCollection<SchemaModelView> attrs = new ObservableCollection<SchemaModelView>();

        public EditingSchema()
        {
            InitializeComponent();
            setUpCbx();
            setUpDtg();
            AddHandler();
        }

        private void AddHandler()
        {
            this.cbxSchName.SelectionChanged += (s, e) =>
            {
                var select = s as ComboBox;
                var schema = StaticParams.currentDb.Schemas.Where(p => p == select.SelectedItem as PSchema).FirstOrDefault();
                attrs.Clear();
                foreach (var item in schema.Attributes)
                {
                    if (!item.AttributeName.Equals(ContantCls.emlementProb, StringComparison.CurrentCultureIgnoreCase))
                        attrs.Add(new SchemaModelView(item.AttributeName, item.primaryKey, item.Type.DataType, item.Type.TypeName, item.Type.DomainString, item.Description));
                }
                this.dtg.ItemsSource = attrs;
                this.txtName.Text = schema.SchemaName.ToUpper();
            };
        }

        private void setUpCbx()
        {
            this.cbxSchName.ItemsSource = StaticParams.currentDb.Schemas.ToList();
            this.cbxSchName.DisplayMemberPath = "SchemaName";
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
            try
            {
                var schema = this.cbxSchName.SelectedItem as PSchema;

                if (RawDatabaseService.Instance().getRelByIdSch(schema).Count > 0)
                {
                    MessageBox.Show("The Schema are being used by another Relation, Fail to Edit!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                //build attrs
                var upAttrs = schema.Attributes.ToList();
                schema.SchemaName = this.txtName.Text.ToLower();
                for (int i = 0; i < upAttrs.Count; i++)
                {
                    upAttrs[i].AttributeName = attrs[i].attrName;
                    upAttrs[i].primaryKey = attrs[i].isPri;
                    upAttrs[i].Type.TypeName = attrs[i].typeName;
                    upAttrs[i].Type.GetDomain(attrs[i].domain);
                    upAttrs[i].Type.GetDataType();
                    upAttrs[i].Schema = schema;
                }
                //update Attrs

                foreach (var item in upAttrs)
                {
                    RawDatabaseService.Instance().Update(item);
                }
                //update Sch
                RawDatabaseService.Instance().Update(schema);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private bool CheckValidatedDataGridView(ObservableCollection<SchemaModelView> attrs)
        {
            if (attrs.Count == 0)
            {
                MessageBox.Show("Error: Unable to create Schema. Schema attribute is required !", "Notification", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.txtName.Text = (this.cbxSchName.SelectedItem as PSchema).SchemaName;
            }
            catch { }
        }


    }
}
