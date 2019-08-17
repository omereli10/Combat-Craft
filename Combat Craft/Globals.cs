using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace Combat_Craft
{
    // The BlockType Enum parameters has to be sort by their row in the atlas file.
    enum BlockType { Grass, Stone, Snow, End_Stone, Water, Sand, Wood_Trunk, Leaf, Error_Block, Israel, Squares, Brick_Wall }
    enum PerlinNoise_Type { Island, Regular, Mountain, Flat, Moon }

    static class Globals
    {
        #region Data

        // Screen
        public static DepthStencilState depthStencilState; // 3D Depth Graphics
        public static Vector2 middleOfTheScreen;
        public static bool Splash_Screen;
        public static bool Splash_HasGameStart;
        public static bool Splash_GameOver;
        public static float GameOverCloseGameTime;
        public static bool mouseLock;

        // Managers
        public static ContentManager contentManager;
        public static GraphicsDevice graphicsDevice;
        public static SpriteBatch spriteBatch;
        public static Random random;

        // World
        public static Perlin_Noise perlin_noise;
        public static BlockType[] addableBlockTypes;
        public const double saftyDistanceFromBlock = 0.15;
        public static float nextEnemieSpawnTime;
        public static float enemiesAmount;

        // Block
        public static Texture2D texturesAtlas;
        public static BasicEffect blockBasicEffect;
        public static BasicEffect waterBasicEffect;
        public static BasicEffect enemieBasicEffect;
        public static VertexBuffer block_ColorPosition_VertexBuffer;
        public static float block_Xoffset;
        public static float block_Yoffset;
        public static float block_Zoffset;

        // Chunk
        public static VertexBuffer chunksBuffer;
        public static int chunksRendering;
        public static int chunksLoad;

        // Status
        public static SpriteFont statusFont;
        
        #endregion Data

        #region Methods

        public static void Initialize(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            Globals.contentManager = contentManager;
            Globals.graphicsDevice = graphicsDevice;
            Globals.depthStencilState = new DepthStencilState();
            Globals.middleOfTheScreen = new Vector2(Globals.graphicsDevice.Viewport.Width / 2, Globals.graphicsDevice.Viewport.Height / 2);
            Globals.Splash_Screen = true;
            Globals.Splash_HasGameStart = false;
            Globals.Splash_GameOver = false;
            Globals.mouseLock = true;
            Globals.depthStencilState.DepthBufferEnable = true;
            Globals.texturesAtlas = Globals.contentManager.Load<Texture2D>("BlocksTextures/TexturesAtlas");
            Globals.random = new Random();
            Globals.perlin_noise = new Perlin_Noise();
            Globals.addableBlockTypes = new BlockType[] { BlockType.Grass, BlockType.Stone, BlockType.Snow, BlockType.Sand, BlockType.Wood_Trunk, BlockType.Leaf, BlockType.Israel, BlockType.Squares, BlockType.Brick_Wall };
            Globals.spriteBatch = new SpriteBatch(Globals.graphicsDevice);
            Globals.block_Xoffset = Settings.BLOCK_SIZE.X * 2;
            Globals.block_Yoffset = Settings.BLOCK_SIZE.Y * 2;
            Globals.block_Zoffset = Settings.BLOCK_SIZE.Z * 2;
            Globals.nextEnemieSpawnTime = 0;
            Globals.enemiesAmount = 0;
            Globals.statusFont = contentManager.Load<SpriteFont>("Fonts/myFont");
            Globals.chunksBuffer = new VertexBuffer(Globals.graphicsDevice, VertexPositionNormalTexture.VertexDeclaration, 36 * Settings.CHUNK_SIZE * Settings.CHUNK_SIZE * Settings.CHUNK_SIZE, BufferUsage.None);
            Globals.block_ColorPosition_VertexBuffer = new VertexBuffer(Globals.graphicsDevice, VertexPositionColor.VertexDeclaration, 36, BufferUsage.WriteOnly);
            Globals.blockBasicEffect = new BasicEffect(Globals.graphicsDevice);
            Globals.blockBasicEffect.TextureEnabled = true;
            Globals.blockBasicEffect.EnableDefaultLighting();
            Globals.blockBasicEffect.FogColor = Color.Blue.ToVector3();
            Globals.blockBasicEffect.FogStart = -10;
            Globals.blockBasicEffect.FogEnd = 40;
            Globals.waterBasicEffect = new BasicEffect(Globals.graphicsDevice);
            Globals.waterBasicEffect.Alpha = Settings.water_transparency;
            Globals.waterBasicEffect.TextureEnabled = true;
            Globals.waterBasicEffect.EnableDefaultLighting();
            Globals.enemieBasicEffect = new BasicEffect(Globals.graphicsDevice);
            Globals.enemieBasicEffect.TextureEnabled = false;
            Globals.enemieBasicEffect.VertexColorEnabled = true;
            BlockTexturesManager.Initialize();
            GameDictionaries.Initialize();
        }

        // Normalize X-Z axis of a vector
        public static Vector3 NormalizeXZ(Vector3 vec)
        {
            double vecLength = Math.Sqrt((vec.X * vec.X) + (vec.Z * vec.Z));
            if (vecLength != 0) { vec.X = vec.X / (float)vecLength; vec.Z = vec.Z / (float)vecLength; }
            return vec;
        }

        public static float DistanceBetweenTwoVector2(Vector2 v1, Vector2 v2)
        {
            float deltaX = v1.X - v2.X;
            float deltaY = v1.Y - v2.Y;

            return (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        }
        #endregion Methods
    }
}