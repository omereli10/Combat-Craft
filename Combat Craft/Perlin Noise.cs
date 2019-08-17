namespace Combat_Craft
{
    class Perlin_Noise
    {
        #region Data

        public int seed { get; }
        private double FREQUENCY;
        private int AMPLITUDE;
        private const int X_PRIME = 1619;
        private const int Y_PRIME = 31337;
        private static readonly float[] GRAD_X = { -1, 1, -1, 1, 0, -1, 0, 1 };
        private static readonly float[] GRAD_Y = { -1, -1, 1, 1, -1, 0, 1, 0 };

        #endregion Data

        #region Constructors

        public Perlin_Noise(int seed = -1)
        {
            #region Set Perlin Noise Types

            if (Settings.perlinNoise_Type == PerlinNoise_Type.Island)
            { this.FREQUENCY = 0.03; this.AMPLITUDE = 12; }

            else if (Settings.perlinNoise_Type == PerlinNoise_Type.Regular)
            { this.FREQUENCY = 0.011; this.AMPLITUDE = 35; }

            else if (Settings.perlinNoise_Type == PerlinNoise_Type.Mountain)
            { this.FREQUENCY = 0.02; this.AMPLITUDE = 50; }

            else if (Settings.perlinNoise_Type == PerlinNoise_Type.Flat)
            { this.FREQUENCY = 1; this.AMPLITUDE = 30; }

            else if (Settings.perlinNoise_Type == PerlinNoise_Type.Moon)
            { this.FREQUENCY = 0.05; this.AMPLITUDE = 12; }

            #endregion Set Perlin Noise Types

            #region Set Seed

            if (seed < 0)
            { this.seed = Globals.random.Next(0, 1000000000); }
            else
            { this.seed = seed; }

            #endregion Set Seed
        }

        #endregion Constructors

        #region Methods

        public double GetPerlin2D(double x, double y)
        {
            double perlinNoise2D = CalculatePerlin2D(this.seed, x * FREQUENCY, y * FREQUENCY) * AMPLITUDE + (AMPLITUDE / 3);
            // Minimum Height
            if (perlinNoise2D <= 4) { return 4; }
            else
            {
                if (perlinNoise2D >= AMPLITUDE) { return AMPLITUDE; }
                else { return perlinNoise2D; }
            }
        }

        // This function was copied from the internet
        private double CalculatePerlin2D(int seed, double x, double y)
        {
            int x0 = FastFloor(x);
            int y0 = FastFloor(y);
            int x1 = x0 + 1;
            int y1 = y0 + 1;

            double xs, ys;
            xs = InterpQuinticFunc(x - x0);
            ys = InterpQuinticFunc(y - y0);

            double xd0 = x - x0;
            double yd0 = y - y0;
            double xd1 = xd0 - 1;
            double yd1 = yd0 - 1;

            double xf0 = Lerp(GradCoord2D(seed, x0, y0, xd0, yd0), GradCoord2D(seed, x1, y0, xd1, yd0), xs);
            double xf1 = Lerp(GradCoord2D(seed, x0, y1, xd0, yd1), GradCoord2D(seed, x1, y1, xd1, yd1), xs);

            return Lerp(xf0, xf1, ys);
        }

        // This function was copied from the internet
        private int FastFloor(double f)
        { return (f >= 0 ? (int)f : (int)f - 1); }

        // This function was copied from the internet
        private double InterpHermiteFunc(double t)
        { return t * t * (3 - 2 * t); }

        // This function was copied from the internet
        private double InterpQuinticFunc(double t)
        { return t * t * t * (t * (t * 6 - 15) + 10); }

        // This function was copied from the internet
        private double Lerp(double a, double b, double t)
        { return a + t * (b - a); }

        // This function was copied from the internet
        private double GradCoord2D(int seed, int x, int y, double xd, double yd)
        {
            int hash = seed;
            hash ^= X_PRIME * x;
            hash ^= Y_PRIME * y;

            hash = hash * hash * hash * 60493;
            hash = (hash >> 13) ^ hash;

            int hashAND7 = hash & 7;
            float gx = GRAD_X[hashAND7];
            float gy = GRAD_Y[hashAND7];

            return xd * gx + yd * gy;
        }

        public int getPerlinNoise_MaxHeight()
        {
            return this.AMPLITUDE + 9; // Amplitude + treeheight
        }

        #endregion Methods
    }
}