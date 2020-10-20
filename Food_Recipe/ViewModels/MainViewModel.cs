using Food_Recipe.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using YoutubePlayerLib.Cef;

namespace Food_Recipe.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        #region private 
        private Visibility _isSettingBackgroundVisible = Visibility.Hidden;
        private Visibility _isVideoShown = Visibility.Hidden;
        private Visibility _isAvaShown = Visibility.Visible;
        private bool _isLoaded = true;
        private Uri _videoSource = new Uri("https://www.google.com");
        private List<Ingredient> _ingredients = new List<Ingredient>();
        private String _videoID = "9i4SKHbhbqk";
        #endregion

        public static bool IsShowed = false;

        public ICommand AllRecipeCommand { get; set; }
        public ICommand FavoriteCommand { get; set; }
        public ICommand NewRecipeCommand { get; set; }
        public ICommand DeleteRecipeCommand { get; set; }

        public ICommand ButtonCloseSettingCommand { get; set; }

        public ICommand SettingCommand { get; set; }

        public ICommand SettingWindowOpenCommand { get; set; }

        public ICommand PrevImgStepCommand { get; set; }
        public ICommand NextImgStepCommand { get; set; }
        public ICommand NextStepCommand { get; set; }
        public ICommand PrevStepCommand { get; set; }

        public ICommand NextListCommand { get; set; }
        public ICommand PrevListCommand { get; set; }

        public ICommand WatchVideoCommand { get; set; }

        //Visibility binding
        public Visibility SettingBackground { get => _isSettingBackgroundVisible; set { _isSettingBackgroundVisible = value; OnPropertyChanged(); } }
        public Visibility VideoVisibility { get => _isVideoShown; set { _isVideoShown = value; OnPropertyChanged(); } }
        public Visibility AvatarVisibility {  get => _isAvaShown; set { _isAvaShown = value; OnPropertyChanged(); }  }

        public String VideoId { get => _videoID; set { _videoID = value; OnPropertyChanged(); } }



        public List<Ingredient> Ingredients { get => _ingredients; set { _ingredients = value; OnPropertyChanged(); } }
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
                //handle setting click
                Setting_Click();
            });

            ButtonCloseSettingCommand = new RelayCommand<object>((prop) => { return true; }, (prop) =>
            {
                //handle setting click
                Setting_Click();
            });

            SettingWindowOpenCommand = new RelayCommand<object>((prop) => { return true; }, (prop) =>
            {
                UserPreferenceWindow userPreferenWindow = new UserPreferenceWindow();
                userPreferenWindow.ShowDialog();
            });

            NewRecipeCommand = new RelayCommand<object>((prop) => { return true; }, (prop) =>
            {
                NewRecipeWindow newRecipeWindow = new NewRecipeWindow();
                newRecipeWindow.ShowDialog();
            });

            NextImgStepCommand = new RelayCommand<object>((prop) => { return true; }, (prop) =>
            {
                //handle next step's image button
            });
            PrevImgStepCommand = new RelayCommand<object>((prop) => { return true; }, (prop) =>
            {
                //handle previous step's image button
            });
            NextStepCommand = new RelayCommand<object>((prop) => { return true; }, (prop) =>
            {
                //handle next step button
            });

            PrevStepCommand = new RelayCommand<object>((prop) => { return true; }, (prop) =>
            {
                //handle previous image button
            });
            NextListCommand = new RelayCommand<object>((prop) => { return true; }, (prop) =>
            {
                //handle next list button
            });
            PrevListCommand = new RelayCommand<object>((prop) => { return true; }, (prop) =>
            {
                //handle next previous button
            });

            WatchVideoCommand = new RelayCommand<Button>((prop) => { return true; }, (StartVideoBtn) =>
            {
                //handle watch video button
                OpenVideo();
            });

            //testing binding
        }
        private void OpenVideo()
        {
            if (VideoVisibility == Visibility.Hidden)
            {
                VideoVisibility = Visibility.Visible;
                AvatarVisibility = Visibility.Hidden;
            }
        }


        /**
         * handle setting click. Change background visibility
         * @param none
         */
        private void Setting_Click()
        {
            if (SettingBackground == Visibility.Hidden)
            {
                SettingBackground = Visibility.Visible;
            }
            else
            {
                SettingBackground = Visibility.Hidden;
            }
        }

        ////class for testing
        //public class Ingredient
        //{
        //    public string Name { get; set; }
        //    public string Quantity { get; set; }

        //    public Ingredient(string a,string b) 
        //    {
        //        Name = a;
        //        Quantity = b;
        //    }
        //}

        //private List<Ingredient> GetIngredients()
        //{
        //    return new List<Ingredient>(){
        //        new Ingredient("Beef","100gr"),
        //        new Ingredient("Beef","100gr"),
        //        new Ingredient("Beef", "100gr")
        //    };
        //}


        //private List<Recipe> GetRecipes()
        //{
        //    return new List<Recipe>()
        //    {
        //        new Recipe("Recipe 1", 205.46, "/Images/hamburger.jpeg"),
        //        new Recipe("Recipe 2", 102.50, "/Images/hamburger.jpeg"),
        //        new Recipe("Recipe 3", 400.99, "/Images/hamburger.jpeg"),
        //        new Recipe("Recipe 4", 350.26, "/Images/hamburger.jpeg"),
        //        new Recipe("Recipe 5", 150.10, "/Images/hamburger.jpeg"),
        //        new Recipe("Recipe 6", 100.02, "/Images/hamburger.jpeg"),
        //        new Recipe("Recipe 7", 295.25, "/Images/hamburger.jpeg"),
        //        new Recipe("Recipe 8", 700.00, "/Images/hamburger.jpeg")
        //     };
        //}

        //public class Recipe
        //{
        //    public string Name { get; set; }
        //    public double Value { get; set; }
        //    public string Image { get; set; }

        //    public Recipe(string name, double value, string image)
        //    {
        //        Name = name;
        //        Value = value;
        //        Image = image;
        //    }
        //}

        
            //private  string GetYouTubeVideoPlayerHTML(string videoCode)
            //{
            //    var sb = new StringBuilder();

            //    const string YOUTUBE_URL = @"http://www.youtube.com/v/";

            //    sb.Append("<html>");
            //    sb.Append("    <head>");
            //    sb.Append("        <meta name=\"viewport\" content=\"width=device-width; height=device-height;\">");
            //    sb.Append("    </head>");
            //    sb.Append("    <body marginheight=\"0\" marginwidth=\"0\" leftmargin=\"0\" topmargin=\"0\" style=\"overflow-y: hidden\">");
            //    sb.Append("        <object width=\"100%\" height=\"100%\">");
            //    sb.Append("            <param name=\"movie\" value=\"" + YOUTUBE_URL + videoCode + "?version=3&amp;rel=0\" />");
            //    sb.Append("            <param name=\"allowFullScreen\" value=\"true\" />");
            //    sb.Append("            <param name=\"allowscriptaccess\" value=\"always\" />");
            //    sb.Append("            <embed src=\"" + YOUTUBE_URL + videoCode + "?version=3&amp;rel=0\" type=\"application/x-shockwave-flash\"");
            //    sb.Append("                   width=\"100%\" height=\"100%\" allowscriptaccess=\"always\" allowfullscreen=\"true\" />");
            //    sb.Append("        </object>");
            //    sb.Append("    </body>");
            //    sb.Append("</html>");

            //    return sb.ToString();
            //}

            //public  void ShowYouTubeVideo(this WebBrowser webBrowser, string videoCode)
            //{
            //    if (webBrowser == null) throw new ArgumentNullException("webBrowser");

            //    webBrowser.NavigateToString(GetYouTubeVideoPlayerHTML(videoCode));
            //}
        
    }
}
