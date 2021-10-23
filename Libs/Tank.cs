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
        public int[]Color { get; set; }

        public float Rotation { get; set; }
        public Tank()
        {
            Random r = new Random();
            X = 300;
            Y = 300;
            Speed = 3;
            Color =new int[]{ r.Next(0,256), r.Next(0, 256), r.Next(0, 256)};
            Rotation = 0f;
        }

        public Tank(int x,int y,int speed, int[]color,int rotation)
        {
            X = x;
            Y = y;
            Speed = speed;
            Color = color;
            Rotation = rotation;
        }

        public override string ToString()
        {
            return $"X: {X}\nY: {Y}\n";
        }
    }
}
