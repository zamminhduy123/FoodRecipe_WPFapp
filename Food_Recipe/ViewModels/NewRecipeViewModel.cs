using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using Food_Recipe.Model;
using Microsoft.Win32;

namespace Food_Recipe.ViewModels
{
    class NewRecipeViewModel : BaseViewModel
    {
        #region Properties

        // Binding show
        private Recipe _newRecipe;
        public Recipe NewRecipe { get => _newRecipe; set { _newRecipe = value; OnPropertyChanged(); } }

        private Ingredient _newIngredient;
        public Ingredient NewIngredient { get => _newIngredient; set { _newIngredient = value; OnPropertyChanged(); } }

        private AsyncObservableCollection<Ingredient> _showNewIngredients;
        public AsyncObservableCollection<Ingredient> ShowNewIngredients { get => _showNewIngredients; set { _showNewIngredients = value; OnPropertyChanged(); } }

        private Step _newStep;
        public Step NewStep { get => _newStep; set { _newStep = value; OnPropertyChanged(); } }

        private AsyncObservableCollection<Step> _showNewSteps;
        public AsyncObservableCollection<Step> ShowNewSteps { get => _showNewSteps; set { _showNewSteps = value; OnPropertyChanged(); } }

        private AsyncObservableCollection<Image> _showNewStepImages;
        public AsyncObservableCollection<Image> ShowNewStepImages { get => _showNewStepImages; set { _showNewStepImages = value; OnPropertyChanged(); } }

        // Binding selected
        private Ingredient _selectedIngredient;
        public Ingredient SelectedIngredient { get => _selectedIngredient; set { _selectedIngredient = value; OnPropertyChanged(); if (SelectedIngredient != null) {  NewIngredient = SelectedIngredient; } } }

        private ObservableCollection<Category> _categoryList = new ObservableCollection<Category>(DataProvider.Ins.DB.Categories);
        public ObservableCollection<Category> CategoryList { get => _categoryList; }

        private Category _selectedCategory;
        public Category SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                OnPropertyChanged();
                if (SelectedCategory != null)
                {
                    NewRecipe.Category = SelectedCategory.Id;
                }
            }
        }

        private Step _selectedStep;
        public Step SelectedStep { get => _selectedStep; 
            set { 
                _selectedStep = value; 
                OnPropertyChanged(); 
                if (SelectedStep != null) { 
                    NewStep = SelectedStep;
                    ShowNewStepImages = new AsyncObservableCollection<Image>();
                    foreach (var item in SelectedStep.Images)
                    {
                        ShowNewStepImages.Add(item);
                    }
                } } }

        // Binding command
        public ICommand AddAvatarImage { get; set; }
        public ICommand AddIngredient { get; set; }
        public ICommand AddStepCommand { get; set; }
        public ICommand DeleteStepCommand { get; set; }
        public ICommand AddStepImage { get; set; }
        public ICommand DeleteImageCommand { get; set; }
        public ICommand DeleteIngredientCommand { get; set; }

        public ICommand FinishCommand { get; set; }
        public ICommand ClosingCommand { get; set; }
        #endregion

        public NewRecipeViewModel()
        {

            NewRecipe = new Recipe();
            NewIngredient = new Ingredient();
            ShowNewIngredients = new AsyncObservableCollection<Ingredient>();
            ShowNewSteps = new AsyncObservableCollection<Step>();
            ShowNewStepImages = new AsyncObservableCollection<Image>();
            NewStep = new Step { OrderNumber = ShowNewSteps.Count + 1 };
            SelectedCategory = DataProvider.Ins.DB.Categories.ToList()[0];

            //set the command

            AddAvatarImage = new RelayCommand<Window>((prop) => { return true; }, (prop) =>
            {
                NewRecipe.AvatarSource = GetImage();
            });

            AddIngredient = new RelayCommand<Window>((prop) => { return true; }, (prop) =>
            {
                if (SelectedIngredient != null)
                {
                    foreach (var item in NewRecipe.Ingredients)
                    {
                        if (item.Name == SelectedIngredient.Name)
                        {
                            item.Quantity = SelectedIngredient.Quantity;
                        }
                    }
                }
                else
                {
                    if (NewIngredient.Name == null || NewIngredient.Name.Length == 0)
                    {
                        MessageBox.Show("You must input Ingredient's name");
                    }
                    else
                    {
                        NewRecipe.Ingredients.Add(NewIngredient);
                        ShowNewIngredients.Add(NewIngredient);
                    }
                }
                SelectedIngredient = null;
                NewIngredient = new Ingredient();
            });

            DeleteIngredientCommand = new RelayCommand<object>((prop) => { return true; }, (prop) =>
            {
                if (SelectedIngredient != null)
                {
                    if (DeleteMessage())
                    {
                        NewRecipe.Ingredients.Remove(SelectedIngredient);
                        ShowNewIngredients.Remove(SelectedIngredient);
                        SelectedIngredient = null;
                        NewIngredient = new Ingredient();
                    }
                }
            });

            AddStepCommand = new RelayCommand<Window>((prop) => { return true; }, (prop) =>
            {
                if (SelectedStep == null)
                {
                    if ((NewStep.Content == null || NewStep.Content.Length == 0) && NewStep.Images.Count == 0)
                    {
                        MessageBox.Show("You must input something");
                    }
                    else
                    {
                        if (ShowNewSteps == null)
                        {
                            NewStep.OrderNumber = 1;
                        }
                        else
                        {
                            NewStep.OrderNumber = ShowNewSteps.Count + 1;
                        }
                        ShowNewSteps.Add(NewStep);
                        
                    }
                }
                else
                {
                    Step tmp = new Step { OrderNumber = SelectedStep.OrderNumber, Content = SelectedStep.Content, Images = SelectedStep.Images };
                    ShowNewSteps.RemoveAt(SelectedStep.OrderNumber - 1);
                    if (ShowNewSteps.Count == 0)
                    {
                        ShowNewSteps.Add(tmp);
                    }
                    else ShowNewSteps.Insert(tmp.OrderNumber - 1, tmp);
                }
                SelectedStep = null;
                NewStep = new Step { OrderNumber = ShowNewSteps.Count + 1 };
                ShowNewStepImages = new AsyncObservableCollection<Image>();
            });

            DeleteStepCommand = new RelayCommand<object>((prop) => { return true; }, (prop) =>
            {
                if (SelectedStep != null)
                {
                    if (DeleteMessage() == true)
                    {
                        int order = SelectedStep.OrderNumber;
                        ShowNewSteps.RemoveAt(order - 1);
                        if (ShowNewSteps.Count != 0)
                        {
                            for (int i = order - 1; i < ShowNewSteps.Count; i++)
                            {
                                ShowNewSteps[i].OrderNumber = i + 1;
                            }
                        }
                    }
                }
                SelectedStep = null;
                NewStep = new Step { OrderNumber = ShowNewSteps.Count + 1 };
                ShowNewStepImages = new AsyncObservableCollection<Image>();
            });

            AddStepImage = new RelayCommand<Window>((prop) => { return true; }, (prop) =>
            {
                List<string> images = GetImages();
                foreach (var image in images)
                {
                    NewStep.Images.Add(new Image { ImageSource = image});
                    ShowNewStepImages.Add(new Image { ImageSource = image });
                }
            });

            DeleteImageCommand = new RelayCommand<Image>((prop) => { return true; }, (prop) =>
            {
                if (DeleteMessage() == true)
                {
                    ShowNewStepImages.Remove(prop);
                    NewStep.Images.Remove(prop);
                }
            });

            FinishCommand = new RelayCommand<Window>((prop) => { return true; }, (prop) =>
            {
                if (NewRecipe.Name == null || NewRecipe.Name.Length == 0)
                {
                    MessageBox.Show("You must add recipe's name");
                }
                else if (ShowNewSteps.Count == 0)
                {
                    MessageBox.Show("You must add at least 1 step");

                }
                else if (NewRecipe.YoutubeSource != null && NewRecipe.YoutubeSource.Length != 0 &&
                    NewRecipe.YoutubeSource.StartsWith("https://www.youtube.com/watch?v=") == false)
                {
                    MessageBox.Show("The video link must like: https://www.youtube.com/watch?v=XXXXXXXXXXXXX");
                }
                else
                {
                    // Add recipe
                    DataProvider.Ins.DB.Recipes.Add(NewRecipe);
                    DataProvider.Ins.DB.SaveChanges();
                    foreach (var item in ShowNewSteps)
                    {
                        NewRecipe.Steps.Add(item);
                        item.Recipe = NewRecipe;
                        item.RecipeId = NewRecipe.Id;
                        DataProvider.Ins.DB.Steps.Add(item);
                    }
                    // Change AvatarSource
                    if (NewRecipe.AvatarSource == null)
                    {
                        NewRecipe.AvatarSource = "noimage.png";
                    }
                    else
                    {
                        string dirFile = $"Data\\recipes\\{NewRecipe.Id}\\thumbnail";
                        CopyFileTo(NewRecipe.AvatarSource, ref dirFile);
                        NewRecipe.AvatarSource = dirFile;
                    }

                    // Add CreatedTime
                    NewRecipe.CreatedTime = DateTime.Now;

                    // Change YoutubeSource
                    if (NewRecipe.YoutubeSource != null && NewRecipe.YoutubeSource.Length != 0)
                    {
                        NewRecipe.YoutubeSource = NewRecipe.YoutubeSource.Substring(32);
                    }

                    // Add IsFavorite
                    NewRecipe.IsFavorite = 0;

                    // Change step
                    foreach (var step in NewRecipe.Steps)
                    {
                        // Change Images
                        int i = 0;
                        foreach (var image in step.Images)
                        {
                            i++;
                            string dirFile = $"Data\\recipes\\{NewRecipe.Id}\\step_{step.OrderNumber}\\images\\i_{i}";
                            CopyFileTo(image.ImageSource, ref dirFile);
                            image.ImageSource = dirFile;
                        }
                    }

                    // save data
                    DataProvider.Ins.DB.SaveChanges();
                    CloseWindow(prop);
                }

            });

            ClosingCommand = new RelayCommand<Window>((prop) => { return true; }, (prop) =>
            {
                OnClosing();
            });
        }


        #region Methods
        private void CloseWindow(Window window)
        {
            if (window != null)
            {
                OnClosing();
                window.Close();
            }
        }

        private string GetImage()
        {
            string result = "";
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "JPG files (*.jpg)|*.jpg| PNG files (*.png)|*.png| All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (openFileDialog.ShowDialog() == true)
            {
                result = openFileDialog.FileName;
            }
            return result;
        }

        private List<string> GetImages()
        {
            List<string> result = new List<string>();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "JPG files (*.jpg)|*.jpg| PNG files (*.png)|*.png| All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (var image in openFileDialog.FileNames)
                {
                    result.Add(image);
                }
            }
            return result;
        }

        private void SeparateDirFile(string sourceFile, ref string sourcePath, ref string fileName)
        {
            int tmp = sourceFile.LastIndexOf("\\") + 1;
            sourcePath = sourceFile.Substring(0, tmp);
            fileName = sourceFile.Substring(tmp);
        }

        private void CopyFileTo(string sourceFile, ref string targetFile)
        {
            string sourcePath = "", sourceFileName = "", targetPath = "", targetFileName = "";

            targetFile += sourceFile.Substring(sourceFile.LastIndexOf('.'));
            string converTargetFile = Convert(targetFile);
            SeparateDirFile(sourceFile, ref sourcePath, ref sourceFileName);
            SeparateDirFile(converTargetFile, ref targetPath, ref targetFileName);


            System.IO.Directory.CreateDirectory(targetPath);

            // To copy a file to another location and
            // overwrite the destination file if it already exists.
            System.IO.File.Copy(sourceFile, targetFile, true);
        }

        private string Convert(string relativeFile)
        {
            string folder = AppDomain.CurrentDomain.BaseDirectory;
            string absolutePath = $"{folder}{relativeFile}";
            return absolutePath;
        }

        private void OnClosing()
        {
            NewRecipe = new Recipe();
            NewIngredient = new Ingredient();
            ShowNewIngredients = new AsyncObservableCollection<Ingredient>();
            ShowNewSteps = new AsyncObservableCollection<Step>();
            ShowNewStepImages = new AsyncObservableCollection<Image>();
            NewStep = new Step { OrderNumber = ShowNewSteps.Count + 1 };
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
        #endregion

        public class AsyncObservableCollection<T> : ObservableCollection<T>
        {
            public override event NotifyCollectionChangedEventHandler CollectionChanged;
            private static object _syncLock = new object();

            public AsyncObservableCollection()
            {
                enableCollectionSynchronization(this, _syncLock);
            }

            protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
            {
                using (BlockReentrancy())
                {
                    var eh = CollectionChanged;
                    if (eh == null) return;

                    var dispatcher = (from NotifyCollectionChangedEventHandler nh in eh.GetInvocationList()
                                      let dpo = nh.Target as DispatcherObject
                                      where dpo != null
                                      select dpo.Dispatcher).FirstOrDefault();

                    if (dispatcher != null && dispatcher.CheckAccess() == false)
                    {
                        dispatcher.Invoke(DispatcherPriority.DataBind, (Action)(() => OnCollectionChanged(e)));
                    }
                    else
                    {
                        foreach (NotifyCollectionChangedEventHandler nh in eh.GetInvocationList())
                            nh.Invoke(this, e);
                    }
                }
            }

            private static void enableCollectionSynchronization(IEnumerable collection, object lockObject)
            {
                var method = typeof(BindingOperations).GetMethod("EnableCollectionSynchronization",
                                        new Type[] { typeof(IEnumerable), typeof(object) });
                if (method != null)
                {
                    // It's .NET 4.5
                    method.Invoke(null, new object[] { collection, lockObject });
                }
            }
        }
    }
}
