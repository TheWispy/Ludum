using System;
using Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Shooter
{
    class Player
    {
        public Vector2 Position;
        public bool Active;
        public int Health;
        public float MOVE_SPEED = 0.4f;
        public Animation PlayerAnimation;

        public int Width
        {
            get { return PlayerAnimation.FrameWidth; }
        }

        public int Height
        {
            get { return PlayerAnimation.FrameHeight; }
        }

        public void Initialize(Animation animation, Vector2 position)
        {
            PlayerAnimation = animation;
            Position = position;
            Active = true;
            Health = 100;
        }

        public void Update(GameTime gameTime)
        
        {
            PlayerAnimation.Position = Position;
            PlayerAnimation.Update(gameTime);
            if (Keyboard.GetState().IsKeyDown(Keys.Left) || Keyboard.GetState().IsKeyDown(Keys.A))
            {
                Position.X -= MOVE_SPEED;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right) || Keyboard.GetState().IsKeyDown(Keys.D))
            {
                Position.X += MOVE_SPEED;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                Shoot();
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            PlayerAnimation.Draw(spriteBatch);
            
        }
        public void SetAnimation(Animation animation)
        {
            PlayerAnimation = animation;
        }
        public void Shoot()
        {

        }
    }
}
