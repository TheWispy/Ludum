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
        public Texture2D Container;
        public Texture2D HealthSegment;
        public Texture2D PowerSegment;
        public Texture2D KeyPadTex;
        public bool KeyPad;
        int Health;
        int Power;
        int HealthSegments;
        int PowerSegments;
        const int HEALTH_X = 10;
        const int HEALTH_Y = 5;
        const int POWER_X = 10;
        const int POWER_Y = 20;

        public void Initialize(Texture2D container, Texture2D HSegment, Texture2D PSegment, Texture2D Pad)
        {
            Container = container;
            Health = 100;
            Power = 100;
            HealthSegment = HSegment;
            PowerSegment = PSegment;
            KeyPadTex = Pad;
        }
        public void Update(int health, int power, bool Pad)
        {
            Health = health;
            Power = power;
            PowerSegments = (int)Math.Round(Power / 4.54);
            HealthSegments = (int)Math.Round(Health / 4.54);
            KeyPad = Pad;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Container, new Vector2(HEALTH_X, HEALTH_Y));
            spriteBatch.Draw(Container, new Vector2(POWER_X, POWER_Y));
            for (int i = 0; i < HealthSegments; i++)
            {
                spriteBatch.Draw(HealthSegment, new Vector2((2 * i) + 3 + HEALTH_X, HEALTH_Y + 2));
            }
            for (int i = 0; i < PowerSegments; i++)
            {
                spriteBatch.Draw(PowerSegment, new Vector2((2 * i) + 3 + HEALTH_X, POWER_Y + 2));
            }
            if (KeyPad)
            {
                spriteBatch.Draw(KeyPadTex, new Vector2(140, 20));
            }
        }
    }
}
