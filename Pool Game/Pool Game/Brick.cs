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
        public int brickType;
        public float mass = 900;

        public Brick(float xPos, float yPos, float height,float width, int brickType, bool isAlive)
        {
            this.xPos = xPos;
            this.yPos = yPos;
            this.height = height;//radius in form1
            this.width = width;
            this.brickType = brickType;
            this.isAlive = isAlive;
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
