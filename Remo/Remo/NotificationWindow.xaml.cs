using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace Remo
{
    /// <summary>
    /// Interaction logic for NotificationWindow.xaml
    /// </summary>
    public partial class NotificationWindow : Window
    {

        Timer t;

        public NotificationWindow(string msg)
        {
            InitializeComponent();
            this.Rona.Content = msg;
            this.Loaded += NotificationWindow_Loaded;
        }

        void NotificationWindow_Loaded(object sender, RoutedEventArgs e)
        {
            t = new Timer();
            t.Interval = 3000;
            t.Elapsed += new ElapsedEventHandler(t_Elapsed);
            t.Start();
        }


        void t_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.Close();
            }), null);
        }

    }
}
