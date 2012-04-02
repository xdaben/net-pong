using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PongServer
{
    public class GameLogic
    {
        //public Paddle paddle1, paddle2;
        public Ball ball;
        List<Player> players;
        //public int playerOneScore, playerTwoScore;

        public GameLogic(List<Player> players)
        {
            ball = new Ball(90, 6, 40, 180);
            this.players = players;
            InitPaddles();

            //playerOneScore, playerTwoScore;
        }

        private void InitPaddles()
        {
            
            players[0].XPos = 15;
            players[0].YPos = 150;

            
            players[1].XPos = 560;
            players[1].YPos = 150;
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
            if (ball.XPos >= 580)
            {
                players[0].Score++;
                ResetTheBall();
            }

            if (ball.XPos <= 0)
            {
                players[1].Score++;
                ResetTheBall();
            }

            ball.XPos += Math.Sin(ball.Angle * (Math.PI / 180)) * ball.Speed;
            ball.YPos += Math.Cos(ball.Angle * (Math.PI / 180)) * ball.Speed;
        }

        public void ResetTheBall()
        {
            if (ball.LastPaddle == 2)
            {
                ball.XPos = players[1].XPos;
                ball.YPos = players[1].YPos + players[1].Height / 2 - 10;                
                ball.Angle = 270;
                ball.Speed = 6;
            }
            else
            {
                ball.XPos = players[0].XPos + 10;
                ball.YPos = players[0].YPos + players[0].Height / 2 - 10;
                ball.Angle = 90;
                ball.Speed = 6;
            }
        }

        public bool DetectHit()
        {
            // paddle1
            if (ball.LastPaddle != 1 && ball.XPos <= players[0].XPos + 20 &&
                ball.YPos + 20 >= players[0].YPos && ball.YPos <= players[0].YPos + 80)
            {
                ball.LastPaddle = 1;
                return true;
            }

            // paddle2
            if (ball.LastPaddle != 2 && ball.XPos + 20 >= players[1].XPos &&
                ball.YPos + 20 >= players[1].YPos && ball.YPos <= players[1].YPos + 80)
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
                hitPosition = (ball.YPos + 10 - players[0].YPos) / players[0].Height;
                return 60 + 60 * hitPosition;
            }
            else
            {
                hitPosition = (ball.YPos + 10 - players[1].YPos) / players[1].Height;
                return 300 - 60 * hitPosition;
            }
        }

        //THIS SHOULD BE CLIENTSIDE
        //public void MovePaddle(Paddle paddle)
        //{
        //    if (paddle.Direction != 0)
        //        paddle.Speed += paddle.Direction * 0.5;
        //    else
        //    {
        //        if (Math.Abs(paddle.Speed) > 0.3)
        //            paddle.Speed *= 0.9;
        //        else
        //            paddle.Speed = 0;
        //    }

        //    paddle.YPos += paddle.Speed;
        //}

    }
}