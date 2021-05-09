﻿using PRDB_Sqlite.Domain.Model;
using PRDB_Sqlite.Infractructure.Common;
using PRDB_Sqlite.Infractructure.Constant;
using PRDB_Sqlite.Presentation.Screen;
using PRDB_Sqlite.Sevice.SysService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

namespace PRDB_Sqlite.Presentation.UserControl
{
    /// <summary>
    /// Interaction logic for ucTabContent.xaml
    /// </summary>
    public partial class ucTabContent : System.Windows.Controls.UserControl
    {
        CollectionView backup;
        public ucTabContent()
        {
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

            var addTupleForm = new addingTuples();
            addTupleForm.ShowDialog();

        }


        private void begin_Edit(object sender, DataGridBeginningEditEventArgs e)
        {
            //if (((DataGridRow)e.Row).Tag != null && (bool)((DataGridRow)e.Row).Tag)
            //{
            //    e.Cancel = true;
            //}
            this.ucEdit.btnApply.IsEnabled = !this.ucEdit.IsEnabled;
            e.Cancel = true;
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

                Parameter.currentColumn = this.dtg.CurrentCell.Column.DisplayIndex;
                Parameter.currentRow = this.dtg.Items.IndexOf(this.dtg.CurrentItem);
            }
            catch { }
            this.ucEdit.rtbxCellContent.Visibility = Visibility.Visible;
            this.ucEdit.dtgCellContent.Visibility = Visibility.Collapsed;
        }

        private void dtg_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //set UC data
        }

        private void btnApply(object sender, RoutedEventArgs e)
        {
            var att = this.ucEdit.txtInfo.Content.ToString();
            if (!this.ucEdit.rowBeingEdited)
            {


                if (att.Substring(att.IndexOf(".") + 1).Equals(ContantCls.emlementProb, StringComparison.CurrentCultureIgnoreCase))
                {
                    var strProb = new TextRange(this.ucEdit.rtbxCellContent.Document.ContentStart, this.ucEdit.rtbxCellContent.Document.ContentEnd).Text.Trim();
                    try
                    {
                        new ElemProb(strProb);
                    }
                    catch
                    {
                        return;
                    }
                }
                try
                {
                    String val = String.Empty;
                    if (this.ucEdit.rtbxCellContent.Visibility == Visibility.Visible)
                    {
                        val = new TextRange(this.ucEdit.rtbxCellContent.Document.ContentStart, this.ucEdit.rtbxCellContent.Document.ContentEnd).Text.Trim();
                        //val = String.Format("'{{ {0} }}", val);
                    }
                    if (this.ucEdit.dtgCellContent.Visibility == Visibility.Visible)
                    {
                        val = String.Join(" , ", this.ucEdit.valueList.Select(p => p.value.Trim()).ToArray());
                        //val = String.Format("'{{ {0} }}", val);

                    }
                    var datatype = this.ucEdit.pAttribute.Type;
                    if (datatype.CheckDataTypeOfVarLs(val))
                    {
                        //check elem trung nhau
                        this.ucEdit.removeDuplicateElements(ref val);


                        var attr = this.ucEdit.txtInfo.Content.ToString().Trim().ToLower();

                        //save
                        if (this.ucEdit.curTuple.valueSet.Keys.Contains(attr))
                        {
                            this.ucEdit.curTuple.valueSet[attr] = val.Split(',').Select(p => p.Trim()).ToList();
                        }
                        // Ps Attr
                        else
                        {
                            this.ucEdit.curTuple.Ps = new ElemProb(val);
                        }
                        try
                        {
                            RawDatabaseService.Instance().Update(this.ucEdit.curTuple, this.ucEdit.pRelation, attr);

                            Parameter.resetMainF = true;
                            Parameter.activeTabIdx = 1;
                            Parameter.RelationIndex = (int)this.ucEdit.pRelation.id - 1;

                            //update dtg


                            if (att.Substring(att.IndexOf(".") + 1).Equals(ContantCls.emlementProb, StringComparison.CurrentCultureIgnoreCase))
                            {
                                this.ucEdit.btnView_Click(null, null);
                                var rowView = (this.dtg.Items[Parameter.currentRow] as DataRowView); //Get RowView
                                rowView.BeginEdit();
                                rowView[Parameter.currentColumn] = new TextRange(this.ucEdit.rtbxCellContent.Document.ContentStart, this.ucEdit.rtbxCellContent.Document.ContentEnd).Text.Trim();
                                rowView.EndEdit();
                                this.dtg.Items.Refresh();
                            }
                            else
                            {
                                this.ucEdit.btnView_Click(null, null);
                                var rowView = (this.dtg.Items[Parameter.currentRow] as DataRowView); //Get RowView
                                rowView.BeginEdit();
                                rowView[Parameter.currentColumn] = "{ " + String.Join(",", this.ucEdit.valueList.Select(p => p.value).ToArray()) + " }";
                                rowView.EndEdit();
                                this.dtg.Items.Refresh();
                            }

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                        MessageBox.Show("The value is invalid with its Datatype", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                }
            }
            else
            {
                MessageBox.Show("the Value have not Edited yet!", "Alert", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.ucEdit.btnApply.Click += this.btnApply;
        }
    }
}
