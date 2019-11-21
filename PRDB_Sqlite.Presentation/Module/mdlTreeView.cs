using PRDB_Sqlite.Domain.Model;
using PRDB_Sqlite.Infractructure.Common;
using System;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Orientation = System.Windows.Controls.Orientation;

namespace PRDB_Sqlite.Presentation.Module
{
    public class MdlTreeView
    {
        private PDatabase pDatabase;
        private static MdlTreeView instance;
        protected MdlTreeView(PDatabase db)
        {
            this.pDatabase = db;
        }
        protected MdlTreeView()
        {
            if (this.pDatabase == null) this.pDatabase = new PDatabase(ConfigurationManager.AppSettings["conectionString"].ToString());
        }
        public static MdlTreeView Instance(PDatabase db = null)
        {
            if (db == null) db = new PDatabase(ConfigurationManager.AppSettings["conectionString"].ToString());
            return instance == null ? instance = new MdlTreeView(db) : instance;
        }
        public StackPanel GetStackPanelRoot1(String RootName , String urlImage)
        {
            //Image schema
            StackPanel stpSchema = new StackPanel() { Orientation = Orientation.Horizontal };
            Image imageShema = new Image() { Source = new BitmapImage(new Uri(urlImage, UriKind.RelativeOrAbsolute)) };
            imageShema.Height = 20;
            imageShema.Width = 20;
            TextBlock txtSchema = new TextBlock() { Text = RootName };
            stpSchema.Children.Add(imageShema);
            stpSchema.Children.Add(txtSchema);
            return stpSchema;
        }

        public TreeViewItem getTreeViewItemFromDb()
        {

            //make a Treeview
            StackPanel stpDatabse = GetStackPanelRoot1("   " + this.pDatabase.DbName.ToString(), @"assets\Images\databseTree.jpg");
            //make a root
            var root = new TreeViewItem();
            root.Header = stpDatabse;
            root.ToolTip = this.pDatabase.ConnectString.ToString();
           // root.Background = Brushes.Aqua;
            root.Padding = new Thickness(5, 3, 5, 3);
           // root.FontWeight = FontWeights.Bold;
            root.FontSize = 14;

            StackPanel stpSchema = GetStackPanelRoot1("   Schema", @"assets\Images\tableTree.png");
            //fetch the Schemas
            var schLst = new TreeViewItem() { Header = stpSchema, ToolTip = String.Format("rows", this.pDatabase.Schemas.Count) };
            foreach (PSchema item in this.pDatabase.Schemas)
            {
                StackPanel stpSchemaItem = GetStackPanelRoot1("   " + item.SchemaName.ToUpper().ToString(), @"assets\Images\tableTree.png");
                var curSch = new TreeViewItem() { Header = stpSchemaItem, ToolTip = String.Format("{0} rows", item.Attributes.Count), Uid = item.id.ToString()/*, Foreground = Brushes.YellowGreen*/ };
                curSch.MouseDoubleClick += (s, e) =>
                {
                    var sender = s as TreeViewItem;
                    Parameter.SchemaIndex = Convert.ToInt32(sender.Uid);
                    Parameter.indexSchChange = true;
                    MainWindow.resetTab("sch");

                };

                //fetch the attributes

              
                foreach (PAttribute attr in item.Attributes)
                {
                   
                    var curAtrr = new TreeViewItem();
                    if (!attr.primaryKey)
                    {
                        StackPanel stpAttribute = GetStackPanelRoot1("   " + attr.AttributeName, @"assets\Images\attributeTree.png");
                        curAtrr.Header = stpAttribute;
                        curAtrr.ToolTip = String.Format("Data Type: {0}", attr.Type.TypeName);
                    }
                    else
                    {
                        StackPanel stpAttribute = GetStackPanelRoot1("   " + attr.AttributeName, @"assets\Images\key-icon.png");
                       
                        curAtrr.Header = stpAttribute;
                        curAtrr.ToolTip = String.Format("Data Type: {0}", attr.Type.TypeName);
                        curAtrr.FontWeight = FontWeights.Bold;
                    }
                    curSch.Items.Add(curAtrr);
                }
                //curSch.Items.Add(attrLst);
                schLst.Items.Add(curSch);
            };
            root.Items.Add(schLst);

            StackPanel stpRelation = GetStackPanelRoot1("   Relations", @"assets\Images\attribute.png");
            //fetch the Relations
            var rlLst = new TreeViewItem() { Header = stpRelation, ToolTip = String.Format("{0} rows", this.pDatabase.Relations.Count) };
            foreach (PRelation item in this.pDatabase.Relations)
            {
                StackPanel stpRelationFile = GetStackPanelRoot1("   " + item.relationName.ToUpper().ToString(), @"assets\Images\attribute.png");
                var rel = new TreeViewItem()
                {
                    Header = stpRelationFile,
                    ToolTip = String.Format("{0} row of tuples", item.tupes.Count),
                    Uid = item.id.ToString()
                };
                rel.MouseDoubleClick += (s, e) =>
                {
                    var sender = s as TreeViewItem;
                    Parameter.RelationIndex = Convert.ToInt32(sender.Uid);
                    Parameter.indexRelChange = true;
                    MainWindow.resetTab("rel");
                };
                rlLst.Items.Add(rel);
            };
            root.Items.Add(rlLst);
            //fetch the Query
            StackPanel stpQuery = GetStackPanelRoot1("   Queries", @"assets\Images\queryTree.png");
            var qrLst = new TreeViewItem() { Header = stpQuery, ToolTip = String.Format("{0} rows", this.pDatabase.Queries.Count) };
            foreach (PQuery item in this.pDatabase.Queries)
            {
                StackPanel stpQueryFile = GetStackPanelRoot1("   " + item.QueryName, @"assets\Images\queryTree.png");
                qrLst.Items.Add(new TreeViewItem() { Header = stpQueryFile, ToolTip = item.QueryString });
            };
            root.Items.Add(qrLst);

            return root;
        }
    }
}
