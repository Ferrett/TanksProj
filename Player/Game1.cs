using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TankLib;
using ClientServer;
using System.Text.Json;
using System.Threading;
using System.Collections.Generic;
using System;
using System.Linq;
using System.IO;

namespace TankGame
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D[] tanktexture;
        private Texture2D[] bullettexture;
        private Texture2D curtanktexture;
        private Texture2D curbullettexture;

        private Texture2D walltexture;
        private Tank tank;
        private Client client;
        private List<Tank> tanks;

        public static class Map
        {
            public static char[,] CharMap { set; get; }

            public static Wall[,] WallMap { set; get; }

            static Map()
            {
                CharMap = new char[12, 12]{
                    {'X','X','X','X','X','X','X','X','X','X','X','X'},
                    {'X',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ','X'},
                    {'X',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ','X'},
                    {'X',' ',' ',' ',' ','X',' ',' ',' ',' ',' ','X'},
                    {'X',' ',' ',' ',' ','X',' ',' ',' ',' ',' ','X'},
                    {'X',' ',' ',' ',' ','X',' ',' ',' ',' ',' ','X'},
                    {'X',' ',' ',' ',' ','X','X','X','X',' ',' ','X'},
                    {'X',' ',' ',' ',' ','X',' ',' ',' ',' ',' ','X'},
                    {'X',' ',' ',' ',' ','X',' ',' ',' ',' ',' ','X'},
                    {'X',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ','X'},
                    {'X',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ','X'},
                    {'X','X','X','X','X','X','X','X','X','X','X','X'},
                };

               

            }

            public static void CharToWall()
            {
                WallMap = new Wall[12, 12];
                for (int i = 0; i < CharMap.GetLength(0); i++)
                {
                    for (int j = 0; j < CharMap.GetLength(1); j++)
                    {
                        WallMap[i, j] = new Wall(new System.Drawing.Rectangle(j * 50, i * 50, 50, 50), CharMap[i, j] == 'X' ? true : false);
                    }
                }
            }
        }

        public class Wall
        {
            public bool IsActive { set; get; }
            public System.Drawing.Rectangle Rectangle { set; get; }

            public Wall(System.Drawing.Rectangle rect, bool active)
            {
                Rectangle = rect;

                IsActive = active;
            }
        }

        public static class FileReader
        {
            public static string DirectoryPath { get; set; }
            public static string FileName { get; set; }

            public static string MapString { get; set; }

            static FileReader()
            {
                DirectoryPath = @"C:\ProgramData\Tanks";
                FileName = "Map.txt";
            }
            public static bool MapExists()
            {
                if (Directory.Exists(DirectoryPath))
                    if (File.Exists($"{DirectoryPath+"\\"}{FileName}"))
                return true;

                return false;
            }

            public static void ReadMap()
            {
                MapString = File.ReadAllText($"{DirectoryPath + "\\"}{FileName}");

                char[] CharArrayMap = MapString.ToCharArray();
                int a = 0;
                for (int i = 0; i < Map.CharMap.GetLength(0); i++)
                {
                    for (int j = 0; j < Map.CharMap.GetLength(1); j++)
                    {
                        if (CharArrayMap[a] != '\n')
                        {
                            Map.CharMap[i, j] = CharArrayMap[a];

                            a++;
                        }
                    }
                }
                
                Map.CharToWall();
            }

            public static void WriteMap()
            {
                MapString = String.Empty;

                for (int i = 0; i < Map.CharMap.GetLength(0); i++)
                {
                    for (int j = 0; j < Map.CharMap.GetLength(1); j++)
                    {
                        MapString += Map.CharMap[i, j];
                    }
                    MapString += "\n";
                }

                File.WriteAllText($"{DirectoryPath + "\\"}{FileName}",MapString);
            }

            public static void CreateFile()
            {
                Directory.CreateDirectory(@"C:\ProgramData\Tanks");

                File.Create($"{DirectoryPath + "\\"}{FileName}").Close();
                
            }

        }

        public Game()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            tank = new Tank(new System.Drawing.Rectangle(100, 100, 40, 40), Direction.UP, 2, new Bullet(new System.Drawing.Rectangle(100, 100, 10, 30), Direction.UP, 2, false));
            client = new Client("127.0.0.1", 8000);
            tanks = new List<Tank>();

            
            while (!client.socket.Connected)
            {
                try
                {
                    client.Connect();

                    if (Client.FromBytesToString(client.Get()) == "full")
                        Exit();
                }
                catch (System.Exception)
                {
                    Thread.Sleep(100);
                }
            }

            if (FileReader.MapExists() == false)
            {
                FileReader.CreateFile();
                FileReader.WriteMap();
            }

            FileReader.ReadMap();
            

            _graphics.PreferredBackBufferWidth = 600;
            _graphics.PreferredBackBufferHeight = 600;
            _graphics.ApplyChanges();
            base.Initialize();

        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            tanktexture = new Texture2D[4] {
                Content.Load<Texture2D>("Tank/tank1"),
                Content.Load<Texture2D>("Tank/tank2"),
                Content.Load<Texture2D>("Tank/tank3"),
                Content.Load<Texture2D>("Tank/tank4")};

            bullettexture = new Texture2D[4] {
                Content.Load<Texture2D>("Bullet/bullet1"),
                Content.Load<Texture2D>("Bullet/bullet2"),
                Content.Load<Texture2D>("Bullet/bullet3"),
                Content.Load<Texture2D>("Bullet/bullet4")};

            curtanktexture = tanktexture[0];
            curbullettexture = bullettexture[0];

            walltexture = Content.Load<Texture2D>("wall");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            var key = Keyboard.GetState();

            if (key.IsKeyDown(Keys.Up))
            {
                tank.Dir = Direction.UP;
                tank.Move();

                if (WallCollisionCheck() == false)
                {
                    tank.Dir = Direction.DOWN;
                    tank.Move();
                    tank.Dir = Direction.UP;
                }

                curtanktexture = tanktexture[0];
            }
            else if (key.IsKeyDown(Keys.Down))
            {
                tank.Dir = Direction.DOWN;
                tank.Move();

                if (WallCollisionCheck() == false)
                {
                    tank.Dir = Direction.UP;
                    tank.Move();
                    tank.Dir = Direction.DOWN;
                }
                curtanktexture = tanktexture[2];
            }
            else if (key.IsKeyDown(Keys.Left))
            {
                tank.Dir = Direction.LEFT;
                tank.Move();

                if (WallCollisionCheck() == false)
                {
                    tank.Dir = Direction.RIGHT;
                    tank.Move();
                    tank.Dir = Direction.LEFT;
                }
                curtanktexture = tanktexture[3];
            }
            else if (key.IsKeyDown(Keys.Right))
            {
                tank.Dir = Direction.RIGHT;
                tank.Move();

                if (WallCollisionCheck() == false)
                {
                    tank.Dir = Direction.LEFT;
                    tank.Move();
                    tank.Dir = Direction.RIGHT;
                }
                curtanktexture = tanktexture[1];
            }

            if (key.IsKeyDown(Keys.Space) && tank.ShootCoolDown == 0)
            {

                tank.Bullet.Rectangle = new System.Drawing.Rectangle(tank.Rectangle.X, tank.Rectangle.Y, 10, 30);
                tank.Bullet.IsActive = true;
                tank.Bullet.Dir = tank.Dir;

                if (tank.Dir == Direction.UP)
                    curbullettexture = bullettexture[0];
                else if (tank.Dir == Direction.RIGHT)
                    curbullettexture = bullettexture[1];
                else if (tank.Dir == Direction.DOWN)
                    curbullettexture = bullettexture[2];
                else
                    curbullettexture = bullettexture[3];
                tank.ShootCoolDown = 120;
            }

            if (tank.Bullet.IsActive == true)
            {
                tank.Bullet.Move();

                if (tank.ShootCoolDown == 0)
                    tank.Bullet.IsActive = false;
            }

            if (tank.ShootCoolDown != 0)
                tank.ShootCoolDown--;

            if (tank.HP <= 0)
                tank.Rectangle = new System.Drawing.Rectangle(-1000, -1000, 40, 40);


            client.Send(Client.FromStringToBytes(JsonSerializer.Serialize<Tank>(this.tank)));

            try
            {
                var item = Client.FromBytesToString(client.Get());
                var tankarr = JsonSerializer.Deserialize<List<Tank>>(item);
                if (tankarr.Count == tanks.Count)
                {

                    for (int i = 0; i < tanks.Count; i++)
                    {
                        tanks[i].Rectangle = new System.Drawing.Rectangle(tankarr[i].Rectangle.X, tankarr[i].Rectangle.Y, 40, 40);
                        tanks[i].Dir = tankarr[i].Dir;
                        tanks[i].HP = tankarr[i].HP;
                        tanks[i].Bullet = tankarr[i].Bullet;
                    }

                    CollisionCheck();
                }
                else
                {
                    tanks.Clear();
                    tanks.AddRange(tankarr);
                }
                tanks = JsonSerializer.Deserialize<List<Tank>>(item);
                GC.Collect(GC.GetGeneration(item));
            }
            catch (System.Exception) { }

            BulletCollisionCheck();

            base.Update(gameTime);
        }
        public void BulletCollisionCheck()
        {
            for (int i = 0; i < Map.WallMap.GetLength(0); i++)
            {
                for (int j = 0; j < Map.WallMap.GetLength(1); j++)
                {
                    if (Map.WallMap[i, j].IsActive == true)
                    {
                        if (tank.Bullet.Rectangle.IntersectsWith(Map.WallMap[i, j].Rectangle))
                            tank.Bullet.IsActive = false;
                    }
                }
            }
        }

        public bool WallCollisionCheck()
        {
            for (int i = 0; i < Map.WallMap.GetLength(0); i++)
            {
                for (int j = 0; j < Map.WallMap.GetLength(1); j++)
                {
                    if (Map.WallMap[i, j].IsActive == true)
                    {
                        if (tank.Rectangle.IntersectsWith(Map.WallMap[i, j].Rectangle))
                            return false;
                        if (tank.Bullet.Rectangle.IntersectsWith(Map.WallMap[i, j].Rectangle))
                            tank.Bullet.IsActive = false;
                    }
                }
            }

            return true;
        }

        public void CollisionCheck()
        {
            for (int i = 0; i < tanks.Count; i++)
            {
                if (tanks[i].Bullet != null)
                {
                    if (tanks[i].Bullet.IsActive == true && tanks[i].Bullet.Rectangle.IntersectsWith(tank.Rectangle))
                    {
                        tank.HP -= tanks[i].Bullet.Damage;
                    }
                }
            }

            if (tank.Bullet.IsActive == true)
            {
                for (int i = 0; i < tanks.Count; i++)
                {
                    if (tank.Bullet.Rectangle.IntersectsWith(tanks[i].Rectangle))
                    {
                        tank.Bullet.IsActive = false;
                    }
                }
            }
        }

        float side = 0;
        private void drawWalls()
        {
            for (int i = 0; i < Map.CharMap.GetLength(0); i++)
            {
                for (int j = 0; j < Map.CharMap.GetLength(1); j++)
                {
                    if (Map.WallMap[i, j].IsActive == true)
                        _spriteBatch.Draw(walltexture, new Vector2(Map.WallMap[i, j].Rectangle.X, Map.WallMap[i, j].Rectangle.Y), Color.White);
                }
            }
        }
        private void drawBullet(Bullet bullet)
        {
            if (bullet.Dir == Direction.UP)
            {
                side = 0f;
                curbullettexture = bullettexture[0];
            }
            else if (bullet.Dir == Direction.DOWN)
            {
                side = 15.7f;
                curbullettexture = bullettexture[2];
            }
            else if (bullet.Dir == Direction.RIGHT)
            {
                side = 7.85f;
                curbullettexture = bullettexture[1];
            }
            else if (bullet.Dir == Direction.LEFT)
            {
                side = 23.55f;
                curbullettexture = bullettexture[3];
            }

            _spriteBatch.Draw(curbullettexture, new Vector2(bullet.Rectangle.X, bullet.Rectangle.Y), Color.White);
            GC.Collect(GC.GetGeneration(side));
        }
        private void drawTank(Tank tank)
        {

            if (tank.Dir == Direction.UP)
            {
                side = 0f;
                curtanktexture = tanktexture[0];
            }
            else if (tank.Dir == Direction.DOWN)
            {
                side = 15.7f;
                curtanktexture = tanktexture[2];
            }
            else if (tank.Dir == Direction.RIGHT)
            {
                side = 7.85f;
                curtanktexture = tanktexture[1];
            }
            else if (tank.Dir == Direction.LEFT)
            {
                side = 23.55f;
                curtanktexture = tanktexture[3];
            }

            _spriteBatch.Draw(curtanktexture, new Vector2(tank.Rectangle.X, tank.Rectangle.Y), new Color(255, 255 - (200 - tank.HP * 2), 255 - (200 - tank.HP * 2)));
            GC.Collect(GC.GetGeneration(side));
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();



            foreach (var t in tanks)
            {
                if (t.HP > 0)
                    drawTank(t);
            }

            for (int i = 0; i < tanks.Count; i++)
            {
                if (tanks[i].Bullet != null && tanks[i].Bullet.IsActive == true)
                    drawBullet(tanks[i].Bullet);
            }

            drawWalls();

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
