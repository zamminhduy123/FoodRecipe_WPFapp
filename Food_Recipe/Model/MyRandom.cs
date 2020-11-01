using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Food_Recipe.Model
{
    public class MyRandom
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
}
