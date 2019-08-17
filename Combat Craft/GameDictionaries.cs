using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Combat_Craft
{
    static class GameDictionaries
    {
        #region Data
        
        public static Dictionary<Vector3, BlockType> blocksDictionary;
        public static Dictionary<Vector3, BlockType> blocksAddedDictionary;
        public static Dictionary<Vector3, bool> blocksDestroyedDictionary;
        public static Dictionary<Vector3, Chunk> chunksRenderingDictionary;
        public static Dictionary<Vector3, int> chunksHeightDictionary;
        public static Dictionary<BlockType, VertexPositionNormalTexture[]> blocksVerticesDictionary;

        #endregion Data

        #region Methods

        public static void Initialize()
        {
            blocksDictionary = new Dictionary<Vector3, BlockType>();
            blocksAddedDictionary = new Dictionary<Vector3, BlockType>();
            blocksDestroyedDictionary = new Dictionary<Vector3, bool>();
            chunksRenderingDictionary = new Dictionary<Vector3, Chunk>();
            chunksHeightDictionary = new Dictionary<Vector3, int>();
            blocksVerticesDictionary = new Dictionary<BlockType, VertexPositionNormalTexture[]>();
            Initialize_BlockTypeVerticesDictionary();
        }

        public static void Initialize_BlockTypeVerticesDictionary()
        {
            #region SET-UP Block Info

            // Normal vectors for each face (needed for lighting / display)
            Vector3 normalTop = new Vector3(1f, -0.1f, 1f);
            Vector3 normalSide = new Vector3(1f, -0.7f, 1f);
            Vector3 normalBottom = new Vector3(0f, -0.1f, 0f);

            #endregion SET-UP Block Info

            VertexPositionNormalTexture[] verticesList;
            BlockType blockType;
            for (int i=0; i < Enum.GetValues(typeof(BlockType)).Length; i++)
            {
                #region SET-UP Vertices List

                // Initialize variables
                verticesList = new VertexPositionNormalTexture[36];
                blockType = (BlockType)i;

                // Add the vertices for the PY face.
                verticesList[0] = new VertexPositionNormalTexture(Vector3.Zero, normalTop, BlockTexturesManager.blockTexturesDictionary[blockType].PY.bottomLeft);
                verticesList[1] = new VertexPositionNormalTexture(Vector3.Zero, normalTop, BlockTexturesManager.blockTexturesDictionary[blockType].PY.topRight);
                verticesList[2] = new VertexPositionNormalTexture(Vector3.Zero, normalTop, BlockTexturesManager.blockTexturesDictionary[blockType].PY.topLeft);
                verticesList[3] = new VertexPositionNormalTexture(Vector3.Zero, normalTop, BlockTexturesManager.blockTexturesDictionary[blockType].PY.bottomLeft);
                verticesList[4] = new VertexPositionNormalTexture(Vector3.Zero, normalTop, BlockTexturesManager.blockTexturesDictionary[blockType].PY.bottomRight);
                verticesList[5] = new VertexPositionNormalTexture(Vector3.Zero, normalTop, BlockTexturesManager.blockTexturesDictionary[blockType].PY.topRight);

                // Add the vertices for the NY face. 
                verticesList[6] = new VertexPositionNormalTexture(Vector3.Zero, normalBottom, BlockTexturesManager.blockTexturesDictionary[blockType].NY.topLeft);
                verticesList[7] = new VertexPositionNormalTexture(Vector3.Zero, normalBottom, BlockTexturesManager.blockTexturesDictionary[blockType].NY.bottomLeft);
                verticesList[8] = new VertexPositionNormalTexture(Vector3.Zero, normalBottom, BlockTexturesManager.blockTexturesDictionary[blockType].NY.bottomRight);
                verticesList[9] = new VertexPositionNormalTexture(Vector3.Zero, normalBottom, BlockTexturesManager.blockTexturesDictionary[blockType].NY.topLeft);
                verticesList[10] = new VertexPositionNormalTexture(Vector3.Zero, normalBottom, BlockTexturesManager.blockTexturesDictionary[blockType].NY.bottomRight);
                verticesList[11] = new VertexPositionNormalTexture(Vector3.Zero, normalBottom, BlockTexturesManager.blockTexturesDictionary[blockType].NY.topRight);

                // Add the vertices for the PX face. 
                verticesList[12] = new VertexPositionNormalTexture(Vector3.Zero, normalSide, BlockTexturesManager.blockTexturesDictionary[blockType].PX.topLeft);
                verticesList[13] = new VertexPositionNormalTexture(Vector3.Zero, normalSide, BlockTexturesManager.blockTexturesDictionary[blockType].PX.bottomLeft);
                verticesList[14] = new VertexPositionNormalTexture(Vector3.Zero, normalSide, BlockTexturesManager.blockTexturesDictionary[blockType].PX.bottomRight);
                verticesList[15] = new VertexPositionNormalTexture(Vector3.Zero, normalSide, BlockTexturesManager.blockTexturesDictionary[blockType].PX.topRight);
                verticesList[16] = new VertexPositionNormalTexture(Vector3.Zero, normalSide, BlockTexturesManager.blockTexturesDictionary[blockType].PX.topLeft);
                verticesList[17] = new VertexPositionNormalTexture(Vector3.Zero, normalSide, BlockTexturesManager.blockTexturesDictionary[blockType].PX.bottomRight);

                // Add the vertices for the NX face.
                verticesList[18] = new VertexPositionNormalTexture(Vector3.Zero, normalSide, BlockTexturesManager.blockTexturesDictionary[blockType].NX.topRight);
                verticesList[19] = new VertexPositionNormalTexture(Vector3.Zero, normalSide, BlockTexturesManager.blockTexturesDictionary[blockType].NX.bottomLeft);
                verticesList[20] = new VertexPositionNormalTexture(Vector3.Zero, normalSide, BlockTexturesManager.blockTexturesDictionary[blockType].NX.bottomRight);
                verticesList[21] = new VertexPositionNormalTexture(Vector3.Zero, normalSide, BlockTexturesManager.blockTexturesDictionary[blockType].NX.topLeft);
                verticesList[22] = new VertexPositionNormalTexture(Vector3.Zero, normalSide, BlockTexturesManager.blockTexturesDictionary[blockType].NX.bottomLeft);
                verticesList[23] = new VertexPositionNormalTexture(Vector3.Zero, normalSide, BlockTexturesManager.blockTexturesDictionary[blockType].NX.topRight);

                // Add the vertices for the PZ face.
                verticesList[24] = new VertexPositionNormalTexture(Vector3.Zero, normalSide, BlockTexturesManager.blockTexturesDictionary[blockType].PZ.topRight);
                verticesList[25] = new VertexPositionNormalTexture(Vector3.Zero, normalSide, BlockTexturesManager.blockTexturesDictionary[blockType].PZ.topLeft);
                verticesList[26] = new VertexPositionNormalTexture(Vector3.Zero, normalSide, BlockTexturesManager.blockTexturesDictionary[blockType].PZ.bottomRight);
                verticesList[27] = new VertexPositionNormalTexture(Vector3.Zero, normalSide, BlockTexturesManager.blockTexturesDictionary[blockType].PZ.bottomRight);
                verticesList[28] = new VertexPositionNormalTexture(Vector3.Zero, normalSide, BlockTexturesManager.blockTexturesDictionary[blockType].PZ.topLeft);
                verticesList[29] = new VertexPositionNormalTexture(Vector3.Zero, normalSide, BlockTexturesManager.blockTexturesDictionary[blockType].PZ.bottomLeft);

                // Add the vertices for the NZ face.
                verticesList[30] = new VertexPositionNormalTexture(Vector3.Zero, normalSide, BlockTexturesManager.blockTexturesDictionary[blockType].NZ.topLeft);
                verticesList[31] = new VertexPositionNormalTexture(Vector3.Zero, normalSide, BlockTexturesManager.blockTexturesDictionary[blockType].NZ.bottomLeft);
                verticesList[32] = new VertexPositionNormalTexture(Vector3.Zero, normalSide, BlockTexturesManager.blockTexturesDictionary[blockType].NZ.topRight);
                verticesList[33] = new VertexPositionNormalTexture(Vector3.Zero, normalSide, BlockTexturesManager.blockTexturesDictionary[blockType].NZ.bottomLeft);
                verticesList[34] = new VertexPositionNormalTexture(Vector3.Zero, normalSide, BlockTexturesManager.blockTexturesDictionary[blockType].NZ.bottomRight);
                verticesList[35] = new VertexPositionNormalTexture(Vector3.Zero, normalSide, BlockTexturesManager.blockTexturesDictionary[blockType].NZ.topRight);

                #endregion SET-UP Vertices List

                GameDictionaries.blocksVerticesDictionary[blockType] = verticesList;
            }
        }

        #endregion Methods
    }
}