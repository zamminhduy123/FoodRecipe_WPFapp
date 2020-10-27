using Food_Recipe.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace Food_Recipe.ViewModels
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

        public Global globalTheme = Global.GetInstance();

        public ICommand ClosePermanent { get; set; }
        public ICommand LoadedWindowCommand { get; set; }
        public int ProgressBarValue { get => _progressBarValue; set { _progressBarValue = value; OnPropertyChanged(); } }
        #endregion
        public SplashScreenViewModel()
        {
            globalTheme.ThemeColor = "#FFa500";
            Tip = DataProvider.Ins.DB.Tips.ToList()[MyRandom.Ins.Next(DataProvider.Ins.DB.Tips.Count())].Content;

            ClosePermanent = new RelayCommand<object>((prop) => { return true; }, (prop) =>
            {
                var config = ConfigurationManager.OpenExeConfiguration(
                ConfigurationUserLevel.None);
                config.AppSettings.Settings["ShowSplashScreen"].Value = "false";
                config.Save(ConfigurationSaveMode.Minimal);

                ConfigurationManager.RefreshSection("appSettings");
            });

            LoadedWindowCommand = new RelayCommand<Window>((prop) => { return true; }, (splash) =>
            {
                
                splash.Hide();
                var value = ConfigurationManager.AppSettings["ShowSplashScreen"];
                bool showSplash = bool.Parse(value);
                if (showSplash == false)
                {
                    if (MainViewModel.IsShowed == false)
                    {
                        MainWindow mW = new MainWindow();
                        mW.Show();
                        MainViewModel.IsShowed = true;
                    }
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