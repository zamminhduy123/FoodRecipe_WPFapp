using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Project_1.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        #region private 
        private Visibility _isSettingBackgroundVisible = Visibility.Hidden;
        private bool _isLoaded = false;
        private List<Recipe> _recipes = new List<Recipe>();
        #endregion

        public ICommand AllRecipeCommand { get; set; }
        public ICommand FavoriteCommand { get; set; }
        public ICommand NewRecipeCommand { get; set; }
        public ICommand DeleteRecipeCommand { get; set; }

        public ICommand ButtonCloseSettingCommand { get; set; }

        public ICommand SettingCommand { get; set; }

        public ICommand SettingWindowOpenCommand { get; set; }

        public Visibility SettingBackground { get => _isSettingBackgroundVisible; set { _isSettingBackgroundVisible = value; OnPropertyChanged(); } }

        public List<Recipe> Recipes { get=>_recipes; set { _recipes = value;OnPropertyChanged(); } }

        public MainViewModel()
        {
            
            if (_isLoaded)
            {
                _isLoaded = true;    
            }
            
            //set the command
            AllRecipeCommand = new RelayCommand<object>((prop) => { return true; }, (prop) =>
            {
            });

            SettingCommand = new RelayCommand<object>((prop) => { return true; }, (prop) =>
            {
                Setting_Click();
            });

            ButtonCloseSettingCommand = new RelayCommand<object>((prop) => { return true; }, (prop) =>
            {
                Setting_Click();
            });

            SettingWindowOpenCommand = new RelayCommand<object>((prop) => { return true; }, (prop) =>
            {
                UserPreferenceWindow userPreferenWindow = new UserPreferenceWindow();
                userPreferenWindow.ShowDialog();
            });








            //testing binding
            Recipes = GetRecipes();
           
        }

   

        /**
         * handle setting click
         * @param none
         */
        private void Setting_Click()
        {
            if (SettingBackground == Visibility.Hidden)
            {
                SettingBackground =  Visibility.Visible;
            }
            else
            {
                SettingBackground = Visibility.Hidden;
            }
        }

        //class for testing
        class ListItemViewModel
        {
            public string Name1 { get; set; }
            public string Name2 { get; set; }
        }
        private List<Recipe> GetRecipes()
        {
            return new List<Recipe>()
            {
                new Recipe("Recipe 1", 205.46, "/Images/hamburger.jpeg"),
                new Recipe("Recipe 2", 102.50, "/Images/hamburger.jpeg"),
                new Recipe("Recipe 3", 400.99, "/Images/hamburger.jpeg"),
                new Recipe("Recipe 4", 350.26, "/Images/hamburger.jpeg"),
                new Recipe("Recipe 5", 150.10, "/Images/hamburger.jpeg"),
                new Recipe("Recipe 6", 100.02, "/Images/hamburger.jpeg"),
                new Recipe("Recipe 7", 295.25, "/Images/hamburger.jpeg"),
                new Recipe("Recipe 8", 700.00, "/Images/hamburger.jpeg")
             };
        }

        public class Recipe
        {
            public string Name { get; set; }
            public double Value { get; set; }
            public string Image { get; set; }

            public Recipe(string name, double value, string image)
            {
                Name = name;
                Value = value;
                Image = image;
            }
        }
    }
}
