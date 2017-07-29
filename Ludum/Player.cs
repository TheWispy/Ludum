using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System.Collections.Generic;

namespace Ludum
{
    class Player
    {
        public Vector2 Position;
        public bool Active;
        public int Health;
        public float MOVE_SPEED = 0.4f;
        public Animation PlayerAnimation;
        public List<Animation> MoveSet;
        KeyboardState currentKeys;
        KeyboardState previousKeys;

        public int Width
        {
            get { return PlayerAnimation.FrameWidth; }
        }

        public int Height
        {
            get { return PlayerAnimation.FrameHeight; }
        }

        public void Initialize(List<Animation> moveSet, Vector2 position)
        {
            MoveSet = moveSet;
            PlayerAnimation = MoveSet[0];
            Position = position;
            Active = true;
            Health = 100;
        }

        public void Update(GameTime gameTime)
        
        {
            currentKeys = Keyboard.GetState();
            if (!Active) return; //TODO Game over

            PlayerAnimation.Position = Position;
            PlayerAnimation.Update(gameTime);
            if (currentKeys.IsKeyDown(Keys.Left) || currentKeys.IsKeyDown(Keys.A))
            {
                Position.X -= MOVE_SPEED;
                PlayerAnimation.Pause = false;
            }
            if (currentKeys.IsKeyDown(Keys.Right) || currentKeys.IsKeyDown(Keys.D))
            {
                Position.X += MOVE_SPEED;
                PlayerAnimation.Pause = false;
            }
            if (currentKeys.IsKeyDown(Keys.Space) && previousKeys.IsKeyUp(Keys.Space))
            {
                Shoot();
            }
            previousKeys = currentKeys;
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
            Debug.WriteLine("BANG"+System.DateTime.Now.Millisecond);
        }
    }
}
