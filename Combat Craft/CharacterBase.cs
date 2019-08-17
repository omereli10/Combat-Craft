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
    class CharacterBase
    {
        #region Data

        public Vector3 characterPosition;
        protected Color characterColor;
        protected int characterDamage;
        protected float characterMovingSpeed;
        protected float characterGravitationForce;

        #endregion Data

        #region Constructors

        public CharacterBase(Vector3 position, Color color, float speed, int damage, int fallHeight, Player player)
        {
            // Intialize character's data
            this.characterPosition = position;
            this.characterColor = color;
            this.characterMovingSpeed = speed;
            this.characterDamage = damage;
            this.characterGravitationForce = 0;

            // Intialize the start position of the character
            this.InitializePosition(player, fallHeight);
        }

        #endregion Constructors

        #region Methods

        public void InitializePosition(Player player, int fallHeight)
        {
            this.characterPosition.Y = Globals.perlin_noise.getPerlinNoise_MaxHeight() + fallHeight;
        }

        // Update the position of the character towards the player
        public void UpdatePosition(Player player, GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            #region Setting Movement Vector

            // The diffrence between the player position and the character position
            Vector3 movementVector;

            // The origin of the enemy and it's center are diffrent so add 0.5f to the origin
            movementVector.X = player.camera.Position.X - (this.characterPosition.X + 0.5f);
            movementVector.Z = player.camera.Position.Z - (this.characterPosition.Z + 0.5f);

            // Moving in Y axis according to the gravitation force applied
            movementVector.Y = this.characterGravitationForce;

            // Normalize the vector so the camera will not move faster diagonally
            movementVector = Globals.NormalizeXZ(movementVector);

            #region Add smooth And speed

            movementVector.Y *= deltaTime;

            // Set the speed of the enemy according to the distance from the player
            if (Globals.DistanceBetweenTwoVector2(new Vector2(player.camera.Position.X, player.camera.Position.Z), new Vector2(this.characterPosition.X, this.characterPosition.Z)) < 50)
            {
                movementVector.X *= deltaTime * this.characterMovingSpeed;
                movementVector.Z *= deltaTime * this.characterMovingSpeed;
            }
            else
            {
                movementVector.X *= deltaTime * Settings.enemySpeedFarFromPlayer;
                movementVector.Z *= deltaTime * Settings.enemySpeedFarFromPlayer;
            }

            #endregion Add smooth And speed

            // Updating falling speed
            this.characterGravitationForce -= deltaTime * Settings.default_gravityPower;

            #endregion Setting Movement Vector

            #region Minuses Check

            // (int)0.3 = 0, (int)-0.3 = 0 but the result needs to be -1.
            double minusX = 0, minusZ = 0;
            // X
            if ((int)(this.characterPosition.X + movementVector.X) <= 0 && movementVector.X <= 0) { minusX = -1; }
            if ((int)(this.characterPosition.X + movementVector.X) <= 0 && movementVector.X > 0)  { minusX = -1; }
            if ((int)(this.characterPosition.X + movementVector.X) > 0  && movementVector.X > 0)  { minusX = -1;  }
            // Z
            if ((int)(this.characterPosition.Z + movementVector.Z) <= 0 && movementVector.Z <= 0) { minusZ = -1; }
            if ((int)(this.characterPosition.Z + movementVector.Z) <= 0 && movementVector.Z > 0)  { minusZ = -1; }
            if ((int)(this.characterPosition.Z + movementVector.Z) > 0  && movementVector.Z > 0)  { minusZ = -1;  }

            #endregion Minuses Check

            // Collision X
            if (!GameDictionaries.blocksDictionary.ContainsKey(new Vector3((int)(this.characterPosition.X + movementVector.X + minusX), (int)(this.characterPosition.Y), (int)(this.characterPosition.Z))))
            { this.characterPosition.X += movementVector.X; }
            // Make a jump if collide
            else
            { this.characterGravitationForce = Settings.default_enemyJumpingPower; }

            // Collision Y
            if (!GameDictionaries.blocksDictionary.ContainsKey(new Vector3((int)(this.characterPosition.X), (int)(this.characterPosition.Y + movementVector.Y), (int)(this.characterPosition.Z))) || this.characterGravitationForce > 0)
            { this.characterPosition.Y += movementVector.Y; }


            // Collision Z
            if (!GameDictionaries.blocksDictionary.ContainsKey(new Vector3((int)(this.characterPosition.X), (int)(this.characterPosition.Y), (int)(this.characterPosition.Z + movementVector.Z + minusZ))))
            { this.characterPosition.Z += movementVector.Z; }
            // Make a jump if collide
            else
            { this.characterGravitationForce = Settings.default_enemyJumpingPower; }
        }

        #endregion Methods
    }
}
