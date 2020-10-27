using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Food_Recipe.ViewModels
{
    class NewRecipeViewModel : BaseViewModel
    {
        #region Properties
        public ICommand AddStepCommand { get; set; }
        public ICommand DeleteStepCommand { get; set; }

        public ICommand FinishCommand { get; set; }

        #endregion
        public NewRecipeViewModel()
        {
            //set the command
            FinishCommand = new RelayCommand<Window>((prop) => { return true; }, (prop) =>
            {
                CloseWindow(prop);
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
