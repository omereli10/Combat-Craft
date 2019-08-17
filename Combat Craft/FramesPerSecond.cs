using Microsoft.Xna.Framework;

namespace Combat_Craft
{
    public class FramesPerSecond
    {
        #region Data

        public float FPS;
        private int currentFrame;
        private float currentTime;
        private float prevTime;
        private float timeDiffrence;
        private float FrameTimeAverage;
        private float[] frames_sample;
        const int NUM_SAMPLES = 30;

        #endregion Data

        #region Constructors

        public FramesPerSecond()
        {
            this.currentTime = 0;
            this.FPS = 0;
            this.frames_sample = new float[NUM_SAMPLES];
            this.prevTime = 0;
        }

        #endregion Constructors

        #region Methods

        public void Update(GameTime gameTime)
        {
            this.currentTime = (float)gameTime.TotalGameTime.TotalMilliseconds;
            this.timeDiffrence = currentTime - prevTime;
            this.frames_sample[currentFrame % NUM_SAMPLES] = timeDiffrence;
            int count;
            if (this.currentFrame < NUM_SAMPLES)
            { count = currentFrame; }
            else
            { count = NUM_SAMPLES; }
            if (this.currentFrame % NUM_SAMPLES == 0)
            {
                this.FrameTimeAverage = 0;
                for (int i = 0; i < count; i++)
                { this.FrameTimeAverage += this.frames_sample[i]; }
                if (count != 0)
                { this.FrameTimeAverage /= count; }
                if (this.FrameTimeAverage > 0)
                { this.FPS = (1000f / this.FrameTimeAverage); }
                else
                { this.FPS = 0; }
            }
            this.currentFrame++;
            this.prevTime = this.currentTime;
        }

        #endregion Methods
    }
}