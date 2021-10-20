using System;
using System.Collections.Generic;
using System.Text;

namespace ClientServer
{
    public class Tank
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Tank()
        {
            X = 300;
            Y = 300;
        }

        public Tank(int x,int y)
        {
            X = x;
            Y = y;
        }
    }
}
