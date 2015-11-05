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
        public bool checkBallCollision(Ball ball)
        {

            xDist = ball.getX() - xPos;//dx
            yDist = ball.getY() - yPos;//dy

            float radDist = radius + ball.getRadius();

            if (radDist * radDist >= (xDist * xDist + yDist * yDist) && isAlive == true && ball.inPlay == true) //check if distance is bigger than the balls touching range
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void calculateBrickCollision(Ball ball)//this should not run on next frame, or balls will return. 
        {
            double collisionTangent = Math.Atan2((double)yDist, (double)xDist); //returns the angle of the tangent of the vector which is the collision x and y distance.
            double sin = Math.Sin(collisionTangent);
            double cos = Math.Cos(collisionTangent);
            //rotate ball 0 pos
            double B0x = 0;//relative x & y pos set
            double B0y = 0;
            //set ball 1 pos relative to ball 0, aka distance.
            double B1x = xDist * cos + yDist * sin;//RELATIVE TO BALL 0!!!
            double B1y = yDist * cos - xDist * sin;
            //rotate ball 0 velocity
            double V0x = 0;
            double V0y = 0;
            //rotate ball 1 velocity
            double V1x = ball.xSpeed * cos + ball.ySpeed * sin;
            double V1y = ball.ySpeed * cos - ball.xSpeed * sin;

            //collision reaction ELASTISK LIGNING I BOKA?, tror denne gjør at de ikke setter seg fast, må plusse på noe ekstra? eller ikke siden det er vel.
            double vxtotal = V0x - V1x;
            V0x = ((mass - ball.getMass()) * V0x + 2 * ball.getMass() * V1x) / (mass + ball.getMass());//new velocity x ball 1
            V1x = vxtotal + V0x; //new velocity x ball 2
            //update position, THIS ONE IS RELATIVE TO MID BALL 0 and BALL 1
            B0x += V0x;
            B1x += V1x;
            //rot pos back? SET NEW POSITION. BALLS SHOULD OVERLAP AFTER THIS.
            double B0newPosx = B0x * cos - B0y * sin;
            double B0newPosy = B0y * cos + B0x * sin;

            double B1newPosx = B1x * cos - B1y * sin;
            double B1newPosy = B1y * cos + B1x * sin;

            //rot vel back?
            double B0newVelx = V0x * cos - V0y * sin;
            double B0newVely = V0y * cos + V0x * sin;

            double B1newVelx = V1x * cos - V1y * sin;
            double B1newVely = V1y * cos + V1x * sin;

            //update pos
            ball.xPos = xPos + (float)B1newPosx;//is this just to set it out of the other balls radius?
            ball.yPos = yPos + (float)B1newPosy;
            //xPos = xPos + (float)B0newPosx;//these 4 new positions will be a little "bigger" than when they entered. this is so that they wont stick. also, they point slightly away from each other.
            //yPos = yPos + (float)B0newPosy;

            //update vel - I WANT THEM TO HAVE PERMANENT SPEEDS if not, I can rearrange the code again.
            //xSpeed = (float)B0newVelx > 0 ? 2 : -2;
            //xSpeed = (float)B0newVelx;
            //ySpeed = (float)B0newVely > 0 ? 2 : -2;
            //ySpeed = (float)B0newVely;
            //ball.setXspeed((float)B1newVelx > 0 ? 2 : -2);
            ball.setXspeed((float)B1newVelx);
            //ball.setYspeed((float)B1newVely > 0 ? 2 : -2);
            ball.setYspeed((float)B1newVely);
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
        

    }
}
