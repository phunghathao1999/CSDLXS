using PRDB_Sqlite.Domain.Unit;
using PRDB_Sqlite.Infractructure.Common;
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
    /// Interaction logic for Inp_DataType.xaml
    /// </summary>
    public partial class Inp_DataType : Window
    {

        #region Properties

        private char[] SpecialCharacter = new char[] { '~', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '+', '`', ';', ',', '<', '>', '?', '/', ':', '\"', '\'', '=', '{', '}', '[', ']', '\\', '|' };
        public string specialcharacter { get; set; }
        public PDataType dataType = new PDataType();
        public string valueType = "";

        #endregion
        public Inp_DataType()
        {
            InitializeComponent();
            this.rbValls.SetValue(Paragraph.LineHeightProperty, 0.5);

        }
        public Inp_DataType(string dataType, string domainString)
        {
            InitializeComponent();

            this.rbValls.SetValue(Paragraph.LineHeightProperty, 0.5);


            if(dataType is null)
            {
                this.cbxDtt.ItemsSource = Parameter.datatype;
                this.cbxDtt.SelectedIndex = 0;
            }
            dataType = this.cbxDtt.SelectedItem.ToString();

            if (SetDomain(dataType) == String.Empty)
            {
                this.dataType.DomainString = domainString;
                this.dataType.TypeName = "UserDefined";
                this.dataType.DataType = dataType;
                this.dataType.GetDomain(domainString);
                this.valueType = dataType;
            }
            else
                this.valueType = dataType;
        }
        private string SetDomain(string S) //Gán trường giá trị cho các kiểu
        {
            switch (S)
            {
                case "Int16": return "[-32768  ...  32767]";
                case "Int32": return "[-2147483648  ...  2147483647]";
                case "Int64": return " [-9223372036854775808  ...  9223372036854775807]";
                case "Byte": return "[0  ...  255]";
                case "String": return "[0  ...  32767] characters";
                case "Single": return "[1.5 x 10^-45  ...  3.4 x 10^38]";
                case "Double": return "[5.0 x 10^-324  ...  1.7 x 10^308]";
                case "Boolean": return "true  /  false";
                case "Decimal": return "[1.0 x 10^-28  ...  7.9 x 10^28]";
                case "DateTime": return "[01/01/0001 C.E  ...  31/12/9999 A.D]";
                case "Binary": return "[1  ...  8000] bytes";
                case "Currency": return "[-2^-63  ...  2^63 - 1]";
            }
            return "";
        }
        private string Stdize(string S) //Standardize String
        {
            // Chuẩn hóa chuỗi cắt bỏ các dấu , dư thừa
            string R = "";
            int i = 0;
            while (S[i] == ',') i++;
            int k = S.Length - 1;
            while (S[k] == ',') k--;
            for (int j = i; j <= k; j++)
                if (S[j] != ',') R += S[j];
                else if (S[j - 1] != ',') R += S[j] + " ";
            return R;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            var content = String.Empty;
            content = new TextRange(this.rbValls.Document.ContentStart, this.rbValls.Document.ContentEnd).Text;
            if (this.cbxDtt.SelectedIndex != -1)
            {

                if (this.txtTypeName.IsEnabled)
                {
                    if (txtTypeName.Text.Trim() == "")
                    {
                        MessageBox.Show("You have not entered type name and value type");
                        return;
                    }

                    if (content.Trim() == "")
                    {
                        MessageBox.Show("You have not entered a value type");
                        return;
                    }

                    dataType.TypeName = txtTypeName.Text;
                    dataType.DataType = cbxDtt.SelectedItem.ToString();
                    dataType.DomainString = String.Format("{{{0}}}", Stdize(content.Replace(Environment.NewLine, ",")));
                }
                else
                {
                    dataType.TypeName = this.valueType;
                    dataType.DataType = cbxDtt.SelectedItem.ToString();
                    dataType.DomainString = SetDomain(dataType.DataType);
                }
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void cbxDtt_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.cbxDtt.SelectedItem.ToString().Contains("UserDefined"))
            {
                this.txtTypeName.IsEnabled = true;
                this.rbValls.IsEnabled = true;
            }
            else
            {
                this.txtTypeName.IsEnabled = false;
                this.rbValls.IsEnabled = false;
            }
            this.valueType = this.cbxDtt.SelectedItem.ToString();
        }
    }
}
