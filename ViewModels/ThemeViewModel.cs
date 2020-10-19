using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Project_1.ViewModels
{
    class ThemeViewModel : BaseViewModel
    {
        private  Brush[] _color = { Brushes.Red, Brushes.Blue, Brushes.White, Brushes.Green, Brushes.Yellow, Brushes.Black,Brushes.Pink,Brushes.Brown,Brushes.Purple,Brushes.Orange };
        public Brush[]  Colors { get => _color; set { _color = value; OnPropertyChanged(); } }

        public ThemeViewModel()
        {
            
        }
    }
}
