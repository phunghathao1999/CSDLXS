   M S B A M L   `   `   `  ���� S  OPRDB_Sqlite.Presentation, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullQ-clr-namespace:PRDB_Sqlite.Presentation.ScreenPRDB_Sqlite.Presentation.Screen  3    -PRDB_Sqlite.Presentation.Screen.EditingSchema   U QUniversalWPF.RelativePanel, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullR NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     ?local-clr-namespace:PRDB_Sqlite.Presentation.Screen       � WindowStartupLocation$  CenterScreen=� � Title$ Edit schema�� � WindowStyle$ ThreeDBorderWindow=� � 
ResizeMode$ NoResize=�$	��685��$	��440�����#f0f0f0$��Arial)�.��]� .+��� $��0,0,0,0q�$	��680��$	��410��.+��� $�� ��$��Normal"�$��16(�$	��0,0q�$��0,0,0,0q�$��50��.���� .����  �� Orientation$ 
Horizontal=�$��Center=�.+��� -    ��Name$ 
cbxSchName��$��13(�$	��200��$��Normal"�$��Center=�
���   �$��Center=��� $��Current name:��$��13(�
���   �$��Normal"��� -    ��Name$ txtName��$��13(�$��20��
���   �$	��200��$��Normal"�$��Center=� �� TextWrapping$ WrapWithOverflow=�$��Center=�`�  `� 
Background �#f0f0f0.�c� .@��� -   	 ��Name$	 	btnAddRow��$��Add New Row��$
��Bold"��� -   $	 	btnDelRow��$��Delele Current Row��$
��Bold"�&   System.Windows.Controls.DataGrid  -   
  Name$	
 dtg��$��14(�$	��200��   CanUserSortColumns .  $ �� IsSynchronizedWithCurrentItem$
 trueY�$	��275��   ColumnWidth5  /System.Windows.Controls.DataGridLengthConverter$ *    Columns 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn   Header$ Attribute Name��1  +System.Windows.Controls.DataGridBoundColumn   Binding �� *
attrName+4  .System.Windows.Controls.DataGridTemplateColumn  $ 	isPrimary��   CellTemplate �� .R���  d� 	IsChecked �� *isPri+ �� UpdateSourceTrigger$ PropertyChanged=�  $ DataType��   
IsReadOnly .  �� *
datatype+   	CellStyle ��  �� 
TargetType*  $System.Windows.Controls.DataGridCell"  .�-     $ Domain�� .  �� *domain+  $ Description�� �� *descs+�� $ 
Horizontal=�$	��185��$��
450,17,0,0q�.+��� -   $	 btnSave��$��0q����� ._��� $��OK��$��80�� �� Padding$ 0,1,0,0q� �� TextAlignment$ Center=�$��Center=��� -   $	 	btnCancel��$��0q�$��20,0,0,0q����� ._��� $��CANCEL��$��80��$ 0,1,0,0q�$ Center=�$��Center=�