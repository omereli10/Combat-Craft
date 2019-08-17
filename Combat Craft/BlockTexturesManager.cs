using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Combat_Craft
{
    static class BlockTexturesManager
    {
        #region Data

        // Data
        public static List<String> blockTypesStrings;
        public static Dictionary<BlockType, BlockTextures> blockTexturesDictionary;
        public const int texturePixelsAmount = 64;

        // Info relative to the texture size
        public static float textureWidth;
        public static float textureHeight;
        public static float pixelWidth;
        public static float pixelHeight;

        #endregion Data

        #region Methods

        public static void Initialize()
        {
            Globals.texturesAtlas = Globals.contentManager.Load<Texture2D>("BlocksTextures/TexturesAtlas");
            BlockTexturesManager.blockTypesStrings = Enum.GetNames(typeof(BlockType)).ToList();
            Globals.blockBasicEffect.Texture = Globals.texturesAtlas;
            Globals.waterBasicEffect.Texture = Globals.blockBasicEffect.Texture;
            BlockTexturesManager.textureHeight = (float)texturePixelsAmount / Globals.texturesAtlas.Height;
            BlockTexturesManager.textureWidth = 1f / 6f;
            BlockTexturesManager.pixelHeight = 1f / Globals.texturesAtlas.Height;
            BlockTexturesManager.pixelWidth = 1f / Globals.texturesAtlas.Width;
            BlockTexturesManager.SetupBlockTexturesDictionary();
        }

        private static void SetupBlockTexturesDictionary()
        {
            BlockTexturesManager.blockTexturesDictionary = new Dictionary<BlockType, BlockTextures>();
            BlockType blockType;
            for (int row = 0; row < BlockTexturesManager.blockTypesStrings.Count; row++)
            {
                Enum.TryParse<BlockType>(blockTypesStrings[row], out blockType);
                BlockTexturesManager.blockTexturesDictionary[blockType] = new BlockTextures(row);
            }
        }
    }

    class BlockTextures
    {
        #region Data

        public BlockFaceTexture PY;
        public BlockFaceTexture PX;
        public BlockFaceTexture NZ;
        public BlockFaceTexture NX;
        public BlockFaceTexture PZ;
        public BlockFaceTexture NY;

        #endregion Data

        #region Constructors

        public BlockTextures(int row)
        {
            this.PY = new BlockFaceTexture(row, 0);
            this.PX = new BlockFaceTexture(row, 1);
            this.NZ = new BlockFaceTexture(row, 2);
            this.NX = new BlockFaceTexture(row, 3);
            this.PZ = new BlockFaceTexture(row, 4);
            this.NY = new BlockFaceTexture(row, 5);
        }

        #endregion Constructors
    }

    class BlockFaceTexture
    {
        #region Data

        public Vector2 topLeft;
        public Vector2 topRight;
        public Vector2 bottomLeft;
        public Vector2 bottomRight;

        #endregion Data

        #region Constructors

        public BlockFaceTexture(int row, int column)
        {
            this.topLeft     = new Vector2(BlockTexturesManager.textureWidth * column + BlockTexturesManager.pixelWidth, BlockTexturesManager.textureHeight * row + BlockTexturesManager.pixelHeight);
            this.topRight    = new Vector2(BlockTexturesManager.textureWidth * (column + 1) - BlockTexturesManager.pixelWidth, BlockTexturesManager.textureHeight * row + BlockTexturesManager.pixelHeight);
            this.bottomLeft  = new Vector2(BlockTexturesManager.textureWidth * column + BlockTexturesManager.pixelWidth, BlockTexturesManager.textureHeight * (row + 1) - BlockTexturesManager.pixelHeight);
            this.bottomRight = new Vector2(BlockTexturesManager.textureWidth * (column + 1) - BlockTexturesManager.pixelWidth, BlockTexturesManager.textureHeight * (row + 1) - BlockTexturesManager.pixelHeight);
        }

        #endregion Constructors
    }

    #endregion Methods
}