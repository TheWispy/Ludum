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
        public int Power;
        public float MOVE_SPEED = 0.4f;
        public Animation PlayerAnimation;
        public List<Animation> MoveSet;
        KeyboardState currentKeys;
        KeyboardState previousKeys;
        private float previousDepletionTime;
        private float depletionTime;

        public int Width
        {
            get { return PlayerAnimation.FrameWidth; }
        }

        public int Height
        {
            get { return PlayerAnimation.FrameHeight; }
        }

        public void Initialize(List<Animation> moveSet, Vector2 position, int Difficulty)
        {
            MoveSet = moveSet;
            PlayerAnimation = MoveSet[0];
            Position = position;
            Active = true;
            Health = 100;
            Power = 100;
            previousDepletionTime = 0f;
            depletionTime = 0.5f;
        }

        public void Update(GameTime gameTime, float time, int Difficulty)
        
        {
            currentKeys = Keyboard.GetState();
            if (Power <= 0)
            {
                Active = false;
            }
            if (!Active)
            {
                return; //TODO Game over
            }
  

                
            if ((time - previousDepletionTime > depletionTime))
            {
                previousDepletionTime = time;
                Power -= Difficulty/5;
            }
            for (int i = 0; i < MoveSet.Count; i++)
            {
                MoveSet[i].Position = Position;
                MoveSet[i].Update(gameTime);
            }
            if (currentKeys.IsKeyDown(Keys.Left) || currentKeys.IsKeyDown(Keys.A))
            {
                Position.X -= MOVE_SPEED;
                PlayerAnimation = MoveSet[0];
                PlayerAnimation.Pause = false;
            }
            else if (currentKeys.IsKeyDown(Keys.Right) || currentKeys.IsKeyDown(Keys.D))
            {
                Position.X += MOVE_SPEED;
                PlayerAnimation = MoveSet[0];
                PlayerAnimation.Pause = false;
            }
            else if (currentKeys.IsKeyDown(Keys.Down) || currentKeys.IsKeyDown(Keys.S))
            {
                PlayerAnimation = MoveSet[1];
            }
            else
            {
                PlayerAnimation = MoveSet[0];
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
    }
}
