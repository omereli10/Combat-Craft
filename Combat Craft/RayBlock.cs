using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Combat_Craft
{
    class RayBlock
    {
        #region Data

        private Player player;
        private Vector3 origin;
        private Vector3 previousOrigin;
        private BasicEffect blocksPointedBasicEffect;
        private List<VertexPositionColor> blockPointedVertices;

        // Size of pointed block
        const float sizeMax = 2f;
        const float sizeMin = 0f;

        // Vertices Positions
        Vector3 topLeftFront, topLeftBack, topRightFront, topRightBack, bottomLeftFront, bottomLeftBack, bottomRightFront, bottomRightBack;

        #endregion Data

        #region Constructors

        public RayBlock(Player player)
        {
            this.player = player;
            this.previousOrigin = Vector3.Zero;

            // Disco Mode
            if (Settings.blockPointed_discoMode)
            { Settings.blockPointed_markStrength = 0.5f; }
            this.blocksPointedBasicEffect = new BasicEffect(Globals.graphicsDevice);
            this.blocksPointedBasicEffect.Projection = this.player.camera.Projection;
            this.blocksPointedBasicEffect.World = Matrix.Identity;
            this.blocksPointedBasicEffect.Alpha = Settings.blockPointed_markStrength;
            this.blocksPointedBasicEffect.VertexColorEnabled = true;
            this.blockPointedVertices = new List<VertexPositionColor>();
        }

        #endregion Constructors

        #region Methods

        public void Update_Render()
        {
            #region Update Add And Destroy

            if (!Globals.Splash_Screen && !Globals.Splash_GameOver)
            {
                this.origin = Raycast.Raycast_destroyBlockOrigin(this.player);
                if (this.origin != this.previousOrigin)
                { this.Update_BlockPointedVertices(); }
                if (GameDictionaries.blocksDictionary.ContainsKey(this.origin))
                {
                    this.Render();
                    if (Globals.mouseLock)
                    {
                        if (Mouse.GetState().LeftButton == ButtonState.Pressed && !player.baseMouseKeyboard.isHolding_MouseLeftButton && !this.player.isInWater)
                        { DestroyBlock(); player.baseMouseKeyboard.isHolding_MouseLeftButton = true; }
                        if (Mouse.GetState().RightButton == ButtonState.Pressed && !player.baseMouseKeyboard.isHolding_MouseRightButton && !this.player.isInWater)
                        { AddBlock(); player.baseMouseKeyboard.isHolding_MouseRightButton = true; }
                    }
                }

                this.previousOrigin = this.origin;
            }
            #endregion Update Add And Destroy
        }

        private void Update_BlockPointedVertices()
        {
            #region SET-UP Block Pointed Info

            // Clear Vertices
            this.blockPointedVertices.Clear();

            // Disco Mode
            if (Settings.blockPointed_discoMode)
            { Settings.blockPointed_markColor = new Color(Globals.random.Next(0, 255), Globals.random.Next(0, 255), Globals.random.Next(0, 255)); }

            // Update The Vertices List
            this.UpdateVerticesPositions();

            #endregion SET-UP Block Pointed Info

            #region Update Vertices List

            #region Add the vertices for the PY face

            if (!GameDictionaries.blocksDictionary.ContainsKey(new Vector3(origin.X, origin.Y + Globals.block_Yoffset, origin.Z)))
            { addVertices_PY(); }
            else
            {
                if (GameDictionaries.blocksDictionary[new Vector3(origin.X, origin.Y + Globals.block_Yoffset, origin.Z)] == BlockType.Water)
                { addVertices_PY(); }
            }

            #endregion Add the vertices for the PY face

            #region Add the vertices for the NY face

            if (!GameDictionaries.blocksDictionary.ContainsKey(new Vector3(origin.X, origin.Y - Globals.block_Yoffset, origin.Z)))
            { addVertices_NY(); }
            else
            {
                if (GameDictionaries.blocksDictionary[new Vector3(origin.X, origin.Y - Globals.block_Yoffset, origin.Z)] == BlockType.Water)
                { addVertices_NY(); }
            }

            #endregion Add the vertices for the NY face

            #region Add the vertices for the PX face

            if (!GameDictionaries.blocksDictionary.ContainsKey(new Vector3(origin.X + Globals.block_Xoffset, origin.Y, origin.Z)))
            { addVertices_PX(); }
            else
            {
                if (GameDictionaries.blocksDictionary[new Vector3(origin.X + Globals.block_Xoffset, origin.Y, origin.Z)] == BlockType.Water)
                { addVertices_PX(); }
            }

            #endregion Add the vertices for the PX face

            #region Add the vertices for the NX face

            if (!GameDictionaries.blocksDictionary.ContainsKey(new Vector3(origin.X - Globals.block_Xoffset, origin.Y, origin.Z)))
            { addVertices_NX(); }
            else
            {
                if (GameDictionaries.blocksDictionary[new Vector3(origin.X - Globals.block_Xoffset, origin.Y, origin.Z)] == BlockType.Water)
                { addVertices_NX(); }
            }

            #endregion Add the vertices for the NX face

            #region Add the vertices for the PZ face

            if (!GameDictionaries.blocksDictionary.ContainsKey(new Vector3(origin.X, origin.Y, origin.Z + Globals.block_Zoffset)))
            { addVertices_PZ(); }
            else
            {
                if (GameDictionaries.blocksDictionary[new Vector3(origin.X, origin.Y, origin.Z + Globals.block_Zoffset)] == BlockType.Water)
                { addVertices_PZ(); }
            }

            #endregion Add the vertices for the PZ face

            #region Add the vertices for the NZ face

            if (!GameDictionaries.blocksDictionary.ContainsKey(new Vector3(origin.X, origin.Y, origin.Z - Globals.block_Zoffset)))
            { addVertices_NZ(); }
            else
            {
                if (GameDictionaries.blocksDictionary[new Vector3(origin.X, origin.Y, origin.Z - Globals.block_Zoffset)] == BlockType.Water)
                { addVertices_NZ(); }
            }

            #endregion Add the vertices for the NZ face

            #endregion Update Vertices List
        }

        private void Render()
        {
            if (this.blockPointedVertices != null)
            {
                if (this.blockPointedVertices.Count != 0)
                {
                    #region SET-UP Basic Effect

                    this.blocksPointedBasicEffect.View = this.player.camera.View;
                    this.blocksPointedBasicEffect.CurrentTechnique.Passes[0].Apply();

                    #endregion SET-UP Basic Effect

                    #region Render

                    Globals.block_ColorPosition_VertexBuffer.SetData(this.blockPointedVertices.ToArray());
                    Globals.graphicsDevice.SetVertexBuffer(Globals.block_ColorPosition_VertexBuffer);
                    Globals.graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, this.blockPointedVertices.Count / 3);
                    
                    #endregion Render
                }
            }
        }

        private void AddBlock()
        {
            Vector3 addBlockOrigin = Raycast.Raycast_addBlockOrigin(player);

            #region Add The Block

            // Check for valid collision with the player and valid placement
            if (addBlockOrigin != new Vector3(0, -1, 0) &&
                !GameDictionaries.blocksAddedDictionary.ContainsKey(addBlockOrigin) &&
                !(GameDictionaries.blocksDictionary.TryGetValue(new Vector3(addBlockOrigin.X, addBlockOrigin.Y - Globals.block_Yoffset, addBlockOrigin.Z), out BlockType value1) && value1 == BlockType.Water) &&
                addBlockOrigin != Chunk.getBlockOffset(new Vector3((int)(player.camera.Position.X + Globals.saftyDistanceFromBlock), (int)(player.camera.Position.Y), (int)(player.camera.Position.Z + Globals.saftyDistanceFromBlock))) &&
                addBlockOrigin != Chunk.getBlockOffset(new Vector3((int)(player.camera.Position.X + Globals.saftyDistanceFromBlock), (int)(player.camera.Position.Y), (int)(player.camera.Position.Z - Globals.saftyDistanceFromBlock))) &&
                addBlockOrigin != Chunk.getBlockOffset(new Vector3((int)(player.camera.Position.X - Globals.saftyDistanceFromBlock), (int)(player.camera.Position.Y), (int)(player.camera.Position.Z + Globals.saftyDistanceFromBlock))) &&
                addBlockOrigin != Chunk.getBlockOffset(new Vector3((int)(player.camera.Position.X - Globals.saftyDistanceFromBlock), (int)(player.camera.Position.Y), (int)(player.camera.Position.Z - Globals.saftyDistanceFromBlock))) &&
                addBlockOrigin != Chunk.getBlockOffset(new Vector3((int)(player.camera.Position.X + Globals.saftyDistanceFromBlock), (int)(player.camera.Position.Y - Settings.playerHeight), (int)(player.camera.Position.Z + Globals.saftyDistanceFromBlock))) &&
                addBlockOrigin != Chunk.getBlockOffset(new Vector3((int)(player.camera.Position.X + Globals.saftyDistanceFromBlock), (int)(player.camera.Position.Y - Settings.playerHeight), (int)(player.camera.Position.Z - Globals.saftyDistanceFromBlock))) &&
                addBlockOrigin != Chunk.getBlockOffset(new Vector3((int)(player.camera.Position.X - Globals.saftyDistanceFromBlock), (int)(player.camera.Position.Y - Settings.playerHeight), (int)(player.camera.Position.Z + Globals.saftyDistanceFromBlock))) &&
                addBlockOrigin != Chunk.getBlockOffset(new Vector3((int)(player.camera.Position.X - Globals.saftyDistanceFromBlock), (int)(player.camera.Position.Y - Settings.playerHeight), (int)(player.camera.Position.Z - Globals.saftyDistanceFromBlock))))
            {
                // Add block to the dictionaries
                GameDictionaries.blocksDictionary[addBlockOrigin] = Globals.addableBlockTypes[player.baseMouseKeyboard.mouseWheel_Value];
                GameDictionaries.blocksAddedDictionary[addBlockOrigin] = Globals.addableBlockTypes[player.baseMouseKeyboard.mouseWheel_Value];

                // Update the max chunk height if the block is above the current max height
                Vector3 chunk_offset = RayBlock.getChunkOrigin_OffSet(new Vector3(addBlockOrigin.X, 0, addBlockOrigin.Z), 0, 0, 0);
                if ((int)addBlockOrigin.Y > World.maxHeightInChunk((int)chunk_offset.X, (int)chunk_offset.Z))
                { GameDictionaries.chunksHeightDictionary[chunk_offset] = (int)addBlockOrigin.Y; }

                // Build chunks mesh
                BuildChunksAroundMesh(addBlockOrigin);
            }

            #endregion Add The Block
        }

        private void DestroyBlock()
        {
            #region Remove The Block

            if ((GameDictionaries.blocksDictionary[this.origin] != BlockType.End_Stone) &&
                !(GameDictionaries.blocksDictionary.TryGetValue(new Vector3(this.origin.X, this.origin.Y + Globals.block_Yoffset, this.origin.Z), out BlockType value1) && value1 == BlockType.Water))
            {
                if (GameDictionaries.blocksDictionary.ContainsKey(this.origin))
                { GameDictionaries.blocksDictionary.Remove(this.origin); }
                if (GameDictionaries.blocksAddedDictionary.ContainsKey(this.origin))
                { GameDictionaries.blocksAddedDictionary.Remove(this.origin); }
                GameDictionaries.blocksDestroyedDictionary[this.origin] = true;

                BuildChunksAroundMesh(this.origin);
            }

            #endregion Remove The Block
        }

        private void BuildChunksAroundMesh(Vector3 blockOrigin)
        {
            #region Current Chunk

            Vector3 chunk_offset = RayBlock.getChunkOrigin_OffSet(blockOrigin, 0, 0, 0);
            if (!GameDictionaries.chunksRenderingDictionary.ContainsKey(chunk_offset))
            {
                GameDictionaries.chunksRenderingDictionary[chunk_offset] = new Chunk(chunk_offset);
                GameDictionaries.chunksHeightDictionary[new Vector3(chunk_offset.X, 0, chunk_offset.Z)] = (int)chunk_offset.Y + Settings.CHUNK_SIZE;
            }
            GameDictionaries.chunksRenderingDictionary[chunk_offset].BuildChunkMesh();

            #endregion Current Chunk

            #region Chunks Around

            #region PX Chunk

            if (GameDictionaries.blocksDictionary.ContainsKey(new Vector3(blockOrigin.X + Globals.block_Xoffset, blockOrigin.Y, blockOrigin.Z)))
            {
                chunk_offset = RayBlock.getChunkOrigin_OffSet(blockOrigin, 1, 0, 0);
                if (GameDictionaries.chunksRenderingDictionary.ContainsKey(chunk_offset))
                { GameDictionaries.chunksRenderingDictionary[chunk_offset].BuildChunkMesh(); }
            }

            #endregion PX Chunk

            #region NX Chunk

            if (GameDictionaries.blocksDictionary.ContainsKey(new Vector3(blockOrigin.X - Globals.block_Xoffset, blockOrigin.Y, blockOrigin.Z)))
            {
                chunk_offset = RayBlock.getChunkOrigin_OffSet(blockOrigin, -1, 0, 0);
                if (GameDictionaries.chunksRenderingDictionary.ContainsKey(chunk_offset))
                { GameDictionaries.chunksRenderingDictionary[chunk_offset].BuildChunkMesh(); }
            }

            #endregion NX Chunk

            #region PY Chunk

            if (GameDictionaries.blocksDictionary.ContainsKey(new Vector3(blockOrigin.X, blockOrigin.Y + Globals.block_Yoffset, blockOrigin.Z)))
            {
                chunk_offset = RayBlock.getChunkOrigin_OffSet(blockOrigin, 0, 1, 0);
                if (GameDictionaries.chunksRenderingDictionary.ContainsKey(chunk_offset))
                { GameDictionaries.chunksRenderingDictionary[chunk_offset].BuildChunkMesh(); }
            }

            #endregion PY Chunk

            #region NY Chunk

            if (GameDictionaries.blocksDictionary.ContainsKey(new Vector3(blockOrigin.X, blockOrigin.Y - Globals.block_Yoffset, blockOrigin.Z)))
            {
                chunk_offset = RayBlock.getChunkOrigin_OffSet(blockOrigin, 0, -1, 0);
                if (GameDictionaries.chunksRenderingDictionary.ContainsKey(chunk_offset))
                { GameDictionaries.chunksRenderingDictionary[chunk_offset].BuildChunkMesh(); }
            }

            #endregion NY Chunk

            #region PZ Chunk

            if (GameDictionaries.blocksDictionary.ContainsKey(new Vector3(blockOrigin.X, blockOrigin.Y, blockOrigin.Z + Globals.block_Zoffset)))
            {
                chunk_offset = RayBlock.getChunkOrigin_OffSet(blockOrigin, 0, 0, 1);
                if (GameDictionaries.chunksRenderingDictionary.ContainsKey(chunk_offset))
                { GameDictionaries.chunksRenderingDictionary[chunk_offset].BuildChunkMesh(); }
            }

            #endregion PZ Chunk

            #region NZ Chunk

            if (GameDictionaries.blocksDictionary.ContainsKey(new Vector3(blockOrigin.X, blockOrigin.Y, blockOrigin.Z - Globals.block_Zoffset)))
            {
                chunk_offset = RayBlock.getChunkOrigin_OffSet(blockOrigin, 0, 0, -1);
                if (GameDictionaries.chunksRenderingDictionary.ContainsKey(chunk_offset))
                { GameDictionaries.chunksRenderingDictionary[chunk_offset].BuildChunkMesh(); }
            }

            #endregion NZ Chunk

            #endregion Chunks Around
        }

        public static Vector3 getChunkOrigin_OffSet(Vector3 origin, float addChunksAmont_X, float addChunksAmont_Y, float addChunksAmont_Z)
        {
            float minusX = 0, minusY = 0, minusZ = 0;
            if (origin.X < 0 && origin.X % Settings.CHUNK_SIZE != 0) { minusX -= Settings.CHUNK_SIZE; }
            if (origin.Y < 0 && origin.Y % Settings.CHUNK_SIZE != 0) { minusY -= Settings.CHUNK_SIZE; }
            if (origin.Z < 0 && origin.Z % Settings.CHUNK_SIZE != 0) { minusZ -= Settings.CHUNK_SIZE; }

            return new Vector3(((int)((origin.X + minusX + addChunksAmont_X * Settings.CHUNK_SIZE) / Settings.CHUNK_SIZE) * Settings.CHUNK_SIZE), ((int)((origin.Y + minusY + addChunksAmont_Y * Settings.CHUNK_SIZE) / Settings.CHUNK_SIZE) * Settings.CHUNK_SIZE), ((int)((origin.Z + minusZ + addChunksAmont_Z * Settings.CHUNK_SIZE) / Settings.CHUNK_SIZE) * Settings.CHUNK_SIZE)); ;
        }

        private void UpdateVerticesPositions()
        {
            // Calculate the position of the vertices on the TOP face.
            this.topLeftFront = origin + new Vector3(sizeMin, sizeMax, sizeMin) * Settings.BLOCK_SIZE;
            this.topLeftBack = origin + new Vector3(sizeMin, sizeMax, sizeMax) * Settings.BLOCK_SIZE;
            this.topRightFront = origin + new Vector3(sizeMax, sizeMax, sizeMin) * Settings.BLOCK_SIZE;
            this.topRightBack = origin + new Vector3(sizeMax, sizeMax, sizeMax) * Settings.BLOCK_SIZE;

            // Calculate the position of the vertices on the BOTTOM face.
            this.bottomLeftFront = origin + new Vector3(sizeMin, sizeMin, sizeMin) * Settings.BLOCK_SIZE;
            this.bottomLeftBack = origin + new Vector3(sizeMin, sizeMin, sizeMax) * Settings.BLOCK_SIZE;
            this.bottomRightFront = origin + new Vector3(sizeMax, sizeMin, sizeMin) * Settings.BLOCK_SIZE;
            this.bottomRightBack = origin + new Vector3(sizeMax, sizeMin, sizeMax) * Settings.BLOCK_SIZE;
        }

        private void addVertices_PY()
        {
            this.blockPointedVertices.Add(new VertexPositionColor(topLeftFront, Settings.blockPointed_markColor));
            this.blockPointedVertices.Add(new VertexPositionColor(topRightBack, Settings.blockPointed_markColor));
            this.blockPointedVertices.Add(new VertexPositionColor(topLeftBack, Settings.blockPointed_markColor));
            this.blockPointedVertices.Add(new VertexPositionColor(topLeftFront, Settings.blockPointed_markColor));
            this.blockPointedVertices.Add(new VertexPositionColor(topRightFront, Settings.blockPointed_markColor));
            this.blockPointedVertices.Add(new VertexPositionColor(topRightBack, Settings.blockPointed_markColor));
        }
        
        private void addVertices_NY()
        {
            this.blockPointedVertices.Add(new VertexPositionColor(bottomLeftFront, Settings.blockPointed_markColor));
            this.blockPointedVertices.Add(new VertexPositionColor(bottomLeftBack, Settings.blockPointed_markColor));
            this.blockPointedVertices.Add(new VertexPositionColor(bottomRightBack, Settings.blockPointed_markColor));
            this.blockPointedVertices.Add(new VertexPositionColor(bottomLeftFront, Settings.blockPointed_markColor));
            this.blockPointedVertices.Add(new VertexPositionColor(bottomRightBack, Settings.blockPointed_markColor));
            this.blockPointedVertices.Add(new VertexPositionColor(bottomRightFront, Settings.blockPointed_markColor));
        }

        private void addVertices_PX()
        {
            this.blockPointedVertices.Add(new VertexPositionColor(topRightFront, Settings.blockPointed_markColor));
            this.blockPointedVertices.Add(new VertexPositionColor(bottomRightFront, Settings.blockPointed_markColor));
            this.blockPointedVertices.Add(new VertexPositionColor(bottomRightBack, Settings.blockPointed_markColor));
            this.blockPointedVertices.Add(new VertexPositionColor(topRightBack, Settings.blockPointed_markColor));
            this.blockPointedVertices.Add(new VertexPositionColor(topRightFront, Settings.blockPointed_markColor));
            this.blockPointedVertices.Add(new VertexPositionColor(bottomRightBack, Settings.blockPointed_markColor));
        }

        private void addVertices_NX()
        {
            this.blockPointedVertices.Add(new VertexPositionColor(topLeftFront, Settings.blockPointed_markColor));
            this.blockPointedVertices.Add(new VertexPositionColor(bottomLeftBack, Settings.blockPointed_markColor));
            this.blockPointedVertices.Add(new VertexPositionColor(bottomLeftFront, Settings.blockPointed_markColor));
            this.blockPointedVertices.Add(new VertexPositionColor(topLeftBack, Settings.blockPointed_markColor));
            this.blockPointedVertices.Add(new VertexPositionColor(bottomLeftBack, Settings.blockPointed_markColor));
            this.blockPointedVertices.Add(new VertexPositionColor(topLeftFront, Settings.blockPointed_markColor));
        }

        private void addVertices_PZ()
        {
            this.blockPointedVertices.Add(new VertexPositionColor(topLeftBack, Settings.blockPointed_markColor));
            this.blockPointedVertices.Add(new VertexPositionColor(topRightBack, Settings.blockPointed_markColor));
            this.blockPointedVertices.Add(new VertexPositionColor(bottomLeftBack, Settings.blockPointed_markColor));
            this.blockPointedVertices.Add(new VertexPositionColor(bottomLeftBack, Settings.blockPointed_markColor));
            this.blockPointedVertices.Add(new VertexPositionColor(topRightBack, Settings.blockPointed_markColor));
            this.blockPointedVertices.Add(new VertexPositionColor(bottomRightBack, Settings.blockPointed_markColor));
        }

        private void addVertices_NZ()
        {
            this.blockPointedVertices.Add(new VertexPositionColor(topLeftFront, Settings.blockPointed_markColor));
            this.blockPointedVertices.Add(new VertexPositionColor(bottomLeftFront, Settings.blockPointed_markColor));
            this.blockPointedVertices.Add(new VertexPositionColor(topRightFront, Settings.blockPointed_markColor));
            this.blockPointedVertices.Add(new VertexPositionColor(bottomLeftFront, Settings.blockPointed_markColor));
            this.blockPointedVertices.Add(new VertexPositionColor(bottomRightFront, Settings.blockPointed_markColor));
            this.blockPointedVertices.Add(new VertexPositionColor(topRightFront, Settings.blockPointed_markColor));
        }

        #endregion Methods
    }
}