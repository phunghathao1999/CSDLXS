   M S B A M L   `   `   `  ���� S  OPRDB_Sqlite.Presentation, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null[2clr-namespace:PRDB_Sqlite.Presentation.UserControl$PRDB_Sqlite.Presentation.UserControl  4    .PRDB_Sqlite.Presentation.UserControl.ucCETuple   U QUniversalWPF.RelativePanel, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullR NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     Dlocal2clr-namespace:PRDB_Sqlite.Presentation.UserControl     
���   �$��2q�$��5q�  � 	Resources   �� 
TargetType   	TabButton �� Property �� Value �� 
TargetType �� CornerRadius ��Name��%�   &	        �� " ��.���  � �� �	#FF5AF711��  �@��TextAlignment$ Center=���  � �� �� " ��.R��� $ 2,2,2,2�����	#FFB5DEFB
���   �$��1,1,1,1q�._��� $ contentPresenter��#��z��#��z��#��z��#��z��#��z��.���� .���� .+�`� .�c� .@��� -    ��Name$ btnClr��$��58��{�.  .����  �� Orientation$ 
Horizontal=�.+��� $��12��$��12��$F��@/PRDB_Sqlite.Presentation;component/assets/Images/clear-icon.png���� $�� Clear��$��12(�$��Center=��� �� -   $ btnView��.���� $ 
Horizontal=�.+��� $��12��$��12��$F��@/PRDB_Sqlite.Presentation;component/assets/Images/changeView.png���� $�� Change View Edit��$��12(�$��Center=��� �� $��Reload Value��$y�	Collapsed=��� $��
Infomation��$��15(�.���� .+��� $ 
Horizontal=�$��30,0,0,0q�.+��� $��Attribute Name:��$��13(��� -   	 ��Name$	 txtInfo��$��13(�
����  ��� $ 
Horizontal=�$��30,0,0,0q�$��33��.+��� $��Is Primary Key:��$��13(��� -   
 ��Name$
 chkPri��$��10q�$��15��
����  �{�.   d� 	IsChecked$ FalseY��� $ 
Horizontal=�$��30,0,0,0q�.+��� $��
Data Type:��$��13(��� -   $	 txtDataType��$��13(�
����  ��� .+��� -    ��Name$ rtbxCellContent��$��14(�$	��100��$��Consolas)�$��
5,10,10,10q�$	��100�����	#FFF7F2D2 �� 
IsReadOnly . $y�Visible=�  ��%3   'J�       �� " J�.���  �@��Margin$ 0q�&   System.Windows.Controls.DataGrid  -     Name$ dtgCellContent��$��Consolas)�$	��100��$	��100��{�.     AutoGenerateColumns .     ColumnWidth5  /System.Windows.Controls.DataGridLengthConverter$ *    CanUserDeleteRows .    CanUserAddRows . $ �� IsSynchronizedWithCurrentItem$
 TrueY�$��10q�$y�	Collapsed=�   Columns 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn   Header$ Value��   FontSize$ 13Q�1  +System.Windows.Controls.DataGridBoundColumn   Binding �� *value+�� $ 
Horizontal=�$��10q�$��LeftToRight=�$��Center=�.+��� -   $ btnCommitEdit��$��Accept Changes��$��25��#��[  `�  `� Orientation$ 
Horizontal=�$��RightToLeft=�.�c� .@��� -	   $ btnApply��{�.   � ToolTip$ Apply to DB��.���� $ Vertical=�.+��� $��70��$��70��$F��@/PRDB_Sqlite.Presentation;component/assets/Images/savedb (2).ico���� $��Apply Changes into DB��$��12(�$��Center=��� -
    ��Name$ 
txtMessage��$	��100��