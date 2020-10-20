using System;
using System.Collections.Generic;
using System.Configuration;
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
using System.Windows.Threading;

namespace Project_1
{
    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        private const int _time_SplashScreen = 3;
        private readonly DispatcherTimer dT = new DispatcherTimer();

        public SplashScreen()
        {
            InitializeComponent();
        }

        private void dt_Tick(object sender, EventArgs e)
        {
            MainWindow mW = new MainWindow();
            mW.Show();

            dT.Stop();
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Hide();
            var value = ConfigurationManager.AppSettings["ShowSplashScreen"];
            bool showSplash = bool.Parse(value);
            if (showSplash == false)
            {
                MainWindow mW = new MainWindow();
                mW.Show();

                this.Close();
            }
            else
            {
                this.Show();
                dT.Tick += new EventHandler(dt_Tick);
                dT.Interval = new TimeSpan(0, 0, _time_SplashScreen);
                dT.Start();
            }
        }
    }
}
