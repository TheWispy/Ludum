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
        Vector2 START_POS = new Vector2(20, VIRTUAL_HEIGHT - 20);

        //Enemies
        Texture2D enemyTexture;
        float enemySpawnTime;
        float previousSpawnTime;

        //Projectiles
        Texture2D projectileTexture;
        TimeSpan projectileSpawnTime;
        TimeSpan previousLaserSpawnTime;
        Random random;

        //Levels
        Level currentLevel;
        public int levelCount;

        //Menu
        Menu MenuManager;
        Texture2D healthContainer;
        Texture2D healthSegement;
        Texture2D powerSegment;
        Texture2D keyPadTex;
        bool KeyPad;

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
            levelCount = 0;
            PresentationParameters pp = graphics.GraphicsDevice.PresentationParameters;
            renderTarget = new RenderTarget2D(graphics.GraphicsDevice, VIRTUAL_WIDTH, VIRTUAL_HEIGHT, false, SurfaceFormat.Color,
                DepthFormat.None, pp.MultiSampleCount, RenderTargetUsage.DiscardContents);

            player = new Player();
            currentLevel = new Level();
            currentLevel.Initialize(levelCount);
            previousSpawnTime = 0f;
            enemySpawnTime = 2f;
            const float SECONDS_IN_MINUTE = 60f;
            const float RATE_OF_FIRE = 200f;
            projectileSpawnTime = TimeSpan.FromSeconds(SECONDS_IN_MINUTE / RATE_OF_FIRE);
            previousLaserSpawnTime = TimeSpan.Zero;

            MenuManager = new Menu();
            

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
            Vector2 playerPosition = START_POS;
            List<Animation> playerMoveSet = new List<Animation>();

            Texture2D idleTexture = Content.Load<Texture2D>("player_idle");
            Animation idle = new Animation();
            idle.Initialize(idleTexture, playerPosition, 32, 32, 4, 150, Color.White, 1f, true);
            playerMoveSet.Add(idle);

            Texture2D crouchTexture = Content.Load<Texture2D>("player_crouch");
            Animation crouch = new Animation();
            crouch.Initialize(crouchTexture, playerPosition, 32, 21, 4, 150, Color.White, 1f, true);
            playerMoveSet.Add(crouch);
            player.Initialize(playerMoveSet, playerPosition);

            enemyTexture = Content.Load<Texture2D>("enemy");
            projectileTexture = Content.Load<Texture2D>("bullet");

            healthContainer = Content.Load<Texture2D>("container");
            healthSegement = Content.Load<Texture2D>("health5");
            powerSegment = Content.Load<Texture2D>("power5");
            keyPadTex = Content.Load<Texture2D>("numpad");
            MenuManager.Initialize(healthContainer, healthSegement, powerSegment);

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
                player.Update(gameTime, time);
                if (currentKeys.IsKeyDown(Keys.Space) && previousKeys.IsKeyUp(Keys.Space) && player.Active)
                {
                    Shoot(player.Position, Vector2.UnitX);
                }
                UpdateEnemies(gameTime);
                currentLevel.Update(gameTime);
                UpdateCollision();
                if (currentLevel.isComplete)
                {
                    currentLevel = newLevel();
                }
            }
            else if (currentKeys.IsKeyDown(Keys.P) && previousKeys.IsKeyUp(Keys.P))
            {
                isPaused = false;
            }
            previousKeys = currentKeys;
            if (!KeyPad)
            {
                if (currentKeys.IsKeyDown(Keys.R) && previousKeys.IsKeyUp(Keys.R))
                {
                    KeyPad = true;
                }
            }
            else if (currentKeys.IsKeyDown(Keys.R) && previousKeys.IsKeyUp(Keys.R))
            {
                KeyPad = false;
            }
            MenuManager.Update(player.Health, player.Power, KeyPad);
            base.Update(gameTime);
        }

        private void AddEnemy()
        {
            Animation enemyAnimation = new Animation();
            Vector2 position = new Vector2(VIRTUAL_WIDTH - 20, VIRTUAL_HEIGHT - 20);
            enemyAnimation.Initialize(enemyTexture, position, 32, 32, 6, 100, Color.White, 1f, true);

            Enemy enemy = new Enemy();
            enemy.Initialize(enemyAnimation, position);
            currentLevel.enemies.Add(enemy);
        }
        private void UpdateCollision()
        {
            // Use the Rectangle’s built-in intersect function to determine if two objects are overlapping
            Rectangle rectangle1;
            Rectangle rectangle2;
            Rectangle rectangle3;
            // Only create the rectangle once for the player
            rectangle1 = new Rectangle((int)player.Position.X, (int)player.Position.Y, player.Width, player.Height);
            // Do the collision between the player and the enemies
            for (int i = 0; i < currentLevel.enemies.Count; i++)
            {
                rectangle2 = new Rectangle((int)currentLevel.enemies[i].Position.X, (int)currentLevel.enemies[i].Position.Y, currentLevel.enemies[i].Width, currentLevel.enemies[i].Height);
                for (int j = 0; j < currentLevel.projectiles.Count; j++)
                {
                    rectangle3 = new Rectangle((int)currentLevel.projectiles[j].Position.X, (int)currentLevel.projectiles[j].Position.Y,
                        currentLevel.projectiles[j].Width, currentLevel.projectiles[j].Height);
                    if (rectangle2.Intersects(rectangle3))
                    {
                        currentLevel.enemies[i].Health -= currentLevel.projectiles[j].Damage;
                        currentLevel.projectiles[j].Active = false;
                        if (currentLevel.enemies[i].Health <= 0)
                        {
                            currentLevel.enemies[i].Active = false;
                        }
                    }
                }
                if (rectangle1.Intersects(rectangle2))
                {
                    player.Health -= currentLevel.enemies[i].Damage;
                    Debug.WriteLine(player.Health);
                    currentLevel.enemies[i].Health = 0;
                    if (player.Health <= 0)
                    {
                        player.Active = false;
                    }
                }
            }
        }

        private void UpdateEnemies(GameTime gameTime)
        {
            if ((time - previousSpawnTime > enemySpawnTime) && !currentLevel.isComplete)
            {
                previousSpawnTime = time;
                AddEnemy();
                currentLevel.enemyCount++;
            }
            for (int i = 0; i < currentLevel.enemies.Count; i++)
            {
                currentLevel.enemies[i].Update(gameTime);
                if (!currentLevel.enemies[i].Active)
                {
                    currentLevel.enemies.RemoveAt(i);
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
            currentLevel.projectiles.Add(projectile);
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
            currentLevel.Draw(_spriteBatch);
            MenuManager.Draw(_spriteBatch);
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

        private Level newLevel()
        {
            levelCount++;
            player.Position = START_POS;
            Level level = new Level();
            level.Initialize(levelCount);
            return level;
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
