using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System.Collections.Generic;

namespace Ludum
{
    class Menu
    {
        public Texture2D HealthContainer;
        public Texture2D HealthSegment;
        int Health;
        int Segments;
        const int HEALTH_X = 50;
        const int HEALTH_Y = 5;
        

        public void Initialize(Texture2D container, Texture2D segment)
        {
            HealthContainer = container;
            Health = 100;
            HealthSegment = segment;
        }
        public void Update(int health)
        {
            Health = health;
            Segments = (int)Math.Round(Health / 4.54);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(HealthContainer, new Vector2(40,40));
            for (int i = 0; i < Segments; i++)
            {
                spriteBatch.Draw(HealthSegment, new Vector2((2 * i) + 3 + 40, (40 + 2)));
            }
        }
    }
}
