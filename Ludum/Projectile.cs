using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ludum
{
    class Projectile
    {
        public Animation LaserAnimation;
        float laserMoveSpeed = 2f;
        public Vector2 Direction;
        public Vector2 Position;
        public int Damage = 1;
        public bool Active;
        public int Range;
        public int Width
        {
            get { return LaserAnimation.FrameWidth; }
        }
        public int Height
        {
            get { return LaserAnimation.FrameHeight; }
        }

        public void Initialize(Animation animation, Vector2 position, Vector2 direction)
        {
            LaserAnimation = animation;
            Position = position;
            Active = true;
            Direction = direction;
        }

        public void Update(GameTime gameTime)
        {
            Position.X += laserMoveSpeed*Direction.X;
            Position.Y += laserMoveSpeed * Direction.Y;
            LaserAnimation.Position = Position;
            LaserAnimation.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            LaserAnimation.Draw(spriteBatch);
        }
    }
}
