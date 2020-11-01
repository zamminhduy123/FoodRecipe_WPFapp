using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Food_Recipe
{
    public class Global : INotifyPropertyChanged
    {
        private static Global _instance = null;
        private  String _themeColor; // Attribute - Backup fields
        //Helper for Thread Safety
        private static object m_lock = new object();

        public static Global GetInstance()
        {
            // DoubleLock
            if (_instance == null)
            {
                lock (m_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new Global();
                    }
                }
            }
            return _instance;
        }
        

        public String ThemeColor
        {
            get => _themeColor;
            set
            {
                _themeColor = value; // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("ThemeColor");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        Global()
        {
            //default color is orange
            ThemeColor = Brushes.Orange.ToString();  
        }
    }
}
