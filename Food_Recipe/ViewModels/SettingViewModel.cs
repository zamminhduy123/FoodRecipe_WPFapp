using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Food_Recipe.ViewModels
{
    class SettingViewModel :BaseViewModel
    {
        private bool _isShowSplash;
        public bool IsShowSplash { get => _isShowSplash; set { _isShowSplash = value; OnPropertyChanged(); } }

        private bool _isSmallItem;
        public bool IsSmallItem { get => _isSmallItem; set { _isSmallItem = value; OnPropertyChanged(); IsSmall = IsSmallItem; IsLarge = !IsSmallItem; } }
        private bool _isSmall;
        public bool IsSmall { get => _isSmall; set { _isSmall = value; OnPropertyChanged(); } }
        private bool _isLarge;
        public bool IsLarge { get => _isLarge; set { _isLarge = value; OnPropertyChanged(); } }

        private string _sortingType;
        public string SortingType
        {
            get => _sortingType; set
            {
                _sortingType = value; OnPropertyChanged(); 
                if (SortingType == "time")
                {
                    IsTimeSort = true;
                    IsAlphabetSort = false;
                }
                else if (SortingType == "alphabet")
                {
                    IsTimeSort = false;
                    IsAlphabetSort = true;
                }
            } }
        private bool _isAlphabetSort;
        public bool IsAlphabetSort { get => _isAlphabetSort; set { _isAlphabetSort = value; OnPropertyChanged(); } }
        private bool _isTimeSort;
        public bool IsTimeSort { get => _isTimeSort; set { _isTimeSort = value; OnPropertyChanged(); } }

        private bool _sortingWay;
        public bool SortingWay { get => _sortingWay; set { _sortingWay = value; OnPropertyChanged(); IsIncreaseSort = SortingWay; IsDecreaseSort = !SortingWay; } }
        private bool _isIncreaseSort;
        public bool IsIncreaseSort { get => _isIncreaseSort; set { _isIncreaseSort = value; OnPropertyChanged(); } }
        private bool _isDecreaseSort;
        public bool IsDecreaseSort { get => _isDecreaseSort; set { _isDecreaseSort = value; OnPropertyChanged(); } }

        public ICommand IsShowSplashCommand { get; set; }
        public ICommand SizeCommand { get; set; }
        public ICommand TypeSortCommand { get; set; }
        public ICommand WaySortCommand { get; set; }

        public SettingViewModel()
        {
            var value = ConfigurationManager.AppSettings["ShowSplashScreen"];
            IsShowSplash = bool.Parse(value);

            value = ConfigurationManager.AppSettings["IsSmallItem"];
            IsSmallItem = bool.Parse(value);

            value = ConfigurationManager.AppSettings["SortingType"];
            SortingType = value;

            value = ConfigurationManager.AppSettings["IsSmallItem"];
            SortingWay = bool.Parse(value);

            IsShowSplashCommand = new RelayCommand<object>((prop) => { return true; }, (prop) =>
            {
                var config = ConfigurationManager.OpenExeConfiguration(
                ConfigurationUserLevel.None);
                config.AppSettings.Settings["ShowSplashScreen"].Value = IsShowSplash.ToString();
                config.Save(ConfigurationSaveMode.Minimal);

                ConfigurationManager.RefreshSection("appSettings");

            });

            SizeCommand = new RelayCommand<string>((prop) => { return true; }, (prop) =>
            {
                var config = ConfigurationManager.OpenExeConfiguration(
                ConfigurationUserLevel.None);
                config.AppSettings.Settings["IsSmallItem"].Value = prop;
                config.Save(ConfigurationSaveMode.Minimal);

                ConfigurationManager.RefreshSection("appSettings");

                IsSmallItem = bool.Parse(prop);
            });

            TypeSortCommand = new RelayCommand<string>((prop) => { return true; }, (prop) =>
            {
                var config = ConfigurationManager.OpenExeConfiguration(
                ConfigurationUserLevel.None);
                config.AppSettings.Settings["SortingType"].Value = prop;
                config.Save(ConfigurationSaveMode.Minimal);

                ConfigurationManager.RefreshSection("appSettings");

                SortingType = prop;
            });

            WaySortCommand = new RelayCommand<string>((prop) => { return true; }, (prop) =>
            {
                var config = ConfigurationManager.OpenExeConfiguration(
                ConfigurationUserLevel.None);
                config.AppSettings.Settings["SortingWay"].Value = prop;
                config.Save(ConfigurationSaveMode.Minimal);

                ConfigurationManager.RefreshSection("appSettings");

                SortingWay = bool.Parse(prop);
            });
        }
    }
}
