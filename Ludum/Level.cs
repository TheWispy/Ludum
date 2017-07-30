using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Diagnostics;

namespace Ludum
{
    class Level
    {
        public int levelNo;
        public List<Enemy> enemies;
        public int enemyCount;
        public List<Projectile> projectiles;
        public bool isComplete = false;

        public void Initialize(int l)
        {
            isComplete = false;
            enemyCount = 0;
            levelNo = l;
            projectiles = new List<Projectile>();
            enemies = new List<Enemy>();
        }
        public void Update(GameTime gameTime)
        {
            if (enemyCount == levelNo && enemies.Count == 0)
            {
                isComplete = true;
            }
            UpdateProjectiles(gameTime);
        }
        public void Draw(SpriteBatch _spriteBatch)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].Draw(_spriteBatch);
                //Debug.WriteLine(enemies[i].Position+ " "+enemies[i].Width+" "+enemies[i].Height );
            }
            for (int i = 0; i < projectiles.Count; i++)
            {
                projectiles[i].Draw(_spriteBatch);
            }
        }
        
        
        private void UpdateProjectiles(GameTime gameTime)
        {
            for (int i = 0; i < projectiles.Count; i++)
            {
                projectiles[i].Update(gameTime);
                if (!projectiles[i].Active)
                {
                    projectiles.RemoveAt(i);
                }
            }
        }
    }
}
