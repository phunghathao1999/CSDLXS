using PRDB_Sqlite.Domain.Model;
using PRDB_Sqlite.Infractructure.Constant;
using PRDB_Sqlite.Sevice.SysService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace PRDB_Sqlite.Presentation.UserControl
{
    /// <summary>
    /// Interaction logic for ucTabContent.xaml
    /// </summary>
    public partial class ucTabContent : System.Windows.Controls.UserControl
    {
        CollectionView backup;
        private bool ins_mode;
        public ucTabContent()
        {
            ins_mode = false;
            InitializeComponent();
            //setAutoBinding();
            this.dtg.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(dgData_SelectionChanged);
        }

        private void dgData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnEdit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.dtg.Items.Count > 0)
            {
                //this.dtg.IsReadOnly = !this.dtg.IsReadOnly;
                //dtg
                this.backup = new CollectionView((CollectionView)CollectionViewSource.GetDefaultView(this.dtg.ItemsSource));
                //others
                if (!(bool)this.ucEdit.chkPri.IsChecked)
                {
                    this.ucEdit.dtgCellContent.IsEnabled = !this.ucEdit.dtgCellContent.IsEnabled;
                    this.ucEdit.rtbxCellContent.IsReadOnly = !this.ucEdit.rtbxCellContent.IsReadOnly;
                    this.ucEdit.btnClr.IsEnabled = !this.ucEdit.btnClr.IsEnabled;
                    this.ucEdit.btnCommitEdit.IsEnabled = !this.ucEdit.btnCommitEdit.IsEnabled;
                    this.ucEdit.btnApply.IsEnabled = !this.ucEdit.btnApply.IsEnabled;
                }

            }
        }

        private void btnIns_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //PRelation relation = new PRelation() { id = int.Parse(this.cbx.SelectedValue.ToString()) };
            //if (RawDatabaseService.Instance().InsertTupleIntoTableRelation(relation))
            //{
            //}
            this.ins_mode = !this.ins_mode;
            //dtg
            if (this.ins_mode)
            {
                for (int i = 0; i < this.dtg.Items.Count; i++)
                {
                    DataGridRow r = this.dtg.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
                    if (r != null)
                        r.Tag = true; // readOnly
                }
                this.dtg.CanUserAddRows = true;
                this.dtg.IsReadOnly = false;
                //add handle
                this.dtg.BeginningEdit += (begin_Edit);
            }
            else
            {
                for (int i = 0; i < this.dtg.Items.Count; i++)
                {
                    DataGridRow r = this.dtg.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
                    if (r != null)
                        r.Tag = null; // readOnly
                }
                this.dtg.CanUserAddRows = false;
                this.dtg.IsReadOnly = true;
                //remove handle
                this.dtg.BeginningEdit -= (begin_Edit);
            }
            
        }

        private void begin_Edit(object sender, DataGridBeginningEditEventArgs e)
        {
            if (((DataGridRow)e.Row).Tag != null && !(bool)((DataGridRow)e.Row).Tag)
            {
                e.Cancel = true;
            }
        }

        private void btnDel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var rs = MessageBox.Show("Delete this Tuple?", "Notification", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (rs == MessageBoxResult.Yes)
            {

                var relation = new PRelation() { id = int.Parse(this.cbx.SelectedValue.ToString()) };
                var tupID = (this.dtg.SelectedItem as DataRowView).Row.ItemArray[0].ToString().Trim();
                var tuple = RawDatabaseService.Instance().GetTuplebyId(ref relation, tupID);
                try
                {
                    RawDatabaseService.Instance().DeleteTupleById(relation, tuple);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btnCmmt_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void btnRbk_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!this.dtg.IsReadOnly)
                this.dtg.ItemsSource = backup.SourceCollection;
        }

        private void dtg_CurrentCellChanged(object sender, EventArgs e)
        {
            this.ucEdit.btnClr.IsEnabled = false;
            this.ucEdit.btnCommitEdit.IsEnabled = false;
            this.ucEdit.btnApply.IsEnabled = false;
            this.ucEdit.dtgCellContent.IsEnabled = false;
            this.ucEdit.rtbxCellContent.IsReadOnly = true;

            var relation = new PRelation() { id = int.Parse(this.cbx.SelectedValue.ToString()) };
            try
            {
                var tupID = (this.dtg.CurrentItem as DataRowView).Row.ItemArray[0].ToString().Trim();
                var tuple = RawDatabaseService.Instance().GetTuplebyId(ref relation, tupID);
                var header = this.dtg.CurrentCell.Column.Header.ToString().ToLower();
                String attrName = header;
                attrName = String.Format("{0}.{1}", relation.relationName, header);

                if (!header.Equals(ContantCls.emlementProb, StringComparison.CurrentCultureIgnoreCase))
                {
                    this.ucEdit.txtInfo.Content = attrName.ToUpper();
                    this.ucEdit.curTuple = tuple;

                    this.ucEdit.setValCell(tuple.valueSet[attrName]);
                }
                else
                {
                    this.ucEdit.txtInfo.Content = attrName.ToUpper();
                    this.ucEdit.curTuple = tuple;

                    this.ucEdit.setValCell(new List<String> { tuple.Ps.ToString() });

                }
                var att = relation.schema.Attributes.Where(p => p.AttributeName.Equals(header, StringComparison.CurrentCultureIgnoreCase)).First();
                this.ucEdit.pAttribute = att;
                this.ucEdit.txtDataType.Content = att.Type.TypeName;
                this.ucEdit.chkPri.IsChecked = att.primaryKey;
                this.ucEdit.pRelation = relation;
            }
            catch //insert
            {
                var tupID = String.Empty;
                var tuple = RawDatabaseService.Instance().GetTuplebyId(ref relation, tupID);
                var header = String.Empty;

                try
                {
                    header = this.dtg.CurrentCell.Column.Header.ToString().ToLower();

                }
                catch
                {
                }
                String attrName = header;
                attrName = String.Format("{0}.{1}", relation.relationName, header);

                if (!header.Equals(ContantCls.emlementProb, StringComparison.CurrentCultureIgnoreCase))
                {
                    this.ucEdit.txtInfo.Content = attrName.ToUpper();
                    this.ucEdit.curTuple = tuple;
                    if(!String.IsNullOrEmpty(header))
                    this.ucEdit.setValCell(tuple.valueSet[attrName]);
                }
                else
                {
                    this.ucEdit.txtInfo.Content = attrName.ToUpper();
                    this.ucEdit.curTuple = tuple;
                    if (!String.IsNullOrEmpty(header))
                        this.ucEdit.setValCell(new List<String> { tuple.Ps.ToString() });

                }
                var att = relation.schema.Attributes.Where(p => p.AttributeName.Equals(header, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                if(att != null)
                {
                    this.ucEdit.pAttribute = att;
                    this.ucEdit.txtDataType.Content = att.Type.TypeName;
                    this.ucEdit.chkPri.IsChecked = att.primaryKey;
                    this.ucEdit.pRelation = relation;
                }
                
            }
        }

        private void dtg_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            //set UC data

        }

    }
}
