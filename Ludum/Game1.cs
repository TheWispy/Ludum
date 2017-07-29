﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Shooter;
using Effects;

namespace Ludum
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch _spriteBatch;
        RenderTarget2D renderTarget;
        const int VIRTUAL_WIDTH = 200;
        const int VIRTUAL_HEIGHT = 100;

        Player player;
        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            PresentationParameters pp = graphics.GraphicsDevice.PresentationParameters;
            renderTarget = new RenderTarget2D(graphics.GraphicsDevice, VIRTUAL_WIDTH, VIRTUAL_HEIGHT, false, SurfaceFormat.Color,
                DepthFormat.None, pp.MultiSampleCount, RenderTargetUsage.DiscardContents);
            player = new Player();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            Vector2 playerPosition = new Vector2(20, VIRTUAL_HEIGHT - 20);
            Texture2D idleTexture = Content.Load<Texture2D>("player_idle");
            Animation idle = new Animation();
            idle.Initialize(idleTexture, Vector2.Zero, 32, 32, 4, 150, Color.White, 1f, true);
            player.Initialize(idle, playerPosition);
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            player.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.SetRenderTarget(renderTarget);

            var scaleX = (float)graphics.GraphicsDevice.Viewport.Width / VIRTUAL_WIDTH;
            var scaleY = (float)graphics.GraphicsDevice.Viewport.Height / VIRTUAL_HEIGHT;
            var matrix = Matrix.CreateScale(scaleX, scaleY, 1.0f);

            //Draw your stuff
            graphics.GraphicsDevice.Clear(Color.LightGray);
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            player.Draw(_spriteBatch);
            _spriteBatch.End();

            float outputAspect = Window.ClientBounds.Width / (float)Window.ClientBounds.Height;
            float preferredAspect = VIRTUAL_WIDTH / (float)VIRTUAL_HEIGHT;

            Rectangle dst;

            if (outputAspect <= preferredAspect)
            {
                // output is taller than it is wider, bars on top/bottom
                int presentHeight = (int)((Window.ClientBounds.Width / preferredAspect) + 0.5f);
                int barHeight = (Window.ClientBounds.Height - presentHeight) / 2;
                dst = new Rectangle(0, barHeight, Window.ClientBounds.Width, presentHeight);
            }
            else
            {
                // output is wider than it is tall, bars left/right
                int presentWidth = (int)((Window.ClientBounds.Height * preferredAspect) + 0.5f);
                int barWidth = (Window.ClientBounds.Width - presentWidth) / 2;
                dst = new Rectangle(barWidth, 0, presentWidth, Window.ClientBounds.Height);
            }

            graphics.GraphicsDevice.SetRenderTarget(null);

            // clear to get black bars
            graphics.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 1.0f, 0);

            // draw a quad to get the draw buffer to the back buffer
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(renderTarget, dst, Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}
