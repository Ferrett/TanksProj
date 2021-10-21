using System;
using System.Collections.Generic;
using System.Text;

namespace ClientServer
{
    public class Tank
    {
        public int X { get; set; }
        public int Y { get; set; }

        public int Speed { get; set; }

        public Tank()
        {
            X = 300;
            Y = 300;
            Speed = 3;
        }

        public Tank(int x,int y,int speed)
        {
            X = x;
            Y = y;
            Speed = speed;
        }

       
    }
}
