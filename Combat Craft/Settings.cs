using Microsoft.Xna.Framework;

namespace Combat_Craft
{
    static class Settings
    {
        #region Data

        // Screen
        public static bool FullScreen { get; set; }
        public static float mouseSensitivity { get; set; }

        // Player
        public static double playerHeight { get; set; }
        public static bool EnableChangingFlyingMode { get; set; }

        // World
        public static PerlinNoise_Type perlinNoise_Type { get; set; }
        public static int worldRenderingDistance { get; set; }
        public static int worldSmoothness { get; set; }

        // Enemy
        public static int maxEnemies { get; set; }
        public static float enemySpawnRateSeconds { get; set; }
        public static float enemyDamage { get; set; }
        public static float enemyPlayerSafeDistance { get; set; }
        public static float enemySpeedFarFromPlayer { get; set; }
        public static float enemyPlayerCloseDistance { get; set; }
        public static float enemyMinSpeedGenerate { get; set; }
        public static float enemyMaxSpeedGenerate { get; set; }
        public static float default_enemyJumpingPower { get; set; }

        // Block
        public static Vector3 BLOCK_SIZE { get; set; }
        private static Vector3 default_blockSize { get; set; }

        // Chunk
        public static int CHUNK_SIZE { get; set; }

        // Status
        public static bool ShowStatus { get; set; }
        public static Vector2 default_statusPosition { get; set; }
        public static Color default_statusColor { get; set; }

        // Physics
        public static float default_gravityPower { get; set; }
        public static float default_gravityWaterPower { get; set; }
        public static float default_jumpingPower { get; set; }
        public static float default_slowDownSpeed { get; set; }
        public static float default_inWaterSpeed { get; set; }
        public static float default_walkingSpeed { get; set; }
        public static float default_runningSpeed { get; set; }
        public static float default_runningFlyingSpeed { get; set; }

        // Effects
        public static float water_transparency { get; set; }
        public static bool  blockPointed_discoMode { get; set; }
        public static Color blockPointed_markColor { get; set; }
        public static float blockPointed_markStrength { get; set; }

        #endregion Data

        #region Methods

        public static void Initialize()
        {
            // Screen Settings
            Settings.FullScreen = true;
            Settings.mouseSensitivity = 0.25f;

            // Player
            Settings.playerHeight = 1; // 1 <= playerHeight < ~1.75
            Settings.EnableChangingFlyingMode = true;

            // World
            Settings.perlinNoise_Type = PerlinNoise_Type.Island;
            Settings.worldRenderingDistance = 5;
            Settings.worldSmoothness = 1;

            // Enemy
            Settings.maxEnemies = 30;
            Settings.enemySpawnRateSeconds = 0.01f;
            Settings.enemyDamage = 5;
            Settings.enemyPlayerSafeDistance = 0.8f;
            Settings.enemySpeedFarFromPlayer = 0.2f;
            Settings.enemyPlayerCloseDistance = 2;
            Settings.enemyMinSpeedGenerate = 0.3f;
            Settings.enemyMaxSpeedGenerate = 1f;
            Settings.default_enemyJumpingPower = 3;

            // Block
            Settings.default_blockSize = new Vector3(0.5f, 0.5f, 0.5f);
            Settings.BLOCK_SIZE = default_blockSize;

            // Chunk
            Settings.CHUNK_SIZE = 10;

            // Status
            Settings.ShowStatus = false;
            Settings.default_statusPosition = new Vector2(20, 20);
            Settings.default_statusColor = Color.White;

            // Physics
            Settings.default_gravityPower       = 1.5f;
            if (Settings.perlinNoise_Type == PerlinNoise_Type.Moon)
            { Settings.default_gravityPower     = 0.35f; }
            Settings.default_gravityWaterPower  = 3f;
            Settings.default_jumpingPower       = -10f;
            Settings.default_slowDownSpeed      = 1f;
            Settings.default_inWaterSpeed       = 3f;
            Settings.default_walkingSpeed       = 6f;
            Settings.default_runningSpeed       = 9f;
            Settings.default_runningFlyingSpeed = 30f;

            // Effects
            Settings.water_transparency         = 0.8f;
            Settings.blockPointed_markColor     = Color.Black;
            Settings.blockPointed_markStrength  = 0.2f;
            Settings.blockPointed_discoMode     = true;
        }
        
        #endregion Methods
    }
}