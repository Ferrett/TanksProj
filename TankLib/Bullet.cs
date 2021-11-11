using System;
using System.Drawing;

namespace TankLib
{

    public class Bullet
    {
        public Rectangle Rectangle { set; get; }
        public Direction Dir { set; get; }
        public int Speed { set; get; }
        public bool IsActive { set; get; }
        public int Damage { set; get; }

        public Bullet()
        {

        }
        public Bullet(Rectangle rect, Direction dir, int speed, bool act)
        {
            this.Rectangle = rect;
            this.Dir = dir;
            this.Speed = speed;
            this.IsActive = act;
            this.Damage = 300;

            FixCords();
        }

        public void FixCords()
        {
            if (Dir == Direction.DOWN)
                Rectangle = new Rectangle(Rectangle.X + 15, Rectangle.Y + 35, 10, 30);
            else if (Dir == Direction.UP)
                Rectangle = new Rectangle(Rectangle.X + 15, Rectangle.Y - 35, 10, 30);
            else if (Dir == Direction.LEFT)
                Rectangle = new Rectangle(Rectangle.X - 35, Rectangle.Y + 15, 10, 30);
            else
                Rectangle = new Rectangle(Rectangle.X + 35, Rectangle.Y + 15, 10, 30);
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
