using System;
using Effects;
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

    }
}
