using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace League_Snake
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Singed singed;
        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;
        Texture2D enemyTexture;
        Texture2D poisonTexture;
        Texture2D mushroomTexture;
        List<Teemo> enemies;
        List<Mushroom> traps;
        TimeSpan enemySpawnTime;
        TimeSpan previousSpawnTime;
        Random random;
        Poison poisonTrail;
        List<Poison> poisonClouds;
        List<Vector2> playerPositionList;

        int score;
        float playerMoveSpeed;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            singed = new Singed();

            playerMoveSpeed = 3.0f;
            score = 0;
            base.Initialize();
            enemies = new List<Teemo>();
            traps = new List<Mushroom>();
            poisonClouds = new List<Poison>();
            previousSpawnTime = TimeSpan.Zero;
            enemySpawnTime = TimeSpan.FromSeconds(3.0f);
            random = new Random();
            AddEnemy();
            playerPositionList = new List<Vector2>();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Vector2 playerPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + GraphicsDevice.Viewport.TitleSafeArea.Height / 2);
            singed.Initialize(Content.Load<Texture2D>("basic singed sprite 50x40"), playerPosition);
            enemyTexture = Content.Load<Texture2D>("basic teemo sprite");
            poisonTexture = Content.Load<Texture2D>("poison trail");
            mushroomTexture = Content.Load<Texture2D>("mushroom sprite");
            // TODO: use this.Content to load your game content here
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            previousKeyboardState = currentKeyboardState;

            currentKeyboardState = Keyboard.GetState();


            UpdateEnemies(gameTime);
            updatePoisonTrail(gameTime);
            UpdatePlayer(gameTime);
            UpdateShrooms(gameTime);
            UpdateCollision();
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].Draw(spriteBatch);
            }
            for (int k = 0; k < poisonClouds.Count; k++)
            {
                poisonClouds[k].Draw(spriteBatch);
            }
            for (int j = 0; j < traps.Count; j++)
            {
                traps[j].Draw(spriteBatch);
            }
            singed.Draw(spriteBatch);

            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void UpdatePlayer(GameTime gameTime)
        {
            // Use the Keyboard / Dpad
        
            if (singed.Health != 0)
            {
                int dir = getDirection();
                if (dir == 0)
                {
                    singed.Position.X += playerMoveSpeed;
                    if (currentKeyboardState.IsKeyDown(Keys.Up) & currentKeyboardState.IsKeyUp(Keys.Right) & currentKeyboardState.IsKeyUp(Keys.Left) & currentKeyboardState.IsKeyUp(Keys.Down))
                    {
                        singed.Direction = 1;
                    }
                    if (currentKeyboardState.IsKeyDown(Keys.Down) & currentKeyboardState.IsKeyUp(Keys.Right) & currentKeyboardState.IsKeyUp(Keys.Left) & currentKeyboardState.IsKeyUp(Keys.Up))
                    {
                        singed.Direction = 3;
                    }
                }
                else if (dir == 1)
                {
                    singed.Position.Y -= playerMoveSpeed;


                    if (currentKeyboardState.IsKeyDown(Keys.Left) & currentKeyboardState.IsKeyUp(Keys.Right) & currentKeyboardState.IsKeyUp(Keys.Up) & currentKeyboardState.IsKeyUp(Keys.Down))
                    {
                        singed.Direction = 2;
                    }
                    if (currentKeyboardState.IsKeyDown(Keys.Right) & currentKeyboardState.IsKeyUp(Keys.Left) & currentKeyboardState.IsKeyUp(Keys.Up) & currentKeyboardState.IsKeyUp(Keys.Down))
                    {
                        singed.Direction = 0;
                    }
                }

                else if (dir == 2)
                {
                    singed.Position.X -= playerMoveSpeed;
                    if (currentKeyboardState.IsKeyDown(Keys.Up) & currentKeyboardState.IsKeyUp(Keys.Right) & currentKeyboardState.IsKeyUp(Keys.Left) & currentKeyboardState.IsKeyUp(Keys.Down))
                    {
                        singed.Direction = 1;
                    }
                    if (currentKeyboardState.IsKeyDown(Keys.Down) & currentKeyboardState.IsKeyUp(Keys.Right) & currentKeyboardState.IsKeyUp(Keys.Left) & currentKeyboardState.IsKeyUp(Keys.Up))
                    {
                        singed.Direction = 3;
                    }
                }

                else
                {
                    singed.Position.Y += playerMoveSpeed;


                    if (currentKeyboardState.IsKeyDown(Keys.Left) & currentKeyboardState.IsKeyUp(Keys.Right) & currentKeyboardState.IsKeyUp(Keys.Up) & currentKeyboardState.IsKeyUp(Keys.Down))
                    {
                        singed.Direction = 2;
                    }
                    if (currentKeyboardState.IsKeyDown(Keys.Right) & currentKeyboardState.IsKeyUp(Keys.Up) & currentKeyboardState.IsKeyUp(Keys.Left) & currentKeyboardState.IsKeyUp(Keys.Down))
                    {
                        singed.Direction = 0;
                    }
                }




                // Make sure that the player does not go out of bounds
                singed.Position.X = MathHelper.Clamp(singed.Position.X, 0, GraphicsDevice.Viewport.Width - singed.Width);
                singed.Position.Y = MathHelper.Clamp(singed.Position.Y, 0, GraphicsDevice.Viewport.Height - singed.Height);
                savePlayerPosition();
            }
            else
            {
                this.Exit();
            }
        }

        private void AddEnemy()
        {
            Vector2 position = new Vector2(random.Next(0, GraphicsDevice.Viewport.Width - 10), random.Next(0, GraphicsDevice.Viewport.Height - 10));
            Teemo enemy = new Teemo();
            enemy.Initialize(enemyTexture, position);
            enemies.Add(enemy);
        }

        private void UpdateEnemies(GameTime gameTime)
        {
            // Spawn a new enemy enemy every 1.5 seconds
            if (gameTime.TotalGameTime - previousSpawnTime > enemySpawnTime)
            {
                previousSpawnTime = gameTime.TotalGameTime;

                // Add an Enemy
                AddEnemy();
            }

            // Update the Enemies
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                enemies[i].Update();
                if (enemies[i].duration == 99)
                {
                    AddMushroom(enemies[i].Position);
                    enemies[i].shroomCount++;
                }
                if (enemies[i].Active == false)
                {
                    traps.RemoveRange(0, enemies[i].shroomCount);
                    enemies.RemoveAt(i);
                    score++;
                    addTrail();
                }

            }
        }

        private void UpdateShrooms(GameTime gameTime)
        {
            for (int i = 0; i < traps.Count; i++)
            {
                traps[i].Update(gameTime);
                if (traps[i].Active == false)
                {
                    traps.RemoveAt(i);
                    singed.Health--;
                }
                
            }
        }
        private void AddMushroom(Vector2 tPosition)
        {
           
            Mushroom shroom = new Mushroom();
            shroom.Initialize(mushroomTexture, getNewShroomPosition(tPosition));
            traps.Add(shroom);
        }

        private Vector2 getNewShroomPosition(Vector2 tPosition)
        {
            Vector2[] positions = new Vector2[4];
            //TOP LEFT QUADRANT
            positions[0] = new Vector2(random.Next((int)tPosition.X - 75, (int)tPosition.X - mushroomTexture.Width), random.Next((int)tPosition.Y - 75, (int)tPosition.Y - mushroomTexture.Height));
            //BOTTOM LEFT QUADRANT
            positions[1] = new Vector2(random.Next((int)tPosition.X - 75, (int)tPosition.X - mushroomTexture.Width), random.Next((int)tPosition.Y + mushroomTexture.Height, (int)tPosition.Y + 75));
            //RIGHT TOP QUADRANT
            positions[2] = new Vector2(random.Next((int)tPosition.X + mushroomTexture.Width, (int)tPosition.X + 75), random.Next((int)tPosition.Y - 75, (int)tPosition.Y - mushroomTexture.Height));
            //RIGHT BOTTOM QUADRANT
            positions[3] = new Vector2(random.Next((int)tPosition.X + mushroomTexture.Width, (int)tPosition.X + 75), random.Next((int)tPosition.Y + mushroomTexture.Height, (int)tPosition.Y + 75));
            Vector2 position = positions[random.Next(0, 3)];
            return position;
        }
        private void updatePoisonTrail(GameTime gameTime)
        {
            Vector2 centerOfPlayer = new Vector2(singed.Position.X + singed.Width / 2, singed.Position.Y + singed.Height / 2);
            if (playerPositionList.Count > 0 && poisonClouds.Count > 0)
                poisonClouds[0].Position = new Vector2(centerOfPlayer.X-poisonTexture.Width/2, centerOfPlayer.Y-poisonTexture.Height/2);// new Vector2(singed.Position.X - 10, singed.Position.Y + singed.Height / 3);
            for (int k = 0; k < poisonClouds.Count; k++)
            {
                if (k > 0)
                {
                    Vector2 nextPosition = new Vector2(playerPositionList[playerPositionList.Count - k*3].X-poisonTexture.Width/2,playerPositionList[playerPositionList.Count - k*3].Y-poisonTexture.Height/2);//new Vector2(playerPositionList[playerPositionList.Count - k].X-10, playerPositionList[playerPositionList.Count - k].Y+singed.Height/3);
                    poisonClouds[k].Position = nextPosition;
                }
            }
        }

        private void UpdateCollision()
        {
            Rectangle rectangle1;
            Rectangle rectangle2;
            Rectangle rectangle3;
            rectangle1 = new Rectangle((int)singed.Position.X, (int)singed.Position.Y, singed.Width, singed.Height);

            for (int i = 0; i < enemies.Count; i++)
            {
                rectangle2 = new Rectangle((int)enemies[i].Position.X, (int)enemies[i].Position.Y, enemies[i].Width, enemies[i].Height);
                if (rectangle1.Intersects(rectangle2))
                {
                    enemies[i].Health = 0;
                }
                for (int k = 0; k < traps.Count; k++)
                {
                    rectangle3 = new Rectangle((int)traps[k].Position.X, (int)traps[k].Position.Y, traps[k].Width, traps[k].Height);
                    if (rectangle3.Intersects(rectangle2))
                    {
                        traps[k].Health = 0;
                        AddMushroom(getNewShroomPosition(enemies[i].Position));
                    }
                }
            }
            for (int k = 0; k < traps.Count; k++)
            {
                rectangle3 = new Rectangle((int)traps[k].Position.X, (int)traps[k].Position.Y, traps[k].Width, traps[k].Height);
                if (rectangle3.Intersects(rectangle1))
                {
                    traps.RemoveAt(k);
                    singed.Health--;
                    for (int j = 0; j < enemies.Count; j++)
                    {
                        if (enemies[j].shroomCount > 0)
                        {
                            enemies[j].shroomCount--;
                            break;
                        }
                        
                    }
                }
            }
        }

        private int getDirection()
        {
            return singed.Direction;
        }

        private void addTrail()
        {
            int snakeLength = score;
           
            poisonTrail = new Poison();
            Vector2 position;
            if (poisonClouds.Count == 0)
                position = new Vector2(singed.Position.X - poisonTexture.Width / 2, singed.Position.Y - poisonTexture.Height / 2);
            else
            {
                position = new Vector2(playerPositionList[playerPositionList.Count - score*3].X - poisonTexture.Width / 2, playerPositionList[playerPositionList.Count - score*3].Y - poisonTexture.Height / 2);
            }
            poisonTrail.Initialize(poisonTexture, position);
            poisonClouds.Add(poisonTrail);

        }

        private void savePlayerPosition()
        {
            Vector2 centerOfPlayer = new Vector2(singed.Position.X + singed.Width / 2, singed.Position.Y + singed.Height / 2);
            if (playerPositionList.Count <= 300) playerPositionList.Add(centerOfPlayer);
            else
            {
                playerPositionList.RemoveAt(0);
                playerPositionList.Add(centerOfPlayer);
            }
        }
    }
}
