using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace League_Snake
{
    class Poison
    {
        public Texture2D poisonTexture;
        public Vector2 Position;
        public int Direction;
        public bool Active;
        public int Health;
        
        public int Width
        {
            get { return poisonTexture.Width; }
        }
        public int Height
        {
            get { return poisonTexture.Height; }
        }

        public void Initialize(Texture2D texture, Vector2 position)
        {
            poisonTexture = texture;
            Position = position;
            Active = true;
            Health = 1;
            Direction = 0;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(poisonTexture, Position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}
