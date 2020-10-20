using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Project_1.ViewModels
{
    public class UserPreferenceViewModel : BaseViewModel
    {
        #region Fields

        private BaseViewModel _currentPageViewModel = new SettingViewModel();

        #endregion

        #region Properties, Command
        public ICommand FinishCommand { get; private set; }
        public ICommand SettingsCommand { get;  set; }
        public ICommand ThemeColorCommand { get; set; }
        public ICommand AboutCommand { get; set; }

        
        public BaseViewModel CurrentPageViewModel { get => _currentPageViewModel; set { _currentPageViewModel = value; OnPropertyChanged(); } }

        #endregion

        public UserPreferenceViewModel()
        {
            //set the command

            //close the window if user click the finish button
            FinishCommand = new RelayCommand<Window>((prop) => { return true; }, (prop) =>
            {
                CloseWindow(prop);
            });

            //switch to view
            SettingsCommand = new RelayCommand<ContentControl>((prop) => { return true; }, (prop) =>
            {
                prop.Content = new SettingViewModel();
                CurrentPageViewModel = new SettingViewModel();
            });
            ThemeColorCommand = new RelayCommand<ContentControl>((prop) => { return true; }, (prop) =>
            {
                prop.Content = new ThemeViewModel();
                CurrentPageViewModel = new ThemeViewModel();
            });
            AboutCommand = new RelayCommand<ContentControl>((prop) => { return true; }, (prop) =>
            {
                prop.Content = new AboutViewModel();
                CurrentPageViewModel = new AboutViewModel();
            });

        }

        #region Methods
        private void CloseWindow(Window window)
        {
            if (window != null)
            {
                window.Close();
            }
        }

        #endregion
    }
}
