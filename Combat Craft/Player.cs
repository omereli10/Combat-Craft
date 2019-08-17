using Microsoft.Xna.Framework;

namespace Combat_Craft
{
    class Player
    {
        #region Data

        // Data
        public Game game;
        public Camera camera;
        public BaseMouseKeyboard baseMouseKeyboard;
        private RayBlock rayBlock;
        private SpriteBatch_Handler status;

        // Player's States
        public float health;
        public bool flyingMode;
        public float fallingSpeed;
        public bool isFalling;
        public bool isInWater;
        public bool isHeadInWater;
        public float playerSpeed;

        #endregion Data

        #region Constructors

        public Player(Game game, BaseMouseKeyboard baseMouseKeyboard)
        {
            this.game = game;
            this.health = 100;
            this.baseMouseKeyboard = baseMouseKeyboard;
            this.flyingMode = true;
            this.fallingSpeed = 0;
            this.EnableCamera(new Camera(game, this, new Vector3(Settings.BLOCK_SIZE.X, Globals.perlin_noise.getPerlinNoise_MaxHeight() + (float)Settings.playerHeight + Globals.block_Yoffset, Settings.BLOCK_SIZE.Z)));
            this.rayBlock = new RayBlock(this);
            this.status = new SpriteBatch_Handler(this);
        }

        #endregion Constructors

        #region Methods

        public void EnableCamera(Camera camera)
        { this.camera = camera; }

        public void Update(GameTime gameTime)
        {
            #region Check Player State

            // Player died
            if (this.health <= 0 && !Globals.Splash_GameOver)
            {
                Globals.Splash_GameOver = true;
                Globals.GameOverCloseGameTime = (float)(gameTime.TotalGameTime.TotalSeconds + 7);

                // Unload the world
                GameDictionaries.blocksAddedDictionary.Clear();
                GameDictionaries.blocksDestroyedDictionary.Clear();
                GameDictionaries.blocksDictionary.Clear();
            }

            // Close the application
            if (Globals.Splash_GameOver)
            {
                if (Globals.GameOverCloseGameTime < gameTime.TotalGameTime.TotalSeconds)
                {
                    // Close the game
                    this.game.Exit();
                }
            }

            #endregion Check Player State

            #region Update Mouse Wheel Value

            if (this.baseMouseKeyboard.currentMouseState.ScrollWheelValue != this.baseMouseKeyboard.previousMouseWheel_Determine)
            {
                if (this.baseMouseKeyboard.currentMouseState.ScrollWheelValue > this.baseMouseKeyboard.previousMouseWheel_Determine)
                { this.baseMouseKeyboard.mouseWheel_Value = ++this.baseMouseKeyboard.mouseWheel_Value % Globals.addableBlockTypes.Length; }
                else
                { this.baseMouseKeyboard.mouseWheel_Value = (--this.baseMouseKeyboard.mouseWheel_Value + Globals.addableBlockTypes.Length) % Globals.addableBlockTypes.Length; }
                this.baseMouseKeyboard.previousMouseWheel_Determine = this.baseMouseKeyboard.currentMouseState.ScrollWheelValue;
            }

            #endregion Update Mouse Wheel Value

            #region Update Effects

            Globals.blockBasicEffect.FogEnabled = this.isHeadInWater;

            #endregion Update Effects
        }

        public void Draw(GameTime gameTime)
        {
            if (this.baseMouseKeyboard.IsJump() && !Globals.Splash_Screen && !Globals.Splash_HasGameStart)
            { Globals.Splash_HasGameStart = true; this.flyingMode = false; }
            this.rayBlock.Update_Render();
            this.status.Update_Render(gameTime);
        }

        #endregion Methods
    }
}