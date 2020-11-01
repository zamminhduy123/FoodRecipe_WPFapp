using Food_Recipe.Model;
using Food_Recipe.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Food_Recipe
{
    /// <summary>
    /// Interaction logic for NewRecipeWindow.xaml
    /// </summary>
    public partial class NewRecipeWindow : Window
    {
        public NewRecipeWindow(Recipe recipe = null)
        {
            NewRecipeViewModel nrvm;
            EditRecipeViewModel ervm;
            InitializeComponent();

            if (recipe == null)
            {
                nrvm = new NewRecipeViewModel();
                this.DataContext = nrvm;
            }
            else
            {
                ervm = new EditRecipeViewModel(recipe);
                this.DataContext = ervm;
            }
        }
    }
}
