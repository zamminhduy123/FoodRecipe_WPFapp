using Food_Recipe.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
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
        private Visibility _isStepImageShow = Visibility.Visible;
        private int _widthItem;
        private bool _isLoaded = true;
        private Uri _videoSource = new Uri("https://www.google.com");
        private String _videoID;
        #endregion

        public static bool IsShowed = false;

        private string _search;
        public string Search{ get => _search; set { _search = value; OnPropertyChanged(); } }

        private ObservableCollection<Category> _categoryList = new ObservableCollection<Category>(DataProvider.Ins.DB.Categories);
        public ObservableCollection<Category> CategoryList { get => _categoryList; }

        private Category _selectedCategory;
        public Category SelectedCategory { get => _selectedCategory; 
            set { 
                _selectedCategory = value; 
                OnPropertyChanged();
                LoadRecipes(IsFavoriteRecipes, SelectedCategory.Id, Search);
                RecipesPage = 1;
            } }

        private bool _isFavoriteRecipes;
        public bool IsFavoriteRecipes { get => _isFavoriteRecipes; set { _isFavoriteRecipes = value; OnPropertyChanged(); LoadRecipes(IsFavoriteRecipes, SelectedCategory.Id, Search); } }

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

        public ICommand FilterRecipesCommand { get; set; }


        //Visibility binding
        public Visibility SettingBackground { get => _isSettingBackgroundVisible; set { _isSettingBackgroundVisible = value; OnPropertyChanged(); } }
        public Visibility VideoVisibility { get => _isVideoShown; set { _isVideoShown = value; OnPropertyChanged(); } }
        public Visibility AvatarVisibility {  get => _isAvaShown; set { _isAvaShown = value; OnPropertyChanged(); }  }
        public Visibility VideoButtonVisibility { get => _isVideoButtonShown; set { _isVideoButtonShown = value; OnPropertyChanged(); } }
        public Visibility StepImageVisibility { get => _isStepImageShow; set { _isStepImageShow = value; OnPropertyChanged(); } }

        public int WidthItem {  get => _widthItem; set { _widthItem = value; OnPropertyChanged(); }  }

        public String VideoId { get => _videoID; set { _videoID = value; OnPropertyChanged(); } }

        public Global themeColor = Global.GetInstance();

        DoubleAnimation fadeIn = new DoubleAnimation
        {
            Duration = new Duration(TimeSpan.FromMilliseconds(250)),
            AutoReverse = false,
            RepeatBehavior = new RepeatBehavior(1),
            From = 0.0,
            To = 1.0
        };

        DoubleAnimation fadeOut = new DoubleAnimation
        {
            Duration = new Duration(TimeSpan.FromMilliseconds(250)),
            AutoReverse = false,
            RepeatBehavior = new RepeatBehavior(1),
            From = 1.0,
            To = 0.0
        };



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
                SelectedCategory = DataProvider.Ins.DB.Categories.Where(x => x.Id == 0).ToList()[0];
                IsFavoriteRecipes = false;
                LoadRecipes(IsFavoriteRecipes, SelectedCategory.Id);
                RecipesPage = 1;
                ShowRecipe = Recipes[MyRandom.Ins.Next(Recipes.Count)];
            }
            
            //set the command
            AllRecipeCommand = new RelayCommand<object>((prop) => { return true; }, (prop) =>
            {
                IsFavoriteRecipes = false;
                LoadRecipes(IsFavoriteRecipes, SelectedCategory.Id, Search);
                RecipesPage = 1;
            });

            FavoriteCommand = new RelayCommand<object>((prop) => { return true; }, (prop) =>
            {
                IsFavoriteRecipes = true;
                LoadRecipes(IsFavoriteRecipes, SelectedCategory.Id, Search);
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
                LoadRecipes(IsFavoriteRecipes, SelectedCategory.Id, Search);
                RecipesPage = 1;
            });

            NewRecipeCommand = new RelayCommand<object>((prop) => { return true; }, (prop) =>
            {
                NewRecipeWindow newRecipeWindow = new NewRecipeWindow();
                newRecipeWindow.ShowDialog();
                LoadRecipes(IsFavoriteRecipes, SelectedCategory.Id, Search);
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
                    LoadRecipes(IsFavoriteRecipes, SelectedCategory.Id, Search);
                    RecipesPage = RecipesPage;
                    ShowRecipe = Recipes[MyRandom.Ins.Next(Recipes.Count)];
                }
            });

            NextImgStepCommand = new RelayCommand<Border>((prop) => { return true; }, async (stepImgHolder) =>
            {
                //fade in animation
                Storyboard storyboard = new Storyboard();
                storyboard.Children.Add(fadeOut);
                Storyboard.SetTarget(fadeOut, stepImgHolder);
                Storyboard.SetTargetProperty(fadeOut, new PropertyPath("Opacity"));
                storyboard.Begin(stepImgHolder);
                await storyboard.BeginAsync();
               
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

                //fade out
                storyboard = new Storyboard();
                storyboard.Children.Add(fadeIn);
                Storyboard.SetTarget(fadeIn, stepImgHolder);
                Storyboard.SetTargetProperty(fadeIn, new PropertyPath("Opacity"));
                storyboard.Begin(stepImgHolder);
                await storyboard.BeginAsync();

            });
            PrevImgStepCommand = new RelayCommand<Border>((prop) => { return true; }, async (stepImgHolder) =>
            {
                //fade in animation
                Storyboard storyboard = new Storyboard();
                storyboard.Children.Add(fadeOut);
                Storyboard.SetTarget(fadeOut, stepImgHolder);
                Storyboard.SetTargetProperty(fadeOut, new PropertyPath("Opacity"));
                storyboard.Begin(stepImgHolder);
                await storyboard.BeginAsync();

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

                //fade out
                storyboard = new Storyboard();
                storyboard.Children.Add(fadeIn);
                Storyboard.SetTarget(fadeIn, stepImgHolder);
                Storyboard.SetTargetProperty(fadeIn, new PropertyPath("Opacity"));
                storyboard.Begin(stepImgHolder);
                await storyboard.BeginAsync();
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
                LoadRecipes(IsFavoriteRecipes, SelectedCategory.Id, Search);
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
                LoadRecipes(IsFavoriteRecipes, SelectedCategory.Id, Search);
                RecipesPage = RecipesPage;
            });

            FilterRecipesCommand = new RelayCommand<Window>((prop) => { return true; }, (prop) =>
            {
                LoadRecipes(IsFavoriteRecipes, SelectedCategory.Id, Search);
                RecipesPage = 1;
                
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

        private void LoadRecipes(bool isFavorite, int CategoryId = 0, string search = null)
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
            }
            else
            {
                _maxRecipesPerPage = _largeItem;
                WidthItem = largeWidth;
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

            if (CategoryId != 0)
            {
                Recipes = new ObservableCollection<Recipe>(Recipes.Where(x => x.Category == CategoryId));
            }

            if (search != null)
            {
                Recipes = SearchRecipe(search, Recipes);
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
            MessageBoxResult result = MessageBox.Show("Are you sure to delete this ? the data will be delete permanently !", "WARNING", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                return true;
            }
            else return false;
        }

        public ObservableCollection<Recipe> SearchRecipe(string inputKeyWord, ObservableCollection<Recipe> inputList)
        {
            ObservableCollection<Recipe> outputList = new ObservableCollection<Recipe>(inputList);
            if (inputKeyWord != "" && inputKeyWord != null)
            {
                List<string> listWordsAND = new List<string>();
                List<string> listWordsOR = new List<string>();
                List<string> listWordsNOT = new List<string>();

                List<string> listWords = new List<string>();
                List<string> listOperators = new List<string>();
                string[] words = inputKeyWord.Trim().ToLower().Split(' ');
                string wordElement = "";
                foreach (string word in words)
                {
                    if (word == "and" || word == "or" || word == "not")
                    {
                        listWords.Add(wordElement);
                        wordElement = "";
                        if (listOperators.Count > 0)
                        {
                            foreach (string item in listWords)
                            {
                                if (listOperators[0] == "and")
                                {
                                    listWordsAND.Add(item);
                                }
                                else if (listOperators[0] == "or")
                                {
                                    listWordsOR.Add(item);
                                }
                                else if (listOperators[0] == "not")
                                {
                                    listWordsNOT.Add(item);
                                }
                            }
                            listWords.RemoveRange(0, listWords.Count);
                            string temp = listOperators[0];
                            listOperators.RemoveAt(0);
                            listOperators.Add(word);
                            listOperators.Add(temp);
                        }
                        else if (word == "not" && listWords.Count > 0)
                        {
                            listWordsOR.Add(listWords[0]);
                            listOperators.Add(word);
                            listWords.RemoveRange(0, listWords.Count);
                        }
                        else
                        {
                            listOperators.Add(word);
                        }
                    }
                    else
                    {
                        if (wordElement == "")
                        {
                            wordElement += word;
                        }
                        else
                        {
                            wordElement += " " + word;
                        }
                    }
                }
                if (wordElement != "")
                {
                    listWords.Add(wordElement);
                    wordElement = "";
                }
                if (listOperators.Count > 0)
                {
                    foreach (string item in listWords)
                    {
                        if (listOperators[0] == "and")
                        {
                            listWordsAND.Add(item);
                        }
                        else if (listOperators[0] == "or")
                        {
                            listWordsOR.Add(item);
                        }
                        else if (listOperators[0] == "not")
                        {
                            listWordsNOT.Add(item);
                        }
                    }
                    listWords.RemoveRange(0, listWords.Count);
                    listOperators.RemoveAt(0);
                }
                else if (listWords.Count > 0)
                {
                    foreach (string item in listWords)
                    {
                        listWordsOR.Add(item);
                    }
                }


                DataTable rawTable = new DataTable();
                rawTable.Columns.Add("Name", typeof(string));
                rawTable.Columns.Add("Order", typeof(int));
                rawTable.Columns.Add("Distance", typeof(int));
                rawTable.Columns.Add("Length", typeof(int));

                DataTable resultTable = new DataTable();
                resultTable = rawTable.Copy();

                foreach (Recipe recipe in inputList)
                {
                    rawTable.Rows.Add(recipe.Name, 0, 0, 0);
                }

                DataTable notTable = new DataTable();
                if (listWordsNOT.Count > 0)
                {
                    notTable = rawTable.Copy();
                    foreach (string word in listWordsNOT)
                    {
                        notTable = SearchOneWord(word, notTable);
                    }
                }


                DataTable andTable = new DataTable();
                if (listWordsAND.Count > 0)
                {
                    andTable = rawTable.Copy();
                    foreach (string word in listWordsAND)
                    {
                        andTable = SearchOneWord(word, andTable);
                    }
                    foreach (DataRow rowData in andTable.Rows)
                    {
                        resultTable.Rows.Add(rowData.ItemArray);
                    }
                }


                foreach (string word in listWordsOR)
                {
                    DataTable orTable = SearchOneWord(word, rawTable);
                    foreach (DataRow rowDataOrTable in orTable.Rows)
                    {
                        bool flag = true;
                        foreach (DataRow rowDataResultTable in resultTable.Rows)
                        {
                            if (rowDataOrTable["Name"] == rowDataResultTable["Name"])
                            {
                                flag = false;
                                break;
                            }
                        }
                        if (flag == true)
                        {
                            resultTable.Rows.Add(rowDataOrTable.ItemArray);
                        }
                    }
                }
                for (int x = 0; x < notTable.Rows.Count; x++)
                {
                    for (int y = 0; y < resultTable.Rows.Count; y++)
                    {
                        if (notTable.Rows[x].Field<string>(0) == resultTable.Rows[y].Field<string>(0))
                        {
                            resultTable.Rows.RemoveAt(y);
                        }
                    }
                }

                int tableRange = resultTable.Rows.Count;
                int listRange = inputList.Count;

                DataView DV = resultTable.DefaultView;
                DV.Sort = "Order DESC, Distance ASC, Length ASC";
                resultTable = DV.ToTable();


                outputList.Clear();

                for (int tableIndex = 0; tableIndex < tableRange; tableIndex++)
                {

                    for (int listIndex = 0; listIndex < listRange; listIndex++)
                    {
                        if (resultTable.Rows[tableIndex].Field<string>(0) == inputList[listIndex].Name)
                        {
                            outputList.Add(inputList[listIndex]);
                            break;
                        }
                    }
                }
            }
            return outputList;
        }

        public DataTable SearchOneWord(string word, DataTable inputTable)
        {
            DataTable resultTable = new DataTable();
            resultTable.Columns.Add("Name", typeof(string));
            resultTable.Columns.Add("Order", typeof(int));
            resultTable.Columns.Add("Distance", typeof(int));
            resultTable.Columns.Add("Length", typeof(int));

            int tableRange = inputTable.Rows.Count;
            for (int tableIndex = 0; tableIndex < tableRange; tableIndex++)
            {
                string recipeName = inputTable.Rows[tableIndex].Field<string>(0);

                int order = -1;
                int Distance = 0;
                int prevWordIndex = 0;
                int signedWordIndex = recipeName.IndexOf(word, StringComparison.OrdinalIgnoreCase);
                int unsignedWordIndex = RemoveSign(recipeName).IndexOf(RemoveSign(word), StringComparison.OrdinalIgnoreCase);


                if (signedWordIndex >= 0)
                {
                    if (IsSeparateWord(word, recipeName, signedWordIndex) == true)
                    {
                        order = word.Length + 2;
                    }
                    else if (IsBeginOfWord(word, recipeName, signedWordIndex) == true)
                    {
                        order = word.Length - 1;
                    }
                    else
                    {
                        continue;
                    }

                    if (signedWordIndex >= prevWordIndex)
                    {
                        Distance += signedWordIndex - prevWordIndex;
                    }
                    else
                    {
                        Distance += recipeName.Length;
                    }
                    prevWordIndex = signedWordIndex;
                }
                else if (unsignedWordIndex >= 0)
                {
                    if (IsSeparateWord(word, recipeName, unsignedWordIndex) == true)
                    {
                        order = word.Length + 1;
                    }
                    else if (IsBeginOfWord(word, recipeName, unsignedWordIndex) == true)
                    {
                        order = word.Length - 2;
                    }
                    else
                    {
                        continue;
                    }
                    if (unsignedWordIndex >= prevWordIndex)
                    {
                        Distance += unsignedWordIndex - prevWordIndex;
                    }
                    else
                    {
                        Distance += recipeName.Length;
                    }
                    prevWordIndex = unsignedWordIndex;
                }

                if (order > -1)
                {
                    resultTable.Rows.Add(recipeName, order, Distance, recipeName.Length);
                }

            }
            return resultTable;
        }
        public static string RemoveSign(string inputString)
        {
            string[] VietNamChar = new string[]
            {
                "aAeEoOuUiIdDyY",
                "áàạảãâấầậẩẫăắằặẳẵ",
                "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
                "éèẹẻẽêếềệểễ",
                "ÉÈẸẺẼÊẾỀỆỂỄ",
                "óòọỏõôốồộổỗơớờợởỡ",
                "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
                "úùụủũưứừựửữ",
                "ÚÙỤỦŨƯỨỪỰỬỮ",
                "íìịỉĩ",
                "ÍÌỊỈĨ",
                "đ",
                "Đ",
                "ýỳỵỷỹ",
                "ÝỲỴỶỸ"
            };

            for (int i = 1; i < VietNamChar.Length; i++)
            {
                for (int j = 0; j < VietNamChar[i].Length; j++)
                    inputString = inputString.Replace(VietNamChar[i][j], VietNamChar[0][i - 1]);
            }
            return inputString;
        }

        private static bool IsSeparateWord(string word, string sentence, int index)
        {
            bool result = true;
            if (index > 0 && sentence[index - 1] != ' ')
            {
                result = false;
            }
            if (index + word.Length < sentence.Length && sentence[index + word.Length] != ' ')
            {
                result = false;
            }
            return result;
        }

        public static bool IsBeginOfWord(string word, string sentence, int index)
        {
            bool result = true;
            if (index > 0 && sentence[index - 1] != ' ')
            {
                result = false;
            }
            return result;
        }
    }

    public static class StoryboardExtensions
    {
        public static Task BeginAsync(this Storyboard storyboard)
        {
            System.Threading.Tasks.TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            if (storyboard == null)
                tcs.SetException(new ArgumentNullException());
            else
            {
                EventHandler onComplete = null;
                onComplete = (s, e) => {
                    storyboard.Completed -= onComplete;
                    tcs.SetResult(true);
                };
                storyboard.Completed += onComplete;
                storyboard.Begin();
            }
            return tcs.Task;
        }
    }
}
