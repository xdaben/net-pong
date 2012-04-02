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
using System.Threading;

namespace PongClient
{
    /// <summary>
    /// doubleeraction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Rectangle paddle1Sprite, paddle2Sprite, ballSprite;
        GameObjects game = new GameObjects();
        Thread networkThread;
        DispatcherTimer displayTimer;
        Networking net;
        int direction;
        int playerNumber;

        public MainWindow()
        {
            InitializeComponent();
            InitSprites();
            ConnectToServer();
            Update();
        }

        #region network initializers
        private void ConnectToServer()
        { 
            /*
             *  Okay, we need to connect to the server,
             *  receive the assigned player number,
             *  and initialize the game object coordinates.
             */
            net = new Networking("192.168.0.176", game);
        }
        #endregion

        #region display initializers
        private void InitSprites()
        {
            ballSprite = MakeBallSprite();
            Canvas.SetLeft(ballSprite, 35);
            Canvas.SetTop(ballSprite, 180);

            paddle1Sprite = MakePaddleSprite();
            Canvas.SetLeft(paddle1Sprite, 15);
            Canvas.SetTop(paddle1Sprite, 150);

            paddle2Sprite = MakePaddleSprite();
            Canvas.SetLeft(paddle2Sprite, 560);
            Canvas.SetTop(paddle2Sprite, 150);

            // these assignments are temporary, just to make
            // the UI look pretty until the client-server stuff
            // is done
            game.BallXPos = Canvas.GetLeft(ballSprite);
            game.BallYPos = Canvas.GetTop(ballSprite);

            game.Paddle1XPos = Canvas.GetLeft(paddle1Sprite);
            game.Paddle1YPos = Canvas.GetTop(paddle1Sprite);

            game.Paddle2XPos = Canvas.GetLeft(paddle2Sprite);
            game.Paddle2YPos = Canvas.GetTop(paddle2Sprite);
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

        private Rectangle MakeBallSprite()
        {
            Rectangle sprite = new Rectangle();
            sprite.Width = 20;
            sprite.Height = 20;
            sprite.Fill = Brushes.White;
            GameCourt.Children.Add(sprite);
            return sprite;
        }
        #endregion

        private void Update()
        {
            networkThread = new Thread(new ThreadStart(NetworkUpdate));
            networkThread.Start(); 
            
            displayTimer = new DispatcherTimer();
            displayTimer.Interval = new TimeSpan(0, 0, 0, 0, 15);
            displayTimer.Tick += DisplayTimerHandler;
            displayTimer.Start();
        }

        private void NetworkUpdate()
        {
            while (true)
            {
                /*
                 *  Here we need to tell the server which
                 *  direction the client's paddle is moving,
                 *  and receive the new coordinates and updated
                 *  scores.
                 */
                net.ToSend = direction.ToString();
                
                Thread.Sleep(10); // we can tweak this value if the buffer overflows
            }
        }

        private void DisplayTimerHandler(object sender, EventArgs e)
        {
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            //update ball
            Canvas.SetLeft(ballSprite, game.BallXPos);
            Canvas.SetTop(ballSprite, game.BallYPos);

            // udpate paddle1
            Canvas.SetLeft(paddle1Sprite, game.Paddle1XPos);
            Canvas.SetTop(paddle1Sprite, game.Paddle1YPos);

            // update paddle2
            Canvas.SetLeft(paddle2Sprite, game.Paddle2XPos);
            Canvas.SetTop(paddle2Sprite, game.Paddle2YPos);

            // update score labels
            playerOneScoreLabel.Content = game.Player1Score;
            playerTwoScoreLabel.Content = game.Player2Score;
        }

        #region user input
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
                direction = -1;
            if (e.Key == Key.Down)
                direction = 1;
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up || e.Key == Key.Down)
                direction = 0;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            networkThread.Abort();
            
        }
        #endregion
    }
}
