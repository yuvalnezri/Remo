   M S B A M L   `   `   `  ����  Remo    Remo.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   h dMicrosoft.Kinect.Toolkit.Controls, Version=1.7.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35_ [Microsoft.Kinect.Toolkit, Version=1.7.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e352k(http://schemas.microsoft.com/kinect/2013   5   	     � Title$
  Remo��5   	   $	��396��6   $	��500��6#    � WindowStyle$
 None=�5   	    � AllowsTransparency . 
������  � WindowStartupLocation$ CenterScreen=�5   	    � WindowState$ 	Maximized=�6.    � Topmost .  � ShowInTaskbar .  .��5
      � .+�5   
   �� -   5         ��Name$ detectedGestureLabel��5      $�� ��6/   $��Center=�6:   $��40��6W   $	��Top=�6c   $	��492��6{   $��24(�6�   $
��Bold"�6�   $��Center=�6�   ���	#FF88D9FF6�   4  .Microsoft.Kinect.Toolkit.KinectSensorChooserUI  5   
   -   5          Name$ sensorChooserUi��5   "   $��Center=�6;   $	��Top=�6X   4  .Microsoft.Kinect.Toolkit.Controls.KinectRegion  5   
   -   5        	  Name$	 kinectRegion��5      
   IsCursorVisible
 .  6/   8  2Microsoft.Kinect.Toolkit.Controls.KinectUserViewer  5   
   -   5        $��userViewerUI��5      $��Bottom=�61   $��Center=�6L   $	��205��5   H   $��128,0,131,0q�6U   $	��233��6j      DefaultUserColor$ 	#FF97908C��6v      PrimaryUserColor$ 	#FF2C6AD6��6�   $y�Hidden=�6�      KinectRegion 6   ��  �� ElementName$ kinectRegion��6�   5      5      