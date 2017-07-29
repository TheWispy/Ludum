using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Effects
{
    class Animation
    {
        Texture2D spriteStrip;
        float scale;
        int elapsedTime; //Time since frame was updated
        int frameTime;
        int frameCount; //Number of frames in strip
        int currentFrame;
        Color color;
        Rectangle sourceRect = new Rectangle(); //Area of strip we want to display
        Rectangle destinationRect = new Rectangle(); //Area we we want to display the image
        public int FrameWidth;
        public int FrameHeight;
        public bool Active;
        public bool Looping;
        public Vector2 Position;
        public void Initialize(Texture2D texture, Vector2 position, int frameWidth, int frameHeight, int frameCount,
            int frametime, Color color, float scale, bool looping)
        {
            this.color = color;
            this.FrameWidth = frameWidth;
            this.FrameHeight = frameHeight;
            this.frameCount = frameCount;
            this.frameTime = frametime;
            this.scale = scale;

            Looping = looping;
            Position = position;
            spriteStrip = texture;

            elapsedTime = 0;
            currentFrame = 0;

            Active = true;
        }
        public void Update(GameTime gameTime)
        {
            if (!Active) return;

            elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (elapsedTime > frameTime)
            {
                // Move to the next frame
                currentFrame++;
                // If the currentFrame is equal to frameCount reset currentFrame to zero
                if (currentFrame == frameCount)
                {
                    currentFrame = 0;
                    // If we are not looping deactivate the animation
                    if (Looping == false)
                        Active = false;
                }
                // Reset the elapsed time to zero
                elapsedTime = 0;
            }
            // Grab the correct frame in the image strip by multiplying the currentFrame index by the frame width
            sourceRect = new Rectangle(currentFrame * FrameWidth, 0, FrameWidth, FrameHeight);
            // Grab the correct frame in the image strip by multiplying the currentFrame index by the frame width
            destinationRect = new Rectangle((int)Position.X - (int)(FrameWidth * scale) / 2,
            (int)Position.Y - (int)(FrameHeight * scale) / 2,
            (int)(FrameWidth * scale),
            (int)(FrameHeight * scale));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Active)
            {
                spriteBatch.Draw(spriteStrip, destinationRect, sourceRect, color);
            }
        }
    }
}
