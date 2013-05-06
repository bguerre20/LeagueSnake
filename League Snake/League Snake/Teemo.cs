using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace League_Snake
{
    class Teemo
    {
        public Vector2 Position;
        public bool Active;
        public int Health;
        public int shroomCount;
        Texture2D enemyTexture;
        public int Value;
        public int duration;
        public int Width
        {
            get { return enemyTexture.Width; }
        }
        public int Height
        {
            get { return enemyTexture.Height; }
        }

        public void Initialize(Texture2D texture, Vector2 position)
        {
            enemyTexture = texture;
            Position = position;
            duration = 0;
            Active = true;
            Health = 1;
            Value = 1;
        }

        public void Update()
        {
            if (duration <= 100)
            {
                duration++;
            }
            else
            {
                duration = 1;
            }
            if (Health <= 0)
            {
                Active = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(enemyTexture, Position, Color.White);
        }
    }
}
