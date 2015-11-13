using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pool_Game
{
    class Ball
    {
        public float xPos, yPos;
        public float xSpeed, ySpeed;
        public float lastxSpeed = 0, lastySpeed = 0;
        private float radius;
        private float mass = 1F;
        private float xDist, yDist;
        public bool inPlay = false;
        private float ForcePositionY = 0;
        private float accelerationY;
        
        public Ball(float x,float y,float xS,float yS, float r, bool inPlay, float acceleration)
        {
            xPos = x;
            yPos = y;
            xSpeed = xS;
            ySpeed = yS;
            radius = r;
            this.inPlay = inPlay;//check if ball is in use
            accelerationY = acceleration;
        }
        
        //collision with wall and speeds
        public void UpdateVars(float TopWall, float BotWall,float LeftWall, float RightWall, float padPosy,float gravity)//updates ball's position 
        {
            accelerationY += gravity;//acceleration Yvelocity += gravity constant
            ForcePositionY += accelerationY;//ball position Y += acceleration yVelocity
            accelerationY = ForcePositionY / -mass; //moving according to mass.

            ySpeed -=  accelerationY; //adding acceleration to movement

            if(Math.Abs(xSpeed) > 12)//speed limit
            {
                xSpeed = xSpeed > 0 ? 12 : -12;
            }
            xPos += xSpeed;//add friction here for pool game
            
           

            if (Math.Abs(ySpeed) > 12)//speed limit
            {
                ySpeed = ySpeed >= 0 ? 12 : -12;
            }
            yPos +=  ySpeed;
            //these update the balls position each frame, according to speed.


            if (xPos > RightWall -radius)//too far right(wall)
            {
                xPos = RightWall - radius;
                xSpeed = -xSpeed;
                
            }
            if (yPos >= BotWall - radius )//too far down
            {
                yPos = BotWall - radius;
                ySpeed = 0;
                if (xPos >= RightWall - radius)
                {
                    xSpeed = 0;
                    inPlay = false;
                }
                else
                {
                    xSpeed = 5;
                }
   
            }
            if (yPos <= TopWall +radius)//too far up
            {
                yPos = TopWall + radius;
                ySpeed = -ySpeed;
            }
            if (xPos < LeftWall +radius)//too far left
            {
                xPos = LeftWall + radius;
                xSpeed = -xSpeed;
            }
        }
        //check pad colliding with ball
        public void checkPadCollision(Paddle pad, float iSpeed)
        {                                 //ball in yAxis inside pad                                                              //ball in xAxis  between points
            if ((yPos + radius >= pad.getY() - pad.getHeight()/2 && yPos - radius <= pad.getY() + pad.getHeight()/2) && (xPos + radius >= pad.getLL() && xPos < pad.getML()))//if left area
            {//bounce back or up(goes back if hit on the side because of xSpeed changed to 0, and next frame still in same "x" of area, so goes back.
                xSpeed += -iSpeed * 1.5F;
                xSpeed = xSpeed < -12 ? -12 : xSpeed * 1.5F;


                ySpeed = ySpeed > 12 ? -12 : -ySpeed * 1.5F;
            }
             if ((yPos + radius >= pad.getY() - pad.getHeight()/2 && yPos + radius <= pad.getY() + pad.getHeight()/2) && (xPos + radius >= pad.getML() && xPos - radius <= pad.getMR()))
            {//bounce MIDDLE
                if (ySpeed < iSpeed )
                    ySpeed = iSpeed;


                ySpeed = -ySpeed;
            }
             if((yPos + radius >= pad.getY() - pad.getHeight()/2 && yPos - radius <= pad.getY() + pad.getHeight()/2) && (xPos > pad.getMR() && xPos - radius <= pad.getRR()))
            {//bounce back or up
                xSpeed += iSpeed * 1.5F;
                xSpeed = xSpeed > 12 ? 12 : xSpeed * 1.5F;


                ySpeed = ySpeed > 12 ? -12 : -ySpeed * 1.5F;
                
            }   
        }
        //brick collision
        public bool checkBrickCollision(Brick brick)
        {

            xDist = brick.getX() - xPos;//dx
            yDist = brick.getY() - yPos;//dy

            float radDist = radius + brick.getWidth();

            if (radDist * radDist >= (xDist * xDist + yDist * yDist) && inPlay == true && brick.isAlive == true) //check if distance is bigger than the balls touching range and is alive
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void calculateBrickCollision(Brick brick)//this should not run on next frame, or balls will return. 
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
            double V0x = xSpeed * cos + ySpeed * sin;
            double V0y = ySpeed * cos - xSpeed * sin;
            //rotate brick 1 velocity
            double V1x = 0;
            double V1y = 0;

            //collision reaction ELASTISK LIGNING I BOKA?, tror denne gjør at de ikke setter seg fast, må plusse på noe ekstra? eller ikke siden det er vel.
            double vxtotal = V0x - V1x;
            V0x = ((mass - brick.getMass()) * V0x + 2 * brick.getMass() * V1x) / (mass + brick.getMass());//new velocity x ball 1
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
            xPos = xPos + (float)B0newPosx;//these 2 new positions will be a little "bigger" than when they entered. this is so that they wont stick. also, they point slightly away from each other.
            yPos = yPos + (float)B0newPosy;

            //update speed
            xSpeed = (float)B0newVelx;
            ySpeed = (float)B0newVely;
        }

        //check if balls collide
        public bool checkBallCollision(Ball otherBall)
        {
            
            xDist = otherBall.getX()-xPos;//dx
            yDist = otherBall.getY()-yPos;//dy

            float radDist = radius + otherBall.getRadius();
            
            if (radDist * radDist >= (xDist * xDist + yDist * yDist) && inPlay == true && otherBall.inPlay == true) //check if distance is bigger than the balls touching range
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //calculate the ball angle and velocity, and execute.
        public void calculateBallCollision(Ball otherBall)//this should not run on next frame, or balls will return. 
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
            double V0x = xSpeed * cos + ySpeed * sin;
            double V0y = ySpeed * cos - xSpeed * sin;
            //rotate ball 1 velocity
            double V1x = otherBall.xSpeed * cos + otherBall.ySpeed * sin;
            double V1y = otherBall.ySpeed * cos - otherBall.xSpeed * sin;

            //collision reaction ELASTISK LIGNING
            double vxtotal = V0x - V1x;
            V0x = ((mass - otherBall.getMass()) * V0x + 2 * otherBall.getMass() * V1x) / (mass + otherBall.getMass());//new velocity x ball 1
            V1x = vxtotal + V0x; //new velocity x ball 2
            //update position, THIS ONE IS RELATIVE TO MID BALL 0 and BALL 1
            B0x += V0x;
            B1x += V1x;
            //rot pos back? SET NEW POSITION. BALLS SHOULD OVERLAP AFTER THIS.
            double B0newPosx = B0x * cos - B0y * sin;
            double B0newPosy = B0y * cos + B0x * sin;

            double B1newPosx = B1x * cos - B1y * sin;
            double B1newPosy = B1y * cos + B1x * sin;

            //rot vel back
            double B0newVelx = V0x * cos - V0y * sin;
            double B0newVely = V0y * cos + V0x * sin;

            double B1newVelx = V1x * cos - V1y * sin;
            double B1newVely = V1y * cos + V1x * sin;

            //update position
            otherBall.xPos = xPos + (float)B1newPosx;//is this just to set it out of the other balls's radius
            otherBall.yPos = yPos + (float)B1newPosy;
            xPos = xPos + (float)B0newPosx;//these 4 new positions will be a little "bigger" than when they entered. this is so that they wont stick. also, they point slightly away from each other.
            yPos = yPos + (float)B0newPosy;

            //update speed
            xSpeed = (float)B0newVelx;
            ySpeed = (float)B0newVely;

            otherBall.setXspeed((float)B1newVelx);
            otherBall.setYspeed((float)B1newVely);
        }



        public float getX()
        {
            return xPos;
        }
        public float getY()
        {
            return yPos;
        }
        public float getXspeed()
        {
            return xSpeed;
        }
        public float getYspeed()
        {
            return ySpeed;
        }
        public float getRadius()
        {
            return radius;
        }
        public float getMass()
        {
            return mass;
        }

        public void setXspeed(float xSp)
        {
            xSpeed = xSp;
        }
        public void setYspeed(float ySp)
        {
            ySpeed = ySp;
        }
        public void setForce(float force)
        {
            this.ForcePositionY = force;
        }
        public float getForce()
        {
            return accelerationY;
        }
        public void setGravity(float acceleration)
        {
            accelerationY = acceleration;

        }
    }
}
