using Food_Recipe.Model;
using Food_Recipe.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace Food_Recipe
{
    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        private const int Interval = 3600;
        private readonly Timer dT = new Timer(Interval);
        public SplashScreen()
        {
            InitializeComponent();
            dT.Elapsed += dt_Tick;
            dT.Start();
        }
        void dt_Tick(object sender, EventArgs e)
        {
            dT.Dispose();
            Dispatcher.Invoke(() =>
            {
                if (MainViewModel.IsShowed == false)
                {
                    MainWindow mW = new MainWindow();
                    mW.Show();
                    MainViewModel.IsShowed = true;
                }
                this.Close();
            });
            

        }
    }
}