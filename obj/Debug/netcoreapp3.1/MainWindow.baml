   M S B A M L   `   `   `  ����L  HQualifierAnalyzer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:QualifierAnalyzerQualifierAnalyzer  5   	   "    QualifierAnalyzer.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   1localclr-namespace:QualifierAnalyzer     5   	   $	��550��5   	   $	��825��5	   	     � 
ResizeMode$  CanMinimize=�5
   	    � Title$ Match Analyzer��5   	   .��5      � ���	#FF303336 � ColumnDefinitions 5   
   �� 5      $
��327* �6   �� 5      $
��337* �6   �� 5      $
��161* �6    � RowDefinitions 5   
   �� 5      $
��102* �6   �� 5      $
��115* �6   .+�5   
   �� $��20(�6   
�������
������ $��
6,0,-6,151q�6U   $��3��6i   $��WrapWithOverflow=�6}   ��Welcome to the Qualifier Analyzer, made by KritzKat. To use this, just put the url of the match in the box, click the button if its a team match or head to head match, and analyze! In it's current state it only tracks scores, but more will be added on later.6�  �� 5   
   -   5         ��Name$ urlBox��5       $��15(�6    J� RenderTransformOrigin$ 0.5,0.5&�60   $��6,83,38,135q�6P   �� 5   
   -   5        $��6,116,186,95q�5   -   .��6C   Analyze Head To Head6Y   �� 5   
   -   5        $��166,116,38,95q�5   *   .��6A   Analyze Team Match6U   �� 5   
   -   5         ��Name$ WinnerBlock��5      
�������$��	0,150,0,0q�6C   $
��Bold"�6V   $��25(�6h   $��3��6v   $��WrapWithOverflow=�6�   $��2��6�   $	��384��6�   $	��Top=�6�   5      5      