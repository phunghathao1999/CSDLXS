﻿using PRDB_Sqlite.Domain.Model;
using PRDB_Sqlite.Domain.ModelView;
using PRDB_Sqlite.Infractructure.Common;
using PRDB_Sqlite.Infractructure.Constant;
using PRDB_Sqlite.Sevice.SysService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Documents;

namespace PRDB_Sqlite.Presentation.UserControl
{
    /// <summary>
    /// Interaction logic for ucCETuple.xaml
    /// </summary>
    public partial class ucCETuple : System.Windows.Controls.UserControl
    {
        public PTuple curTuple { get; set; }
        public PRelation pRelation { get; set; }
        public PAttribute pAttribute { get; set; }
        public ObservableCollection<ValueCellView> valueList;
        public bool rowBeingEdited;
        //public bool ins_mode { get; set; }


        public ucCETuple()
        {
            //ins_mode = false;
            InitializeComponent();
            this.valueList = new ObservableCollection<ValueCellView>();
            this.dtgCellContent.ItemsSource = valueList;


        }

        public void btnView_Click(object sender, RoutedEventArgs e)
        {
            var att = this.txtInfo.Content.ToString();

            if (att.Substring(att.IndexOf(".") + 1).Equals(ContantCls.emlementProb, StringComparison.CurrentCultureIgnoreCase))
            {
                return;
            }
            if (this.dtgCellContent.Visibility == Visibility.Visible)
            {
                //tat dtg
                this.dtgCellContent.Visibility = Visibility.Collapsed;
                this.rtbxCellContent.Visibility = Visibility.Visible;

            }
            else
            {
                //bat dtg
                this.dtgCellContent.Visibility = Visibility.Visible;
                this.rtbxCellContent.Visibility = Visibility.Collapsed;
                this.rowBeingEdited = true;
                var strVals = new TextRange(this.rtbxCellContent.Document.ContentStart, this.rtbxCellContent.Document.ContentEnd).Text.Trim();
                removeDuplicateElements(ref strVals);
                this.valueList.Clear();
                strVals = strVals.Replace("[", "");
                strVals = strVals.Replace("]", "");
                strVals.Split(',')
                    .ToList()
                    .ForEach(p => this.valueList.Add(new ValueCellView() { value = p.Trim() }));
            }
            setValCell(valueList.Select(p => p.value).ToList());
        }

        private void btnClr_Click(object sender, RoutedEventArgs e)
        {
            this.rtbxCellContent.Document.Blocks.Clear();
            this.dtgCellContent.ItemsSource = null;
        }
        public void setValCell(IList<String> val)
        {
            this.valueList.Clear();
            val.ToList().ForEach(p => { if (!(p is null)) this.valueList.Add(new ValueCellView() { value = p }); });
            btnClr_Click(null, null);
            {
                this.dtgCellContent.ItemsSource = valueList;
                this.dtgCellContent.Items.Refresh();
            }
            {
                this.rtbxCellContent.Document.Blocks.Add(new Paragraph(new Run(getDataFromList(val))));

            }
        }
        public void setProbVal(String str)
        {

        }
        private String getDataFromList(IList<String> ls)
        {
            return String.Join(" , ", ls.ToArray()).Trim();
        }
        private IList<String> getLsValfromStr(String str)
        {
            return str.Split(',').ToList();
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {

            if (!rowBeingEdited)
            {
                var att = this.txtInfo.Content.ToString();

                if (att.Substring(att.IndexOf(".") + 1).Equals(ContantCls.emlementProb, StringComparison.CurrentCultureIgnoreCase))
                {
                    var strProb = new TextRange(this.rtbxCellContent.Document.ContentStart, this.rtbxCellContent.Document.ContentEnd).Text.Trim();
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
                    if (this.rtbxCellContent.Visibility == Visibility.Visible)
                    {
                        val = new TextRange(this.rtbxCellContent.Document.ContentStart, this.rtbxCellContent.Document.ContentEnd).Text.Trim();
                        //val = String.Format("'{{ {0} }}", val);
                    }
                    if (this.dtgCellContent.Visibility == Visibility.Visible)
                    {
                        val = String.Join(" , ", valueList.Select(p => p.value.Trim()).ToArray());
                        //val = String.Format("'{{ {0} }}", val);

                    }
                    var datatype = this.pAttribute.Type;
                    if (datatype.CheckDataTypeOfVarLs(val))
                    {
                        //check elem trung nhau
                        removeDuplicateElements(ref val);


                        var attr = this.txtInfo.Content.ToString().Trim().ToLower();

                        //save
                        if (this.curTuple.valueSet.Keys.Contains(attr))
                        {
                            curTuple.valueSet[attr] = val.Split(',').Select(p => p.Trim()).ToList();
                        }
                        // Ps Attr
                        else
                        {
                            curTuple.Ps = new ElemProb(val);
                        }
                        try
                        {
                            RawDatabaseService.Instance().Update(curTuple, pRelation, attr);

                            Parameter.resetMainF = true;
                            Parameter.activeTabIdx = 1;
                            Parameter.RelationIndex = (int)pRelation.id - 1;
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

        public void removeDuplicateElements(ref string rawVal)
        {
            rawVal = rawVal.Replace("{", "");
            rawVal = rawVal.Replace("}", "");
            if (rawVal.IndexOf(',') > -1)
            {
                var list = rawVal.Split(',').Select(e => e.Trim()).Distinct(); ;
                rawVal = String.Join(",", list.ToArray());
            }
        }

        private void btnCommitEdit_Click(object sender, RoutedEventArgs e)
        {
            this.dtgCellContent.CommitEdit();
            this.rowBeingEdited = false;
            this.rtbxCellContent.IsReadOnly = true;
            this.dtgCellContent.IsEnabled = false;


        }
    }
}
