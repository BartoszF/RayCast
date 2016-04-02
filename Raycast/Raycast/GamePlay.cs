using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Raycast
{
    public class GamePlay
    {
        int[,] map = new int[,]
        {
            {1,1,1,1,1,1,1,1,1,1,1,1,1 },
            {1,0,0,0,0,0,0,0,0,0,0,0,1 },
            {1,0,0,0,0,0,0,0,0,0,0,0,1 },
            {1,1,1,1,0,0,0,0,0,0,0,0,1 },
            {1,0,0,0,0,0,0,0,0,0,0,0,1 },
            {1,0,0,0,0,0,0,0,0,0,0,0,1 },
            {1,0,0,0,0,0,0,0,0,0,0,0,1 },
            {1,0,0,0,0,0,0,0,0,0,0,0,1 },
            {1,1,1,1,1,1,1,1,1,1,1,1,1 }
        };

        Texture2D wallTex;
        Texture2D whiteTex;

        public Vector2 playerPos = new Vector2(2, 2);

        float dirX = -1, dirY = 0;

        float planeX = 0, planeY = 0.66f;

        Color darker = new Color(140, 140, 140);

        Random rand;

        public GamePlay(GraphicsDevice gd)
        {
            this.wallTex = new Texture2D(gd,2, 2);
            wallTex.SetData<Color>(new Color[] { new Color(50, 50, 150), new Color(80, 80, 150), new Color(80, 80, 150), new Color(50, 50, 150) });

            whiteTex = new Texture2D(gd, 1, 1);
            whiteTex.SetData<Color>(new Color[] { new Color(255, 255, 255) });

            rand = new Random();
        }


        public void Update(GameTime gameTime)
        {
            KeyboardState ks = Keyboard.GetState();
            float rotSpeed = 2 * (float)gameTime.ElapsedGameTime.Milliseconds / 1000;
            if (ks.IsKeyDown(Keys.Left))
            {
                Rotate(rotSpeed);
            }
            if (ks.IsKeyDown(Keys.Right))
            {
                Rotate(-rotSpeed);
            }
            if (ks.IsKeyDown(Keys.Up)) move(3 * (float)gameTime.ElapsedGameTime.Milliseconds / 1000);
            if (ks.IsKeyDown(Keys.Down)) move(-3 * (float)gameTime.ElapsedGameTime.Milliseconds / 1000);
        }

        private void Rotate(float rotSpeed)
        {
            float oldDirX = dirX;
            dirX = dirX * (float)Math.Cos(rotSpeed) - dirY * (float)Math.Sin(rotSpeed);
            dirY = oldDirX * (float)Math.Sin(rotSpeed) + dirY * (float)Math.Cos(rotSpeed);
            float oldPlaneX = planeX;
            planeX = planeX * (float)Math.Cos(rotSpeed) - planeY * (float)Math.Sin(rotSpeed);
            planeY = oldPlaneX * (float)Math.Sin(rotSpeed) + planeY * (float)Math.Cos(rotSpeed);
        }

        public void Draw(SpriteBatch sb, int w, int h)
        {
            for (int x = 0; x < w; x++)
            {
                //calculate ray position and direction
                float cameraX = 2 * (x / (float)w) - 1; //x-coordinate in camera space
                float rayPosX = playerPos.X;
                float rayPosY = playerPos.Y;
                float rayDirX = dirX + planeX * cameraX;
                float rayDirY = dirY + planeY * cameraX;

                //which box of the map we're in
                int mapX = (int)rayPosX;
                int mapY = (int)rayPosY;

                //length of ray from current position to next x or y-side
                float sideDistX;
                float sideDistY;

                //length of ray from one x or y-side to next x or y-side
                float deltaDistX = (float)Math.Sqrt(1.0f + (rayDirY * rayDirY) / (rayDirX * rayDirX));
                float deltaDistY = (float)Math.Sqrt(1.0f + (rayDirX * rayDirX) / (rayDirY * rayDirY));
                float perpWallDist;

                //what direction to step in x or y-direction (either +1 or -1)
                int stepX;
                int stepY;

                int hit = 0; //was there a wall hit?
                int side = 0; //was a NS or a EW wall hit?

                //calculate step and initial sideDist
                if (rayDirX < 0)
                {
                    stepX = -1;
                    sideDistX = (rayPosX - mapX) * deltaDistX;
                }
                else
                {
                    stepX = 1;
                    sideDistX = (mapX + 1.0f - rayPosX) * deltaDistX;
                }
                if (rayDirY < 0)
                {
                    stepY = -1;
                    sideDistY = (rayPosY - mapY) * deltaDistY;
                }
                else
                {
                    stepY = 1;
                    sideDistY = (mapY + 1.0f - rayPosY) * deltaDistY;
                }

                while (hit == 0)
                {
                    //jump to next map square, OR in x-direction, OR in y-direction
                    if (sideDistX < sideDistY)
                    {
                        sideDistX += deltaDistX;
                        mapX += stepX;
                        side = 0;
                    }
                    else
                    {
                        sideDistY += deltaDistY;
                        mapY += stepY;
                        side = 1;
                    }
                    //Check if ray has hit a wall
                    if (map[mapX,mapY] > 0) hit = 1;
                }

                if (side == 0) perpWallDist = (mapX - rayPosX + (1 - stepX) / 2) / rayDirX;
                else perpWallDist = (mapY - rayPosY + (1 - stepY) / 2) / rayDirY;

                //Calculate height of line to draw on screen
                int lineHeight = (int)(h / perpWallDist);

                //calculate lowest and highest pixel to fill in current stripe
                int drawStart = -lineHeight / 2 + h / 2;
                if (drawStart < 0) drawStart = 0;
                int drawEnd = lineHeight / 2 + h / 2;
                if (drawEnd >= h) drawEnd = h - 1;

                /*Color color;
                switch (map[mapX,mapY])
                {
                    case 1: color = Color.Blue; break; //red
                    case 2: color = Color.Green; break; //green
                    case 3: color = Color.Red; break; //blue
                    case 4: color = Color.White; break; //white
                    default: color = Color.Yellow; break; //yellow
                }*/

                //calculate value of wallX
                float wallX; //where exactly the wall was hit
                if (side == 0) wallX = rayPosY + perpWallDist * rayDirY;
                else wallX = rayPosX + perpWallDist * rayDirX;
                wallX -= (float)Math.Floor((wallX));

                //x coordinate on the texture
                int texX = (int)(wallX * (float)wallTex.Width);
                if (side == 0 && rayDirX > 0) texX = wallTex.Width - texX - 1;
                if (side == 1 && rayDirY < 0) texX = wallTex.Width - texX - 1;

                //give x and y sides different brightness
                //if (side == 1) { color = new Color(color.R/2, color.G/2,color.B/2); }

                sb.Draw(wallTex, new Rectangle(x, drawStart, 1, drawEnd-drawStart),new Rectangle(texX,0,1,wallTex.Height), side==0?Color.White:darker);
            }
        }

        public void move(float dist)
        {
            if (map[(int)(playerPos.X + dirX * dist),(int)playerPos.Y] == 0) playerPos.X += dirX * dist;
            if (map[(int)playerPos.X,(int)(playerPos.Y + dirY * dist)] == 0) playerPos.Y += dirY * dist;
        }


    }
}
