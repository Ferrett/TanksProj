using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ClientServer;
using System.Text.Json;
using System.Collections.Generic;

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
        Tank tank;
        Sprite TankSprite;
        Client client;
       
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

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

          
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                SentData();
                TankSprite.Tank.X+= TankSprite.Tank.Speed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                SentData();
                TankSprite.Tank.X -= TankSprite.Tank.Speed;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                SentData();
                TankSprite.Tank.Y -= TankSprite.Tank.Speed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                SentData();
                TankSprite.Tank.Y += TankSprite.Tank.Speed;
            }

            TankSprite.SpriteRectangle = new Rectangle(TankSprite.Tank.X , TankSprite.Tank.Y, 40, 49);

           
            base.Update(gameTime);
        }
        int a = 0;
        public void SentData()
        {
            a++;

            if (a == 5)
            {
                a = 0;
                string json = JsonSerializer.Serialize<Tank>(TankSprite.Tank);
                client.Send(Client.FromStringToBytes(json));
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            _spriteBatch.Draw(TankSprite.Texture, new Vector2(TankSprite.SpriteRectangle.X, TankSprite.SpriteRectangle.Y),TankSprite.Color);

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
