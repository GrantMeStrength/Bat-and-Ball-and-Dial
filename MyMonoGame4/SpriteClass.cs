using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMonoGame4
{
    class SpriteClass
    {
 
        Texture2D texture;

        public bool isCircular
        {
            get;
            set;
        }
        public float X
        {
            get;
            set;
        }

        public float Y
        {
            get;
            set;
        }

        public float DX
        {
            get;
            set;
        }

        public float DY
        {
            get;
            set;
        }

        public SpriteClass (GraphicsDevice graphicsDevice, string textureName)
        {
            if (texture == null)
            {
                using (var stream = TitleContainer.OpenStream(textureName))
                {
                    texture = Texture2D.FromStream(graphicsDevice, stream);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
          Vector2 spritePosition = new Vector2(this.X - texture.Width / 2, this.Y - texture.Height / 2);
           Color tintColor = Color.White;
           spriteBatch.Draw(texture, spritePosition, tintColor);
        }

        public void DrawColor(SpriteBatch spriteBatch, int c)
        {
            // Special version of draw that uses tintColor to shade the blocks.

            Vector2 spritePosition = new Vector2(this.X - texture.Width / 2, this.Y - texture.Height / 2);
            Color tintColor = Color.Violet;

            if (c == 1) tintColor = Color.Red;
            if (c == 2) tintColor = Color.Orange;
            if (c == 3) tintColor = Color.Yellow;
            if (c == 4) tintColor = Color.Green;
            if (c == 5) tintColor = Color.Indigo;

            spriteBatch.Draw(texture, spritePosition, tintColor);
        }


        public bool Collision (SpriteClass otherSprite)
        {
            // Do some rudementary collision detection, based on the shape of the sprites
            // which we assume to be either rectangular or circular.

            if (this.isCircular && otherSprite.isCircular)
                return CircleAndCircleCollision(otherSprite);

            if (!this.isCircular && !otherSprite.isCircular)
                return RectangleCollision(otherSprite);

            if (!this.isCircular && otherSprite.isCircular)
                return RectangleAndCircleCollision(otherSprite);

            return false;
        }

        bool CircleAndCircleCollision(SpriteClass otherSprite)
        {
            float r1 = this.texture.Width / 2;
            float r2 = otherSprite.texture.Width / 2;

            if (this.X - r1 > otherSprite.X + r2) return false;
            if (this.Y - r1 > otherSprite.Y + r2) return false;
            if (this.X + r1 < otherSprite.X - r2) return false;
            if (this.Y + r1 < otherSprite.Y - r2) return false;

            return true;
        }
         bool RectangleCollision(SpriteClass otherSprite)
        {
            if (this.X + this.texture.Width/2 < otherSprite.X - otherSprite.texture.Width/2) return false;
            if (this.Y + this.texture.Height/2 < otherSprite.Y - otherSprite.texture.Height/2) return false;
            if (this.X - this.texture.Width / 2 > otherSprite.X + otherSprite.texture.Width) return false;
            if (this.Y - this.texture.Height / 2 > otherSprite.Y + otherSprite.texture.Height) return false;
            return true;
        }

         bool RectangleAndCircleCollision(SpriteClass otherSprite)
        {
            float r2 = otherSprite.texture.Width / 2;

            if (this.X + this.texture.Width/2 < otherSprite.X - r2) return false;
            if (this.Y + this.texture.Height/2 < otherSprite.Y - r2) return false;
            if (this.X - this.texture.Width / 2 > otherSprite.X + r2) return false;
            if (this.Y - this.texture.Height / 2 > otherSprite.Y + r2) return false;
            return true;
        }
    }
}
