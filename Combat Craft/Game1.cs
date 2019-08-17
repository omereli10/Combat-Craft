using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Combat_Craft
{
    public class Game1 : Game
    {
        #region Data

        GraphicsDeviceManager graphics;
        Player player;
        World world;

        #endregion Data

        #region Constructors

        public Game1()
        {
            Settings.Initialize();
            graphics = new GraphicsDeviceManager(this);
            if(Settings.FullScreen)
            {
                graphics.IsFullScreen = true;
                graphics.PreferredBackBufferWidth = 1920;
                graphics.PreferredBackBufferHeight = 1080;
            }
            Content.RootDirectory = "Content";
        }

        #endregion Constructors

        #region Methods

        protected override void Initialize()
        {
            Globals.Initialize(this.Content, this.GraphicsDevice);
            player = new Player(this, new UserKeyboard(Keys.A, Keys.D, Keys.W, Keys.S, Keys.C, Keys.X, Keys.LeftShift, Keys.LeftControl, Keys.Z, Keys.Space, Keys.P, Keys.Right, Keys.Left));
            Components.Add(player.camera);
            world = new World(player);

            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            if (Globals.mouseLock)
            { this.IsMouseVisible = false; }
            else
            { this.IsMouseVisible = true; }
            player.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            world.Update_Render(player, gameTime);
            player.Draw(gameTime);

            base.Draw(gameTime);
        }

        #endregion Methods
    }
}