#pragma checksum "..\..\..\UserControl\ucCETuple.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "E6DA2180D9BA6548CECAC4DBF44DE0176E8C6C6AD4B45CA0C31006CDA32E4AA1"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using PRDB_Sqlite.Presentation.UserControl;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using UniversalWPF;


namespace PRDB_Sqlite.Presentation.UserControl {
    
    
    /// <summary>
    /// ucCETuple
    /// </summary>
    public partial class ucCETuple : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 33 "..\..\..\UserControl\ucCETuple.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnClr;
        
        #line default
        #line hidden
        
        
        #line 40 "..\..\..\UserControl\ucCETuple.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnView;
        
        #line default
        #line hidden
        
        
        #line 54 "..\..\..\UserControl\ucCETuple.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label txtInfo;
        
        #line default
        #line hidden
        
        
        #line 58 "..\..\..\UserControl\ucCETuple.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox chkPri;
        
        #line default
        #line hidden
        
        
        #line 62 "..\..\..\UserControl\ucCETuple.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label txtDataType;
        
        #line default
        #line hidden
        
        
        #line 69 "..\..\..\UserControl\ucCETuple.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RichTextBox rtbxCellContent;
        
        #line default
        #line hidden
        
        
        #line 82 "..\..\..\UserControl\ucCETuple.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid dtgCellContent;
        
        #line default
        #line hidden
        
        
        #line 102 "..\..\..\UserControl\ucCETuple.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnCommitEdit;
        
        #line default
        #line hidden
        
        
        #line 108 "..\..\..\UserControl\ucCETuple.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnApply;
        
        #line default
        #line hidden
        
        
        #line 115 "..\..\..\UserControl\ucCETuple.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock txtMessage;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/PRDB_Sqlite.Presentation;component/usercontrol/uccetuple.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\UserControl\ucCETuple.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.btnClr = ((System.Windows.Controls.Button)(target));
            
            #line 33 "..\..\..\UserControl\ucCETuple.xaml"
            this.btnClr.Click += new System.Windows.RoutedEventHandler(this.btnClr_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            this.btnView = ((System.Windows.Controls.Button)(target));
            
            #line 40 "..\..\..\UserControl\ucCETuple.xaml"
            this.btnView.Click += new System.Windows.RoutedEventHandler(this.btnView_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.txtInfo = ((System.Windows.Controls.Label)(target));
            return;
            case 4:
            this.chkPri = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 5:
            this.txtDataType = ((System.Windows.Controls.Label)(target));
            return;
            case 6:
            this.rtbxCellContent = ((System.Windows.Controls.RichTextBox)(target));
            return;
            case 7:
            this.dtgCellContent = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 8:
            this.btnCommitEdit = ((System.Windows.Controls.Button)(target));
            
            #line 103 "..\..\..\UserControl\ucCETuple.xaml"
            this.btnCommitEdit.Click += new System.Windows.RoutedEventHandler(this.btnCommitEdit_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.btnApply = ((System.Windows.Controls.Button)(target));
            return;
            case 10:
            this.txtMessage = ((System.Windows.Controls.TextBlock)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

