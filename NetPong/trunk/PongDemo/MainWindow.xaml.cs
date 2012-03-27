using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PongDemo
{
    public partial class MainWindow : Window
    {
        Ball ball;
        Paddle paddle1, paddle2;
        DispatcherTimer timer;
        int playerOneScore = 0;
        int playerTwoScore = 0;

        public MainWindow()
        {
            InitializeComponent();
            InitBall();
            InitPaddles();
            Update();
        }

        private void InitBall()
        {
            ball = new Ball();
            ball.Sprite = MakeBallSprite();
            ball.XPos = 40;
            ball.YPos = 180;
            ball.Angle = 90;
            ball.Speed = 6;
        }

        private Rectangle MakeBallSprite()
        {
            Rectangle sprite = new Rectangle();
            sprite.Width = 20;
            sprite.Height = 20;
            sprite.Fill = Brushes.White;
            GameCourt.Children.Add(sprite);
            return sprite;
        }

        private void InitPaddles()
        {
            paddle1 = new Paddle(1);
            paddle1.Sprite = MakePaddleSprite();
            paddle1.XPos = 15;
            paddle1.YPos = 150;
            paddle1.Speed = 0;

            paddle2 = new Paddle(2);
            paddle2.Sprite = MakePaddleSprite();
            paddle2.XPos = 560;
            paddle2.YPos = 150;
            paddle2.Speed = 0;
        }

        private Rectangle MakePaddleSprite()
        {
            Rectangle sprite = new Rectangle();
            sprite.Width = 20;
            sprite.Height = 80;
            sprite.Fill = Brushes.Green;
            GameCourt.Children.Add(sprite);
            return sprite;
        }

        private void Update()
        {
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            timer.Tick += TimerHandler;
            timer.Start();
        }

        private void TimerHandler(object sender, EventArgs e)
        {
            MovePaddle(paddle1);
            MovePaddle(paddle2);
            MoveBall(ball);
            updateDisplay();
        }

        private void MoveBall(Ball ball)
        {
            // change angle if ball hits paddle
            if (DetectHit())
            {
                //ball.Angle = 360 - ball.Angle;
                ball.Angle = GetNewAngle();
                ball.Speed *= 1.02;
            }
            
            // changle angle if ball hits top or bottom
            if (ball.YPos <= 0 | ball.YPos >= 370)
            {
                ball.Angle = 180 - ball.Angle;
            }      

            // add score and reset the ball if a player scored
            if (ball.XPos >= 570)
            {
                playerOneScoreLabel.Content = ++playerOneScore;
                ResetTheBall();
            }

            if (ball.XPos <= 0)
            {
                playerTwoScoreLabel.Content = ++playerTwoScore;
                ResetTheBall();
            }
            
            ball.XPos += Math.Sin(ball.Angle * (Math.PI / 180)) * ball.Speed;
            ball.YPos += Math.Cos(ball.Angle * (Math.PI / 180)) * ball.Speed;         
        }

        private void ResetTheBall()
        {
            if (ball.LastPaddle == 2)
            {
                ball.YPos = paddle2.YPos + paddle2.Sprite.Height / 2 - 10;
                ball.XPos = paddle2.XPos - 15;
                ball.Angle = 270;
                ball.Speed = 6;
            }
            else
            {
                ball.XPos = paddle1.XPos;
                ball.YPos = paddle1.YPos + paddle1.Sprite.Height / 2 - 10;
                ball.Angle = 90;
                ball.Speed = 6;
            }
        }

        private bool DetectHit()
        {
            // paddle1
            if (ball.LastPaddle != 1 && ball.XPos <= paddle1.XPos + 20 &&
                ball.YPos + 20 >= paddle1.YPos && ball.YPos <= paddle1.YPos + 80)
            {
                ball.LastPaddle = 1;
                return true;
            }

            // paddle2
            if (ball.LastPaddle != 2 && ball.XPos + 20 >= paddle2.XPos &&
                ball.YPos + 20 >= paddle2.YPos && ball.YPos <= paddle2.YPos + 80)
            {
                ball.LastPaddle = 2;
                return true;
            }
            else
                return false;
        }

        // calculates the return angle of the ball based upon where
        // the ball hits the paddle
        private double GetNewAngle()
        {
            double hitPosition; // 0 == top of paddle, 1 == bottom

            if (ball.LastPaddle == 1)
            {
                hitPosition = (ball.YPos + 10 - paddle1.YPos) / paddle1.Sprite.Height;
                return 60 + 60 * hitPosition;
            }
            else
            {
                hitPosition = (ball.YPos + 10 - paddle2.YPos) / paddle2.Sprite.Height;
                return 300 - 60 * hitPosition;
            }
        }

        private void MovePaddle(Paddle paddle)
        {
            if (paddle.Direction != 0)
                paddle.Speed += paddle.Direction * 0.5;
            else
            {
                if (Math.Abs(paddle.Speed) > 0.3)
                    paddle.Speed *= 0.9;
                else
                    paddle.Speed = 0;
            }

            paddle.YPos += paddle.Speed;
        }

        private void updateDisplay()
        {
            // udpate paddle1
            Canvas.SetLeft(paddle1.Sprite, paddle1.XPos);
            Canvas.SetTop(paddle1.Sprite, paddle1.YPos);

            // update paddle2
            Canvas.SetLeft(paddle2.Sprite, paddle2.XPos);
            Canvas.SetTop(paddle2.Sprite, paddle2.YPos);

            //update ball
            Canvas.SetLeft(ball.Sprite, ball.XPos);
            Canvas.SetTop(ball.Sprite,ball.YPos);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
                paddle2.Direction = -1;
            else if (e.Key == Key.Down)
                paddle2.Direction = 1;
            else if (e.Key == Key.A)
                paddle1.Direction = -1;
            else if (e.Key == Key.Z)
                paddle1.Direction = 1;
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up || e.Key == Key.Down)
                paddle2.Direction = 0;
            else if (e.Key == Key.A || e.Key == Key.Z)
                paddle1.Direction = 0;

        }
    }
}
