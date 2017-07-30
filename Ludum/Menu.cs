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
    }
}
