using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Ludum
{
    class Enemy
    {
        public Vector2 Position;
        public bool Active;
        public int Health;
        public float MOVE_SPEED = 0.4f;
        public Animation EnemyAnimation;
        public int Damage;
        
        public int Width
        {
            get { return EnemyAnimation.FrameWidth; }
        }

        public int Height
        {
            get { return EnemyAnimation.FrameHeight; }
        }

        float enemyMoveSpeed;

        public void Initialize(Animation animation, Vector2 position)
        {
            EnemyAnimation = animation;
            Position = position;
            Active = true;
            Health = 10;
            Damage = 20;
            enemyMoveSpeed = 0.4f;
        }

        public void Update(GameTime gameTime)
        {
            Position.X -= enemyMoveSpeed;
            EnemyAnimation.Position = Position;
            EnemyAnimation.Update(gameTime);
            if (Position.X < -20 || Health <= 0)
            {
                Active = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            EnemyAnimation.Draw(spriteBatch);
        }
    }
}
