using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Combat_Craft
{
    class Skybox
    {
        #region Data

        private Player player;
        private VertexBuffer vertexBuffer;
        private BasicEffect skyboxBasicEffect;
        private List<VertexPositionTexture> verticesList;
        private Vector2[,] textureCoordinates;
        private Texture2D skyboxTexture;
        private const int skyboxSize = 2000;

        #endregion Data

        #region Constructors

        public Skybox(Player player)
        {
            this.InitializeTexture();
            this.player = player;
            this.verticesList = new List<VertexPositionTexture>();
            this.vertexBuffer = new VertexBuffer(Globals.graphicsDevice, VertexPositionTexture.VertexDeclaration, 36, BufferUsage.None);
            this.skyboxBasicEffect = new BasicEffect(Globals.graphicsDevice);
            this.skyboxBasicEffect.Projection = this.player.camera.Projection;
            this.skyboxBasicEffect.World = Matrix.Identity;
            this.skyboxBasicEffect.Texture = this.skyboxTexture;
            this.skyboxBasicEffect.TextureEnabled = true;
            this.skyboxBasicEffect.FogColor = Color.Blue.ToVector3();
            this.skyboxBasicEffect.FogStart = 0;
            this.skyboxBasicEffect.FogEnd = skyboxSize * 2;
        }

        #endregion Constructors

        #region Methods

        public void Render()
        {
            #region SET-UP Basic Effect

            this.skyboxBasicEffect.View = this.player.camera.View;
            this.skyboxBasicEffect.FogEnabled = this.player.isInWater;

            #endregion SET-UP Basic Effect

            #region Render

            if (this.verticesList != null)
            {
                if (this.verticesList.Count != 0)
                {
                    Globals.graphicsDevice.SetVertexBuffer(this.vertexBuffer);
                    this.vertexBuffer.SetData(this.verticesList.ToArray());
                    this.skyboxBasicEffect.CurrentTechnique.Passes[0].Apply();
                    Globals.graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, this.verticesList.Count / 3);
                }
            }

            #endregion Render
        }

        public void Update()
        {
            #region SET-UP Skybox Info

            // Calculate the origin of the skybox (relative to the player's position without the Y axis)
            Vector3 origin = new Vector3(this.player.camera.Position.X, 0, this.player.camera.Position.Z);

            // Calculate the position of the vertices on the TOP face.
            Vector3 topLeftFront = origin + new Vector3(-1f, 1f, 1f) * skyboxSize;
            Vector3 topLeftBack = origin + new Vector3(-1f, 1f, -1f) * skyboxSize;
            Vector3 topRightFront = origin + new Vector3(1f, 1f, 1f) * skyboxSize;
            Vector3 topRightBack = origin + new Vector3(1f, 1f, -1f) * skyboxSize;
        
            // Calculate the position of the vertices on the BOTTOM face.
            Vector3 bottomLeftFront = origin + new Vector3(-1f, -1f, 1f) * skyboxSize;
            Vector3 bottomLeftBack = origin + new Vector3(-1f, -1f, -1f) * skyboxSize;
            Vector3 bottomRightFront = origin + new Vector3(1f, -1f, 1f) * skyboxSize;
            Vector3 bottomRightBack = origin + new Vector3(1f, -1f, -1f) * skyboxSize;

            #endregion SET-UP Skybox Info

            #region SET-UP Vertices List

            // Clear the vertices list
            this.verticesList.Clear();

            // Add the vertices for the PY face.
            verticesList.Add(new VertexPositionTexture(topLeftFront, this.textureCoordinates[0, 3]));
            verticesList.Add(new VertexPositionTexture(topRightBack, this.textureCoordinates[0, 0]));
            verticesList.Add(new VertexPositionTexture(topLeftBack, this.textureCoordinates[0, 1]));
            verticesList.Add(new VertexPositionTexture(topLeftFront, this.textureCoordinates[0, 3]));
            verticesList.Add(new VertexPositionTexture(topRightFront, this.textureCoordinates[0, 2]));
            verticesList.Add(new VertexPositionTexture(topRightBack, this.textureCoordinates[0, 0]));

            // Add the vertices for the NY face. 
            verticesList.Add(new VertexPositionTexture(bottomLeftFront, this.textureCoordinates[5, 0]));
            verticesList.Add(new VertexPositionTexture(bottomLeftBack, this.textureCoordinates[5, 1]));
            verticesList.Add(new VertexPositionTexture(bottomRightBack, this.textureCoordinates[5, 3]));
            verticesList.Add(new VertexPositionTexture(bottomLeftFront, this.textureCoordinates[5, 0]));
            verticesList.Add(new VertexPositionTexture(bottomRightBack, this.textureCoordinates[5, 3]));
            verticesList.Add(new VertexPositionTexture(bottomRightFront, this.textureCoordinates[5, 2]));

            // Add the vertices for the PX face. 
            verticesList.Add(new VertexPositionTexture(topRightFront, this.textureCoordinates[1, 1]));
            verticesList.Add(new VertexPositionTexture(bottomRightFront , this.textureCoordinates[1, 3]));
            verticesList.Add(new VertexPositionTexture(bottomRightBack, this.textureCoordinates[1, 2]));
            verticesList.Add(new VertexPositionTexture(topRightBack, this.textureCoordinates[1, 0]));
            verticesList.Add(new VertexPositionTexture(topRightFront, this.textureCoordinates[1, 1]));
            verticesList.Add(new VertexPositionTexture(bottomRightBack, this.textureCoordinates[1, 2]));

            // Add the vertices for the NX face.
            verticesList.Add(new VertexPositionTexture(topLeftFront, this.textureCoordinates[3, 0]));
            verticesList.Add(new VertexPositionTexture(bottomLeftBack, this.textureCoordinates[3, 3]));
            verticesList.Add(new VertexPositionTexture(bottomLeftFront, this.textureCoordinates[3, 2]));
            verticesList.Add(new VertexPositionTexture(topLeftBack, this.textureCoordinates[3, 1]));
            verticesList.Add(new VertexPositionTexture(bottomLeftBack, this.textureCoordinates[3, 3]));
            verticesList.Add(new VertexPositionTexture(topLeftFront, this.textureCoordinates[3, 0]));

            // Add the vertices for the PZ face.
            verticesList.Add(new VertexPositionTexture(topLeftBack, this.textureCoordinates[4, 0]));
            verticesList.Add(new VertexPositionTexture(topRightBack, this.textureCoordinates[4, 1]));
            verticesList.Add(new VertexPositionTexture(bottomLeftBack, this.textureCoordinates[4, 2]));
            verticesList.Add(new VertexPositionTexture(bottomLeftBack, this.textureCoordinates[4, 2]));
            verticesList.Add(new VertexPositionTexture(topRightBack, this.textureCoordinates[4, 1]));
            verticesList.Add(new VertexPositionTexture(bottomRightBack, this.textureCoordinates[4, 3]));

            // Add the vertices for the NZ face.
            verticesList.Add(new VertexPositionTexture(topLeftFront, this.textureCoordinates[2, 1]));
            verticesList.Add(new VertexPositionTexture(bottomLeftFront, this.textureCoordinates[2, 3]));
            verticesList.Add(new VertexPositionTexture(topRightFront, this.textureCoordinates[2, 0]));
            verticesList.Add(new VertexPositionTexture(bottomLeftFront, this.textureCoordinates[2, 3]));
            verticesList.Add(new VertexPositionTexture(bottomRightFront, this.textureCoordinates[2, 2]));
            verticesList.Add(new VertexPositionTexture(topRightFront, this.textureCoordinates[2, 0]));

            #endregion SET-UP Vertices List
        }

        private void InitializeTexture()
        {
            #region Initialize the texture

            if (Settings.perlinNoise_Type == PerlinNoise_Type.Moon)
            { this.skyboxTexture = Globals.contentManager.Load<Texture2D>("Asset/MoonBox"); }

            else
            { this.skyboxTexture = Globals.contentManager.Load<Texture2D>("Asset/SkyBox"); }

            #endregion Initialize the texture

            #region Initialize the texture coordinates

            // Initialize the array
            this.textureCoordinates = new Vector2[6, 4];

            // Set-Up info
            float textureWidth = 1f / 6f;
            float pixelWidth = 1f / this.skyboxTexture.Width;
            float pixelHeight = 1f / this.skyboxTexture.Height;

            for (int row = 0; row < 6; row++)
            {
                // Top Left
                this.textureCoordinates[row, 0] = new Vector2(((float)row) * textureWidth + pixelWidth, 0f + pixelHeight);
                // Top Right
                this.textureCoordinates[row, 1] = new Vector2(((float)row + 1) * textureWidth - pixelWidth, 0f + pixelHeight);
                // Bottom Left
                this.textureCoordinates[row, 2] = new Vector2(((float)row) * textureWidth + pixelWidth, 1f - pixelHeight);
                // Bottom Right
                this.textureCoordinates[row, 3] = new Vector2(((float)row + 1) * textureWidth - pixelWidth, 1f - pixelHeight);
            }

            #endregion Initialize the texture coordinates
        }

        #endregion Methods
    }
}