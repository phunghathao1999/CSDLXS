using PRDB_Sqlite.Domain.Model;
using PRDB_Sqlite.Infractructure.Common;
using PRDB_Sqlite.SystemParam;
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
       // private PDatabase pDatabase;
        private static MdlTreeView instance;
       
        protected MdlTreeView()
        {
            //if (StaticParams.currentDb == null) StaticParams.currentDb = new PDatabase(ConfigurationManager.AppSettings["conectionString"].ToString());
        }
        public static MdlTreeView Instance()
        {
           // if (db == null) db = new PDatabase(ConfigurationManager.AppSettings["conectionString"].ToString());
            return instance ??  (instance = new MdlTreeView());
        }
        public StackPanel GetStackPanelRoot1(String RootName , String urlImage , int sizeX = 20, int sizeY = 15)
        {
            //Image schema
            StackPanel stpSchema = new StackPanel() { Orientation = Orientation.Horizontal };
            Image imageShema = new Image() { Source = new BitmapImage(new Uri(urlImage, UriKind.RelativeOrAbsolute)) };
            imageShema.Height = sizeY;
            imageShema.Width = sizeX;
            TextBlock txtSchema = new TextBlock() { Text = RootName };
            stpSchema.Children.Add(imageShema);
            stpSchema.Children.Add(txtSchema);
            return stpSchema;
        }

        public TreeViewItem getTreeViewItemFromDb()
        {

            //make a Treeview
            StackPanel stpDatabse = GetStackPanelRoot1("   " + StaticParams.currentDb.DbName.ToString(), @"assets\Images\Icondatabase.png", 20 , 20);
            //make a root
            var root = new TreeViewItem();
            root.Header = stpDatabse;
            root.ToolTip = StaticParams.currentDb.ConnectString.ToString();
           // root.Background = Brushes.Aqua;
            root.Padding = new Thickness(5, 3, 5, 3);
           // root.FontWeight = FontWeights.Bold;
            root.FontSize = 14;

            StackPanel stpSchema = GetStackPanelRoot1("   Schema", @"assets\Images\iconFoldel.jpg");
            //fetch the Schemas
            var schLst = new TreeViewItem() { Header = stpSchema, ToolTip = String.Format("rows", StaticParams.currentDb.Schemas.Count) };
            foreach (PSchema item in StaticParams.currentDb.Schemas)
            {
                StackPanel stpSchemaItem = GetStackPanelRoot1("   " + item.SchemaName.ToUpper().ToString(), @"assets\Images\iconFoldel.jpg");
                var curSch = new TreeViewItem() { Header = stpSchemaItem, ToolTip = String.Format("{0} rows", item.Attributes.Count), Uid = StaticParams.currentDb.Schemas.IndexOf(item).ToString()/*, Foreground = Brushes.YellowGreen*/ };
                curSch.MouseDoubleClick += (s, e) =>
                {
                    var sender = s as TreeViewItem;
                    Parameter.SchemaIndex = Convert.ToInt32(sender.Uid);
                    Parameter.activeTabIdx = 0;
                    //Parameter.indexSchChange = true;
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

            StackPanel stpRelation = GetStackPanelRoot1("   Relations", @"assets\Images\iconFoldel.jpg");
            //fetch the Relations
            var rlLst = new TreeViewItem() { Header = stpRelation, ToolTip = String.Format("{0} rows", StaticParams.currentDb.Relations.Count) };
            foreach (PRelation item in StaticParams.currentDb.Relations)
            {
                StackPanel stpRelationFile = GetStackPanelRoot1("   " + item.relationName.ToUpper().ToString(), @"assets\Images\attributeTree.png");
                var rel = new TreeViewItem()
                {
                    Header = stpRelationFile,
                    ToolTip = String.Format("{0} row of tuples", item.tupes.Count),
                    Uid = StaticParams.currentDb.Relations.IndexOf(item).ToString()
                };
                rel.MouseDoubleClick += (s, e) =>
                {
                    var sender = s as TreeViewItem;
                    Parameter.RelationIndex = Convert.ToInt32(sender.Uid);
                    Parameter.activeTabIdx = 1;
                    //Parameter.indexRelChange = true;
                    MainWindow.resetTab("rel");
                };
                rlLst.Items.Add(rel);
            };
            root.Items.Add(rlLst);
            //fetch the Query
            StackPanel stpQuery = GetStackPanelRoot1("   Queries", @"assets\Images\iconFoldel.jpg");
            var qrLst = new TreeViewItem() { Header = stpQuery, ToolTip = String.Format("{0} rows", StaticParams.currentDb.Queries.Count) };
            foreach (PQuery item in StaticParams.currentDb.Queries)
            {
                StackPanel stpQueryFile = GetStackPanelRoot1("   " + item.QueryName, @"assets\Images\queryTree.png");
                qrLst.Items.Add(new TreeViewItem() { Header = stpQueryFile, ToolTip = item.QueryString });
            };
            root.Items.Add(qrLst);

            return root;
        }
    }
}
