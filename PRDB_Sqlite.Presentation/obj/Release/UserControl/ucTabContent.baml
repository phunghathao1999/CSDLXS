   M S B A M L   `   `   `  ���� S  OPRDB_Sqlite.Presentation, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null[2clr-namespace:PRDB_Sqlite.Presentation.UserControl$PRDB_Sqlite.Presentation.UserControl  7    1PRDB_Sqlite.Presentation.UserControl.ucTabContent   U QUniversalWPF.RelativePanel, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullR NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     Dlocal2clr-namespace:PRDB_Sqlite.Presentation.UserControl     -   $��12(�
������ 
����  �  � 	Resources     	cellStyle �� 
TargetType*  $System.Windows.Controls.DataGridCell �� Property �� Value �� 
TargetType ��Name �� CornerRadius �� Triggers �� Value	 �� Binding
 �� RelativeSource �� Path �� 
TargetName  rowStyle)  #System.Windows.Controls.DataGridRow  TabButtonDelete ��Name  TabButtonUpdate��%�  &	        &	 %    &	 �    &	 y    �� "  .���  � ��$ 0q���  � ��$ 2q���  � ��
 �������  � �� �� "  .R��� $��0q�._��� $ border��
�������$��2q�
���   �$ 5��._���  �� $
 true��	 �� 
 �� *Self+$ 
IsSelected�.a��� $ border�� � ��
 � ����� "  .���  � ��$ 0q���  � ��$ 0q���  � ��$ 0q���  � ��
 ������� " ��.���  � �� �	#FF5AF711��  �@��TextAlignment$ Center=���  � �� �� " ��.R��� $ 2,2,2,2��
���   �$��1,1,1,1q����	#FFF99F8B._��� $ contentPresenter��#��z��#��z��#��z��#��z��#��z���� " ��.���  � �� �	#FF5AF711��  �@��TextAlignment$ Center=���  � �� �� " ��.R��� $ 2,2,2,2��$��0,0,5,0q����	#FFF3E277
���   �$��1,1,1,1q�._��� $ contentPresenter��#��z��#��z��#��z��#��z��#��z��.��� $��	0,10,10,0q� � RowDefinitions �� $��* ��� $��* ��� $��9* �.+���  �� Orientation$ 
Horizontal=�$��0��.+��� -   $��lblCbx��$��Center=�$��13(��� -   $	��cbx��$	��300��$��25��
����� � �� SelectedValuePath$	 Key�� �� DisplayMemberPath$ Value���� -   $��	stpAction��$��RightToLeft=�$ 
Horizontal=�$��0,36,0,0q�$��2��.+��� $��Action CRUD��$��50��$��13(�$��0,-6,0,0q�.���� $ 
Horizontal=�$��0,-17,0,-14q�.+��� -    ��Name$ btnEdit�� � ToolTip$ Enable Editing��$��Enable Update��$��13(�$	��100��$��25��#��[ ���	#FF11E2F7$	��0,4q��� -   $ btnDel��$ Delelte New Record��$��Delete��$��13(�$��25��$	��100��#��[ ���	#FF85E25A�� �� -    ��Name$ gbDbact��$��Action Database��$y�	Collapsed=�.���� $ 
Horizontal=�.+��� -   $��btnCmmt��$ Commit Changes��$��Commit���� -	   $��btnRbk��$ Rollback Changes��$��Rollback��� -
    �Name$ grdDtg��$��2�� � ColumnDefinitions �� $��5* ��� $��2* �.+�&   System.Windows.Controls.DataGrid  -   $	��dtg��$��0��   
IsReadOnly .    SelectionMode$ Single=�   CanUserAddRows . 
�������   ColumnWidth5  /System.Windows.Controls.DataGridLengthConverter$ * $	��300��   GridLinesVisibility$	 All=�$��Center=�4   .PRDB_Sqlite.Presentation.UserControl.ucCETuple  -     Name$ ucEdit��$��1��$��2��$	��100��
�������{�. 