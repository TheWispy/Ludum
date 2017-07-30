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
        public int Difficulty;

        //Menu
        Menu MenuManager;
        Texture2D healthContainer;
        Texture2D healthSegement;
        Texture2D powerSegment;
        Texture2D keyPadTex;
        public bool KeyPad;

        //Keypad
        public List<int> codeBox;
        public List<int> currentCode;
        public List<int> previousCode;
        public KeyboardState currentPad;
        public KeyboardState previousPad;

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
            random = new Random();
            levelCount = 0;
            PresentationParameters pp = graphics.GraphicsDevice.PresentationParameters;
            renderTarget = new RenderTarget2D(graphics.GraphicsDevice, VIRTUAL_WIDTH, VIRTUAL_HEIGHT, false, SurfaceFormat.Color,
                DepthFormat.None, pp.MultiSampleCount, RenderTargetUsage.DiscardContents);
            previousPad = Keyboard.GetState();
            player = new Player();
            currentLevel = new Level();
            currentLevel.Initialize(levelCount, (int)Math.Round(Difficulty / 1.34));
            previousSpawnTime = 0f;
            enemySpawnTime = 2f;
            const float SECONDS_IN_MINUTE = 60f;
            const float RATE_OF_FIRE = 200f;
            projectileSpawnTime = TimeSpan.FromSeconds(SECONDS_IN_MINUTE / RATE_OF_FIRE);
            previousLaserSpawnTime = TimeSpan.Zero;
            Difficulty = 5;
            MenuManager = new Menu();
            codeBox = new List<int>();
            previousCode = GetKeyCode();
            currentCode = GetKeyCode();
            while (previousCode.Equals(currentCode))
            {
                currentCode = GetKeyCode();
            }
            
            KeyPad = false;
            currentPad = Keyboard.GetState();

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

            List<Texture2D> numberTexture = new List<Texture2D>();

            MenuManager.Initialize(healthContainer, healthSegement, powerSegment, keyPadTex);
            

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
            if (KeyPad)
            {
                if (currentKeys.IsKeyDown(Keys.R) && previousKeys.IsKeyUp(Keys.R))
                {
                    KeyPad = false;

                }
                UpdateKeyPad();
            }
            else if (!KeyPad)
            {
                if (currentKeys.IsKeyDown(Keys.R) && previousKeys.IsKeyUp(Keys.R))
                {
                    KeyPad = true;
                    for (int i = 0; i < currentCode.Count; i++)
                    {
                        Debug.Write(currentCode[i]);
                    }
                    Debug.WriteLine(" ");

                    UpdateKeyPad();
                }
            }
            
            previousKeys = currentKeys;
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
            }
            for (int i = 0; i < currentLevel.enemies.Count; i++)
            {
                currentLevel.enemies[i].Update(gameTime);
                if (!currentLevel.enemies[i].Active)
                {
                    currentLevel.enemies.RemoveAt(i);
                    currentLevel.killCount++;
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
            Difficulty = Difficulty + random.Next(0, 2);
            level.Initialize(levelCount, (int)Math.Round(Difficulty/1.34));
            Debug.WriteLine("Difficulty: " + Difficulty + "  N: " + levelCount + "  Enemies:" + (int)Math.Round(Difficulty / 1.34));
            currentCode = GetKeyCode();
            codeBox.Clear();
            return level;
        }

        private void UpdateKeyPad()
        {
            if (KeyPad)
            {
                currentPad = Keyboard.GetState();
                if (SequenceEqual(currentCode, previousCode))
                {
                    currentCode = GetKeyCode();
                    Debug.WriteLine(currentCode[0] + " " + currentCode[1] + " " + currentCode[2] + " " + currentCode[3]);
                }
                if (SequenceEqual(codeBox, currentCode))
                {
                    player.Power = 100;
                    KeyPad = false;
                    currentCode = GetKeyCode();
                    codeBox.Clear();
                }
                else
                {   
                    int correctKey = currentCode[codeBox.Count];
                    int input = -1;
                    if ((currentPad.IsKeyDown(Keys.D0) || currentPad.IsKeyDown(Keys.NumPad0)) && (previousPad.IsKeyUp(Keys.D0) || previousPad.IsKeyUp(Keys.NumPad0)))
                    {
                        input = 0;
                    }
                    else if ((currentPad.IsKeyDown(Keys.D1) || currentPad.IsKeyDown(Keys.NumPad1)) && (previousPad.IsKeyUp(Keys.D1) || previousPad.IsKeyUp(Keys.NumPad1)))
                    {
                        input = 1;
                    }
                    else if ((currentPad.IsKeyDown(Keys.D2) || currentPad.IsKeyDown(Keys.NumPad2)) && (previousPad.IsKeyUp(Keys.D2) || previousPad.IsKeyUp(Keys.NumPad2)))
                    {
                        input = 2;
                    }
                    else if ((currentPad.IsKeyDown(Keys.D3) || currentPad.IsKeyDown(Keys.NumPad3)) && (previousPad.IsKeyUp(Keys.D3) || previousPad.IsKeyUp(Keys.NumPad3)))
                    {
                        input = 3;
                    }
                    else if ((currentPad.IsKeyDown(Keys.D4) || currentPad.IsKeyDown(Keys.NumPad4)) && (previousPad.IsKeyUp(Keys.D4) || previousPad.IsKeyUp(Keys.NumPad4)))
                    {
                        input = 4;
                    }
                    else if ((currentPad.IsKeyDown(Keys.D5) || currentPad.IsKeyDown(Keys.NumPad5)) && (previousPad.IsKeyUp(Keys.D5) || previousPad.IsKeyUp(Keys.NumPad5)))
                    {
                        input = 5;
                    }
                    else if ((currentPad.IsKeyDown(Keys.D6) || currentPad.IsKeyDown(Keys.NumPad6)) && (previousPad.IsKeyUp(Keys.D6) || previousPad.IsKeyUp(Keys.NumPad6)))
                    {
                        input = 6;
                    }
                    else if ((currentPad.IsKeyDown(Keys.D7) || currentPad.IsKeyDown(Keys.NumPad7)) && (previousPad.IsKeyUp(Keys.D7) || previousPad.IsKeyUp(Keys.NumPad7)))
                    {
                        input = 7;
                    }
                    else if ((currentPad.IsKeyDown(Keys.D8) || currentPad.IsKeyDown(Keys.NumPad8)) && (previousPad.IsKeyUp(Keys.D8) || previousPad.IsKeyUp(Keys.NumPad8)))
                    {
                        input = 8;
                    }
                    else if ((currentPad.IsKeyDown(Keys.D9) || currentPad.IsKeyDown(Keys.NumPad9)) && (previousPad.IsKeyUp(Keys.D9) || previousPad.IsKeyUp(Keys.NumPad9)))
                    {
                        input = 9;
                    }
                    if (input == -1 || currentPad.Equals(previousPad))
                    {
                        previousPad = currentPad;
                        return;
                    }
                    previousPad = currentPad;
                    Debug.WriteLine("input "+input);
                    codeBox.Add(input);
                    if (input != correctKey)
                    {
                        codeBox.Clear();
                        Debug.Write(input + "IS WRONG, Correct key was "+correctKey);
                    }
                    if (input == correctKey)
                    {
                        Debug.WriteLine("CORRECT");
                    }
                }
            }
        }

        private bool SequenceEqual(List<int> list1, List<int> list2)
        {
            if(list1.Count == 0 || list2.Count == 0 || list1.Count != list2.Count)
            {
                return false;
            }
            for (int i = 0; i < list1.Count; i++)
            {
                if (list1[i] != list2[i])
                {
                    return false;
                }
            }
            return true;
        }

        private List<int> GetKeyCode()
        {
            List<int> list = new List<int>();
            for (int i = 0; i < Difficulty; i++)
            {
                list.Add(random.Next(0, 9));
            }
            return list;
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
