using Food_Recipe.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using YoutubePlayerLib.Cef;
using Image = Food_Recipe.Model.Image;

namespace Food_Recipe.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private const int _smallItem = 6;
        private const int _largeItem = 12;

        #region private 
        private Visibility _isSettingBackgroundVisible = Visibility.Hidden;
        private Visibility _isVideoShown = Visibility.Hidden;
        private Visibility _isAvaShown = Visibility.Visible;
        private bool _isLoaded = true;
        private Uri _videoSource = new Uri("https://www.google.com");
        private String _videoID = "9i4SKHbhbqk";
        private bool _isFavorite = false;
        #endregion

        public static bool IsShowed = false;

        private bool _isFavoriteRecipes;
        public bool IsFavoriteRecipes { get => _isFavoriteRecipes; set { _isFavoriteRecipes = value; OnPropertyChanged(); LoadRecipes(IsFavoriteRecipes); } }

        private ObservableCollection<Recipe> _recipes;
        public ObservableCollection<Recipe> Recipes { get => _recipes; set { _recipes = value; OnPropertyChanged(); } }

        private int _maxRecipesPerPage;

        private ObservableCollection<Recipe> _recipesPerPage;
        public ObservableCollection<Recipe> RecipesPerPage { get => _recipesPerPage; set { _recipesPerPage = value; OnPropertyChanged(); } }

        private int _recipesPage;
        public int RecipesPage { get => _recipesPage;
            set
            {
                _recipesPage = value;
                OnPropertyChanged();
                IndexRecipes = $"{_recipesPage}/{Recipes.Count / _maxRecipesPerPage + ((Recipes.Count % _maxRecipesPerPage != 0) ? 1 : 0)}";
                LoadRecipesPerPage(_maxRecipesPerPage);
            }
        }

        private string _indexRecipes;
        public string IndexRecipes { get => _indexRecipes; set { _indexRecipes = value; OnPropertyChanged(); } }

        private Recipe _showRecipe;
        public Recipe ShowRecipe { get => _showRecipe; 
            set { 
                _showRecipe = value; 
                OnPropertyChanged();
                StepPage = 1;
            } 
        }

        private Step _nowStep;
        public Step NowStep { get => _nowStep; 
            set { 
                _nowStep = value; 
                OnPropertyChanged();
                if (NowStep.Images.Count != 0)
                {
                    NowImageStep = NowStep.Images.ToList()[0];
                }
                else
                {
                    NowImageStep = new Image{ ImageSource = "Data/noimage.png" };
                }
            } 
        }

        private int _stepsPage;
        public int StepPage { get => _stepsPage; 
            set { 
                _stepsPage = value; 
                OnPropertyChanged();
                IndexStep = $"{StepPage}/{ShowRecipe.Steps.Count}";
                NowStep = ShowRecipe.Steps.ToList()[StepPage - 1];
            } 
        }

        private string _indexStep;
        public string IndexStep { get => _indexStep; set { _indexStep = value; OnPropertyChanged(); } }

        private Image _nowImageStep;
        public Image NowImageStep { get => _nowImageStep; set { _nowImageStep = value; OnPropertyChanged(); } }


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

        public ICommand ClickItemCommand { get; set; }
        public ICommand FavoriteChanged { get; set; }

        //Visibility binding
        public Visibility SettingBackground { get => _isSettingBackgroundVisible; set { _isSettingBackgroundVisible = value; OnPropertyChanged(); } }
        public Visibility VideoVisibility { get => _isVideoShown; set { _isVideoShown = value; OnPropertyChanged(); } }
        public Visibility AvatarVisibility {  get => _isAvaShown; set { _isAvaShown = value; OnPropertyChanged(); }  }

        public String VideoId { get => _videoID; set { _videoID = value; OnPropertyChanged(); } }

        public MainViewModel()
        {
            if (_isLoaded)
            {
                _isLoaded = true;

                IsFavoriteRecipes = false;
                LoadRecipes(IsFavoriteRecipes);
                RecipesPage = 1;
                ShowRecipe = Recipes[MyRandom.Ins.Next(Recipes.Count)];
            }

            //set the command
            AllRecipeCommand = new RelayCommand<object>((prop) => { return true; }, (prop) =>
            {
                IsFavoriteRecipes = false;
                LoadRecipes(IsFavoriteRecipes);
                RecipesPage = 1;
            });

            FavoriteCommand = new RelayCommand<object>((prop) => { return true; }, (prop) =>
            {
                IsFavoriteRecipes = true;
                LoadRecipes(IsFavoriteRecipes);
                RecipesPage = 1;
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
                LoadRecipes(IsFavoriteRecipes);
                RecipesPage = 1;
            });

            NewRecipeCommand = new RelayCommand<object>((prop) => { return true; }, (prop) =>
            {
                NewRecipeWindow newRecipeWindow = new NewRecipeWindow();
                newRecipeWindow.ShowDialog();
            });

            NextImgStepCommand = new RelayCommand<object>((prop) => { return true; }, (prop) =>
            {
                //handle next step's image button
                if (NowStep.Images.Count > 1)
                {
                    if (NowImageStep == NowStep.Images.ToList()[NowStep.Images.Count - 1])
                    {
                        NowImageStep = NowStep.Images.ToList()[0];
                    }
                    else
                    {
                        for (int i = 0; i < NowStep.Images.Count - 1; i++)
                        {
                            if (NowImageStep == NowStep.Images.ToList()[i])
                            {
                                NowImageStep = NowStep.Images.ToList()[i + 1];
                                break;
                            }
                        }
                    }
                }
                
            });
            PrevImgStepCommand = new RelayCommand<object>((prop) => { return true; }, (prop) =>
            {
                //handle previous step's image button
                if (NowStep.Images.Count > 1)
                {
                    if (NowImageStep == NowStep.Images.ToList()[0])
                    {
                        NowImageStep = NowStep.Images.ToList()[NowStep.Images.Count - 1];
                    }
                    else
                    {
                        for (int i = 0; i < NowStep.Images.Count - 1; i++)
                        {
                            if (NowImageStep == NowStep.Images.ToList()[i + 1])
                            {
                                NowImageStep = NowStep.Images.ToList()[i];
                                break;
                            }
                        }
                    }
                }
            });
            NextStepCommand = new RelayCommand<object>((prop) => { return true; }, (prop) =>
            {
                //handle next step button
                if (StepPage == ShowRecipe.Steps.Count)
                {
                    StepPage = 1;
                }
                else
                {
                    StepPage++;
                }
            });

            PrevStepCommand = new RelayCommand<object>((prop) => { return true; }, (prop) =>
            {
                //handle previous image button
                if (StepPage == 1)
                {
                    StepPage = ShowRecipe.Steps.Count;
                }
                else
                {
                    StepPage--;
                }
            });
            NextListCommand = new RelayCommand<object>((prop) => { return true; }, (prop) =>
            {
                //handle next list button
                if (RecipesPage < (Recipes.Count / _maxRecipesPerPage + ((Recipes.Count % _maxRecipesPerPage != 0) ? 1 : 0)))
                {
                    RecipesPage++;
                }
            });
            PrevListCommand = new RelayCommand<object>((prop) => { return true; }, (prop) =>
            {
                //handle next previous button
                if (RecipesPage > 1)
                {
                    RecipesPage--;
                }
            });

            WatchVideoCommand = new RelayCommand<Button>((prop) => { return true; }, (StartVideoBtn) =>
            {
                //handle watch video button
                OpenVideo();
            });

            ClickItemCommand = new RelayCommand<Recipe>((prop) => { return true; }, (prop) =>
            {
                ShowRecipe = prop;
            });

            FavoriteChanged = new RelayCommand<Recipe>((prop) => { return true; }, (prop) =>
            {
                DataProvider.Ins.DB.SaveChanges();
                LoadRecipes(IsFavoriteRecipes);
                RecipesPage = RecipesPage;
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

        private void LoadRecipesPerPage(int MaxRecipesPerPage)
        {
            RecipesPerPage = new ObservableCollection<Recipe>();
            for (int i = 0; i < MaxRecipesPerPage && ((_recipesPage - 1) * MaxRecipesPerPage + i) < Recipes.Count; i++)
            {
                RecipesPerPage.Add(Recipes[(_recipesPage - 1) * MaxRecipesPerPage + i]);
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

        private void LoadRecipes(bool isFavorite)
        {
            var value1 = ConfigurationManager.AppSettings["IsSmallItem"];
            bool IsSmallItem = bool.Parse(value1);
            var value2 = ConfigurationManager.AppSettings["SortingType"];
            string SortingType = value2;
            var value3 = ConfigurationManager.AppSettings["SortingWay"];
            bool IsIncrease = bool.Parse(value3);

            if (IsSmallItem == false)
            {
                _maxRecipesPerPage = _smallItem;
            }
            else
            {
                _maxRecipesPerPage = _largeItem;
            }

            if (isFavorite == true)
            {
                Recipes = new ObservableCollection<Recipe>(DataProvider.Ins.DB.Recipes.Where(x => x.IsFavorite == 1).ToList());
            }
            else
            {
                Recipes = new ObservableCollection<Recipe>(DataProvider.Ins.DB.Recipes.ToList());
            }

            if (SortingType == "time")
            {
                for (int i = 0; i < Recipes.Count - 1; i++)
                {
                    for (int j = i + 1; j < Recipes.Count; j++)
                    {
                        if (Recipes[i].CreatedTime > Recipes[j].CreatedTime)
                        {
                            Recipe tmp = Recipes[i];
                            Recipes[i] = Recipes[j];
                            Recipes[j] = tmp;
                        }
                    }
                }
            }
            else if (SortingType == "alphabet")
            {
                for (int i = 0; i < Recipes.Count - 1; i++)
                {
                    for (int j = i + 1; j < Recipes.Count; j++)
                    {
                        if (StringCompare(Recipes[i].Name, Recipes[j].Name) == 1)
                        {
                            Recipe tmp = Recipes[i];
                            Recipes[i] = Recipes[j];
                            Recipes[j] = tmp;
                        }
                    }
                }
            }

            if (IsIncrease == false)
            {
                for (int i = 0; i < Recipes.Count / 2; i++)
                {
                    Recipe tmp = Recipes[i];
                    Recipes[i] = Recipes[Recipes.Count - 1 - i];
                    Recipes[Recipes.Count - 1 - i] = tmp;
                }
            }

        }

        public int StringCompare(string a, string b)
        {
            int result = 0;
            if (a == b)
            {
                result = 0;
            }
            else
            {
                int t = 0;
                int t_max = (a.Length > b.Length) ? a.Length : b.Length;
                while (t < t_max && result == 0)
                {
                    if (a[t] > b[t])
                    {
                        result = 1;
                    }
                    else if (a[t] < b[t])
                    {
                        result = -1;
                    }
                    else
                    {
                        t++;
                    }
                }
            }
            return result;
        }

        class MyRandom
        {
            private static MyRandom _ins = null;
            private Random _rng;

            public static MyRandom Ins
            {
                get
                {
                    if (_ins == null)
                    {
                        _ins = new MyRandom();
                    }
                    return _ins;
                }
            }

            public int Next(int ceiling)
            {
                int value = _rng.Next(ceiling);
                return value;
            }

            private MyRandom()
            {
                _rng = new Random();
            }
        }


        ////class for testing
        //public class Ingredient
        //{
        //    public string Name { get; set; }
        //    public string Quantity { get; set; }

        //    public Ingredient(string a, string b)
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
