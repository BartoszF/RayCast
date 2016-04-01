using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Raycast
{
    public class Ray
    {
        public float x, y;
        public float height;
        public float distance;
        public float length2;
        public bool noWall;
        public int shading;
        public float offset;

        public Ray()
        {
            this.noWall = true;
            this.length2 = float.PositiveInfinity;
        }

        public Ray(bool noWall)
        {
            this.noWall = noWall;
        }

        public Ray(float x, float y, float height, float distance)
        {
            this.x = x;
            this.y = y;
            this.height = height;
            this.distance = distance;
        }
    }

    public class GamePlay
    {
        int[,] map = new int[,]
{
            {1,1,1,1,1,1,1,1 },
            {1,0,0,0,0,0,0,1 },
            {1,0,0,0,0,0,0,1 },
            {1,1,1,1,0,0,0,1 },
            {1,0,0,0,0,0,0,1 },
            {1,1,1,1,1,1,1,1 }
};

        public Vector2 playerPos;
        public float playerRot;

        public float focalLength = 0.8f;

        float CIRCLE = (float)(Math.PI * 2);

        List<Ray> ray(Ray origin, float cos, float sin, float range)
        {
            List<Ray> l = new List<Ray>();
            Ray stepX = step(sin, cos, origin.x, origin.y, false);
            Ray stepY = step(cos, sin, origin.y, origin.x, true);
            Ray nextStep = stepX.length2 < stepY.length2
              ? inspect(stepX, 1, 0, origin.distance, stepX.y,cos,sin)
              : inspect(stepY, 0, 1, origin.distance, stepY.x,cos,sin);
            if (nextStep.distance > range)
            {
                l.Add(origin);
                return l;
            }
            else
            {
                l.Add(origin);
                l.Concat(ray(nextStep,cos,sin,range));
                return l;
            }
        }

        public Ray step(float rise, float run, float x, float y, bool inverted)
        {
            if (run == 0) return new Ray();
            float dx = run > 0 ? (float)Math.Floor(x + 1) - x : (float)Math.Ceiling(x - 1) - x;
            float dy = dx * (rise / run);

            Ray ret = new Ray();
            ret.x = inverted ? (y + dy) : (x + dx);
            ret.y = inverted ? (x + dx) : (y + dy);
            ret.length2 = dx * dx + dy * dy;

            return ret;
        }

        public Ray inspect(Ray step, int shiftX, int shiftY, float distance, float offset, float cos, float sin)
        {
            var dx = cos < 0 ? shiftX : 0;
            var dy = sin < 0 ? shiftY : 0;
            step.height = map[(int)(step.x - dx), (int)(step.y - dy)];
            step.distance = distance + (float)Math.Sqrt(step.length2);
            if (shiftX == 1) step.shading = cos < 0 ? 2 : 0;
            else step.shading = sin < 0 ? 2 : 1;
            step.offset = offset - (float)Math.Floor(offset);
            return step;
        }

        public List<Ray> cast (Vector2 point, float angle, float range)
        {
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);

            return ray(new Ray(point.X, point.Y, 0, 0), cos, sin, range);
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.Left)) rotate(-(float)Math.PI * gameTime.ElapsedGameTime.Milliseconds / 1000);
            if (ks.IsKeyDown(Keys.Right)) rotate(-(float)Math.PI * gameTime.ElapsedGameTime.Milliseconds / 1000);
            if (ks.IsKeyDown(Keys.Up)) move(3 * gameTime.ElapsedGameTime.Milliseconds / 1000);
            if (ks.IsKeyDown(Keys.Down)) move(-3 * gameTime.ElapsedGameTime.Milliseconds / 1000);
        }

        public void Draw(SpriteBatch sb)
        {

        }

        public void rotate(float angle)
        {
           this.playerRot = (this.playerRot + angle + CIRCLE) % (CIRCLE);
        }

        public void move(float dist)
        {
            float dx = (float)(Math.Cos(this.playerRot) * dist);
            float dy = (float)(Math.Sin(this.playerRot) * dist);
            if (map[(int)(playerPos.X + dx), (int)playerPos.Y] <= 0) playerPos.X += dx;
            if (map[(int)playerPos.X, (int)(playerPos.Y + dy)] <= 0) playerPos.Y += dy;
        }


    }
}
