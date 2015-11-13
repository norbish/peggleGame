using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Pool_Game
{
    public partial class Form1 : Form
    {
        float topWall = 0;
        float botWall = 600;//y
        static float leftWall = 0;
        static float rightWall = 1000;//x
        static float padMidPos = rightWall / 2;
        static float padPosy = 530;
        static float padHeight = 15;
        static float gravity = .5981F;
        float TempGravity = 0;

        float brickGroupPosX = 10;
        float brickGroupPosY = 150;
        float brickRadius = 7F;
        float brickDistanceX = 50;
        float brickDistanceY = 50;
        int normal = 0;
        int doubleBall = 1;
        int slowBalls = 2;
        int fastBalls = 3;
        Timer Timer1 = new Timer();//create timer
        float radius =7F;//radius of all the balls
        
        Ball[] Ballz = new Ball[5];
        Brick[] Brickz = new Brick[45];

        int tempBrickNumber;
        int tempBallNumber;
        bool hasStarted = false;

        int numBricksDestroyed = 0;
        int numBallsActive = 1;

        static Panel Canvas = new Panel();
        static Panel sideCanvas = new Panel();
        static Panel ball0Canvas = new Panel();
        static Panel ball1Canvas = new Panel();
        static Panel ball2Canvas = new Panel();
        static Panel ball3Canvas = new Panel();
        static Panel ball4Canvas = new Panel();
        static Panel midCanvas = new Panel();
        static Label score = new Label();
        static Label simSpeedLabel = new Label();
        int simSpeed = 30;
        static Label ballSpeedLabel = new Label();
        int startBallspeedX = 2;
        int padBounceSpeed = 2;
        static Label forceValLabel = new Label();
        int forceVal = 6;
        static Label gravityValLabel = new Label();
        float gravityVal = 0.1F;

        Button restartButton = new Button();
        Button startButton = new Button();
        Button pauseButton = new Button();
        Button resumeButton = new Button();
        Button simSpeedButton = new Button();

        NumericUpDown setSimSpeed = new NumericUpDown();
        NumericUpDown setBallSpeed = new NumericUpDown();
        NumericUpDown setForceVall = new NumericUpDown();
        TextBox setGravityVal = new TextBox();

        Label ballLabels = new Label();
        Label ball0Vars = new Label();
        Label ball1Vars = new Label();
        Label ball2Vars = new Label();
        Label ball3Vars = new Label();
        Label ball4Vars = new Label();
        
        Paddle pad = new Paddle(padMidPos, padPosy, leftWall, rightWall, padHeight);//initiates this in the constructor, because it only needs ever to run once.
        public Form1()
        {
            this.DoubleBuffered = true;//potentially reduces flickering
            InitializeComponent();//initialize visual studio's own packets.
            InitializeGUI();//add the GUI's (including BOT panel)
            InitializeBallz();//add the balls
            InitializeBrickz();//setup bricks
            InitializeSidepanelObjects();//setup the sidepanel
            BotPanelValues();//setup the bottom panel's values


            
            Timer1.Enabled = true;
            Timer1.Tick += new EventHandler(FixedUpdate);//make timer time.
                
        }

        public void update()
        {
            
        }
        

        //UPDATE FUNCTION - the main method
        public void FixedUpdate(object sender, EventArgs e) // this is where everything happens
        {
            Graphics drawing = Canvas.CreateGraphics();//this is the main forms canvas
            drawing.Clear(Canvas.BackColor);//this clears all the drawings, but also creates flickering. 

            for (int b = 0; b < Ballz.Length; b++)//updates the balls' positions
            {
                if (Ballz[b].inPlay)//only updates the balls position if it is in play
                    Ballz[b].UpdateVars(topWall, botWall, leftWall, rightWall, padPosy, TempGravity);
                else
                {
                    Ballz[b].setXspeed(0);//ensures the balls won't move after deactivated.
                    Ballz[b].setYspeed(0);
                }                
            }
            if (!hasStarted)//sets ball position to paddle position before game has started
            {
                Ballz[0].xPos = pad.getX();
                startBallspeedX = Int32.Parse(setBallSpeed.Text);//sets ball speed
                forceVal = -Int32.Parse(setForceVall.Text);// set the force value. minus for accuracy with canvas. + is -
                drawVector(drawing); //calls the function that draws the vector line for the ball.
            }
            
            updateBotPanelValues();//updates the ball variables
            drawBalls(drawing);//draws the balls
            drawPad(drawing);//draws the paddle
            drawBricks(drawing);//draws the bricks
            
           CheckCollisions();//checks all the collisions in the game
        }

        //COLLISIONS METHOD
        public void CheckCollisions()
        {
            for (int f = 0; f < numBallsActive; f++)//we do not update more balls than possibly active.
            {
                //check and resolve collision with PADDLE
                Ballz[f].checkPadCollision(pad, padBounceSpeed);
                
                for (int s = f+1; s < Ballz.Length; s++)
                {
                    //check collision with BALLS
                    if (Ballz[f].checkBallCollision(Ballz[s]))
                        //there is a collision. resolve it:
                        Ballz[f].calculateBallCollision(Ballz[s]);
                }
                for (int b = 0; b < Brickz.Length; b++)
                {
                    //check collisions with BRICKS
                    if(Ballz[f].checkBrickCollision(Brickz[b]))
                    {
                        // there is a collision:
                        Ballz[f].calculateBrickCollision(Brickz[b]);
                        Brickz[b].isAlive = false;//hides the brick
                        numBricksDestroyed += 1;//adds to score:
                        score.Text = numBricksDestroyed.ToString();

                        //check bricktype of destroyed brick and react accordingly
                        switch(Brickz[b].brickType)
                        {
                            case 1: tempBrickNumber = b; tempBallNumber = f; DropNewBall(b, f); break;// drop a new ball on destroyed brick
                            case 2: Ballz[f].setXspeed(Ballz[f].getXspeed() * .5F); Ballz[f].setYspeed(Ballz[f].getYspeed() * .5F); break;// set slow speed on ball
                            case 3: Ballz[f].setXspeed(Ballz[f].getXspeed() * 1.5F); Ballz[f].setYspeed(Ballz[f].getYspeed() * 1.5F); break;// set fast speed on ball
                            default: break;
                        }
                        

                    }
                }
            }
        }

        //DRAWINGS  - BALLS PAD AND BRICKS
        public void drawBalls(Graphics drawing)
        {
            for(int b = 0; b < numBallsActive; b++)//never draws more than the number of balls that have been active before.
                switch(b)
                {
                    case 0: drawing.FillEllipse(Brushes.Black, Ballz[b].getX() - Ballz[b].getRadius(), Ballz[b].getY() - Ballz[b].getRadius(), Ballz[b].getRadius() * 2, Ballz[b].getRadius() * 2); break;
                    case 1: drawing.FillEllipse(Brushes.Blue, Ballz[b].getX() - Ballz[b].getRadius(), Ballz[b].getY() - Ballz[b].getRadius(), Ballz[b].getRadius() * 2, Ballz[b].getRadius() * 2); break;
                    case 2: drawing.FillEllipse(Brushes.Yellow, Ballz[b].getX() - Ballz[b].getRadius(), Ballz[b].getY() - Ballz[b].getRadius(), Ballz[b].getRadius() * 2, Ballz[b].getRadius() * 2); break;
                    case 3: drawing.FillEllipse(Brushes.Red, Ballz[b].getX() - Ballz[b].getRadius(), Ballz[b].getY() - Ballz[b].getRadius(), Ballz[b].getRadius() * 2, Ballz[b].getRadius() * 2); break;
                    case 4: drawing.FillEllipse(Brushes.Green, Ballz[b].getX() - Ballz[b].getRadius(), Ballz[b].getY() - Ballz[b].getRadius(), Ballz[b].getRadius() * 2, Ballz[b].getRadius() * 2); break;
                }
        }
        public void drawPad(Graphics drawing)
        {
            drawing.FillRectangle(Brushes.Red, pad.getLL(), padPosy, pad.getML() - pad.getLL(), padHeight);
            drawing.FillRectangle(Brushes.DarkBlue, pad.getML(), padPosy, pad.getMR() - pad.getML(), padHeight);//this one is pad's middle position.
            drawing.FillRectangle(Brushes.Red, pad.getMR()     , padPosy, pad.getRR() - pad.getMR(), padHeight);
        }

        public void drawBricks(Graphics drawing)
        {
            Brush normal = Brushes.Gray;//change these to change colors.
            Brush newBall = Brushes.Green;
            Brush slowDown = Brushes.Blue;
            Brush speedup = Brushes.Red;
            
            for (int i = 0; i < Brickz.Length; i++)//iterates through, draws bricks depending on whether active or not, and according to brick type(color, speed/new-ball)
            {
                if (Brickz[i].isAlive && Brickz[i].brickType == 0)//normal balls/gray
                {
                    drawing.FillEllipse(normal, Brickz[i].getX() - Brickz[i].getWidth() / 2, Brickz[i].getY() - Brickz[i].getHeight() / 2, Brickz[i].getWidth()*2, Brickz[i].getHeight()*2);
                }
                if (Brickz[i].isAlive && Brickz[i].brickType == 1)//newball / green
                {
                    drawing.FillEllipse(newBall, Brickz[i].getX() - Brickz[i].getWidth() / 2, Brickz[i].getY() - Brickz[i].getHeight() / 2, Brickz[i].getWidth()*2, Brickz[i].getHeight()*2);
                }
                if (Brickz[i].isAlive && Brickz[i].brickType == 2)//slow ball / blue
                {
                    drawing.FillEllipse(slowDown, Brickz[i].getX() - Brickz[i].getWidth() / 2, Brickz[i].getY() - Brickz[i].getHeight() / 2, Brickz[i].getWidth()*2, Brickz[i].getHeight()*2);
                }
                if (Brickz[i].isAlive && Brickz[i].brickType == 3)//fast ball / red
                {
                    drawing.FillEllipse(speedup, Brickz[i].getX() - Brickz[i].getWidth() / 2, Brickz[i].getY() - Brickz[i].getHeight() / 2, Brickz[i].getWidth()*2, Brickz[i].getHeight()*2);
                }
            }
        }

        public void drawVector(Graphics drawing)
        {
            PointF[] Vector = new PointF[40];//set up array
            if(!hasStarted)//before you press start
            for (int i = 0; i < Vector.Length; i++)//iterate through 
                Vector[i] = new PointF( Ballz[0].getX() + (float)i * startBallspeedX , Ballz[0].getY() + (float)i * -Math.Abs(forceVal));//set up the points according to ball position and x/yspeed

            Pen pen = new Pen(Color.Red);
            drawing.DrawCurve(pen, Vector);//draw the points on canvas
        }

        public void InitializeBrickz()
        {
            //
            for(int i = 0; i<Brickz.Length; i++)//SETS LOCATION, SIZE, AND TYPES OF BRICKS. initializing.
            {
                //ROW ONE
                if (i < 1)//the first brick
                    //              (xPos + int for not sticking,    yPos + int for bricks not sticking, height,width, bricktype, is active?)
                    Brickz[i] = new Brick(brickGroupPosX + i * brickDistanceX, brickGroupPosY, brickRadius, brickRadius, slowBalls, true);//slow balls / blue
                if (i >= 1 && i < 7)
                    Brickz[i] = new Brick(brickGroupPosX + i * brickDistanceX + i, brickGroupPosY, brickRadius, brickRadius, normal, true);//normal / gray
                if (i >= 7 && i < 14)
                    Brickz[i] = new Brick(brickGroupPosX+250 + i * brickDistanceX + i, brickGroupPosY, brickRadius, brickRadius, normal, true);//normal / gray
                if (i >= 14 && i < 15)
                    Brickz[i] = new Brick(brickGroupPosX+250 + i * brickDistanceX + i, brickGroupPosY, brickRadius, brickRadius, fastBalls, true);//fast balls / red

                // ROW TWO
                else if (i >= 15 && i < 16)
                    Brickz[i] = new Brick(brickGroupPosX + 20 + (i - 15) * brickDistanceX + (i - 15), brickGroupPosY + brickDistanceY, brickRadius, brickRadius, doubleBall, true);//double / gray
                else if (i >= 16 && i < 22)
                    Brickz[i] = new Brick(brickGroupPosX + 20 + (i - 15) * brickDistanceX + (i - 15), brickGroupPosY + brickDistanceY, brickRadius, brickRadius, normal, true);//normal / gray
                else if (i >= 22 && i < 29)
                    Brickz[i] = new Brick(brickGroupPosX + 230 + (i - 15) * brickDistanceX + (i - 15), brickGroupPosY + brickDistanceY, brickRadius, brickRadius, normal, true);//normal / gray
                else if (i >= 29 && i < 30)
                    Brickz[i] = new Brick(brickGroupPosX + 230 + (i - 15) * brickDistanceX + (i - 15), brickGroupPosY + brickDistanceY, brickRadius, brickRadius, doubleBall, true);//double / gray

                //ROW THREE
                else if (i >= 30 && i < 31)
                    Brickz[i] = new Brick(brickGroupPosX + 40 + (i - 30) * brickDistanceX + (i - 30), brickGroupPosY + 2 * brickDistanceY, brickRadius, brickRadius, slowBalls, true);//slow / blue
                else if (i >= 31 && i < 37)
                    Brickz[i] = new Brick(brickGroupPosX + 40 + (i - 30) * brickDistanceX + (i - 30), brickGroupPosY + 2 * brickDistanceY , brickRadius, brickRadius, normal, true);//normal / gray
                else if (i >= 37 && i < 44)
                    Brickz[i] = new Brick(brickGroupPosX + 210 + (i - 30) * brickDistanceX + (i - 30), brickGroupPosY + 2 * brickDistanceY, brickRadius, brickRadius, normal, true);//normal / gray
                else if (i >= 44 && i < 45)
                    Brickz[i] = new Brick(brickGroupPosX + 210 + (i - 30) * brickDistanceX + (i - 30), brickGroupPosY + 2 * brickDistanceY, brickRadius, brickRadius, fastBalls, true);//fast / red

                //ROW FOUR - not existing. possible to add new bricks. 
                else if (i >= 45 && i < 60)
                    Brickz[i] = new Brick(brickGroupPosX + (i - 45) * brickRadius + (i - 45), brickGroupPosY + 3 * brickRadius + 3, brickRadius, brickRadius, normal, true);//normal / gray
            }
        }

        public void InitializeBallz()
        {
            Ballz[0] = new Ball(pad.getX(), padPosy-10, startBallspeedX, forceVal, radius, false, 0);//xPos, yPos, xSpeed,  ySpeed, radius, inPlay, gravity value
            Ballz[1] = new Ball(1000, botWall - radius, 0, 0, radius, false,gravity);//all other balls start off to the bottom right corner.
            Ballz[2] = new Ball(1000, botWall - radius, 0, 0, radius, false,gravity);
            Ballz[3] = new Ball(1000, botWall - radius, 0, 0, radius, false,gravity);
            Ballz[4] = new Ball(1000, botWall - radius, 0, 0, radius, false,gravity);
        }

        //drop a new ball from CheckCollision()
        public void DropNewBall(int b, int f)
        {
           int count = 0;//used count and while loop since the "count" must be changed when a ball is being activated, so that we don't put out more than one ball.
           while(count < Ballz.Length)
            {
                if(!Ballz[count].inPlay)//if the ball is not yet in play, we can deploy it.
                {
                    Ballz[count] = new Ball(Brickz[b].getX(),Brickz[b].getY(), Ballz[f].getXspeed() , Ballz[f].getYspeed(), radius, true, gravity);//sets new balls' position(the brick), and speeds(colliding ball's speed)
                    numBallsActive += 1;//keep track for update purposes
                    count = Ballz.Length;//exits loop
                }
                count++;
            }
        }
        
        //BUTTONS
        public void restartGame(object sender, EventArgs e)
        {
            InitializeBallz();//re-starts the balls
            InitializeBrickz();//re-draws the bricks
            score.Text = "0"; // reset score text
            numBricksDestroyed = 0;//reset score var
            numBallsActive = 1;
            hasStarted = false;//stops game, lets ball 1 be right over paddle.
            TempGravity = 0;//sets the first balls gravity to 0, so it doesn't move before game has started.
        }
        public void StartButton(object sender, EventArgs e)
        {
            
            Timer1.Interval = Int32.Parse(setSimSpeed.Text);//user inputs simulation speed
            gravity =  float.Parse(setGravityVal.Text);//user inputs gravity
            Ballz[0].setXspeed(startBallspeedX);//sets user defined x speed, ball will start moving.
            Ballz[0].setForce(forceVal);//sets user defined force(Y-speed), ball will start moving.
            Ballz[0].inPlay = true;// update loops including ball 1 will start running. 
            TempGravity = gravity;//Sets the first ball's gravity so "physics" will start pulling it down.
            hasStarted = true;//game has started. vector will disappear, ball will not follow pad, etc.
        }
        public void pauseGame(object sender, EventArgs e)
        {
            if(Timer1.Enabled)
                Timer1.Stop();//staps the timer to pause the game.
        }
        public void resumeGame(object sender, EventArgs e)
        {
            if (!Timer1.Enabled)
            { 
                Timer1.Start(); //starts the timer to resume the game.
            }
        }

        //PANELS
        private void InitializeSidepanelObjects()
        {
            simSpeedLabel.Top = 0;
            simSpeedLabel.Text = "Sim Speed:";


            setSimSpeed.Top = 15;
            setSimSpeed.Left = 1;
            setSimSpeed.Width = 65;
            setSimSpeed.Value = simSpeed;

            ballSpeedLabel.Top = 40;
            ballSpeedLabel.Text = "Ball x Speed:";

            setBallSpeed.Top = 55;
            setBallSpeed.Left = 1;
            setBallSpeed.Width = 65;
            setBallSpeed.Value = startBallspeedX;

            forceValLabel.Top = 80;
            forceValLabel.Text = "Ball Force:";

            setForceVall.Top = 95;
            setForceVall.Left = 1;
            setForceVall.Width = 65;
            setForceVall.Value = forceVal;

            gravityValLabel.Top = 120;
            gravityValLabel.Text = "Gravity: ";

            setGravityVal.Top = 135;
            setGravityVal.Left = 1;
            setGravityVal.Width = 65;
            setGravityVal.Text = gravityVal.ToString();

            startButton.Top = 480;
            startButton.Left = 1;
            startButton.Width = 65;
            startButton.Text = "start";
            startButton.Click += new EventHandler(StartButton);

            pauseButton.Top = 510;
            pauseButton.Left = 1;
            pauseButton.Width = 65;
            pauseButton.Text = "pause";
            pauseButton.Click += new EventHandler(pauseGame);

            resumeButton.Top = 540;
            resumeButton.Left = 1;
            resumeButton.Width = 65;
            resumeButton.Text = "resume";
            resumeButton.Click += new EventHandler(resumeGame);

            restartButton.Top = 570;
            restartButton.Left = 1;
            restartButton.Width = 65;
            restartButton.Text = "Stop";
            restartButton.Click += new EventHandler(restartGame);


            score.Font = new Font(score.Font.FontFamily, 17);
            score.Top = 0;
            score.Left = 965;
            score.Text = numBricksDestroyed.ToString();


            //first gets put on top of the others
            sideCanvas.Controls.Add(setGravityVal);
            sideCanvas.Controls.Add(gravityValLabel);
            sideCanvas.Controls.Add(setForceVall);
            sideCanvas.Controls.Add(forceValLabel);
            sideCanvas.Controls.Add(setBallSpeed);
            sideCanvas.Controls.Add(ballSpeedLabel);
            sideCanvas.Controls.Add(setSimSpeed);
            sideCanvas.Controls.Add(simSpeedLabel);

            
            
            
            sideCanvas.Controls.Add(restartButton);
            sideCanvas.Controls.Add(startButton);
            sideCanvas.Controls.Add(pauseButton);
            sideCanvas.Controls.Add(resumeButton);
            Canvas.Controls.Add(score);
        }
        public void BotPanelValues()
        {
            ballLabels.Font = new Font(ballLabels.Font.FontFamily, 10);
            ballLabels.Left = 80;
            ballLabels.Text = "Ball1\t\t Ball2\t\tBall3\t\tBall4\t\tBall5".Replace("\t", "                     ");
            ballLabels.Width = 1000;

            ball0Vars.Top = 0;
            ball0Vars.Height = 100;
            ball1Vars.Top = 0;
            ball1Vars.Height = 100;
            ball2Vars.Top = 0;
            ball2Vars.Height = 100;
            ball3Vars.Top = 0;
            ball3Vars.Height = 100;
            ball4Vars.Top = 0;
            ball4Vars.Height = 100;

            midCanvas.Controls.Add(ballLabels);

            ball0Canvas.Controls.Add(ball0Vars);
            ball1Canvas.Controls.Add(ball1Vars);
            ball2Canvas.Controls.Add(ball2Vars);
            ball3Canvas.Controls.Add(ball3Vars);
            ball4Canvas.Controls.Add(ball4Vars);
        }
        public void updateBotPanelValues()//just type in if more variables are needed.
        {
            
         ball0Vars.Text = "x position: " + (int)Ballz[0].getX() + "\ny position: " + (int)Ballz[0].getY() + "\nmass:        " + Ballz[0].getMass() + "\nx speed:    " + (int)Ballz[0].getXspeed() + "\ny speed:    " + -(int)Ballz[0].getYspeed() + "\nG Force:   " + Ballz[0].getForce(); 
         ball1Vars.Text = "x position: " + (int)Ballz[1].getX() + "\ny position: " + (int)Ballz[1].getY() + "\nmass:        " + Ballz[1].getMass() + "\nx speed:    " + (int)Ballz[1].getXspeed() + "\ny speed:    " + -(int)Ballz[1].getYspeed() + "\nG Force:   " + Ballz[0].getForce(); 
         ball2Vars.Text = "x position: " + (int)Ballz[2].getX() + "\ny position: " + (int)Ballz[2].getY() + "\nmass:        " + Ballz[2].getMass() + "\nx speed:    " + (int)Ballz[2].getXspeed() + "\ny speed:    " + -(int)Ballz[2].getYspeed() + "\nG Force:   " + Ballz[0].getForce(); 
         ball3Vars.Text = "x position: " + (int)Ballz[3].getX() + "\ny position: " + (int)Ballz[3].getY() + "\nmass:        " + Ballz[3].getMass() + "\nx speed:    " + (int)Ballz[3].getXspeed() + "\ny speed:    " + -(int)Ballz[3].getYspeed() + "\nG Force:   " + Ballz[0].getForce(); 
         ball4Vars.Text = "x position: " + (int)Ballz[4].getX() + "\ny position: " + (int)Ballz[4].getY() + "\nmass:        " + Ballz[4].getMass() + "\nx speed:    " + (int)Ballz[4].getXspeed() + "\ny speed:    " + -(int)Ballz[4].getYspeed() + "\nG Force:   " + Ballz[0].getForce(); 
        
        }
        public void InitializeGUI()
        {
            Canvas.Top = (int)topWall;
            Canvas.Left = (int)leftWall;
            Canvas.Height = (int)botWall;
            Canvas.Width = (int)rightWall;
            Canvas.BorderStyle = BorderStyle.FixedSingle;
            Canvas.BackColor = Color.White;

            sideCanvas.Top = 0;
            sideCanvas.Left = 1010;
            sideCanvas.Height = 600;
            sideCanvas.Width = 70;
            sideCanvas.BorderStyle = BorderStyle.FixedSingle;

            midCanvas.Top = 600;
            midCanvas.Left = 0;
            midCanvas.Height = 20;
            midCanvas.Width = 3000;

            ball0Canvas.Top = 620;
            ball0Canvas.Left = 50;
            ball0Canvas.Height = 200;
            ball0Canvas.Width = 100;
            ball0Canvas.BackColor = Color.White;

            ball1Canvas.Top = 620;
            ball1Canvas.Left = 250;
            ball1Canvas.Height = 200;
            ball1Canvas.Width = 100;
            ball1Canvas.BackColor = Color.White;

            ball2Canvas.Top = 620;
            ball2Canvas.Left = 450;
            ball2Canvas.Height = 200;
            ball2Canvas.Width = 100;
            ball2Canvas.BackColor = Color.White;

            ball3Canvas.Top = 620;
            ball3Canvas.Left = 650;
            ball3Canvas.Height = 200;
            ball3Canvas.Width = 100;
            ball3Canvas.BackColor = Color.White;

            ball4Canvas.Top = 620;
            ball4Canvas.Left = 850;
            ball4Canvas.Height = 200;
            ball4Canvas.Width = 100;
            ball4Canvas.BackColor = Color.White;





            this.Controls.Add(Canvas);
            this.Controls.Add(sideCanvas);
            this.Controls.Add(midCanvas);
            this.Controls.Add(ball0Canvas);
            this.Controls.Add(ball1Canvas);
            this.Controls.Add(ball2Canvas);
            this.Controls.Add(ball3Canvas);
            this.Controls.Add(ball4Canvas);
            
        }
        
        //KEY STROKES - MOVEMENT
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if(keyData == Keys.Left && keyData == Keys.Right)
            {
                return true;
            }
            else if (keyData == Keys.Left)
            {
                pad.updateVars(false);
                return true;
            }
            else if(keyData == Keys.Right)
            {
                pad.updateVars(true);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }  
}
