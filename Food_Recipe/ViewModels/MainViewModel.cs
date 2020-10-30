using Food_Recipe.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
        #region const variables
        private const int _smallItem = 9;
        private const int _largeItem = 6;
        private const int largeWidth = 220;
        private const int smallWidth = 140;
        #endregion

        #region private 
        private Visibility _isSettingBackgroundVisible = Visibility.Hidden;
        private Visibility _isVideoShown = Visibility.Hidden;
        private Visibility _isAvaShown = Visibility.Visible;
        private Visibility _isVideoButtonShown = Visibility.Visible;
        private int _widthItem;
        private int _height;
        private bool _isLoaded = true;
        private Uri _videoSource = new Uri("https://www.google.com");
        private String _videoID;
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
                AvatarVisibility = Visibility.Visible;
                VideoVisibility = Visibility.Hidden;
                if (ShowRecipe.YoutubeSource == null || ShowRecipe.YoutubeSource.Length == 0)
                {
                    VideoButtonVisibility = Visibility.Hidden;
                }
                else
                {
                    VideoButtonVisibility = Visibility.Visible;
                }
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
        public ICommand EditRecipeCommand { get; set; }

        public ICommand ClickItemCommand { get; set; }
        public ICommand FavoriteChanged { get; set; }

        //Visibility binding
        public Visibility SettingBackground { get => _isSettingBackgroundVisible; set { _isSettingBackgroundVisible = value; OnPropertyChanged(); } }
        public Visibility VideoVisibility { get => _isVideoShown; set { _isVideoShown = value; OnPropertyChanged(); } }
        public Visibility AvatarVisibility {  get => _isAvaShown; set { _isAvaShown = value; OnPropertyChanged(); }  }
        public Visibility VideoButtonVisibility { get => _isVideoButtonShown; set { _isVideoButtonShown = value; OnPropertyChanged(); } }

        public int WidthItem {  get => _widthItem; set { _widthItem = value; OnPropertyChanged(); }  }
        public int Height { get => _height; set { _height = value; OnPropertyChanged(); } }


        public String VideoId { get => _videoID; set { _videoID = value; OnPropertyChanged(); } }

        public Global themeColor = Global.GetInstance();

        public MainViewModel()
        {
            if (_isLoaded)
            {
                _isLoaded = true;
                foreach (var dir in Directory.GetDirectories(Convert("Data\\recipes")))
                {
                    bool IsExist = false;
                    foreach (var recipe in DataProvider.Ins.DB.Recipes)
                    {
                        if (dir == Convert($"Data\\recipes\\{recipe.Id}"))
                        {
                            IsExist = true;
                        }
                    }
                    if (IsExist == false)
                    {
                        DeleteDirectory(dir);
                    }
                }
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
                LoadRecipes(IsFavoriteRecipes);
                RecipesPage = RecipesPage;
            });
            DeleteRecipeCommand = new RelayCommand<object>((prop) => { return true; }, (prop) =>
            {
                if (DeleteMessage() == true)
                {
                    foreach (var item in DataProvider.Ins.DB.Images)
                    {
                        if (item.RecipeId == ShowRecipe.Id)
                        {
                            DataProvider.Ins.DB.Images.Remove(item);
                        }
                    }
                    DataProvider.Ins.DB.Recipes.Remove(ShowRecipe);
                    DataProvider.Ins.DB.SaveChanges();
                    LoadRecipes(IsFavoriteRecipes);
                    RecipesPage = RecipesPage;
                    ShowRecipe = Recipes[MyRandom.Ins.Next(Recipes.Count)];
                }
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
            EditRecipeCommand = new RelayCommand<object>((prop) => { return true; }, (StartVideoBtn) =>
            {
                NewRecipeWindow newRecipeWindow = new NewRecipeWindow(ShowRecipe);
                newRecipeWindow.ShowDialog();
                LoadRecipes(IsFavoriteRecipes);
                RecipesPage = RecipesPage;
                ShowRecipe = Recipes[MyRandom.Ins.Next(Recipes.Count)];

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
            RecipesPerPage = new ObservableCollection<Recipe>(Recipes.Skip(MaxRecipesPerPage*(RecipesPage - 1)).Take(MaxRecipesPerPage));
            //for (int i = 0; i < MaxRecipesPerPage && ((_recipesPage - 1) * MaxRecipesPerPage + i) < Recipes.Count; i++)
            //{
            //    RecipesPerPage.Add(Recipes[(_recipesPage - 1) * MaxRecipesPerPage + i]);
            //}
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

            if (IsSmallItem == true)
            {
                _maxRecipesPerPage = _smallItem;
                WidthItem = smallWidth;
                Height = 150;
            }
            else
            {
                _maxRecipesPerPage = _largeItem;
                WidthItem = largeWidth;
                Height = 160;
            }

            if (isFavorite == true)
            {
                Recipes = new ObservableCollection<Recipe>(DataProvider.Ins.DB.Recipes.Where(x => x.IsFavorite == 1).ToList());
            }
            else
            {
                Recipes = new ObservableCollection<Recipe>(DataProvider.Ins.DB.Recipes.ToList());
            }

            if (IsIncrease == true)
            {
                if (SortingType == "time")
                {
                    Recipes = new ObservableCollection<Recipe>(Recipes.OrderBy(x => x.CreatedTime));
                }
                else if (SortingType == "alphabet")
                {
                    Recipes = new ObservableCollection<Recipe>(Recipes.OrderBy(x => x.Name));
                }
            }
            else
            {
                if (SortingType == "time")
                {
                    Recipes = new ObservableCollection<Recipe>(Recipes.OrderByDescending(x => x.CreatedTime));

                }
                else if (SortingType == "alphabet")
                {
                    Recipes = new ObservableCollection<Recipe>(Recipes.OrderByDescending(x => x.Name));

                }
            }
        }

        private void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);
            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }
            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                File.Delete(file);
            }

            

            Directory.Delete(target_dir, false);
        }

        private string Convert(string relativeFile)
        {
            string folder = AppDomain.CurrentDomain.BaseDirectory;
            string absolutePath = $"{folder}{relativeFile}";
            return absolutePath;
        }

        private bool DeleteMessage()
        {
            MessageBoxResult result = MessageBox.Show("Are yeo sure to delete this ? the data will be delete permanently !", "WARNING", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                return true;
            }
            else return false;
        }
    }
}
