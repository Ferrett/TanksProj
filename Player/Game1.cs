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
        public Color Color;

        public Sprite(Rectangle spriteRectangle, Texture2D texture, string texturePath, Tank tank,Color color)
        {

            SpriteRectangle = spriteRectangle;
            Texture = texture;
            TexturePath = texturePath;
            Tank = tank;
            Color = color;
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

           
            //AllTanks = JsonSerializer.Deserialize<List<Tank>>(Server.FromBytesToString(client.Get()));

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
            TankSprite = new Sprite(new Rectangle(0, 0, 40, 49), null, "Textures/tank",tank,Color.White);
            

            TankSprite.Texture = Content.Load<Texture2D>(TankSprite.TexturePath);
        }
        int a = 0;
        int b = 0;
        bool keyPressed = false;
        protected override void Update(GameTime gameTime)
        {
            Window.Title = b.ToString();
            b++;
            a++;
            if (a %6 == 0&& a!=0)
            GetData();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

          
            if (Keyboard.GetState().IsKeyDown(Keys.Right)&& keyPressed ==false)
            {
                TankSprite.Tank.X+= TankSprite.Tank.Speed;
                TankSprite.SpriteRectangle = new Rectangle(TankSprite.Tank.X, TankSprite.Tank.Y, 40, 49);
                keyPressed = true;
                SentData();
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left) && keyPressed == false)
            {
                TankSprite.Tank.X -= TankSprite.Tank.Speed;
                TankSprite.SpriteRectangle = new Rectangle(TankSprite.Tank.X, TankSprite.Tank.Y, 40, 49);
                keyPressed = true;
                SentData();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Up) && keyPressed == false)
            {
                TankSprite.Tank.Y -= TankSprite.Tank.Speed;
                TankSprite.SpriteRectangle = new Rectangle(TankSprite.Tank.X, TankSprite.Tank.Y, 40, 49);
                keyPressed = true;
                SentData();
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down) && keyPressed == false)
            {
                TankSprite.Tank.Y += TankSprite.Tank.Speed;
                TankSprite.SpriteRectangle = new Rectangle(TankSprite.Tank.X, TankSprite.Tank.Y, 40, 49);
                keyPressed = true;
                SentData();
            }

            keyPressed = false;

           
            base.Update(gameTime);
        }

        public void GetData()
        {
            try
            {
                AllTanks = JsonSerializer.Deserialize<List<Tank>>(Client.FromBytesToString(client.Get()));


                AllTankSprites.Clear();
                a = 0;
            for (int i = 0; i < AllTanks.Count; i++)
            {
                AllTankSprites.Add(new Sprite(new Rectangle(AllTanks[i].X, AllTanks[i].Y, 40, 49), null, "Textures/tank", AllTanks[i], Color.White));
                AllTankSprites[AllTankSprites.Count - 1].Texture = Content.Load<Texture2D>(TankSprite.TexturePath);
            }
            }
            catch (System.Exception ex)
            {
            }
        }
 
        public void SentData()
        {
                string json = JsonSerializer.Serialize<Tank>(TankSprite.Tank);
                client.Send(Client.FromStringToBytes(json));
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            //_spriteBatch.Draw(TankSprite.Texture, new Vector2(TankSprite.SpriteRectangle.X, TankSprite.SpriteRectangle.Y),TankSprite.Color);

            foreach (var item in AllTankSprites)
            {
                _spriteBatch.Draw(item.Texture, new Vector2(item.SpriteRectangle.X, item.SpriteRectangle.Y), item.Color);
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
