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
        private float topWall = 0;
        private float botWall = 600;//y
        private static float leftWall = 0;
        private static float rightWall = 1000;//x
        private static float padMidPos = rightWall / 2;
        private static float padPosy = 530;
        private static float padHeight = 5;

        private float brickGroupPosX = 150;
        private float brickGroupPosY = 100;
        private float brickRadius = 10F;
        private float brickDistanceX = 30;
        private float brickDistanceY = 30;
        int normal = 0;
        int doubleBall = 1;
        int slowBalls = 2;
        int fastBalls = 3;
        public Timer Timer1 = new Timer();//create timer
        public float ballspeed = 2F;// the SPEED the balls are set to!
        float radius =6F;//radius of all the balls
        
        Ball[] Ballz = new Ball[5];
        Brick[] Brickz = new Brick[45];

        Paddle pad = new Paddle(padMidPos, padPosy, leftWall, rightWall, padHeight);

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
        static Label ballSpeedLabel = new Label();
        Button restartButton = new Button();
        Button startButton = new Button();
        Button pauseButton = new Button();
        Button resumeButton = new Button();
        Button simSpeedButton = new Button();

        TextBox Textbox1 = new TextBox();
        TextBox Textbox2 = new TextBox();

        Label ballLabels = new Label();
        Label ball0Vars = new Label();
        Label ball1Vars = new Label();
        Label ball2Vars = new Label();
        Label ball3Vars = new Label();
        Label ball4Vars = new Label();



        int tempBrickNumber;
        int tempBallNumber;
        private bool hasStarted = false;

        private int numBricksDestroyed = 0;
        private int numBallsActive = 1;

        public Form1()
        {
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
            Graphics drawing = Canvas.CreateGraphics();
            for (int b = 0; b < Ballz.Length; b++)
            {
                if (Ballz[b].inPlay)
                    Ballz[b].UpdateVars(topWall, botWall, leftWall, rightWall, padPosy);
                else
                {
                    Ballz[b].setXspeed(0);
                    Ballz[b].setYspeed(0);
                }                
            }
            if (!hasStarted)//sets ball position to paddle position before game has started
                Ballz[0].xPos = pad.getX();

            updateBotPanelValues();
            drawBalls(drawing);
            drawPad(drawing);
            drawBricks(drawing);
            
           CheckCollisions();//if this is inside loop, code doesnt work with less than 5 balls. 
        }

        //COLLISIONS METHOD
        public void CheckCollisions()
        {
            for (int f = 0; f < Ballz.Length; f++)//num balls active instead?
            {//check collision with PADDLE
                Ballz[f].checkPadCollision(pad, ballspeed);
                
                for (int s = f+1; s < Ballz.Length; s++)
                {//check collision with BALLS
                    if (Ballz[f].checkBallCollision(Ballz[s]))
                    {//there is a collision
                        Ballz[f].calculateBallCollision(Ballz[s]);
                    } 
                }
                for (int b = 0; b < Brickz.Length; b++)
                {//check collisions with BRICKS
                    if(Brickz[b].checkBallCollision(Ballz[f]))//må endre til ballz.checkcollision.
                    {// there is a collision
                        Brickz[b].calculateBrickCollision(Ballz[f]);
                        Brickz[b].isAlive = false;
                        numBricksDestroyed += 1;
                        score.Text = numBricksDestroyed.ToString();

                        //check bricktype and react accordingly
                        switch(Brickz[b].brickType)
                        {
                            case 1: tempBrickNumber = b; tempBallNumber = f; DropNewBall(b, f); break;// drop ball on destroyed brick
                            case 2: Ballz[f].setXspeed(Ballz[f].getXspeed() * (float).5); Ballz[f].setYspeed(Ballz[f].getYspeed() * (float).5); break;// set slow speed on ball
                            case 3: Ballz[f].setXspeed(Ballz[f].getXspeed() * (float)2); Ballz[f].setYspeed(Ballz[f].getYspeed() * (float)2); break;// set fast speed on ball
                            default: break;
                        }
                        

                    }
                }
            }
        }

        //DRAWINGS  - BALLS PAD AND BRICKS
        public void drawBalls(Graphics drawing)
        {
            
            drawing.Clear(Canvas.BackColor);
            for(int b = 0; b < numBallsActive; b++)//can be removed if drawBalls() is put in for loop
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
            drawing.FillRectangle(Brushes.Red, pad.getLL(), padPosy, pad.getWidth()/4, padHeight);//height = 5 
            drawing.FillRectangle(Brushes.Yellow, pad.getML(), padPosy, pad.getWidth()/2, padHeight);//height = 5 REAL POSITION
            drawing.FillRectangle(Brushes.Blue, pad.getMR()     , padPosy, pad.getWidth()/4, padHeight);//height = 5
        }
        //draw bricks if not deactivated.
        public void drawBricks(Graphics drawing)
        {
            Brush normal = Brushes.Gray;
            Brush newBall = Brushes.Green;
            Brush slowDown = Brushes.Blue;
            Brush speedup = Brushes.Orange;
            
            for (int i = 0; i < Brickz.Length; i++)
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
                if (Brickz[i].isAlive && Brickz[i].brickType == 3)//fast ball / orange
                {
                    drawing.FillEllipse(speedup, Brickz[i].getX() - Brickz[i].getWidth() / 2, Brickz[i].getY() - Brickz[i].getHeight() / 2, Brickz[i].getWidth()*2, Brickz[i].getHeight()*2);
                }
            }
        }


        //initialize BALLS AND BRICKS
        public void InitializeBrickz()
        {
            //
            for(int i = 0; i<Brickz.Length; i++)//SETS LOCATION AND TYPES OF BRICKS. initializing.
            {//if I want a special brick, make a small if statement in between.
                //ROW ONE
                if(i < 1)//the first brick
                    Brickz[i] = new Brick(brickGroupPosX + i * brickDistanceX, brickGroupPosY, brickRadius, brickRadius, slowBalls, true);//slow balls / blue

                if (i >= 1 && i < 14)
                    Brickz[i] = new Brick(brickGroupPosX + i * brickDistanceX + i, brickGroupPosY, brickRadius, brickRadius, normal, true);//normal / gray

                if (i >= 14 && i < 15)
                    Brickz[i] = new Brick(brickGroupPosX + i * brickDistanceX + i, brickGroupPosY, brickRadius, brickRadius, fastBalls, true);//fast balls / orange

                // ROW TWO
                else if(i >= 15 && i < 20)
                    Brickz[i] = new Brick(brickGroupPosX + (i - 15) * brickDistanceX + (i - 15), brickGroupPosY + brickDistanceY, brickRadius, brickRadius, normal, true);//normal / gray
                else if (i >= 20 && i < 25)
                    Brickz[i] = new Brick(brickGroupPosX + (i - 15) * brickDistanceX + (i - 15), brickGroupPosY + brickDistanceY, brickRadius, brickRadius, doubleBall, true);//double ball / green
                else if (i >= 25 && i < 30)
                    Brickz[i] = new Brick(brickGroupPosX + (i - 15) * brickDistanceX + (i - 15), brickGroupPosY + brickDistanceY, brickRadius, brickRadius, normal, true);//normal / gray

                //ROW THREE
                else if(i >= 30 && i < 35)
                    Brickz[i] = new Brick(brickGroupPosX + (i - 30) * brickDistanceX + (i - 30), brickGroupPosY + 2 * brickDistanceY, brickRadius, brickRadius, normal, true);//normal / gray
                else if (i >= 35 && i < 40)
                    Brickz[i] = new Brick(brickGroupPosX + (i - 30) * brickDistanceX + (i - 30), brickGroupPosY + 2 * brickDistanceY , brickRadius, brickRadius, normal, true);//normal / gray
                else if (i >= 40 && i < 45)
                    Brickz[i] = new Brick(brickGroupPosX + (i - 30) * brickDistanceX + (i - 30), brickGroupPosY + 2 * brickDistanceY, brickRadius, brickRadius, doubleBall, true);//normal / gray

                //ROW FOUR - not existing yet
                else if (i >= 45 && i < 60)//last bricks
                    Brickz[i] = new Brick(brickGroupPosX + (i - 45) * brickRadius + (i - 45), brickGroupPosY + 3 * brickRadius + 3, brickRadius, brickRadius, normal, true);//normal / gray
                //                                        (xPos + int for not sticking,  yPos + int for bricks not sticking  ,   height,    width, bricktype, is active?)
            }
        }

        public void InitializeBallz()
        {
            Ballz[0] = new Ball(pad.getX(), padPosy-10, 0, 0, radius, true);//xPos, yPos, xSpeed,  ySpeed, radius, inPlay
            Ballz[1] = new Ball(1000, botWall - radius, 0, 0, radius, false);
            
            Ballz[2] = new Ball(1000, botWall - radius, 0, 0, radius, false);
            Ballz[3] = new Ball(1000, botWall - radius, 0, 0, radius, false);
            Ballz[4] = new Ball(1000, botWall - radius, 0, 0, radius, false);
        }

        //drop a new ball from CheckCollision()
        public void DropNewBall(int b, int f)
        {
            int count = 0;
           while(count < Ballz.Length)
            {
                if(!Ballz[count].inPlay)
                {
                    Ballz[count] = new Ball(Brickz[b].getX(),Brickz[b].getY(), Ballz[f].getXspeed() , Ballz[f].getYspeed(), radius, true);
                    numBallsActive += 1;
                    count = Ballz.Length;
                }
                count++;
            }
        }
        
        //BUTTONS
        public void restartGame(object sender, EventArgs e)
        {
            InitializeBallz();//add the balls
            InitializeBrickz();//re draws the bricks
            score.Text = "0";
            numBricksDestroyed = 0;//resets score
            hasStarted = false;//stops game
        }
        public void StartButton(object sender, EventArgs e)
        {
            ballspeed = Int32.Parse(Textbox2.Text);
            Timer1.Interval = Int32.Parse(Textbox1.Text);//get string from textbox to the timer interval to set simulation speed.
            Ballz[0].setXspeed(ballspeed);
            Ballz[0].setYspeed(-ballspeed);
            hasStarted = true;
        }
        public void pauseGame(object sender, EventArgs e)
        {
            if(Timer1.Enabled)
                Timer1.Stop();
        }
        public void resumeGame(object sender, EventArgs e)
        {
            if (!Timer1.Enabled)
            { 
                Timer1.Start(); //Timer1.Tick += new EventHandler(FixedUpdate);//make timer time.
            }
        }

        //PANELS
        private void InitializeSidepanelObjects()
        {
            simSpeedLabel.Top = 0;
            simSpeedLabel.Text = "Sim Speed:";

            Textbox1.Top = 15;
            Textbox1.Left = 1;
            Textbox1.Width = 65;
            Textbox1.Text = "10";

            ballSpeedLabel.Top = 40;
            ballSpeedLabel.Text = "Ball Speed:";

            Textbox2.Top = 55;
            Textbox2.Left = 1;
            Textbox2.Width = 65;
            Textbox2.Text = "2";

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

            sideCanvas.Controls.Add(Textbox2);
            sideCanvas.Controls.Add(ballSpeedLabel);
            sideCanvas.Controls.Add(Textbox1);
            sideCanvas.Controls.Add(simSpeedLabel);
            sideCanvas.Controls.Add(restartButton);
            sideCanvas.Controls.Add(startButton);
            sideCanvas.Controls.Add(pauseButton);
            sideCanvas.Controls.Add(resumeButton);
            Canvas.Controls.Add(score);
        }
        public void BotPanelValues()
        {
            //ball 0

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
        public void updateBotPanelValues()
        {
            
         ball0Vars.Text = "x position: " + (int)Ballz[0].getX() + "\ny position: " + (int)Ballz[0].getY() + "\nmass:        " + Ballz[0].getMass() + "\nx speed:    " + Ballz[0].getXspeed() + "\ny speed:    " + Ballz[0].getYspeed(); 
         ball1Vars.Text = "x position: " + (int)Ballz[1].getX() + "\ny position: " + (int)Ballz[1].getY() + "\nmass:        " + Ballz[1].getMass() + "\nx speed:    " + Ballz[1].getXspeed() + "\ny speed:    " + Ballz[1].getYspeed(); 
         ball2Vars.Text = "x position: " + (int)Ballz[2].getX() + "\ny position: " + (int)Ballz[2].getY() + "\nmass:        " + Ballz[2].getMass() + "\nx speed:    " + Ballz[2].getXspeed() + "\ny speed:    " + Ballz[2].getYspeed(); 
         ball3Vars.Text = "x position: " + (int)Ballz[3].getX() + "\ny position: " + (int)Ballz[3].getY() + "\nmass:        " + Ballz[3].getMass() + "\nx speed:    " + Ballz[3].getXspeed() + "\ny speed:    " + Ballz[3].getYspeed(); 
         ball4Vars.Text = "x position: " + (int)Ballz[4].getX() + "\ny position: " + (int)Ballz[4].getY() + "\nmass:        " + Ballz[4].getMass() + "\nx speed:    " + Ballz[4].getXspeed() + "\ny speed:    " + Ballz[4].getYspeed(); 
        
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
