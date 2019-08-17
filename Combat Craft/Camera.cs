using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Combat_Craft
{
    class Camera : GameComponent
    {
        #region Data

        private Player player;
        private Vector3 camPosition;
        private Vector3 camTarget;
        private Vector3 camRotation;
        private Vector3 mouseRotationBuffer;
        private BoundingFrustum boundingFrustumView;
        private const float camAngel = 90f;
        public Vector3 Position { get { return camPosition; } set { camPosition = value; UpdateCameraTarget(); } }
        public Vector3 Rotation { get { return camRotation; } set { camRotation = value; UpdateCameraTarget(); } }
        public Matrix Projection { get; protected set; }
        public Matrix View { get { return Matrix.CreateLookAt(camPosition, camTarget, Vector3.Up); } }

        #endregion Data

        #region Constructors

        public Camera(Game game, Player player, Vector3 position) : base(game)
        {
            this.player = player;
            this.camPosition = position;
            this.camRotation = Vector3.Zero;
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(camAngel), Globals.graphicsDevice.Viewport.AspectRatio, 0.05f, 100000f);
            player.baseMouseKeyboard.previosMouseState = Mouse.GetState();
        }

        #endregion Constructors

        #region Methods

        // Update Camera
        public override void Update(GameTime gameTime)
        {
            if (!Globals.Splash_GameOver)
            {
                float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
                player.baseMouseKeyboard.currentMouseState = Mouse.GetState();
                Matrix rotationMatrix = Matrix.CreateRotationY(camRotation.Y);
                Vector3 movementVector = HandlePlayerPhysics(gameTime);

                // Normalize the vector so the camera will not move faster diagonally
                movementVector = Globals.NormalizeXZ(movementVector);

                // Add smooth and speed
                movementVector.X *= deltaTime * this.player.playerSpeed;
                movementVector.Y *= deltaTime;
                movementVector.Z *= deltaTime * this.player.playerSpeed;

                // Moving the camera
                MoveCamera(movementVector);

                //Handle Mouse
                if (Globals.mouseLock)
                {
                    float deltaX, deltaY;
                    if (player.baseMouseKeyboard.currentMouseState != player.baseMouseKeyboard.previosMouseState)
                    {
                        // Catch mouse location
                        deltaX = player.baseMouseKeyboard.currentMouseState.X - (Globals.graphicsDevice.Viewport.Width / 2);
                        deltaY = player.baseMouseKeyboard.currentMouseState.Y - (Globals.graphicsDevice.Viewport.Height / 2);
                        mouseRotationBuffer.X -= Settings.mouseSensitivity * deltaX * deltaTime;
                        mouseRotationBuffer.Y -= Settings.mouseSensitivity * deltaY * deltaTime;
                        // Don't let the person move their head more than 88 degrees up or down
                        if (mouseRotationBuffer.Y > MathHelper.ToRadians(88f))
                        { mouseRotationBuffer.Y = MathHelper.ToRadians(88f); }
                        if (mouseRotationBuffer.Y < MathHelper.ToRadians(-88f))
                        { mouseRotationBuffer.Y = MathHelper.ToRadians(-88f); }
                        Rotation = new Vector3(-MathHelper.Clamp(mouseRotationBuffer.Y, MathHelper.ToRadians(-88f), MathHelper.ToRadians(88f)), mouseRotationBuffer.X, 0);
                    }
                    Mouse.SetPosition((int)Globals.middleOfTheScreen.X, (int)Globals.middleOfTheScreen.Y);
                }
                player.baseMouseKeyboard.previosMouseState = player.baseMouseKeyboard.currentMouseState;
                this.boundingFrustumView = new BoundingFrustum(View * Projection);
            }
        }

        // Move the position of the camera by given scale
        private void MoveCamera(Vector3 scale)
        {
            MoveTo(movePreview(scale), Rotation);
        }

        // Calculate the new position of the camera
        private Vector3 movePreview(Vector3 moveAmount)
        {
            // Create rotation matrix
            Matrix rotationMatrix;
            if (!Globals.Splash_HasGameStart)
            { rotationMatrix = Matrix.CreateRotationY(0); }
            else
            { rotationMatrix = Matrix.CreateRotationY(camRotation.Y); }
            // Create movement vector
            Vector3 movementVector = Vector3.Transform(moveAmount, rotationMatrix);
            // Return a new vector of the new camera postion preview
            return camPosition + movementVector;
        }

        // Moving the camera
        private void MoveTo(Vector3 position, Vector3 rotation)
        {
            #region Position Collision

            #region Minuses Check

            // (int)0.3 = 0, (int)-0.3 = 0 but the result needs to be -1.
            double minusX = 0, minusY = 0, minusZ = 0;
            if ((int)(position.X) <= 0) { minusX -= 1; }
            if ((int)(position.Y) <= 0) { minusY -= 1; }
            if ((int)(position.Z) <= 0) { minusZ -= 1; }

            #endregion Minuses Check

            #region Y Axis

            this.player.isHeadInWater = false;
            if (!GameDictionaries.blocksDictionary.ContainsKey(new Vector3((int)(Position.X + minusX + Globals.saftyDistanceFromBlock), (int)(position.Y + minusY - Settings.playerHeight - Globals.saftyDistanceFromBlock), (int)(Position.Z + minusZ + Globals.saftyDistanceFromBlock))) &&
                !GameDictionaries.blocksDictionary.ContainsKey(new Vector3((int)(Position.X + minusX - Globals.saftyDistanceFromBlock), (int)(position.Y + minusY - Settings.playerHeight - Globals.saftyDistanceFromBlock), (int)(Position.Z + minusZ - Globals.saftyDistanceFromBlock))) &&
                !GameDictionaries.blocksDictionary.ContainsKey(new Vector3((int)(Position.X + minusX - Globals.saftyDistanceFromBlock), (int)(position.Y + minusY - Settings.playerHeight - Globals.saftyDistanceFromBlock), (int)(Position.Z + minusZ + Globals.saftyDistanceFromBlock))) &&
                !GameDictionaries.blocksDictionary.ContainsKey(new Vector3((int)(Position.X + minusX + Globals.saftyDistanceFromBlock), (int)(position.Y + minusY - Settings.playerHeight - Globals.saftyDistanceFromBlock), (int)(Position.Z + minusZ - Globals.saftyDistanceFromBlock))))
            {
                this.player.isFalling = true;
                this.player.isInWater = false;
                if (!GameDictionaries.blocksDictionary.ContainsKey(new Vector3((int)(Position.X + minusX + Globals.saftyDistanceFromBlock), (int)(position.Y + minusY + Globals.saftyDistanceFromBlock), (int)(Position.Z + minusZ + Globals.saftyDistanceFromBlock))) &&
                    !GameDictionaries.blocksDictionary.ContainsKey(new Vector3((int)(Position.X + minusX - Globals.saftyDistanceFromBlock), (int)(position.Y + minusY + Globals.saftyDistanceFromBlock), (int)(Position.Z + minusZ - Globals.saftyDistanceFromBlock))) &&
                    !GameDictionaries.blocksDictionary.ContainsKey(new Vector3((int)(Position.X + minusX - Globals.saftyDistanceFromBlock), (int)(position.Y + minusY + Globals.saftyDistanceFromBlock), (int)(Position.Z + minusZ + Globals.saftyDistanceFromBlock))) &&
                    !GameDictionaries.blocksDictionary.ContainsKey(new Vector3((int)(Position.X + minusX + Globals.saftyDistanceFromBlock), (int)(position.Y + minusY + Globals.saftyDistanceFromBlock), (int)(Position.Z + minusZ - Globals.saftyDistanceFromBlock))))
                { Position = new Vector3(Position.X, position.Y, Position.Z); }
                // Get down because of the block above
                else
                { this.player.fallingSpeed = Settings.default_gravityPower; }
            }
            else
            {
                // Water
                if (GameDictionaries.blocksDictionary.TryGetValue(new Vector3((int)(Position.X + minusX), (int)(position.Y + minusY - Settings.playerHeight - Globals.saftyDistanceFromBlock), (int)(Position.Z + minusZ)), out BlockType value1) && value1 == BlockType.Water)
                { this.player.isInWater = true; Position = new Vector3(Position.X, position.Y, Position.Z); }
                else
                { this.player.isFalling = false; this.player.fallingSpeed = Settings.default_gravityPower; }

                if (GameDictionaries.blocksDictionary.TryGetValue(new Vector3((int)(Position.X + minusX), (int)(position.Y + minusY - Globals.block_Yoffset + 1.5*Globals.saftyDistanceFromBlock), (int)(Position.Z + minusZ)), out BlockType value2) && value2 == BlockType.Water)
                { this.player.isHeadInWater = true; }
            }

            #endregion Y Axis

            #region X & Z Axis

            if (this.player.isInWater)
            {
                #region X Axis

                if (GameDictionaries.blocksDictionary.TryGetValue(new Vector3((int)(position.X + minusX + Globals.saftyDistanceFromBlock), (int)(Position.Y + minusY), (int)(Position.Z + minusZ + Globals.saftyDistanceFromBlock)), out BlockType value1) && value1 == BlockType.Water &&
                    GameDictionaries.blocksDictionary.TryGetValue(new Vector3((int)(position.X + minusX + Globals.saftyDistanceFromBlock), (int)(Position.Y + minusY), (int)(Position.Z + minusZ - Globals.saftyDistanceFromBlock)), out BlockType value2) && value2 == BlockType.Water &&
                    GameDictionaries.blocksDictionary.TryGetValue(new Vector3((int)(position.X + minusX - Globals.saftyDistanceFromBlock), (int)(Position.Y + minusY), (int)(Position.Z + minusZ + Globals.saftyDistanceFromBlock)), out BlockType value3) && value3 == BlockType.Water &&
                    GameDictionaries.blocksDictionary.TryGetValue(new Vector3((int)(position.X + minusX - Globals.saftyDistanceFromBlock), (int)(Position.Y + minusY), (int)(Position.Z + minusZ - Globals.saftyDistanceFromBlock)), out BlockType value4) && value4 == BlockType.Water &&
                    GameDictionaries.blocksDictionary.TryGetValue(new Vector3((int)(position.X + minusX + Globals.saftyDistanceFromBlock), (int)(Position.Y + minusY - Settings.playerHeight), (int)(Position.Z + minusZ + Globals.saftyDistanceFromBlock)), out BlockType value5) && value5 == BlockType.Water &&
                    GameDictionaries.blocksDictionary.TryGetValue(new Vector3((int)(position.X + minusX + Globals.saftyDistanceFromBlock), (int)(Position.Y + minusY - Settings.playerHeight), (int)(Position.Z + minusZ - Globals.saftyDistanceFromBlock)), out BlockType value6) && value6 == BlockType.Water &&
                    GameDictionaries.blocksDictionary.TryGetValue(new Vector3((int)(position.X + minusX - Globals.saftyDistanceFromBlock), (int)(Position.Y + minusY - Settings.playerHeight), (int)(Position.Z + minusZ + Globals.saftyDistanceFromBlock)), out BlockType value7) && value7 == BlockType.Water &&
                    GameDictionaries.blocksDictionary.TryGetValue(new Vector3((int)(position.X + minusX - Globals.saftyDistanceFromBlock), (int)(Position.Y + minusY - Settings.playerHeight), (int)(Position.Z + minusZ - Globals.saftyDistanceFromBlock)), out BlockType value8) && value8 == BlockType.Water)
                { Position = new Vector3(position.X, Position.Y, Position.Z); }

                #endregion X Axis

                #region Z Axis

                // Z :
                if (GameDictionaries.blocksDictionary.TryGetValue(new Vector3((int)(Position.X + minusX + Globals.saftyDistanceFromBlock), (int)(Position.Y + minusY), (int)(position.Z + minusZ + Globals.saftyDistanceFromBlock)), out BlockType value9) && value9 == BlockType.Water &&
                    GameDictionaries.blocksDictionary.TryGetValue(new Vector3((int)(Position.X + minusX - Globals.saftyDistanceFromBlock), (int)(Position.Y + minusY), (int)(position.Z + minusZ + Globals.saftyDistanceFromBlock)), out BlockType value10) && value10 == BlockType.Water &&
                    GameDictionaries.blocksDictionary.TryGetValue(new Vector3((int)(Position.X + minusX + Globals.saftyDistanceFromBlock), (int)(Position.Y + minusY), (int)(position.Z + minusZ - Globals.saftyDistanceFromBlock)), out BlockType value11) && value11 == BlockType.Water &&
                    GameDictionaries.blocksDictionary.TryGetValue(new Vector3((int)(Position.X + minusX - Globals.saftyDistanceFromBlock), (int)(Position.Y + minusY), (int)(position.Z + minusZ - Globals.saftyDistanceFromBlock)), out BlockType value12) && value12 == BlockType.Water &&
                    GameDictionaries.blocksDictionary.TryGetValue(new Vector3((int)(Position.X + minusX + Globals.saftyDistanceFromBlock), (int)(Position.Y + minusY - Settings.playerHeight), (int)(position.Z + minusZ + Globals.saftyDistanceFromBlock)), out BlockType value13) && value13 == BlockType.Water &&
                    GameDictionaries.blocksDictionary.TryGetValue(new Vector3((int)(Position.X + minusX - Globals.saftyDistanceFromBlock), (int)(Position.Y + minusY - Settings.playerHeight), (int)(position.Z + minusZ + Globals.saftyDistanceFromBlock)), out BlockType value14) && value14 == BlockType.Water &&
                    GameDictionaries.blocksDictionary.TryGetValue(new Vector3((int)(Position.X + minusX + Globals.saftyDistanceFromBlock), (int)(Position.Y + minusY - Settings.playerHeight), (int)(position.Z + minusZ - Globals.saftyDistanceFromBlock)), out BlockType value15) && value15 == BlockType.Water &&
                    GameDictionaries.blocksDictionary.TryGetValue(new Vector3((int)(Position.X + minusX - Globals.saftyDistanceFromBlock), (int)(Position.Y + minusY - Settings.playerHeight), (int)(position.Z + minusZ - Globals.saftyDistanceFromBlock)), out BlockType value16) && value16 == BlockType.Water)
                { Position = new Vector3(Position.X, Position.Y, position.Z); }

                #endregion Z Axis
            }
            else
            {
                #region X Axis

                if (!GameDictionaries.blocksDictionary.ContainsKey(new Vector3((int)(position.X + minusX + Globals.saftyDistanceFromBlock), (int)(Position.Y + minusY), (int)(Position.Z + minusZ + Globals.saftyDistanceFromBlock))) &&
                    !GameDictionaries.blocksDictionary.ContainsKey(new Vector3((int)(position.X + minusX + Globals.saftyDistanceFromBlock), (int)(Position.Y + minusY), (int)(Position.Z + minusZ - Globals.saftyDistanceFromBlock))) &&
                    !GameDictionaries.blocksDictionary.ContainsKey(new Vector3((int)(position.X + minusX - Globals.saftyDistanceFromBlock), (int)(Position.Y + minusY), (int)(Position.Z + minusZ + Globals.saftyDistanceFromBlock))) &&
                    !GameDictionaries.blocksDictionary.ContainsKey(new Vector3((int)(position.X + minusX - Globals.saftyDistanceFromBlock), (int)(Position.Y + minusY), (int)(Position.Z + minusZ - Globals.saftyDistanceFromBlock))) &&
                    !GameDictionaries.blocksDictionary.ContainsKey(new Vector3((int)(position.X + minusX + Globals.saftyDistanceFromBlock), (int)(Position.Y + minusY - Settings.playerHeight), (int)(Position.Z + minusZ + Globals.saftyDistanceFromBlock))) &&
                    !GameDictionaries.blocksDictionary.ContainsKey(new Vector3((int)(position.X + minusX + Globals.saftyDistanceFromBlock), (int)(Position.Y + minusY - Settings.playerHeight), (int)(Position.Z + minusZ - Globals.saftyDistanceFromBlock))) &&
                    !GameDictionaries.blocksDictionary.ContainsKey(new Vector3((int)(position.X + minusX - Globals.saftyDistanceFromBlock), (int)(Position.Y + minusY - Settings.playerHeight), (int)(Position.Z + minusZ + Globals.saftyDistanceFromBlock))) &&
                    !GameDictionaries.blocksDictionary.ContainsKey(new Vector3((int)(position.X + minusX - Globals.saftyDistanceFromBlock), (int)(Position.Y + minusY - Settings.playerHeight), (int)(Position.Z + minusZ - Globals.saftyDistanceFromBlock))))
                { Position = new Vector3(position.X, Position.Y, Position.Z); }

                #endregion X Axis

                #region Z Axis

                // Z :
                if (!GameDictionaries.blocksDictionary.ContainsKey(new Vector3((int)(Position.X + minusX + Globals.saftyDistanceFromBlock), (int)(Position.Y + minusY), (int)(position.Z + minusZ + Globals.saftyDistanceFromBlock))) &&
                    !GameDictionaries.blocksDictionary.ContainsKey(new Vector3((int)(Position.X + minusX - Globals.saftyDistanceFromBlock), (int)(Position.Y + minusY), (int)(position.Z + minusZ + Globals.saftyDistanceFromBlock))) &&
                    !GameDictionaries.blocksDictionary.ContainsKey(new Vector3((int)(Position.X + minusX + Globals.saftyDistanceFromBlock), (int)(Position.Y + minusY), (int)(position.Z + minusZ - Globals.saftyDistanceFromBlock))) &&
                    !GameDictionaries.blocksDictionary.ContainsKey(new Vector3((int)(Position.X + minusX - Globals.saftyDistanceFromBlock), (int)(Position.Y + minusY), (int)(position.Z + minusZ - Globals.saftyDistanceFromBlock))) &&
                    !GameDictionaries.blocksDictionary.ContainsKey(new Vector3((int)(Position.X + minusX + Globals.saftyDistanceFromBlock), (int)(Position.Y + minusY - Settings.playerHeight), (int)(position.Z + minusZ + Globals.saftyDistanceFromBlock))) &&
                    !GameDictionaries.blocksDictionary.ContainsKey(new Vector3((int)(Position.X + minusX - Globals.saftyDistanceFromBlock), (int)(Position.Y + minusY - Settings.playerHeight), (int)(position.Z + minusZ + Globals.saftyDistanceFromBlock))) &&
                    !GameDictionaries.blocksDictionary.ContainsKey(new Vector3((int)(Position.X + minusX + Globals.saftyDistanceFromBlock), (int)(Position.Y + minusY - Settings.playerHeight), (int)(position.Z + minusZ - Globals.saftyDistanceFromBlock))) &&
                    !GameDictionaries.blocksDictionary.ContainsKey(new Vector3((int)(Position.X + minusX - Globals.saftyDistanceFromBlock), (int)(Position.Y + minusY - Settings.playerHeight), (int)(position.Z + minusZ - Globals.saftyDistanceFromBlock))))
                { Position = new Vector3(Position.X, Position.Y, position.Z); }

                #endregion Z Axis
            }

            #endregion X & Z Axis

            #endregion Position Collision
        }

        private void UpdateCameraTarget()
        {
            // Build rotation matrix
            Matrix rotationMatrix = Matrix.CreateRotationX(camRotation.X) * Matrix.CreateRotationY(camRotation.Y);
            // Build target offset vector
            Vector3 targetOffSet = Vector3.Transform(Vector3.Backward, rotationMatrix);
            // Update camera's target
            camTarget = camPosition + targetOffSet;
        }

        // Furstum Culling
        public bool IsBlockInView(BoundingBox boundingBox)
        { return this.boundingFrustumView.Intersects(boundingBox); }

        // Handel the direction and the speed which the player is moving by
        private Vector3 HandlePlayerPhysics(GameTime gameTime)
        {
            Vector3 movementVector = Vector3.Zero;
            this.player.playerSpeed = Settings.default_walkingSpeed;
            if (player.baseMouseKeyboard.IsWalkForward() || player.baseMouseKeyboard.IsRunForward() || player.baseMouseKeyboard.IsSlowdownForward() || (!Globals.Splash_HasGameStart && !Globals.Splash_Screen))
            { movementVector.Z += 1; }
            if ((player.baseMouseKeyboard.IsWalkBackward() || player.baseMouseKeyboard.IsRunBackward() || player.baseMouseKeyboard.IsSlowdownBackward()) && Globals.Splash_HasGameStart)
            { movementVector.Z -= 1; }
            if ((player.baseMouseKeyboard.IsWalkRight() || player.baseMouseKeyboard.IsRunRight() || player.baseMouseKeyboard.IsSlowdownRight()) && Globals.Splash_HasGameStart)
            { movementVector.X += 1; }
            if ((player.baseMouseKeyboard.IsWalkLeft() || player.baseMouseKeyboard.IsRunLeft() || player.baseMouseKeyboard.IsSlowdownLeft()) && Globals.Splash_HasGameStart)
            { movementVector.X -= 1; }
            // Gravity Handler
            if (!this.player.flyingMode)
            {
                // Fall
                if (this.player.isFalling && gameTime.TotalGameTime.Milliseconds % 10 == 0)
                {
                    // Fall in the water
                    if (this.player.isInWater)
                    { this.player.fallingSpeed = Settings.default_gravityWaterPower; }
                    // Regular fall
                    else
                    { this.player.fallingSpeed += Settings.default_gravityPower; }
                }
                // Jump
                if (player.baseMouseKeyboard.IsJump() && !this.player.isFalling)
                { this.player.fallingSpeed = Settings.default_jumpingPower; }
                // Apply Velocity
                movementVector.Y -= this.player.fallingSpeed;
            }
            else
            {
                if (player.baseMouseKeyboard.IsWalkUp() || player.baseMouseKeyboard.IsRunUp() || player.baseMouseKeyboard.IsSlowdownUp())
                { movementVector.Y += Settings.default_runningFlyingSpeed; }
                if (player.baseMouseKeyboard.IsWalkDown() || player.baseMouseKeyboard.IsRunDown() || player.baseMouseKeyboard.IsSlowdownDown())
                { movementVector.Y -= Settings.default_runningFlyingSpeed; }
            }

            bool isRunning = false, isSlowingDown = false;
            if (player.baseMouseKeyboard.IsRunRight() || player.baseMouseKeyboard.IsRunLeft() || player.baseMouseKeyboard.IsRunForward() || player.baseMouseKeyboard.IsRunBackward() || player.baseMouseKeyboard.IsRunUp() || player.baseMouseKeyboard.IsRunDown())
            { isRunning = true; }
            else
            {
                if (player.baseMouseKeyboard.IsSlowdownRight() || player.baseMouseKeyboard.IsSlowdownLeft() || player.baseMouseKeyboard.IsSlowdownForward() || player.baseMouseKeyboard.IsSlowdownBackward() || player.baseMouseKeyboard.IsSlowdownUp() || player.baseMouseKeyboard.IsSlowdownDown())
                { isSlowingDown = true; }
            }
            if (this.player.isInWater)
            { this.player.playerSpeed = Settings.default_inWaterSpeed; }
            else
            {
                if (isRunning && this.player.flyingMode)
                { this.player.playerSpeed = Settings.default_runningFlyingSpeed; }
                if (isRunning && !this.player.flyingMode)
                { this.player.playerSpeed = Settings.default_runningSpeed; }
                if (isSlowingDown)
                { this.player.playerSpeed = Settings.default_slowDownSpeed; }
            }

            // Changing between Flying mode and Normal mode
            if (Settings.EnableChangingFlyingMode)
            {
                if (player.baseMouseKeyboard.IsFlyingMode() && !player.baseMouseKeyboard.isHolding_KeyboardFlyingMode)
                {
                    this.player.flyingMode = !this.player.flyingMode;
                    if (this.player.flyingMode)
                    { this.player.isFalling = false; this.player.fallingSpeed = Settings.default_gravityPower; }
                    player.baseMouseKeyboard.isHolding_KeyboardFlyingMode = true;
                }
            }

            // Mouse Lock Press
            if (player.baseMouseKeyboard.IsMouseLock() && !player.baseMouseKeyboard.isHolding_KeyboardMouseLock)
            { Globals.mouseLock = !Globals.mouseLock; player.baseMouseKeyboard.isHolding_KeyboardMouseLock = true; }

            // Scroll Up Press
            if (player.baseMouseKeyboard.IsScrollUp() && !player.baseMouseKeyboard.isHolding_KeyboardScrollUp)
            {
                player.baseMouseKeyboard.mouseWheel_Value = ++player.baseMouseKeyboard.mouseWheel_Value % Globals.addableBlockTypes.Length;
                player.baseMouseKeyboard.isHolding_KeyboardScrollUp = true;
            }

            // Scroll Down Press
            if (player.baseMouseKeyboard.IsScrollDown() && !player.baseMouseKeyboard.isHolding_KeyboardScrollDown)
            {
                player.baseMouseKeyboard.mouseWheel_Value = (--player.baseMouseKeyboard.mouseWheel_Value + Globals.addableBlockTypes.Length) % Globals.addableBlockTypes.Length;
                player.baseMouseKeyboard.isHolding_KeyboardScrollDown = true;
            }

            // Update Button Holds
            if (!player.baseMouseKeyboard.IsFlyingMode())
            { player.baseMouseKeyboard.isHolding_KeyboardFlyingMode = false; }
            if (player.baseMouseKeyboard.currentMouseState.LeftButton != ButtonState.Pressed)
            { player.baseMouseKeyboard.isHolding_MouseLeftButton = false; }
            if (player.baseMouseKeyboard.currentMouseState.RightButton != ButtonState.Pressed)
            { player.baseMouseKeyboard.isHolding_MouseRightButton = false; }
            if (!player.baseMouseKeyboard.IsMouseLock())
            { player.baseMouseKeyboard.isHolding_KeyboardMouseLock = false; }
            if (!player.baseMouseKeyboard.IsScrollUp())
            { player.baseMouseKeyboard.isHolding_KeyboardScrollUp = false; }
            if (!player.baseMouseKeyboard.IsScrollDown())
            { player.baseMouseKeyboard.isHolding_KeyboardScrollDown = false; }

            return movementVector;
        }

        #endregion Methods
    }
}