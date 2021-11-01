using System;
using System.Drawing;

namespace TankLib
{
    public enum Direction
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
        NONE
    }
    public class Tank
    {
        public Direction Dir { set; get; }
        public int Speed { set; get; }
        public int HP { set; get; }
        public Rectangle Rectangle { set; get; }
        public Bullet Bullet { set; get; }
        public int ShootCoolDown { set; get; }

        public Tank()
        {

        }
        public Tank(Rectangle rect, Direction dir, int speed, Bullet blt)
        {
            this.Dir = dir;
            this.Speed = speed;
            this.Bullet = blt;
            this.Rectangle = rect;
            this.ShootCoolDown = 0;

            this.HP = 100;
        }



        public void Move()
        {
            switch (this.Dir)
            {
                case Direction.UP:
                    this.Rectangle = new Rectangle(Rectangle.X, Rectangle.Y - Speed, 40, 40);
                    break;
                case Direction.DOWN:
                    this.Rectangle = new Rectangle(Rectangle.X, Rectangle.Y + Speed, 40, 40);
                    break;
                case Direction.LEFT:
                    this.Rectangle = new Rectangle(Rectangle.X - Speed, Rectangle.Y, 40, 40);
                    break;
                case Direction.RIGHT:
                    this.Rectangle = new Rectangle(Rectangle.X + Speed, Rectangle.Y, 40, 40);
                    break;
                case Direction.NONE:
                    break;
                default:
                    break;
            }
        }





    }
}