using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace Food_Recipe.ViewModels
{
    class ThemeViewModel : BaseViewModel
    {
        private Brush[] _color = { Brushes.Red, Brushes.Blue, Brushes.DarkSalmon, Brushes.Green, Brushes.DarkOrange, Brushes.DarkMagenta, Brushes.HotPink, Brushes.Brown, Brushes.Chocolate, Brushes.Orange, Brushes.YellowGreen, Brushes.DarkOrange, Brushes.DarkCyan };

        public Brush[]  Colors { get => _color; set { _color = value; OnPropertyChanged(); } }

        public ICommand ThemeButtonCommand { get; set; }

        public Global globalTheme = Global.GetInstance();

        public ThemeViewModel()
        {
            ThemeButtonCommand = new RelayCommand<Brush>((prop) => { return true; }, (prop) =>
            {
                globalTheme.ThemeColor = prop.ToString();
            });
        }
    }
}
