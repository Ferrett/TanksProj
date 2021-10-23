using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ClientServer;
using System.Text.Json;
using System.Collections.Generic;
using System;

namespace Player
{
    public class Sprite
    {

        public Rectangle SpriteRectangle;
        public Texture2D Texture;
        public string TexturePath;
        public Tank Tank;


        public Sprite(Rectangle spriteRectangle, Texture2D texture, string texturePath, Tank tank)
        {

            SpriteRectangle = spriteRectangle;
            Texture = texture;
            TexturePath = texturePath;
            Tank = tank;

        }
    }
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        List<Tank> AllTanks;
        List<Sprite> AllTankSprites;
        Tank tank;
        Sprite TankSprite;
        Client client;

        Random rnd = new Random();
       
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            AllTanks = new List<Tank>();
            AllTankSprites = new List<Sprite>();

            client = new Client("127.0.0.1", 8000);
            client.Connect();

            this.IsFixedTimeStep = true;//false;
            this.TargetElapsedTime = TimeSpan.FromSeconds(1d / 60d); //60);


            base.Initialize();
            _graphics.PreferredBackBufferWidth = 500;
            _graphics.PreferredBackBufferHeight = 500;
            _graphics.ApplyChanges();

            Window.Title = "TANCHIKI";
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            tank = new Tank();
            TankSprite = new Sprite(new Rectangle(0, 0, 40, 49), null, "Textures/tank",tank);
            

            TankSprite.Texture = Content.Load<Texture2D>(TankSprite.TexturePath);
        }

        bool keyPressed = false;
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //Window.Title ="FPS: "+ (1f / gameTime.ElapsedGameTime.TotalSeconds).ToString();
           


            if (Keyboard.GetState().IsKeyDown(Keys.Right) && keyPressed == false)
             {
                    TankSprite.Tank.X += TankSprite.Tank.Speed;
                    TankSprite.Tank.Rotation = 7.85f;
                    TankSprite.SpriteRectangle = new Rectangle(TankSprite.Tank.X, TankSprite.Tank.Y, 40, 49);
                    keyPressed = true;
                    SentData();
            }
                if (Keyboard.GetState().IsKeyDown(Keys.Left) && keyPressed == false)
                {
                    TankSprite.Tank.X -= TankSprite.Tank.Speed;
                    TankSprite.Tank.Rotation = 23.55f;
                    TankSprite.SpriteRectangle = new Rectangle(TankSprite.Tank.X, TankSprite.Tank.Y, 40, 49);
                    keyPressed = true;
                    SentData();
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Up) && keyPressed == false)
                {
                    TankSprite.Tank.Y -= TankSprite.Tank.Speed;
                    TankSprite.Tank.Rotation = 0f;
                    TankSprite.SpriteRectangle = new Rectangle(TankSprite.Tank.X, TankSprite.Tank.Y, 40, 49);
                    keyPressed = true;
                    SentData();
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Down) && keyPressed == false)
                {
                    TankSprite.Tank.Y += TankSprite.Tank.Speed;
                    TankSprite.Tank.Rotation = 15.7f;
                    TankSprite.SpriteRectangle = new Rectangle(TankSprite.Tank.X, TankSprite.Tank.Y, 40, 49);
                    keyPressed = true;
                    SentData();
                }

                keyPressed = false;

            GetData();
            base.Update(gameTime);
        }
        int a = 0;
        public void GetData()
        {
            a++;
            Window.Title = $"Get Try #{a}";
            try
            {
                AllTanks = JsonSerializer.Deserialize<List<Tank>>(Client.FromBytesToString(client.Get()));

                AllTankSprites.Clear();
                Window.Title = "Get Succsess";
                for (int i = 0; i < AllTanks.Count; i++)
            {
                AllTankSprites.Add(new Sprite(new Rectangle(AllTanks[i].X, AllTanks[i].Y, 40, 49), null, "Textures/tank", AllTanks[i]));
                AllTankSprites[AllTankSprites.Count - 1].Texture = Content.Load<Texture2D>(TankSprite.TexturePath);
            }
            }
            catch (System.Exception ex)
            {
                Window.Title = "Get fail";
            }
        }
 
        public void SentData()
        {
            Window.Title = "Send try";
            string json = JsonSerializer.Serialize<Tank>(TankSprite.Tank);
                client.Send(Client.FromStringToBytes(json));
            Window.Title = "Send success";

        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            //_spriteBatch.Draw(TankSprite.Texture, new Vector2(TankSprite.SpriteRectangle.X, TankSprite.SpriteRectangle.Y),TankSprite.Color);
            
           
            foreach (var item in AllTankSprites)
            {
                _spriteBatch.Draw(item.Texture, new Rectangle(item.Tank.X, item.Tank.Y, item.Texture.Width, item.Texture.Height), null,new Color( item.Tank.Color[0], item.Tank.Color[1], item.Tank.Color[2]),item.Tank.Rotation, new Vector2(item.Texture.Width / 2f, item.Texture.Height / 2f), SpriteEffects.None,0f);
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
