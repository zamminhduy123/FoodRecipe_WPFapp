using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace Project_1.ViewModels
{
    public class SplashScreenViewModel : BaseViewModel
    {
        #region private 
        private string _tip;
        private const int _time_SplashScreen = 3000;
        private const int Interval = 3000 / (24 * 3); //24 fps
        private readonly Timer dT = new Timer(Interval);

        private int _progressBarValue = 0;

        #endregion

        #region Properties

        public string Tip
        {
            get { return _tip; }
            set { _tip = value; }
        }



        public ICommand ClosePermanent { get; set; }
        public ICommand LoadedWindowCommand { get; set; }
        public int ProgressBarValue { get => _progressBarValue; set { _progressBarValue = value; OnPropertyChanged(); } }
        #endregion
        public SplashScreenViewModel()
        {
            Tip = "Bạn có thể đổ rượu vang uống không hết vào khay và làm đông, đợi đến khi nấu các món soup hoặc hấp sẽ mang ra dùng.";

            ClosePermanent = new RelayCommand<object>((prop) => { return true; }, (prop) =>
            {
                var config = ConfigurationManager.OpenExeConfiguration(
                ConfigurationUserLevel.None);
                config.AppSettings.Settings["ShowSplashScreen"].Value = "false";
                config.Save(ConfigurationSaveMode.Minimal);
            });

            LoadedWindowCommand = new RelayCommand<Window>((prop) => { return true; }, (splash) =>
            {
                splash.Hide();
                var value = ConfigurationManager.AppSettings["ShowSplashScreen"];
                bool showSplash = true;
                if (showSplash == false)
                {
                    MainWindow mW = new MainWindow();
                    mW.Show();

                    splash.Close();
                }
                else
                {
                    splash.Show();
                    dT.Elapsed += dt_Tick;
                    dT.Start();
                }

            });
        }

        void dt_Tick(object sender, EventArgs e)
        {
            ProgressBarValue += Interval;
            if (ProgressBarValue >= _time_SplashScreen)
            {
                dT.Dispose();
                //thisWindow.Close();
                //MainWindow mW = new MainWindow();
                //mW.Show();
            }
            else
            {
                dT.Start();
            }

        }



    }
}