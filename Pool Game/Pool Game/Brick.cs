using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pool_Game
{
    class Brick
    {
        private float xPos, yPos, height, width;
        public bool isAlive;
        public float top, bot, left, right;
        public int brickType;
        public float xDist, yDist;
        public float radius;
        public float mass = 900;

        public Brick(float xPos, float yPos, float height,float width, int brickType, bool isAlive)
        {
            this.xPos = xPos;
            this.yPos = yPos;
            this.height = height;
            this.width = width;
            radius = height / width;
            this.brickType = brickType;
            this.isAlive = isAlive;

            top = yPos - height / 2;
            bot = yPos + height / 2;
            left = xPos - width / 2;
            right = xPos + width / 2;
            
        }
        

        public float getX()
        {
            return xPos;
        }
        public float getY()
        {
            return yPos;
        }
        public float getHeight()
        {
            return height;
        }
        public float getWidth()
        {
            return width;
        }
        public float getMass()
        {
            return mass;
        }
        

    }
}
