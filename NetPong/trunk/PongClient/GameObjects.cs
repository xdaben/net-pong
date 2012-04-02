using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PongClient
{
    class GameObjects
    {
        #region fields
        private double ballXPos, ballYPos;
        private double paddle1XPos, paddle1YPos;
        private double paddle2XPos, paddle2YPos;
        private double player1Score, player2Score;
        #endregion

        #region properties
        public double BallXPos
        {
            get { return ballXPos; }
            set { ballXPos = value; }
        }

        public double BallYPos
        {
            get { return ballYPos; }
            set { ballYPos = value; }
        }

        public double Paddle1XPos
        {
            get { return paddle1XPos; }
            set { paddle1XPos = value; }
        }

        public double Paddle1YPos
        {
            get { return paddle1YPos; }
            set { paddle1YPos = value; }
        }

        public double Paddle2XPos
        {
            get { return paddle2XPos; }
            set { paddle2XPos = value; }
        }

        public double Paddle2YPos
        {
            get { return paddle2YPos; }
            set { paddle2YPos = value; }
        }

        public double Player1Score
        {
            get { return player1Score; }
            set { player1Score = value; }
        }

        public double Player2Score
        {
            get { return player2Score; }
            set { player2Score = value; }
        }
        #endregion

    }
}
