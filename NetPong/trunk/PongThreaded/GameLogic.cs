using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PongThreaded
{
    public class GameLogic
    {
        public Paddle paddle1, paddle2;
        public Ball ball;

        public int playerOneScore, playerTwoScore;

        public GameLogic()
        {
            ball = new Ball(90, 6, 40, 180);
            InitPaddles();

            //playerOneScore, playerTwoScore;
        }

        private void InitPaddles()
        {
            paddle1 = new Paddle(1);
            paddle1.XPos = 15;
            paddle1.YPos = 150;

            paddle2 = new Paddle(2);
            paddle2.XPos = 560;
            paddle2.YPos = 150;
        }

        public void MoveBall(Ball ball)
        {
            // change angle if ball hits paddle
            if (DetectHit())
            {
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
                playerOneScore++;
                ResetTheBall();
            }

            if (ball.XPos <= 0)
            {
                playerTwoScore++;
                ResetTheBall();
            }

            ball.XPos += Math.Sin(ball.Angle * (Math.PI / 180)) * ball.Speed;
            ball.YPos += Math.Cos(ball.Angle * (Math.PI / 180)) * ball.Speed;
        }

        public void ResetTheBall()
        {
            if (ball.LastPaddle == 2)
            {
                ball.YPos = paddle2.YPos + paddle2.Height / 2 - 10;
                ball.XPos = paddle2.XPos - 15;
                ball.Angle = 270;
                ball.Speed = 6;
            }
            else
            {
                ball.XPos = paddle1.XPos;
                ball.YPos = paddle1.YPos + paddle1.Height / 2 - 10;
                ball.Angle = 90;
                ball.Speed = 6;
            }
        }

        public bool DetectHit()
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
        public double GetNewAngle()
        {
            double hitPosition; // 0 == top of paddle, 1 == bottom

            if (ball.LastPaddle == 1)
            {
                hitPosition = (ball.YPos + 10 - paddle1.YPos) / paddle1.Height;
                return 60 + 60 * hitPosition;
            }
            else
            {
                hitPosition = (ball.YPos + 10 - paddle2.YPos) / paddle2.Height;
                return 300 - 60 * hitPosition;
            }
        }

        public void MovePaddle(Paddle paddle)
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

    }
}