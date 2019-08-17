using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Combat_Craft
{
    class Chunk
    {
        #region Data

        public Vector3 offset;
        public List<VertexPositionNormalTexture> chunkBlockVertices;
        public List<VertexPositionNormalTexture> chunkAboveWaterVertices;
        public List<VertexPositionNormalTexture> chunkBelowWaterVertices;
        public BoundingBox chunkBoundingBox;
        public bool hasChunkMeshBuilt;

        #endregion Data

        #region Constructors

        public Chunk(Vector3 offset)
        {
            this.offset = offset;
            this.hasChunkMeshBuilt = false;
            this.chunkBlockVertices = new List<VertexPositionNormalTexture>();
            this.chunkAboveWaterVertices = new List<VertexPositionNormalTexture>();
            this.chunkBelowWaterVertices = new List<VertexPositionNormalTexture>();
            Globals.chunksLoad++;

            BuildBlocksChunk();
        }

        #endregion Constructors

        #region Methods

        public void BuildChunkMesh()
        {
            #region Build Mesh

            this.chunkBlockVertices.Clear();
            this.chunkAboveWaterVertices.Clear();
            this.chunkBelowWaterVertices.Clear();

            Vector3 block_offset;
            List<VertexPositionNormalTexture> allVertices;
            for (float x = this.offset.X; x < this.offset.X + Settings.CHUNK_SIZE; x += Globals.block_Xoffset)
            {
                for (float y = this.offset.Y; y < this.offset.Y + Settings.CHUNK_SIZE; y += Globals.block_Yoffset)
                {
                    for (float z = this.offset.Z; z < this.offset.Z + Settings.CHUNK_SIZE; z += Globals.block_Zoffset)
                    {
                        block_offset = new Vector3(x, y, z);
                        if (GameDictionaries.blocksDictionary.ContainsKey(block_offset))
                        {
                            // Get block vertices
                            allVertices = Chunk.GetBlockVerticesList(GameDictionaries.blocksDictionary[block_offset], block_offset);

                            #region Water Block

                            if (GameDictionaries.blocksDictionary[block_offset] == BlockType.Water)
                            {
                                if (!GameDictionaries.blocksDictionary.ContainsKey(new Vector3(x, y + Globals.block_Yoffset, z)))
                                {
                                    //PY
                                    this.chunkAboveWaterVertices.AddRange(allVertices.Skip(0).Take(6));
                                    // NY
                                    this.chunkBelowWaterVertices.AddRange(allVertices.Skip(6).Take(6));
                                }
                            }

                            #endregion Water Block

                            #region Regular Block

                            else
                            {
                                //PY
                                if (!GameDictionaries.blocksDictionary.ContainsKey(new Vector3(x, y + Globals.block_Yoffset, z)))
                                { this.chunkBlockVertices.AddRange(allVertices.Skip(0).Take(6)); }
                                else
                                {
                                    if (GameDictionaries.blocksDictionary[new Vector3(x, y + Globals.block_Yoffset, z)] == BlockType.Water)
                                    { this.chunkBlockVertices.AddRange(allVertices.Skip(0).Take(6)); }
                                }

                                //NY
                                if (!GameDictionaries.blocksDictionary.ContainsKey(new Vector3(x, y - Globals.block_Yoffset, z)))
                                {
                                    // Don't add the bottom world layer to the chunk mesh
                                    if (y != 0) { this.chunkBlockVertices.AddRange(allVertices.Skip(6).Take(6)); }
                                }
                                else
                                {
                                    if (GameDictionaries.blocksDictionary[new Vector3(x, y - Globals.block_Yoffset, z)] == BlockType.Water)
                                    { this.chunkBlockVertices.AddRange(allVertices.Skip(6).Take(6)); }
                                }

                                // PX
                                if (!GameDictionaries.blocksDictionary.ContainsKey(new Vector3(x + Globals.block_Xoffset, y, z)))
                                { this.chunkBlockVertices.AddRange(allVertices.Skip(12).Take(6)); }
                                else
                                {
                                    if (GameDictionaries.blocksDictionary[new Vector3(x + Globals.block_Xoffset, y, z)] == BlockType.Water)
                                    { this.chunkBlockVertices.AddRange(allVertices.Skip(12).Take(6)); }
                                }

                                // NX
                                if (!GameDictionaries.blocksDictionary.ContainsKey(new Vector3(x - Globals.block_Xoffset, y, z)))
                                { this.chunkBlockVertices.AddRange(allVertices.Skip(18).Take(6)); }
                                else
                                {
                                    if (GameDictionaries.blocksDictionary[new Vector3(x - Globals.block_Xoffset, y, z)] == BlockType.Water)
                                    { this.chunkBlockVertices.AddRange(allVertices.Skip(18).Take(6)); }
                                }

                                // PZ
                                if (!GameDictionaries.blocksDictionary.ContainsKey(new Vector3(x, y, z + Globals.block_Zoffset)))
                                { this.chunkBlockVertices.AddRange(allVertices.Skip(24).Take(6)); }
                                else
                                {
                                    if (GameDictionaries.blocksDictionary[new Vector3(x, y, z + Globals.block_Zoffset)] == BlockType.Water)
                                    { this.chunkBlockVertices.AddRange(allVertices.Skip(24).Take(6)); }
                                }

                                // NZ
                                if (!GameDictionaries.blocksDictionary.ContainsKey(new Vector3(x, y, z - Globals.block_Zoffset)))
                                { this.chunkBlockVertices.AddRange(allVertices.Skip(30).Take(6)); }
                                else
                                {
                                    if (GameDictionaries.blocksDictionary[new Vector3(x, y, z - Globals.block_Zoffset)] == BlockType.Water)
                                    { this.chunkBlockVertices.AddRange(allVertices.Skip(30).Take(6)); }
                                }
                            }

                            #endregion Regular Block
                        }
                    }
                }
            }

            #endregion Build Mesh

            this.hasChunkMeshBuilt = true;
        }

        public void RenderChunk_Blocks()
        {
            #region Render Blocks

            if (this.chunkBlockVertices != null)
            {
                if (this.chunkBlockVertices.Count != 0)
                {
                    Globals.graphicsDevice.SetVertexBuffer(Globals.chunksBuffer);
                    Globals.chunksBuffer.SetData(this.chunkBlockVertices.ToArray());
                    Globals.graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, this.chunkBlockVertices.Count / 3);
                }
            }

            #endregion Render Blocks

            Globals.chunksRendering++;
        }

        public void RenderChunk_AboveWater()
        {
            #region Render Above Water

            if (this.chunkAboveWaterVertices != null)
            {
                if (this.chunkAboveWaterVertices.Count != 0)
                {
                    Globals.graphicsDevice.SetVertexBuffer(Globals.chunksBuffer);
                    Globals.chunksBuffer.SetData(this.chunkAboveWaterVertices.ToArray());
                    Globals.graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, this.chunkAboveWaterVertices.Count / 3);
                }
            }

            #endregion Render Above Water
        }

        public void RenderChunk_BelowWater()
        {
            #region Render Below Water

            if (this.chunkBelowWaterVertices != null)
            {
                if (this.chunkBelowWaterVertices.Count != 0)
                {
                    Globals.graphicsDevice.SetVertexBuffer(Globals.chunksBuffer);
                    Globals.chunksBuffer.SetData(this.chunkBelowWaterVertices.ToArray());
                    Globals.waterBasicEffect.CurrentTechnique.Passes[0].Apply();
                    Globals.graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, this.chunkBelowWaterVertices.Count / 3);
                }
            }

            #endregion Render Below Water
        }

        public void UnLoadChunk()
        {
            #region Clear Chunk Mesh

            this.chunkBlockVertices = null;
            this.chunkAboveWaterVertices = null;
            this.chunkBelowWaterVertices = null;

            #endregion Clear Chunk Mesh

            #region Remove All The Blocks Inside The Chunk

            Vector3 block_offset;
            for (int x = (int)this.offset.X; x < this.offset.X + Settings.CHUNK_SIZE; x++)
            {
                for (int y = (int)this.offset.Y; y < this.offset.Y + Settings.CHUNK_SIZE; y++)
                {
                    for (int z = (int)this.offset.Z; z < this.offset.Z + Settings.CHUNK_SIZE; z++)
                    {
                        block_offset = new Vector3(x, y, z);
                        if (GameDictionaries.blocksDictionary.ContainsKey(block_offset))
                        { GameDictionaries.blocksDictionary.Remove(block_offset); }
                    }
                }
            }

            #endregion Remove All The Blocks Inside The Chunk

            Globals.chunksLoad--;
        }

        public static List<VertexPositionNormalTexture> GetBlockVerticesList(BlockType blockType, Vector3 origin)
        {
            #region SET-UP Block Info

            Vector3 topLeftFront, topLeftBack, topRightFront, topRightBack, bottomLeftFront, bottomLeftBack, bottomRightFront, bottomRightBack;
            if (blockType != BlockType.Water)
            {
                // Calculate the position of the vertices on the TOP face.
                topLeftFront = origin + new Vector3(0f, 2f, 0f) * Settings.BLOCK_SIZE;
                topLeftBack = origin + new Vector3(0f, 2f, 2f) * Settings.BLOCK_SIZE;
                topRightFront = origin + new Vector3(2f, 2f, 0f) * Settings.BLOCK_SIZE;
                topRightBack = origin + new Vector3(2f, 2f, 2f) * Settings.BLOCK_SIZE;

                // Calculate the position of the vertices on the BOTTOM face.
                bottomLeftFront = origin + new Vector3(0f, 0f, 0f) * Settings.BLOCK_SIZE;
                bottomLeftBack = origin + new Vector3(0f, 0f, 2f) * Settings.BLOCK_SIZE;
                bottomRightFront = origin + new Vector3(2f, 0f, 0f) * Settings.BLOCK_SIZE;
                bottomRightBack = origin + new Vector3(2f, 0f, 2f) * Settings.BLOCK_SIZE;
            }
            else
            {
                // Calculate the position of the vertices on the TOP face.
                topLeftFront = origin + new Vector3(0f, 2f, 0f) * Settings.BLOCK_SIZE;
                topLeftBack = origin + new Vector3(0f, 2f, 2f) * Settings.BLOCK_SIZE;
                topRightFront = origin + new Vector3(2f, 2f, 0f) * Settings.BLOCK_SIZE;
                topRightBack = origin + new Vector3(2f, 2f, 2f) * Settings.BLOCK_SIZE;

                // Calculate the position of the vertices on the BOTTOM face.
                bottomLeftFront = origin + new Vector3(0f, 2f, 0f) * Settings.BLOCK_SIZE;
                bottomLeftBack = origin + new Vector3(0f, 2f, 2f) * Settings.BLOCK_SIZE;
                bottomRightFront = origin + new Vector3(2f, 2f, 0f) * Settings.BLOCK_SIZE;
                bottomRightBack = origin + new Vector3(2f, 2f, 2f) * Settings.BLOCK_SIZE;
            }

            #endregion SET-UP Block Info

            #region SET-UP Vertices List

            // Vertices List
            VertexPositionNormalTexture[] vertices = GameDictionaries.blocksVerticesDictionary[blockType];

            // Set the vertices for the PY face.
            vertices[0].Position = topLeftFront;
            vertices[1].Position = topRightBack;
            vertices[2].Position = topLeftBack;
            vertices[3].Position = topLeftFront;
            vertices[4].Position = topRightFront;
            vertices[5].Position = topRightBack;

            // Set the vertices for the NY face. 
            vertices[6].Position = bottomLeftFront;
            vertices[7].Position = bottomLeftBack;
            vertices[8].Position = bottomRightBack;
            vertices[9].Position = bottomLeftFront;
            vertices[10].Position = bottomRightBack;
            vertices[11].Position = bottomRightFront;

            // Set the vertices for the PX face. 
            vertices[12].Position = topRightFront;
            vertices[13].Position = bottomRightFront;
            vertices[14].Position = bottomRightBack;
            vertices[15].Position = topRightBack;
            vertices[16].Position = topRightFront;
            vertices[17].Position = bottomRightBack;

            // Set the vertices for the NX face.
            vertices[18].Position = topLeftFront;
            vertices[19].Position = bottomLeftBack;
            vertices[20].Position = bottomLeftFront;
            vertices[21].Position = topLeftBack;
            vertices[22].Position = bottomLeftBack;
            vertices[23].Position = topLeftFront;

            // Set the vertices for the PZ face.
            vertices[24].Position = topLeftBack;
            vertices[25].Position = topRightBack;
            vertices[26].Position = bottomLeftBack;
            vertices[27].Position = bottomLeftBack;
            vertices[28].Position = topRightBack;
            vertices[29].Position = bottomRightBack;

            // Add the vertices for the NZ face.
            vertices[30].Position = topLeftFront;
            vertices[31].Position = bottomLeftFront;
            vertices[32].Position = topRightFront;
            vertices[33].Position = bottomLeftFront;
            vertices[34].Position = bottomRightFront;
            vertices[35].Position = topRightFront;

            #endregion SET-UP Vertices List

            return new List<VertexPositionNormalTexture>(vertices);
        }

        public static Vector3 getChunkOffSet(float x, float y, float z)
        {
            return new Vector3(((int)(x / Settings.CHUNK_SIZE) * Settings.CHUNK_SIZE), ((int)(y / Settings.CHUNK_SIZE) * Settings.CHUNK_SIZE), ((int)(z / Settings.CHUNK_SIZE) * Settings.CHUNK_SIZE));
        }

        public static Vector3 getBlockOffset(Vector3 position)
        {
            double minusX = 0, minusY = 0, minusZ = 0;
            if (position.X <= 0) { minusX -= 1; }
            if (position.Y <= 0) { minusY -= 1; }
            if (position.Z <= 0) { minusZ -= 1; }
            return new Vector3((int)(position.X + minusX), (int)(position.Y + minusY), (int)(position.Z + minusZ));
        }

        private void BuildBlocksChunk()
        {
            #region Build Blocks Inside The Chunk

            BlockType blockType;
            int y_perlinNoise;
            for (int x = (int)this.offset.X; x < this.offset.X + Settings.CHUNK_SIZE; x++)
            {
                for (int z = (int)this.offset.Z; z < this.offset.Z + Settings.CHUNK_SIZE; z++)
                {
                    y_perlinNoise = (int)Globals.perlin_noise.GetPerlin2D(x, z);
                    for (int y = (int)this.offset.Y; y < this.offset.Y + Settings.CHUNK_SIZE; y++)
                    {
                        #region Define Terrain's Blocks

                        if (GameDictionaries.blocksAddedDictionary.ContainsKey(new Vector3(x, y, z)))
                        {
                            // Add To The Dictionary
                            GameDictionaries.blocksDictionary[new Vector3(x, y, z)] = GameDictionaries.blocksAddedDictionary[new Vector3(x, y, z)];
                        }
                        if (!GameDictionaries.blocksDestroyedDictionary.ContainsKey(new Vector3(x, y, z)) &&
                            y < y_perlinNoise)
                        {
                            // Default
                            blockType = BlockType.Error_Block;

                            #region Moon Mode

                            if (Settings.perlinNoise_Type == PerlinNoise_Type.Moon)
                            { blockType = BlockType.Stone; }

                            #endregion Moon Mode

                            #region Earth Mode

                            else
                            {
                                // Water
                                if (y_perlinNoise <= 4)
                                {
                                    if (y <= 1) blockType = BlockType.Sand;
                                    else if (y > 1) blockType = BlockType.Water;
                                }
                                // Sand
                                else if (y_perlinNoise <= 7)
                                {
                                    if (y <= 2) blockType = BlockType.Stone;
                                    else if (y > 2) blockType = BlockType.Sand;
                                }
                                // Grass
                                else if (y_perlinNoise <= 18)
                                {
                                    if (y <= 3) blockType = BlockType.Stone;
                                    else if (y > 3 && y <= 18) blockType = BlockType.Grass;
                                    else if (y > 18) blockType = BlockType.Snow;
                                }
                                // Snow
                                else if (y_perlinNoise > 18)
                                {
                                    if (y <= 5) blockType = BlockType.Stone;
                                    else if (y > 5 && y <= 18) blockType = BlockType.Grass;
                                    else if (y > 18) blockType = BlockType.Snow;
                                }
                            }

                            #endregion Earth Mode

                            // End Stone
                            if (y == 0) { blockType = BlockType.End_Stone; }

                            // Add To The Dictionary
                            GameDictionaries.blocksDictionary[new Vector3(x, y, z)] = blockType;
                        }
                        #endregion Define Terrain's Blocks

                        #region Plant Trees

                        if (y == (int)(y_perlinNoise - Globals.block_Yoffset))
                        {
                            if (GameDictionaries.blocksDictionary.ContainsKey(new Vector3(x, (int)y_perlinNoise - Globals.block_Yoffset, z)))
                            {
                                if (GameDictionaries.blocksDictionary[new Vector3(x, (int)y_perlinNoise - Globals.block_Yoffset, z)] == BlockType.Grass)
                                {
                                    if (x % 28 == 0 && z % 28 == 0 && (x + z != 0))
                                    { Spawner.plantTree(new Vector3(x, y + Globals.block_Yoffset, z)); }
                                }
                            }
                        }

                        #endregion Plant Trees
                    }
                }
            }

            #endregion Build Blocks Inside The Chunk

            #region Build Chunk Bounding Box

            List<Vector3> verticesPositionsList = new List<Vector3>();
            verticesPositionsList.Add(new Vector3(offset.X, offset.Y, offset.Z));
            verticesPositionsList.Add(new Vector3(offset.X + Settings.CHUNK_SIZE, offset.Y, offset.Z));
            verticesPositionsList.Add(new Vector3(offset.X, offset.Y + Settings.CHUNK_SIZE, offset.Z));
            verticesPositionsList.Add(new Vector3(offset.X, offset.Y, offset.Z + Settings.CHUNK_SIZE));
            verticesPositionsList.Add(new Vector3(offset.X + Settings.CHUNK_SIZE, offset.Y + Settings.CHUNK_SIZE, offset.Z));
            verticesPositionsList.Add(new Vector3(offset.X, offset.Y + Settings.CHUNK_SIZE, offset.Z + Settings.CHUNK_SIZE));
            verticesPositionsList.Add(new Vector3(offset.X + Settings.CHUNK_SIZE, offset.Y, offset.Z + Settings.CHUNK_SIZE));
            verticesPositionsList.Add(new Vector3(offset.X + Settings.CHUNK_SIZE, offset.Y + Settings.CHUNK_SIZE, offset.Z + Settings.CHUNK_SIZE));
            this.chunkBoundingBox = BoundingBox.CreateFromPoints(verticesPositionsList);

            #endregion Build Chunk Bounding Box
        }

        #endregion Methods
    }
}