﻿using PRDB_Sqlite.Domain.Model;
using PRDB_Sqlite.Infractructure.Common;
using PRDB_Sqlite.Infractructure.Constant;
using PRDB_Sqlite.Sevice.CommonService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using MessageBox = System.Windows.MessageBox;

namespace PRDB_Sqlite.Presentation.UserControl
{
    /// <summary>
    /// Interaction logic for ucTabQry.xaml
    /// </summary>
    public partial class ucTabQry : System.Windows.Controls.UserControl
    {
        public ucTabQry()
        {
            InitializeComponent();

            if (ConfigurationManager.AppSettings["devmode"].Contains("1"))
            {
                var sampleSql1 = "select * from patient where (patient.bloodtype = 'A' ⊗_in patient.height > 130)[0.3,0.8]";
                var sampleSql2 = "select * from patient natural join in physician";
                var sampleSql3 = "select patient.firstname from patient";
                this.rbxQry.Document.Blocks.Add(new Paragraph(new Run(sampleSql1.Trim())));
                this.rbxQry.Document.Blocks.Add(new Paragraph(new Run(sampleSql2.Trim())));
                this.rbxQry.Document.Blocks.Add(new Paragraph(new Run(sampleSql3.Trim())));
            }
            setDefaultDismension();
            AddHandler();

            //reFresh();
        }

        private void setDefaultDismension()
        {
            var rate = System.Windows.SystemParameters.PrimaryScreenHeight / 23;
            this.rbxQry.MinHeight = 4 * rate;
            this.rbxQry.MaxHeight = 4 * rate;
            this.txtMessage.MinHeight = 2 * rate;
            this.txtMessage.MaxHeight = 2 * rate;

            this.dtgDataResult.MinHeight = 10 * rate;
            this.dtgDataResult.MaxHeight = 10 * rate;
            this.rbxQry.SetValue(Paragraph.LineHeightProperty, 0.5);
        }

        private void AddHandler()
        {
            //this.btnExec.Click += (s,e) => {
            //    MessageBox.Show("oc");
            //};
            this.btnExec.Click += new RoutedEventHandler(this.executeQuery);
            this.btnNewtab.Click += new RoutedEventHandler(this.btnNewtab_Click);
            this.btnView.Click += new RoutedEventHandler(this.btnChangeView_Click);

        }

        private void btnChangeView_Click(object sender, RoutedEventArgs e)
        {
            if (this.dtgDataResult.Items.Count != 0)
                if (this.dtgDataResult.Columns[0].Width.IsStar)
                    foreach (var col in this.dtgDataResult.Columns) col.Width = new DataGridLength(1.0, DataGridLengthUnitType.Auto);
                else
                    foreach (var col in this.dtgDataResult.Columns) col.Width = new DataGridLength(1.0, DataGridLengthUnitType.Star);

        }
        private void AddStrategies(String text)
        {
            foreach (TabItem tabItem in this.TbQry.Items)
            {
                if (tabItem.IsSelected)
                {
                    this.rbxQry.CaretPosition.InsertTextInRun(text);
                }
            }
        }
        private void btnNewtab_Click(object sender, RoutedEventArgs e)
        {
            if (this.TbQry.Items.Count < 15)
            {
                var name = String.Format("{0} {1}", "New Tab", this.TbQry.Items.Count + 1);
                //var newTAb = new TabItem() { Header = name };
                TabQuery newTAb = new TabQuery();
                newTAb.Height = 25;
                newTAb.Title = name;

                newTAb.Content = new RichTextBox() { MaxHeight = 200, MinHeight = 200, FontFamily = new FontFamily("Consolas"), FontSize = 14f };
                this.TbQry.Items.Add(newTAb);
                newTAb.Focus();
            }
            else
                MessageBox.Show("Too more Tab cause your bad experience");
        }

        private void executeQuery(object sender, EventArgs e)
        {
            try
            {
                this.dtgDataResult.ItemsSource = null;
                this.dtgDataResult.Columns.Clear();

                var strQry = String.Empty;
                strQry = new TextRange(this.rbxQry.Document.ContentStart, this.rbxQry.Document.ContentEnd).Text;
                strQry = (String.IsNullOrEmpty(this.rbxQry.Selection.Text) ? strQry : this.rbxQry.Selection.Text);

                if (String.IsNullOrEmpty(strQry))
                {
                    MessageBox.Show("Query does not exist!", "Notification", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var query = new QueryExecutor(strQry, (SystemParam.StaticParams.currentDb));
                //txtMessage.Text = "";

                if (query.ExecuteQuery())
                {
                    this.dtgDataResult.ItemsSource = dynamicGenDataTable(getDataSource(query.relationResult)).DefaultView;
                    this.txtMessage.Text = String.Empty;

                    if (query.relationResult.tupes.Count <= 0)
                    {
                        txtMessage.Foreground = Brushes.IndianRed;

                        txtMessage.Text += "No tuple satisfies the condition";
                    }
                    else
                    {
                        txtMessage.Foreground = Brushes.Black;
                        txtMessage.Text += String.Format("ε threshold: {0}", Parameter.eulerThreshold.ToString());
                        txtMessage.Text += String.Format("\nProjection Strategy: {0}", Parameter.curStrategy.ToString());
                        txtMessage.Text += String.Format("\nResult Tuple Count: {0}", query.relationResult.tupes.Count);


                    }

                }
                else
                {
                    txtMessage.Foreground = Brushes.Red;
                    txtMessage.Text = query.MessageError;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                ClearAll(); // đưa csdl về trạng thái ban đầu
            }
        }

        private void ClearAll()
        {
        }
        private IList<IDictionary<string, string>> getDataSource(PRelation relations)
        {
            var reVal = new List<IDictionary<string, string>>();

            foreach (var tuple in relations.tupes)
            {
                IDictionary<string, string> dataSource = new Dictionary<string, string>();
                foreach (var key in tuple.valueSet.Keys)
                {
                    dataSource.Add(key, getValCell(tuple.valueSet[key]));

                }
                dataSource.Add(ContantCls.emlementProb, tuple.Ps.ToString());
                reVal.Add((dataSource));
            }
            return reVal;
        }
        private DataTable dynamicGenDataTable(IList<IDictionary<string, string>> data)
        {
            if (data is null || data.Count == 0) return new DataTable();
            var dt = new DataTable();
            dt.Columns.Clear();
            foreach (var key in data[0].Keys)
            {
                // if (dt.Columns.Count == data[0].Keys.Count) break;
                //if (dt.Columns.Count != data[0].Keys.Count || dt.Columns.Count == 0)
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
            return dt;
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

        private void UserControl_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F5:
                    ButtonAutomationPeer peer = new ButtonAutomationPeer(this.btnExec);
                    IInvokeProvider invokeProv = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                    invokeProv.Invoke();
                    break;
                default: break;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.Filter = "Rich Text File (*.psql)|*.psql|All Files (*.*)|*.*";
            dialog.FileName = "PRDB_Query.psql"; //set initial filename
            if (dialog.ShowDialog() == true)
            {
                foreach (TabItem tabItem in TbQry.Items)
                {
                    if (tabItem.IsSelected)
                    {
                        using (var stream = dialog.OpenFile())
                        {
                            RichTextBox richTextBox = (RichTextBox)tabItem.Content;
                            var range = new TextRange(richTextBox.Document.ContentStart,
                                                      richTextBox.Document.ContentEnd);
                            range.Save(stream, DataFormats.Text);
                            var name = dialog.FileName;
                            int index = name.LastIndexOf(@"\");
                            tabItem.Header = name.Substring(index + 1, name.Length - index - 1);
                        }
                        break;
                    }
                }
            }
        }
        private void btnOpnQry_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "Rich Text File (*.psql)|*.psql|All Files (*.*)|*.*";
            if (dialog.ShowDialog() == true)
            {
                if (this.TbQry.Items.Count < 15)
                {
                    var name = dialog.FileName;
                    int index = name.LastIndexOf(@"\");
                    TabQuery newTAb = new TabQuery();
                    newTAb.Title = name.Substring(index + 1, name.Length - index - 1);

                    newTAb.Content = new RichTextBox() { MaxHeight = 200, MinHeight = 200, FontFamily = new FontFamily("Consolas"), FontSize = 14f };
                    this.TbQry.Items.Add(newTAb);
                    // read file
                    FileStream fStream;
                    RichTextBox richTextBox = (RichTextBox)newTAb.Content;
                    var range = new TextRange(richTextBox.Document.ContentStart,
                                              richTextBox.Document.ContentEnd);

                    if (File.Exists(dialog.FileName))

                    {
                        fStream = new FileStream(dialog.FileName, FileMode.OpenOrCreate);

                        range.Load(fStream, DataFormats.Text);

                        fStream.Close();

                    }
                    newTAb.Focus();
                }
                else
                    MessageBox.Show("Too more Tab cause your bad experience");

            }
        }

        private void rbxQry_MouseDown(object sender, MouseButtonEventArgs e)
        {


        }
        // "⊕_in", "⊕_ig", "⊕_me","⊗_in", "⊗_ig", "⊗_me"
        private void btnCon_in_Click(object sender, RoutedEventArgs e)
        {
            AddStrategies(String.Format(" {0} ", Parameter.strategies.ToArray().ElementAt(3)));
        }

        private void btnDis_in_Click(object sender, RoutedEventArgs e)
        {
            AddStrategies(String.Format(" {0} ", Parameter.strategies.ToArray().ElementAt(0)));
        }

        private void btnCon_me_Click(object sender, RoutedEventArgs e)
        {
            AddStrategies(String.Format(" {0} ", Parameter.strategies.ToArray().ElementAt(5)));
        }

        private void btnDis_me_Click(object sender, RoutedEventArgs e)
        {
            AddStrategies(String.Format(" {0} ", Parameter.strategies.ToArray().ElementAt(2)));
        }

        private void btnCon_ig_Click(object sender, RoutedEventArgs e)
        {
            AddStrategies(String.Format(" {0} ", Parameter.strategies.ToArray().ElementAt(4)));
        }

        private void btnDis_ig_Click(object sender, RoutedEventArgs e)
        {
            AddStrategies(String.Format(" {0} ", Parameter.strategies.ToArray().ElementAt(1)));

        }

        private void rbxQry_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = new TextRange(this.rbxQry.Document.ContentStart, this.rbxQry.Document.ContentEnd).Text;
        }

        private void btnExecCur_Click(object sender, RoutedEventArgs e)
        {
            var currentLine = new TextRange(this.rbxQry.CaretPosition.GetLineStartPosition(0), this.rbxQry.CaretPosition.GetLineStartPosition(1) ?? this.rbxQry.CaretPosition.DocumentEnd).Text.Trim();
            try
            {
                var query = new QueryExecutor(currentLine, (SystemParam.StaticParams.currentDb));
                //txtMessage.Text = "";

                if (query.ExecuteQuery())
                {
                    this.dtgDataResult.ItemsSource = dynamicGenDataTable(getDataSource(query.relationResult)).DefaultView;
                    this.txtMessage.Text = String.Empty;

                    if (query.relationResult.tupes.Count <= 0)
                    {
                        txtMessage.Foreground = Brushes.IndianRed;

                        txtMessage.Text += "No tuple satisfies the condition";
                    }
                    else
                    {
                        txtMessage.Foreground = Brushes.Black;
                        txtMessage.Text += String.Format("ε threshold: {0}", Parameter.eulerThreshold.ToString());
                        txtMessage.Text += String.Format("\nProjection Strategy: {0}", Parameter.curStrategy.ToString());
                        txtMessage.Text += String.Format("\nResult Tuple Count: {0}", query.relationResult.tupes.Count);


                    }

                }
                else
                {
                    txtMessage.Foreground = Brushes.Red;
                    txtMessage.Text = query.MessageError;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                ClearAll(); // đưa csdl về trạng thái ban đầu
            }
        }

        private void btnJoin_in_Click(object sender, RoutedEventArgs e)
        {
            AddStrategies(String.Format(" {0} ", "natural join in"));
        }

        private void btnJoin_ig_Click(object sender, RoutedEventArgs e)
        {
            AddStrategies(String.Format(" {0} ", "natural join ig"));
        }

        private void btnJoin_me_Click(object sender, RoutedEventArgs e)
        {
            AddStrategies(String.Format(" {0} ", "natural join me"));
        }
    }
}
