using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Ludum
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
        public void Update()
        {

        }
        public void Draw()
        {

        }
    }
}
