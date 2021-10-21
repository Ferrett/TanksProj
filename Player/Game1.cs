using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ClientServer;
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

        Tank tank;
        Sprite TankSprite;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            _graphics.PreferredBackBufferWidth = 1200;
            _graphics.PreferredBackBufferHeight = 800;
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

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                TankSprite.Tank.X+= TankSprite.Tank.Speed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                TankSprite.Tank.X -= TankSprite.Tank.Speed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                TankSprite.Tank.Y -= TankSprite.Tank.Speed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                TankSprite.Tank.Y += TankSprite.Tank.Speed;
            }

            TankSprite.SpriteRectangle = new Rectangle(TankSprite.Tank.X , TankSprite.Tank.Y, 40, 49);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            _spriteBatch.Draw(TankSprite.Texture, new Vector2(TankSprite.SpriteRectangle.X, TankSprite.SpriteRectangle.Y), Color.White);

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
