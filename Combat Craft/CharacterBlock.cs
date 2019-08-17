using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Combat_Craft
{
    class CharacterBlock : CharacterBase
    {
        #region Data

        private List<VertexPositionColor> characterVerticesList;

        // Size of pointed block
        const float sizeMax = 2f;
        const float sizeMin = 0f;

        #endregion Data

        #region Constructors

        public CharacterBlock(Vector3 position, Color color, float speed, int damage, int fallHeight, Player player) : base(position, color, speed, damage, fallHeight, player)
        {
            // Set the vertices list
            characterVerticesList = new List<VertexPositionColor>();

            // Build the mesh of the character
            BuildCharacterMesh();
        }

        #endregion Constructors

        #region Methods

        public void BuildCharacterMesh()
        {
            // Clear Vertices
            this.characterVerticesList.Clear();

            #region Calculate the position of the vertices

            // Calculate the position of the vertices on the TOP face.
            Vector3 topLeftFront = this.characterPosition + new Vector3(sizeMin, sizeMax, sizeMin) * Settings.BLOCK_SIZE;
            Vector3 topLeftBack = this.characterPosition + new Vector3(sizeMin, sizeMax, sizeMax) * Settings.BLOCK_SIZE;
            Vector3 topRightFront = this.characterPosition + new Vector3(sizeMax, sizeMax, sizeMin) * Settings.BLOCK_SIZE;
            Vector3 topRightBack = this.characterPosition + new Vector3(sizeMax, sizeMax, sizeMax) * Settings.BLOCK_SIZE;

            // Calculate the position of the vertices on the BOTTOM face.
            Vector3 bottomLeftFront = this.characterPosition + new Vector3(sizeMin, sizeMin, sizeMin) * Settings.BLOCK_SIZE;
            Vector3 bottomLeftBack = this.characterPosition + new Vector3(sizeMin, sizeMin, sizeMax) * Settings.BLOCK_SIZE;
            Vector3 bottomRightFront = this.characterPosition + new Vector3(sizeMax, sizeMin, sizeMin) * Settings.BLOCK_SIZE;
            Vector3 bottomRightBack = this.characterPosition + new Vector3(sizeMax, sizeMin, sizeMax) * Settings.BLOCK_SIZE;

            #endregion Calculate the position of the vertices

            #region Add the vertices to the vertices list

            // Add Vertices PY
            this.characterVerticesList.Add(new VertexPositionColor(topLeftFront, this.characterColor));
            this.characterVerticesList.Add(new VertexPositionColor(topRightBack, this.characterColor));
            this.characterVerticesList.Add(new VertexPositionColor(topLeftBack, this.characterColor));
            this.characterVerticesList.Add(new VertexPositionColor(topLeftFront, this.characterColor));
            this.characterVerticesList.Add(new VertexPositionColor(topRightFront, this.characterColor));
            this.characterVerticesList.Add(new VertexPositionColor(topRightBack, this.characterColor));


            // Add Vertices NY
            this.characterVerticesList.Add(new VertexPositionColor(bottomLeftFront, this.characterColor));
            this.characterVerticesList.Add(new VertexPositionColor(bottomLeftBack, this.characterColor));
            this.characterVerticesList.Add(new VertexPositionColor(bottomRightBack, this.characterColor));
            this.characterVerticesList.Add(new VertexPositionColor(bottomLeftFront, this.characterColor));
            this.characterVerticesList.Add(new VertexPositionColor(bottomRightBack, this.characterColor));
            this.characterVerticesList.Add(new VertexPositionColor(bottomRightFront, this.characterColor));

            // Add Vertices PX
            this.characterVerticesList.Add(new VertexPositionColor(topRightFront, this.characterColor));
            this.characterVerticesList.Add(new VertexPositionColor(bottomRightFront, this.characterColor));
            this.characterVerticesList.Add(new VertexPositionColor(bottomRightBack, this.characterColor));
            this.characterVerticesList.Add(new VertexPositionColor(topRightBack, this.characterColor));
            this.characterVerticesList.Add(new VertexPositionColor(topRightFront, this.characterColor));
            this.characterVerticesList.Add(new VertexPositionColor(bottomRightBack, this.characterColor));


            // Add Vertices NX
            this.characterVerticesList.Add(new VertexPositionColor(topLeftFront, this.characterColor));
            this.characterVerticesList.Add(new VertexPositionColor(bottomLeftBack, this.characterColor));
            this.characterVerticesList.Add(new VertexPositionColor(bottomLeftFront, this.characterColor));
            this.characterVerticesList.Add(new VertexPositionColor(topLeftBack, this.characterColor));
            this.characterVerticesList.Add(new VertexPositionColor(bottomLeftBack, this.characterColor));
            this.characterVerticesList.Add(new VertexPositionColor(topLeftFront, this.characterColor));


            // Add Vertices PZ
            this.characterVerticesList.Add(new VertexPositionColor(topLeftBack, this.characterColor));
            this.characterVerticesList.Add(new VertexPositionColor(topRightBack, this.characterColor));
            this.characterVerticesList.Add(new VertexPositionColor(bottomLeftBack, this.characterColor));
            this.characterVerticesList.Add(new VertexPositionColor(bottomLeftBack, this.characterColor));
            this.characterVerticesList.Add(new VertexPositionColor(topRightBack, this.characterColor));
            this.characterVerticesList.Add(new VertexPositionColor(bottomRightBack, this.characterColor));


            // Add Vertices NZ
            this.characterVerticesList.Add(new VertexPositionColor(topLeftFront, this.characterColor));
            this.characterVerticesList.Add(new VertexPositionColor(bottomLeftFront, this.characterColor));
            this.characterVerticesList.Add(new VertexPositionColor(topRightFront, this.characterColor));
            this.characterVerticesList.Add(new VertexPositionColor(bottomLeftFront, this.characterColor));
            this.characterVerticesList.Add(new VertexPositionColor(bottomRightFront, this.characterColor));
            this.characterVerticesList.Add(new VertexPositionColor(topRightFront, this.characterColor));

            #endregion Add the vertices to the vertices list
        }

        public void Update(Player player, GameTime gameTime)
        {
            // Update the position of the character
            UpdatePosition(player, gameTime);

            // Build the mesh according to the new position
            BuildCharacterMesh();
        }

        public void Render(Player player)
        {
            if (this.characterVerticesList != null)
            {
                if (this.characterVerticesList.Count != 0)
                {
                    #region SET-UP Basic Effect

                    Globals.enemieBasicEffect.View = player.camera.View;
                    Globals.enemieBasicEffect.CurrentTechnique.Passes[0].Apply();

                    #endregion SET-UP Basic Effect

                    #region Render

                    Globals.block_ColorPosition_VertexBuffer.SetData(this.characterVerticesList.ToArray());
                    Globals.graphicsDevice.SetVertexBuffer(Globals.block_ColorPosition_VertexBuffer);
                    Globals.graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, this.characterVerticesList.Count / 3);

                    #endregion Render
                }
            }
        }

        #endregion Methods
    }
}
