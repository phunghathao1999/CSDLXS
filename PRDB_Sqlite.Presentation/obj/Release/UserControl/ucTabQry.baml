   M S B A M L   `   `   `  ���� S  OPRDB_Sqlite.Presentation, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null[2clr-namespace:PRDB_Sqlite.Presentation.UserControl$PRDB_Sqlite.Presentation.UserControl  3    -PRDB_Sqlite.Presentation.UserControl.ucTabQry   U QUniversalWPF.RelativePanel, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullR NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     Dlocal2clr-namespace:PRDB_Sqlite.Presentation.UserControl     -   .���� .���� -   $��stpQry��.+��� -     ��Name$  stpTbl��   UniversalWPF.RelativePanel   AlignRightWithPanel .    AlignLeftWithPanel .  �� Orientation$ 
Horizontal=�.+�`� -    `�Name$ tlbQry��$��0��.�c� $��Normal"�.@��� -   $��	btnNewtab�� � ToolTip$ 	New a tab��.���� $ 
Horizontal=�.+��� $��12��$��12��$F��@/PRDB_Sqlite.Presentation;component/assets/Images/iconNewtab.png���� $�� New Tab��$��12(�$��Center=��� �� -   $��	btnOpnQry��$ Open a query file��.���� $ 
Horizontal=�.+��� $��12��$��12��$G��A/PRDB_Sqlite.Presentation;component/assets/Images/iconOpenTab.Png���� $�� Open Query��$��12(�$��Center=��� �� -   $��btnSave��$ 	Save file��.���� $ 
Horizontal=�.+��� $��12��$��12��$D��>/PRDB_Sqlite.Presentation;component/assets/Images/iconSave.png���� $�� Save��$��12(�$��Center=��� �� -   $��btnFind��$ Find in text��.���� $ 
Horizontal=�.+��� $��12��$��12��$D��>/PRDB_Sqlite.Presentation;component/assets/Images/iconFind.png���� $�� Find��$��12(�$��Center=��� �� -	   $��btnExec��$ Execute query��.���� $ 
Horizontal=�.+��� $��12��$��12��$F��@/PRDB_Sqlite.Presentation;component/assets/Images/IconExcute.png���� $�� Execute��$��12(�$��Center=��� �� -
   $��
btnExecCur��$ Execute Current line��.���� $ 
Horizontal=�.+��� $��12��$��12��$J��D/PRDB_Sqlite.Presentation;component/assets/Images/IconExcuteLine.png���� $�� Execute Line��$��12(�$��Center=��� �� �� -   $  stpTab��.+��� -   $��TbQry��.@��� -   $��label_TabTitle��$��	New Tab 1��$��0,0,0,0q�$��25��$��Bottom=�$��Courier)�$��14(�.���� -   $��rbxQry��$��Consolas)�$��20(� � ContextMenu �� $
��Left=�.@�m� $	��Cut�� m� Command$ ApplicationCommands.Cut��m� $
��Copy��$ ApplicationCommands.Copy��m� $��Paste��$ ApplicationCommands.Paste��m� $��Strategy��$��14(�.@��� $ Independence���� -   $��	btnCon_in��$��⊗_in (Conjunction) ��$
��Left=����	#FFC6E8FF$��10,0,0,0q�$	��150��$��25���� -   $��	btnDis_in��$��⊕_in (Disjunction)��$
��Left=����	#FFC6E8FF$��10,0,0,0q�$	��150��$��25���� -   $��	btnSub_in��$��⊖_in (Subtraction)��$
��Left=����	#FFC6E8FF$��10,0,0,0q�$	��150��$��25���� $ 	Ignorance���� -   $��	btnCon_ig��$��⊗_ig (Conjunction)��$
��Left=����	#FFC6E8FF$��10,0,0,0q�$	��150��$��25���� -   $��	btnDis_ig��$��⊕_ig (Disjunction)��$
��Left=����	#FFC6E8FF$��10,0,0,0q�$	��150��$��25���� -   $��	btnSub_ig��$��⊖_ig (Subtraction)��$
��Left=����	#FFC6E8FF$��10,0,0,0q�$	��150��$��25���� $ Mutual Exclusion���� -   $��	btnCon_me��$��⊗_me (Conjunction)��$
��Left=����	#FFC6E8FF$��10,0,0,0q�$	��150��$��25���� -   $��	btnDis_me��$��⊕_me (Disjunction)��$
��Left=����	#FFC6E8FF$��10,0,0,0q�$	��150��$��25���� -   $��	btnSub_me��$��⊖_me (Subtraction)��$
��Left=����	#FFC6E8FF$��10,0,0,0q�$	��150��$��25��m� $��Join Operator��.@��� -   $��
btnJoin_in��$��⋈ in��$��14(����	#FFB6D9F3$ Natural Join Independence��$	��120��$��25���� -   $��
btnJoin_ig��$��⋈ ig��$��14(����	#FFB6D9F3$ Natural Join Ignorance��$	��120��$��25���� -   $��
btnJoin_me��$��⋈ me��$��14(����	#FFB6D9F3$# Natural Join Mutual Exclusion��$	��120��$��25��m� $��Logic Operator��.@��� -   $��btnAnd��$��∧-AND��$��14(����	#FF7BDECC$ Natural Join Independence��$	��120��$��25���� -   $��btnOr��$��∨-OR��$��14(����	#FF7BDECC$ Natural Join Independence��$	��120��$��25��m� $��Query Operator��.@��� -   $��btnUnion��$��	⋃-UNION��$��14(����	#FFA9E8A3$ Union��$	��120��$��25���� -   $��btnInter��$��⋂-INTERSERT��$��14(����	#FFA9E8A3$ Intersertion��$	��120��$��25���� -   $��	btnExcept��$��\-EXCEPT��$��14(����	#FFA9E8A3$ Minus��$	��120��$��25���� -    $��stpData��.+�`� -!   $ tbDtgAction�� `� Orientation$ 
Horizontal=�.�c� .@��� -"   $��btnView��$
��View��$ Change View���� �� -#   $��
btnViewMsg��$��Show Message��$ View Message���� $��1��$��1��	 J� Opacity$	 1Q����	#FF9DBCCB
 �� CornerRadius$
 0��$��	20,2,20,5q�._��� -$    ��Name$ 
txtMessage��$y�	Collapsed=�$��Center=�$��Consolas)�$��13(�
�������
��� E�� �� 
IsReadOnly . $��70��.���� $ Vertical=�.+�&   System.Windows.Controls.DataGrid  -%   $��dtgDataResult��   
IsReadOnly .    CanUserAddRows .     ColumnWidth5  /System.Windows.Controls.DataGridLengthConverter$ * $��14(�$��Center=� .  .    GridLinesVisibility$	 All=�