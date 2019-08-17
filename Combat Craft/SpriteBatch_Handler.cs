using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Combat_Craft
{
    class SpriteBatch_Handler
    {
        #region Data

        // Player's status
        private Player player;
        private int totalChunksAroundPlayer;

        // Status & Splash settings
        private Color color;
        private Texture2D crossImage;
        private Texture2D splashImage;
        private Texture2D gameOverImage;
        private Texture2D squareFrameImage;
        private Texture2D heartImage;
        private SpriteFont statusFont;
        private SpriteFont splashFont;
        private SpriteFont loadingFont;
        private String statusString;
        private String splashString;
        private String loadingString;
        private String playerHealthString;
        private Vector2 statusPosition;
        private Vector2 splashPosition;
        private Vector2 playerHealthPosition;
        private Vector2 loadingPosition;
        private Rectangle splashRectangle;
        private Rectangle heartRectangle;
        private Rectangle squareFrameRectangle;
        private Rectangle crossRectangle;

        // Tool Box
        private String toolBox_BlockTypeString;
        private Vector2 toolBox_BlockTypeStringPosition;
        private Vector2 toolBox_imagePosition;
        private Vector2 toolBox_imageSize;
        private Rectangle toolBox_Rectangle;

        // Status info
        private FramesPerSecond fps;

        #endregion Data

        #region Constructors

        public SpriteBatch_Handler(Player player)
        {
            this.player = player;
            Globals.chunksLoad = 0;
            this.totalChunksAroundPlayer = getAmountOfChunkAroundPlayer();
            this.splashFont = Globals.contentManager.Load<SpriteFont>("Fonts/splashFont");
            this.loadingFont = Globals.contentManager.Load<SpriteFont>("Fonts/loadingFont");
            this.statusFont = Globals.contentManager.Load<SpriteFont>("Fonts/statusFont");
            this.color = Settings.default_statusColor;
            this.fps = new FramesPerSecond();
            this.crossImage = Globals.contentManager.Load<Texture2D>("Asset/Cross");
            this.splashImage = Globals.contentManager.Load<Texture2D>("Asset/Combat Craft Splash");
            this.gameOverImage = Globals.contentManager.Load<Texture2D>("Asset/GameOver");
            this.heartImage = Globals.contentManager.Load<Texture2D>("Asset/Heart");
            this.squareFrameImage = Globals.contentManager.Load<Texture2D>("Asset/Square Frame");
            this.splashString = "Press The Space Bar To Start";
            this.splashPosition = new Vector2(Globals.middleOfTheScreen.X - (splashFont.MeasureString(splashString) / 2).X, Globals.middleOfTheScreen.Y - (splashFont.MeasureString(splashString) / 2).Y);
            this.loadingPosition = new Vector2(0, Globals.graphicsDevice.Viewport.Height - loadingFont.MeasureString("Loading").Y);
            this.toolBox_imageSize = new Vector2(100, 100);
            this.toolBox_imagePosition = new Vector2(toolBox_imageSize.X / 4, Globals.graphicsDevice.Viewport.Height - toolBox_imageSize.Y - toolBox_imageSize.X / 4);
            this.toolBox_BlockTypeStringPosition = new Vector2(toolBox_imageSize.X / 4, toolBox_imagePosition.Y + toolBox_imageSize.Y + 5);
            this.splashRectangle = new Rectangle(new Point(0, 0), new Point(Globals.graphicsDevice.Viewport.Width, Globals.graphicsDevice.Viewport.Height));
            this.squareFrameRectangle = new Rectangle(new Point((int)this.toolBox_imagePosition.X - 5, (int)toolBox_imagePosition.Y - 5), new Point((int)this.toolBox_imageSize.X + 10, (int)toolBox_imageSize.Y + 10));
            this.heartRectangle = new Rectangle(new Point((int)this.toolBox_imagePosition.X + 120, (int)toolBox_imagePosition.Y - 5), new Point((int)this.toolBox_imageSize.X + 30, (int)toolBox_imageSize.Y + 10));
            this.crossRectangle = new Rectangle(new Point(Globals.graphicsDevice.Viewport.Width / 2 - 6, Globals.graphicsDevice.Viewport.Height / 2 - 6), new Point(12, 12));
            this.toolBox_Rectangle = new Rectangle(new Point((int)this.toolBox_imagePosition.X, (int)toolBox_imagePosition.Y), new Point((int)this.toolBox_imageSize.X, (int)toolBox_imageSize.Y));
            if (Settings.ShowStatus)
            { this.statusPosition = Settings.default_statusPosition; }
        }

        #endregion Constructors

        #region Methods

        public void Update_Render(GameTime gameTime)
        {
            Globals.spriteBatch.Begin();
            if (Globals.Splash_HasGameStart && !Globals.Splash_GameOver)
            {
                // Cross in the middle of the screen
                Globals.spriteBatch.Draw(crossImage, this.crossRectangle, Color.White);
                if (Settings.ShowStatus)
                {
                    // Update the status
                    this.Update(gameTime);
                    // Status
                    this.statusString = "FPS: " + this.fps.FPS.ToString("00.000") +
                                          "\nChunks Load: " + Globals.chunksLoad +
                                          "\nChunks Rendering: " + Globals.chunksRendering +
                                          "\nSeed: " + Globals.perlin_noise.seed +
                                          "\nX: " + this.player.camera.Position.X.ToString("00.000") +
                                          "\nY: " + this.player.camera.Position.Y.ToString("00.000") +
                                          "\nZ: " + this.player.camera.Position.Z.ToString("00.000") +
                                          "\nFlying Mode: " + this.player.flyingMode +
                                          "\nFalling Speed: " + this.player.fallingSpeed +
                                          "\nIs In Water: " + this.player.isInWater +
                                          "\nEnemies Amount: " + Globals.enemiesAmount +
                                          "\nPlayer Health: " + player.health + "%";
                    Globals.spriteBatch.DrawString(this.statusFont, statusString, this.statusPosition, this.color);
                }

                // Draw Tool Box
                Globals.spriteBatch.Draw(this.squareFrameImage, this.squareFrameRectangle, this.color);
                this.toolBox_BlockTypeString = Globals.addableBlockTypes[player.baseMouseKeyboard.mouseWheel_Value].ToString();
                Globals.spriteBatch.Draw(Globals.texturesAtlas, this.toolBox_Rectangle, new Rectangle(64, 0 + (int)Globals.addableBlockTypes[player.baseMouseKeyboard.mouseWheel_Value] * 64, 64, 64), this.color);
                Globals.spriteBatch.DrawString(this.statusFont, this.toolBox_BlockTypeString, this.toolBox_BlockTypeStringPosition, this.color);

                // Draw player's health
                Globals.spriteBatch.Draw(this.heartImage, this.heartRectangle, this.color);
                this.playerHealthString = player.health.ToString() + "%";
                this.playerHealthPosition = new Vector2(250 - splashFont.MeasureString(playerHealthString).X, Globals.graphicsDevice.Viewport.Height - (splashFont.MeasureString(playerHealthString) / 2).Y - 80);
                Globals.spriteBatch.DrawString(this.splashFont, this.playerHealthString, this.playerHealthPosition, this.color);
            }
            else
            {
                if (Globals.Splash_Screen)
                {
                    // Drawing Splash Image
                    Globals.spriteBatch.Draw(this.splashImage, this.splashRectangle, Color.White);

                    // Draw loading string with load precntage
                    this.loadingString = "Loading " + MathHelper.Clamp((((Globals.chunksLoad * 100)/ totalChunksAroundPlayer) / 10) * 10, 0, 100) + "%";
                    Globals.spriteBatch.DrawString(this.loadingFont, this.loadingString, loadingPosition, this.color);
                }
                else
                {
                    if (Globals.Splash_GameOver)
                    {
                        // Drawing Game Over Splash Image
                        Globals.spriteBatch.Draw(this.gameOverImage, this.splashRectangle, Color.White);
                    }
                    else
                    {
                        this.UnLoad_SplashScreen();
                        Globals.spriteBatch.DrawString(this.splashFont, this.splashString, this.splashPosition, this.color);
                    }
                }
            }
            Globals.spriteBatch.End();
        }

        private void Update(GameTime gameTime)
        {
            this.fps.Update(gameTime);
        }

        private int getAmountOfChunkAroundPlayer()
        {
            #region Define World Current Range

            int current_PX = (int)player.camera.Position.X + Settings.worldRenderingDistance * Settings.CHUNK_SIZE;
            int current_NX = (int)player.camera.Position.X - Settings.worldRenderingDistance * Settings.CHUNK_SIZE;
            int current_PZ = (int)player.camera.Position.Z + Settings.worldRenderingDistance * Settings.CHUNK_SIZE;
            int current_NZ = (int)player.camera.Position.Z - Settings.worldRenderingDistance * Settings.CHUNK_SIZE;

            #endregion Define World Current Range

            int total = 0;
            for (int x = current_NX; x < current_PX; x += Settings.CHUNK_SIZE)
            {
                for (int z = current_NZ; z < current_PZ; z += Settings.CHUNK_SIZE)
                { total += ((World.maxHeightInChunk(x, z) + Settings.CHUNK_SIZE) / Settings.CHUNK_SIZE) * 2; }
            }

            return total;
        }

        private void UnLoad_SplashScreen()
        {
            if (this.splashImage != null)
            { this.splashImage = null; }
            if (this.loadingFont != null)
            { this.loadingFont = null; }
            if (this.loadingString != null)
            { this.loadingString = null; }
        }

        #endregion Methods
    }
}