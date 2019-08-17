using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Threading;

namespace Combat_Craft
{
    class World
    {
        #region Data

        // Skybox
        private Skybox skybox;

        // Enemies
        private List<CharacterBlock> characterBlockList;

        // Does the world finish loading
        private bool isFinishLoadWorld;
        private bool hasEverFinishedLoadWorld;

        // Current Rendering Range
        private int current_PX;
        private int current_NX;
        private int current_PZ;
        private int current_NZ;

        // Previous Rendering Range
        private int previous_PX;
        private int previous_NX;
        private int previous_PZ;
        private int previous_NZ;

        #endregion Data

        #region Constructors

        public World(Player player)
        {
            isFinishLoadWorld = false;
            hasEverFinishedLoadWorld = false;

            // Setting up a skybox
            this.skybox = new Skybox(player);

            // Setting enemies list
            this.characterBlockList = new List<CharacterBlock>();

            // Basic effect - player's setup
            Globals.blockBasicEffect.Projection = player.camera.Projection;
            Globals.blockBasicEffect.World = Matrix.Identity;
            Globals.waterBasicEffect.Projection = player.camera.Projection;
            Globals.waterBasicEffect.World = Matrix.Identity;
            Globals.enemieBasicEffect.Projection = player.camera.Projection;
            Globals.enemieBasicEffect.World = Matrix.Identity;
        }

        #endregion Constructors

        #region Methods

        public void Update_Render(Player player, GameTime gameTime)
        {
            #region Graphics Card Set-Up

            if (Settings.perlinNoise_Type != PerlinNoise_Type.Moon)
            { Globals.graphicsDevice.Clear(Color.CornflowerBlue); }

            else
            { Globals.graphicsDevice.Clear(Color.Black); }

            Globals.graphicsDevice.DepthStencilState = Globals.depthStencilState;

            #endregion Graphics Card Set-Up

            #region Render World

            Globals.chunksRendering = 0;
            List<Chunk> chunksDictionary_values = GameDictionaries.chunksRenderingDictionary.Values.ToList();
            if (!Globals.Splash_Screen && !Globals.Splash_GameOver)
            {
                #region SET-UP Basic Effect

                Globals.blockBasicEffect.View = player.camera.View;
                Globals.waterBasicEffect.View = player.camera.View;

                #endregion SET-UP Basic Effect

                #region Render

                #region Render Enemies

                // Render any enemie on the list
                for (int i = 0; i < characterBlockList.Count; i++)
                {
                    characterBlockList[i].Render(player);
                }

                #endregion Render Enemies

                #region Regular Blocks

                Globals.blockBasicEffect.CurrentTechnique.Passes[0].Apply();
                for (int i = 0; i < chunksDictionary_values.Count; i++)
                {
                    if (player.camera.IsBlockInView(chunksDictionary_values[i].chunkBoundingBox))
                    { chunksDictionary_values[i].RenderChunk_Blocks(); }
                }

                #endregion Regular Blocks

                #region Water Blocks

                if (!player.isHeadInWater)
                {
                    Globals.waterBasicEffect.CurrentTechnique.Passes[0].Apply();
                    for (int i = 0; i < chunksDictionary_values.Count; i++)
                    {
                        if (player.camera.IsBlockInView(chunksDictionary_values[i].chunkBoundingBox))
                        { chunksDictionary_values[i].RenderChunk_AboveWater(); }
                    }

                    this.skybox.Render();
                }
                else
                {
                    this.skybox.Render();

                    Globals.waterBasicEffect.CurrentTechnique.Passes[0].Apply();
                    for (int i = 0; i < chunksDictionary_values.Count; i++)
                    {
                        if (player.camera.IsBlockInView(chunksDictionary_values[i].chunkBoundingBox))
                        { chunksDictionary_values[i].RenderChunk_BelowWater(); }
                    }
                }

                #endregion Water Blocks
            }

            #endregion Render

            #endregion Render World

            #region Define World Current Range

            current_PX = (int)player.camera.Position.X + Settings.worldRenderingDistance * Settings.CHUNK_SIZE;
            current_NX = (int)player.camera.Position.X - Settings.worldRenderingDistance * Settings.CHUNK_SIZE;
            current_PZ = (int)player.camera.Position.Z + Settings.worldRenderingDistance * Settings.CHUNK_SIZE;
            current_NZ = (int)player.camera.Position.Z - Settings.worldRenderingDistance * Settings.CHUNK_SIZE;

            #endregion Define World Current Range

            #region Update World

            if (!Globals.Splash_GameOver)
            {
                // Update the enemies in the world
                UpdateEnemies(gameTime, player);

                if (!isFinishLoadWorld || (((int)current_PX / Settings.CHUNK_SIZE != (int)previous_PX / Settings.CHUNK_SIZE) || ((int)current_NX / Settings.CHUNK_SIZE != (int)previous_NX / Settings.CHUNK_SIZE) || ((int)current_PZ / Settings.CHUNK_SIZE != (int)previous_PZ / Settings.CHUNK_SIZE) || ((int)current_NZ / Settings.CHUNK_SIZE != (int)previous_NZ / Settings.CHUNK_SIZE)))
                {
                    // Slow down the updating
                    if (!hasEverFinishedLoadWorld || (int)gameTime.TotalGameTime.TotalMilliseconds % Settings.worldSmoothness == 0)
                    {
                        UpdateWorldChunks(player);
                        this.skybox.Update();
                    }
                }
            }

            #endregion Update World

            #region Define World Previous Range
            previous_PX = current_PX;
            previous_NX = current_NX;
            previous_PZ = current_PZ;
            previous_NZ = current_NZ;
            #endregion Define World Previous Range
        }

        private void UpdateWorldChunks(Player player)
        {
            #region Remove Unrelevent Chunks

            List<Chunk> chunksDictionary_values = GameDictionaries.chunksRenderingDictionary.Values.ToList();
            for (int i = 0; i < chunksDictionary_values.Count; i++)
            {
                if (Math.Abs(chunksDictionary_values[i].offset.X - player.camera.Position.X) >= 2 * Settings.CHUNK_SIZE + Settings.CHUNK_SIZE * (Settings.worldRenderingDistance) ||
                    Math.Abs(chunksDictionary_values[i].offset.Z - player.camera.Position.Z) >= 2 * Settings.CHUNK_SIZE + Settings.CHUNK_SIZE * (Settings.worldRenderingDistance))
                {
                    GameDictionaries.chunksRenderingDictionary[chunksDictionary_values[i].offset].UnLoadChunk();
                    GameDictionaries.chunksRenderingDictionary.Remove(chunksDictionary_values[i].offset);
                }
            }

            #endregion Remove Unrelevent Chunks

            #region Build New Relevent Chunks

            Vector3 chunk_offset;
            int maxHeight;
            bool gotIn = false;
            for (int x = current_NX; x < current_PX; x += Settings.CHUNK_SIZE)
            {
                for (int z = current_NZ; z < current_PZ; z += Settings.CHUNK_SIZE)
                {
                    maxHeight = maxHeightInChunk(x, z);
                    for (int y = 0; y <= maxHeight; y += Settings.CHUNK_SIZE)
                    {
                        #region Build Blocks Inside And Around The Chunk

                        // Build Blocks Inside The Current Chunk
                        chunk_offset = Chunk.getChunkOffSet(x, y, z);
                        if (!GameDictionaries.chunksRenderingDictionary.ContainsKey(chunk_offset))
                        { GameDictionaries.chunksRenderingDictionary[chunk_offset] = new Chunk(chunk_offset); }

                        // Chunk Above
                        chunk_offset = Chunk.getChunkOffSet(x, y + Settings.CHUNK_SIZE, z);
                        if (!GameDictionaries.chunksRenderingDictionary.ContainsKey(chunk_offset))
                        { GameDictionaries.chunksRenderingDictionary[chunk_offset] = new Chunk(chunk_offset); }

                        // Chunk Below
                        chunk_offset = Chunk.getChunkOffSet(x, y - Settings.CHUNK_SIZE, z);
                        if (y - Settings.CHUNK_SIZE > 0 && !GameDictionaries.chunksRenderingDictionary.ContainsKey(chunk_offset))
                        { GameDictionaries.chunksRenderingDictionary[chunk_offset] = new Chunk(chunk_offset); }

                        // Chunk Front
                        chunk_offset = Chunk.getChunkOffSet(x, y, z - Settings.CHUNK_SIZE);
                        if (!GameDictionaries.chunksRenderingDictionary.ContainsKey(chunk_offset))
                        { GameDictionaries.chunksRenderingDictionary[chunk_offset] = new Chunk(chunk_offset); }

                        // Chunk Back
                        chunk_offset = Chunk.getChunkOffSet(x, y, z + Settings.CHUNK_SIZE);
                        if (!GameDictionaries.chunksRenderingDictionary.ContainsKey(chunk_offset))
                        { GameDictionaries.chunksRenderingDictionary[chunk_offset] = new Chunk(chunk_offset); }

                        // Chunk Right
                        chunk_offset = Chunk.getChunkOffSet(x + Settings.CHUNK_SIZE, y, z);
                        if (!GameDictionaries.chunksRenderingDictionary.ContainsKey(chunk_offset))
                        { GameDictionaries.chunksRenderingDictionary[chunk_offset] = new Chunk(chunk_offset); }

                        // Chunk Left
                        chunk_offset = Chunk.getChunkOffSet(x - Settings.CHUNK_SIZE, y, z);
                        if (!GameDictionaries.chunksRenderingDictionary.ContainsKey(chunk_offset))
                        { GameDictionaries.chunksRenderingDictionary[chunk_offset] = new Chunk(chunk_offset); }

                        #endregion Build Blocks Inside And Around The Chunk

                        #region Build Chunk Mesh

                        chunk_offset = Chunk.getChunkOffSet(x, y, z);
                        if (GameDictionaries.chunksRenderingDictionary.ContainsKey(chunk_offset) &&
                           !GameDictionaries.chunksRenderingDictionary[chunk_offset].hasChunkMeshBuilt)
                        {
                            GameDictionaries.chunksRenderingDictionary[chunk_offset].BuildChunkMesh();
                            // skip the loops
                            x = (int)current_PX; z = (int)current_PZ; y = maxHeight; gotIn = true;
                        }

                        #endregion Build Chunk Mesh
                    }
                }
            }
            this.isFinishLoadWorld = !gotIn;
            if (gotIn == false)
            { hasEverFinishedLoadWorld = true; Globals.Splash_Screen = false; }

            #endregion Build New Relevent Chunks
        }

        private void UpdateEnemies(GameTime gameTime, Player player)
        {
            #region Adding Enemies

            if (Globals.enemiesAmount < Settings.maxEnemies)
            {
                // if the game loaded
                if (!Globals.Splash_Screen)
                {
                    // Spawn every couple of seconds that showen in settings
                    if (gameTime.TotalGameTime.TotalSeconds > Globals.nextEnemieSpawnTime)
                    {
                        this.characterBlockList.Add(new CharacterBlock(new Vector3(Globals.random.Next(this.current_NX, this.current_PX), 0, Globals.random.Next(this.current_NZ, this.current_PZ)), new Color(Globals.random.Next(0, 255), Globals.random.Next(0, 255), Globals.random.Next(0, 255)), (float)(Settings.enemyMinSpeedGenerate + Globals.random.NextDouble() * Settings.enemyMaxSpeedGenerate), Globals.random.Next(0, 10), Globals.random.Next(10, 20), player));
                        Globals.nextEnemieSpawnTime = (float)(gameTime.TotalGameTime.TotalSeconds + Settings.enemySpawnRateSeconds);
                        Globals.enemiesAmount++;
                    }
                }
            }

            #endregion Adding Enemies


            #region Deleting Enemies

            // if the game loaded
            if (!Globals.Splash_Screen)
            {
                // Check on every enemie that its not out of the bounderies of the world and
                // not touching the player
                for(int i=0; i< characterBlockList.Count; i++)
                {
                    // Delete the enemy from the list
                    if (characterBlockList[i].characterPosition.Y <= 0)
                    {
                        characterBlockList.RemoveAt(i);
                        Globals.enemiesAmount--;
                    }
                    else
                    {
                        // if the character is touching the player
                        if (Math.Abs(characterBlockList[i].characterPosition.X - player.camera.Position.X) <= Settings.enemyPlayerSafeDistance &&
                            ((player.camera.Position.Y - characterBlockList[i].characterPosition.Y <= Settings.playerHeight + Settings.enemyPlayerSafeDistance &&
                            player.camera.Position.Y > characterBlockList[i].characterPosition.Y) ||
                            (characterBlockList[i].characterPosition.Y - player.camera.Position.Y <= Settings.enemyPlayerSafeDistance &&
                            player.camera.Position.Y < characterBlockList[i].characterPosition.Y)) &&
                            Math.Abs(characterBlockList[i].characterPosition.Z - player.camera.Position.Z) <= Settings.enemyPlayerSafeDistance &&
                            !player.flyingMode)
                        {
                            // Remove the enemy
                            characterBlockList.RemoveAt(i);
                            Globals.enemiesAmount--;

                            // Damage the player health
                            player.health -= Settings.enemyDamage;
                        }
                    }
                }
            }

            #endregion Deleting Enemies

            #region Updating Meshes And Positions

            for (int i = 0; i < this.characterBlockList.Count; i++)
            { this.characterBlockList[i].Update(player, gameTime); }

            #endregion Updating Meshes And Positions
        }

        // Checks the max height of the chunk relevent only to its X and Z axies
        public static int maxHeightInChunk(int xIN, int zIN)
        {
            #region Calculate Max Height

            Vector3 chunk_offset = Chunk.getChunkOffSet(xIN, 0, zIN);
            int maxHeight = 0, y_perlinNoise;
            for (int x = (int)chunk_offset.X; x < chunk_offset.X + Settings.CHUNK_SIZE; x++)
            {
                for (int z = (int)chunk_offset.Z; z < chunk_offset.Z + Settings.CHUNK_SIZE; z++)
                {
                    // Perlin Noise - Ground Height
                    y_perlinNoise = (int)Globals.perlin_noise.GetPerlin2D(x, z);
                    if (maxHeight < y_perlinNoise)
                    { maxHeight = y_perlinNoise; }
                }
            }

            if (GameDictionaries.chunksHeightDictionary.ContainsKey(chunk_offset))
            {
                int maxHeightByDictionary = GameDictionaries.chunksHeightDictionary[chunk_offset];
                if (maxHeight < maxHeightByDictionary)
                { maxHeight = maxHeightByDictionary; }
            }

            return maxHeight;

            #endregion Calculate Max Height
        }

        #endregion Methods
    }
}