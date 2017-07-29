using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Diagnostics;

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
        const int VIRTUAL_WIDTH = 256;
        const int VIRTUAL_HEIGHT = 144;
        public bool isPaused = false;
        float time;
        Player player;

        //Enemies
        Texture2D enemyTexture;
        List<Enemy> enemies;
        float enemySpawnTime;
        float previousSpawnTime;

        //Projectiles
        Texture2D projectileTexture;
        TimeSpan projectileSpawnTime;
        TimeSpan previousLaserSpawnTime;
        List<Projectile> projectiles;
        Random random;

        KeyboardState currentKeys;
        KeyboardState previousKeys;

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

            enemies = new List<Enemy>();
            previousSpawnTime = 0f;
            enemySpawnTime = 2f;

            projectiles = new List<Projectile>();
            const float SECONDS_IN_MINUTE = 60f;
            const float RATE_OF_FIRE = 200f;
            projectileSpawnTime = TimeSpan.FromSeconds(SECONDS_IN_MINUTE / RATE_OF_FIRE);
            previousLaserSpawnTime = TimeSpan.Zero;

            random = new Random();

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
            List<Animation> playerMoveSet = new List<Animation>();
            Texture2D idleTexture = Content.Load<Texture2D>("player_idle");
            Animation idle = new Animation();
            idle.Initialize(idleTexture, playerPosition, 32, 32, 4, 150, Color.White, 1f, true);
            playerMoveSet.Add(idle);
            player.Initialize(playerMoveSet, playerPosition);

            enemyTexture = Content.Load<Texture2D>("enemy");
            projectileTexture = Content.Load<Texture2D>("bullet");

            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            currentKeys = Keyboard.GetState();
            if (currentKeys.IsKeyDown(Keys.Escape)) Exit();
            if (!isPaused)
            {
                time += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (currentKeys.IsKeyDown(Keys.P) && previousKeys.IsKeyUp(Keys.P))
                {
                    isPaused = true;
                }
                player.Update(gameTime);
                if (currentKeys.IsKeyDown(Keys.Space) && previousKeys.IsKeyUp(Keys.Space))
                {
                    Shoot(player.Position, Vector2.UnitX);
                }
                UpdateEnemies(gameTime);
                UpdateProjectiles(gameTime);
                UpdateCollision();
            }
            else
            {
                if (currentKeys.IsKeyDown(Keys.P) && previousKeys.IsKeyUp(Keys.P))
                {
                    isPaused = false;
                }
            }
            previousKeys = currentKeys;
            base.Update(gameTime);
        }

        private void AddEnemy()
        {
            Animation enemyAnimation = new Animation();
            Vector2 position = new Vector2(VIRTUAL_WIDTH - 20, VIRTUAL_HEIGHT - 20);
            enemyAnimation.Initialize(enemyTexture, position, 32, 32, 6, 100, Color.White, 1f, true);

            Enemy enemy = new Enemy();
            enemy.Initialize(enemyAnimation, position);
            enemies.Add(enemy);
        }

        private void UpdateCollision()
        {
            // Use the Rectangle’s built-in intersect function to determine if two objects are overlapping
            Rectangle rectangle1;
            Rectangle rectangle2;
            // Only create the rectangle once for the player
            rectangle1 = new Rectangle((int)player.Position.X, (int)player.Position.Y, player.Width, player.Height);
            // Do the collision between the player and the enemies
            for (int i = 0; i < enemies.Count; i++)
            {
                rectangle2 = new Rectangle((int)enemies[i].Position.X, (int)enemies[i].Position.Y, enemies[i].Width, enemies[i].Height);

                if (rectangle1.Intersects(rectangle2))
                {
                    player.Health -= enemies[i].Damage;
                    Debug.WriteLine(player.Health);
                    enemies[i].Health = 0;
                    if (player.Health <= 0)
                    {
                        player.Active = false;
                    }
                }
            }
        }

        private void UpdateEnemies(GameTime gameTime)
        {
            if (time - previousSpawnTime > enemySpawnTime)
            {
                previousSpawnTime = time;
                AddEnemy();
            }
            if (!isPaused)
            {
                for (int i = 0; i < enemies.Count; i++)
                {
                    enemies[i].Update(gameTime);
                    if (!enemies[i].Active)
                    {
                        enemies.RemoveAt(i);
                    }
                }
            }
        }

        public void Shoot(Vector2 position, Vector2 direction)
        {
            Debug.WriteLine("BANG" + System.DateTime.Now.Millisecond);
            Projectile projectile = new Projectile();
            Animation projectileAnim = new Animation();
            projectileAnim.Initialize(projectileTexture, position, 16, 8, 6, 80, Color.White, 1f, true);
            projectile.Initialize(projectileAnim, position, direction);
            projectiles.Add(projectile);
        }

        private void UpdateProjectiles(GameTime gameTime)
        {
            if (!isPaused)
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

        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.SetRenderTarget(renderTarget);

            //Draw your stuff
            graphics.GraphicsDevice.Clear(Color.LightGray);
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            player.Draw(_spriteBatch);
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].Draw(_spriteBatch);
                //Debug.WriteLine(enemies[i].Position+ " "+enemies[i].Width+" "+enemies[i].Height );
            }
            for (int i = 0; i < projectiles.Count; i++)
            {
                projectiles[i].Draw(_spriteBatch);
                Debug.WriteLine(projectiles[i].Position+ " "+projectiles[i].Width+" "+projectiles[i].Height );
            }
            _spriteBatch.End();

            // clear to get black bars
            Rectangle dst = barPlace();
            graphics.GraphicsDevice.SetRenderTarget(null);
            graphics.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 1.0f, 0);

            // draw a quad to get the draw buffer to the back buffer
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(renderTarget, dst, Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private Rectangle barPlace()
        {
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
            return dst;
        }

    }
}
