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
    public class SplashScreenViewModel : BaseViewModel
    {
        #region private 
        private string _tip;
        #endregion

        public string Tip
        {
            get { return _tip; }
            set { _tip = value; }
        }

        public ICommand ClosePermanent { get; private set; }

        public SplashScreenViewModel()
        {
            Tip = "Bạn có thể đổ rượu vang uống không hết vào khay và làm đông, đợi đến khi nấu các món soup hoặc hấp sẽ mang ra dùng.";

            ClosePermanent = new RelayCommand<object>((prop) => { return true; }, (prop) =>
            {
            });
        }

    }
}
