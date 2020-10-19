using System;
using System.Collections.Generic;
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
        private readonly DispatcherTimer dT;

        private bool _isShow = true;

        public SplashScreen()
        {
            InitializeComponent();

            if (_isShow == false)
            {
                MainWindow mW = new MainWindow();
                mW.Show();

                this.Close();
            }

            dT = new DispatcherTimer();
            dT.Tick += new EventHandler(dt_Tick);
            dT.Interval = new TimeSpan(0, 0, _time_SplashScreen);
            dT.Start();
        }
        
        private void dt_Tick(object sender, EventArgs e)
        {
            MainWindow mW = new MainWindow();
            mW.Show();

            dT.Stop();
            this.Close();
        }
    }
}
