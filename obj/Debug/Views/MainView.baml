   M S B A M L   `   `   `  ����  EtaModemConfigurator)    #EtaModemConfigurator.Views.MainView   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35a ]DevExpress.Xpf.Core.v18.1, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      j fDevExpress.Xpf.LayoutControl.v18.1, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49aDlc;http://schemas.devexpress.com/winfx/2008/xaml/layoutcontrol  5      e aDevExpress.Xpf.Controls.v18.1, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49aOdxwuinBhttp://schemas.devexpress.com/winfx/2008/xaml/windowsui/navigation  5      <dxc2http://schemas.devexpress.com/winfx/2008/xaml/core  5      $��0,0,0,0q�5
      &   DevExpress.Xpf.Core.ThemeManager    	ThemeName0  *DevExpress.Xpf.Core.ThemeNameTypeConverter$  MetropolisDark 5       � 	Resources 5         VerticalTile �� 
TargetType'  !DevExpress.Xpf.LayoutControl.Tile �� Property �� Value4  .DevExpress.Xpf.Core.HeaderedContentControlBase,  &DevExpress.Xpf.Core.ContentControlBase �� Path �� RelativeSource �� AncestorType  HorizontalTile �� Orientation��%/  &	        &	     �� 5   
   "  6%   .�5      ��  � ��$ Center=�69   �� 5       � ��$ Center=�6;   �� 5       �@ Size$ Small=�6%   �� 5       �@ HeaderTemplate 5      �� 5      5      �� 5       �@ ContentTemplate 5      �� 5      .R�5      �� .+�5      �� $
��None=�67   ��6$   �� 67   �� 5      $��14(�6{   $��0,5,0,0q�6�   $��Center=�6�   ��6(   �� $ Header� �� $ lc:TileL�6�   5      5      5      5       �� 5!   
   "  6'   .�5"      ��  � ��$ Center=�69   �� 5#       � ��$ Center=�6;   �� 5$       �@ Size$ Large=�6%   �� 5%       �@ HeaderTemplate 5&      �� 5'      5)      �� 5*       �@ ContentTemplate 5+      �� 5,      .R�5-      �� $ 
Horizontal=�6%   .+�5.      �� $
��None=�67   ��6$   �� 67   �� 5/      $��14(�6{   $��20,0,0,0q�6�   $��Center=�6�   ��6(   �� $ Header� �� $ lc:TileL�6�   50      51      53      54      55      .��56      � $��0,0,0,0q�6   .+�57   
   4  .DevExpress.Xpf.LayoutControl.TileLayoutControl  $��0,0,0,0q�6   $��Center=�60   $��Center=�6M   4  .DevExpress.Xpf.LayoutControl.LayoutControlBase	   Padding$	 10,10,10,10q�6h   '  !DevExpress.Xpf.Core.ScrollControl
   
ScrollBars$

 None=�6~   4	  .DevExpress.Xpf.LayoutControl.FlowLayoutControl 	  Orientation$ 
Horizontal=�6�   .+�58        -   5        #��[  58         Header$ COM-порт��6<   ���	#000000004
  .DevExpress.Xpf.WindowsUI.Navigation.Navigation 
  
NavigateTo$ ComPortSettingsView��6e   $��0,0,0,0q�6�   
���iii�$��3q�6�      Content. 59      ��  �� 	UriSource$8 2/EtaModemConfigurator;component/Images/ComPort.png?�6   5:        5;      -   5        #��[  5;      $ Сервер��6<   ���	#00000000$ 
ServerView��6c   $��45,0,0,0q�6�   
���iii�$��3q�6�   . 5<      �� $7 1/EtaModemConfigurator;component/Images/Server.png?�6   5=        5>      -   5        #��[  5>      $ Клиент��6<   ���	#00000000$ 
ClientView��6c   $��45,0,0,0q�6�   
���iii�$��3q�6�   . 5?      �� $7 1/EtaModemConfigurator;component/Images/Client.png?�6   5@      5A      5B      5C      