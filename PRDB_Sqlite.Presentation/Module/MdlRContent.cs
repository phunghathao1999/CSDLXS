using PRDB_Sqlite.Domain.Model;
using PRDB_Sqlite.Infractructure.Common;
using PRDB_Sqlite.Infractructure.Constant;
using PRDB_Sqlite.Presentation.UserControl;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PRDB_Sqlite.Presentation.Module
{
    public class MdlRContent
    {
        private PDatabase pDatabase;
        private static MdlRContent instance;
        private DataGrid dtg_temp;
        protected MdlRContent(PDatabase db)
        {
            this.pDatabase = db;
            this.dtg_temp = new DataGrid();

        }
        protected MdlRContent()
        {
            if (this.pDatabase == null) this.pDatabase = new PDatabase(ConfigurationManager.AppSettings["conectionString"].ToString());
            this.dtg_temp = new DataGrid();
        }
        public static MdlRContent Instance(PDatabase db = null)
        {
            if (db == null) db = new PDatabase(ConfigurationManager.AppSettings["conectionString"].ToString());
            return instance ?? (instance = new MdlRContent(db));
        }
        //get Tab Content
        public StackPanel getTabByUid(ref TabItem tab)
        {
            var reControl = new StackPanel() { Orientation = Orientation.Vertical };
            var dataGrid = new DataGrid();
            
            switch (tab.Uid.ToString().ToLower())
            {
                case "sch":
                    getChild(ref reControl, tab.Uid);
                    break;
                case "rel":
                    getChild(ref reControl, tab.Uid);
                    break;
                case "qry":
                    getChildQry(ref tab, tab.Uid);
                    break;
            }

            //makup dataGrid
            dataGrid.GridLinesVisibility = DataGridGridLinesVisibility.Vertical;
            dataGrid.FontSize = 14f;
            dataGrid.Background = Brushes.Transparent;

            reControl.Children.Add(dataGrid);
            if (!"qry".Equals(tab.Uid.ToString().ToLower()))
                tab.Content = reControl;
            return reControl;
        }

        private void getChildQry(ref TabItem tab, string uid)
        {
            var reControl = new ucTabQry();
            reControl.HorizontalAlignment = HorizontalAlignment.Stretch;
            reControl.VerticalAlignment = VerticalAlignment.Stretch;
            tab.Content = reControl;
        }

        private StackPanel getChild(ref StackPanel stp, string uid)
        {
            var reControl = new ucTabContent();
            #region Cbx setup
            if ("rel".Equals(uid.ToLower()))
            {
                reControl.lblCbx.Content = "Relation list";

                foreach (var item in this.pDatabase.Relations)
                    reControl.cbx.Items.Add(new { Key = item.id, Value = item.relationName.ToUpper() });
                if (this.pDatabase.Relations.Count >= Parameter.RelationIndex)
                    reControl.cbx.SelectedIndex = Parameter.RelationIndex;
            }
            else
            if ("sch".Equals(uid.ToLower()))
            {
                reControl.lblCbx.Content = "Schema list";

                foreach (var item in this.pDatabase.Schemas)
                    reControl.cbx.Items.Add(new { Key = item.id, Value = item.SchemaName.ToUpper() });
                if (this.pDatabase.Relations.Count >= Parameter.SchemaIndex)
                    reControl.cbx.SelectedIndex = Parameter.SchemaIndex;
            }

            //make up
            reControl.cbx.MinWidth = 500f;

            #endregion
            #region DataGrid setup
            //default val DataGrid

            if ("sch".Equals(uid.ToLower()))
            {
                reControl.dtg.ItemsSource = getDataSourceSch((int)reControl.cbx.SelectedValue);
            }
            if ("rel".Equals(uid.ToLower()))
            {
                var data = getDataSourceRel((int)reControl.cbx.SelectedValue);
                var dtS = dynamicGenDataTable(data);
                
                {
                    reControl.dtg.Columns.Clear();
                    //make up dtg
                    reControl.dtg.ItemsSource = dtS.DefaultView;
                }
            }

            reControl.cbx.SelectionChanged += (s, e) =>
            {
                if ("rel".Equals(uid.ToLower()))
                {
                    var data = getDataSourceRel((int)reControl.cbx.SelectedValue);
                    var dtS = dynamicGenDataTable(data);
                    if (dtS is null)
                    {
                        reControl.dtg.AutoGenerateColumns = true;
                        reControl.dtg.ItemsSource = dtS.DefaultView;
                    }
                    else
                    {
                        reControl.dtg.Columns.Clear();
                        //make up dtg
                        reControl.dtg.ItemsSource = dtS.DefaultView;
                    }
                }
                if ("sch".Equals(uid.ToLower()))
                {
                    reControl.dtg.ItemsSource = getDataSourceSch((int)reControl.cbx.SelectedValue);
                    reControl.dtg.Columns[reControl.dtg.Columns.Count - 1].Visibility = Visibility.Collapsed;
                }
            };


            #endregion
            stp.Children.Add(reControl);
            return stp;
        }

        private DataTable dynamicGenDataTable(IList<IDictionary<string, string>> data)
        {
            var dt = new DataTable();

            if (data.Count != 0)
            {
                dt.Columns.Clear();
                foreach (var key in data[0].Keys)
                {
                    {
                        if (key.IndexOf('.') != -1)
                            dt.Columns.Add(new DataColumn(key.Split('.')[1].ToUpper()));
                        else
                            dt.Columns.Add(new DataColumn(key.ToUpper()));

                    }
                }
                foreach (var item in data)
                {

                    var dr = dt.NewRow();
                    foreach (var key in item.Keys)
                    {
                        if (key.IndexOf('.') != -1)

                            dr[key.Split('.')[1].ToUpper()] = item[key];
                        else
                            dr[key.ToUpper()] = item[key];

                    }
                    dt.Rows.Add(dr);
                }
            }
            else
            {
                dt.Columns.Add(new DataColumn("No Tuple".ToUpper()));
                var dr = dt.NewRow();
                dr["No Tuple".ToUpper()] = "Nothing";
                dt.Rows.Add(dr);
            }
            return dt;

        }

        private IList<PAttribute> getDataSourceSch(int cbxIdx)
        {
            IList<PAttribute> dataSource = new List<PAttribute>();
            foreach (var item in this.pDatabase.Schemas)
                if (item.id.Equals(cbxIdx)) dataSource = item.Attributes;
            return dataSource;
        }
        private IList<IDictionary<string, string>> getDataSourceRel(int cbxIdx)
        {
            var reVal = new List<IDictionary<string, string>>();
            foreach (var item in this.pDatabase.Relations)
            {

                if (item.id == cbxIdx)
                    foreach (var tuple in item.tupes)
                    {
                        IDictionary<string, string> dataSource = new Dictionary<string, string>();
                        foreach (var key in tuple.valueSet.Keys)
                        {
                            dataSource.Add(key, getValCell(tuple.valueSet[key]));
                        }
                        dataSource.Add(ContantCls.emlementProb, tuple.Ps.ToString());
                        reVal.Add((dataSource));
                    }
            }
            return new List<IDictionary<string, string>>(reVal);
        }
        private string getValCell(IList<string> list)
        {
            var reStr = "";
            for (int i = 0; i < list.Count; i++)
            {
                reStr += list[i].ToString();
                if (i != list.Count - 1) reStr += ",";
            }
            return String.Format("{{ {0} }}", reStr);
        }
    }
}
