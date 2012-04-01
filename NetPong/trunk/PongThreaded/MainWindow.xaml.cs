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

namespace PongThreaded
{
    public partial class MainWindow : Window
    {
        Rectangle paddleOneSprite, paddleTwoSprite, ballSprite;
        GameLogic game;
        DispatcherTimer /*logicTimer,*/ displayTimer;
        Thread logicThread, displayThread;
        private int player;
        Networking network;

        public MainWindow()
        {
            InitializeComponent();
            InitSprites();
            InitNetwork();
            //Temporary, use some sort of input to get the address            
            game = new GameLogic();
            Update();
        }

        private void InitNetwork()
        {
            network = new Networking("192.168.0.176");
            Thread.Sleep(1000);
            player = network.Player;
            Console.WriteLine("Player {0}", player);
        }

        private void InitSprites()
        {
            ballSprite = MakeBallSprite();
            Canvas.SetLeft(ballSprite, 35);
            Canvas.SetTop(ballSprite, 180);

            paddleOneSprite = MakePaddleSprite();
            Canvas.SetLeft(paddleOneSprite, 15);
            Canvas.SetTop(paddleOneSprite, 150);

            paddleTwoSprite = MakePaddleSprite();
            Canvas.SetLeft(paddleTwoSprite, 560);
            Canvas.SetTop(paddleTwoSprite, 150);
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

        // creates and starts the timers that handle logic
        // and display upates
        private void Update()
        {
            //logicTimer = new DispatcherTimer();
            //logicTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            //logicTimer.Tick += LogicTimerHandler;
            //logicTimer.Start();

            logicThread = new Thread(new ThreadStart(LogicUpdate));
            logicThread.Start();

            //Thread displayThread = new Thread(new ThreadStart(UpdateDisplay));
            //displayThread.Start();

            displayTimer = new DispatcherTimer();
            displayTimer.Interval = new TimeSpan(0, 0, 0, 0, 15);
            displayTimer.Tick += DisplayTimerHandler;
            displayTimer.Start();
        }

        private void LogicUpdate()
        {
            while (true)
            {
                NetworkUpdate();
                game.MovePaddle(game.paddle1);
                game.MovePaddle(game.paddle2);
                game.MoveBall(game.ball);
                Thread.Sleep(15);
            }
        }

        private void NetworkUpdate()
        {
            StringBuilder sb = new StringBuilder();
            if (player == 1)
            {
                sb.AppendFormat("{0} {1} {2}", game.paddle1.XPos, game.paddle1.YPos, game.playerOneScore);
            }
            if (player == 2)
            {
                sb.AppendFormat("{0} {1} {2}", game.paddle2.XPos, game.paddle2.YPos, game.playerTwoScore);
            }
            network.ToSend = sb.ToString();
            if (player == 1)
            {
                double xPos, yPos;
                int score;
                network.GetData(out xPos, out yPos, out score);
                Console.WriteLine(xPos);
                game.paddle2.XPos = xPos;
                game.paddle2.YPos = yPos;
                game.playerTwoScore = score;
            }
            if (player == 2)
            {
                double xPos, yPos;
                int score;
                network.GetData(out xPos, out yPos, out score);
                game.paddle1.XPos = xPos;
                game.paddle1.YPos = yPos;
                game.playerOneScore = score;
            }
        }

        private void LogicTimerHandler(object sender, EventArgs e)
        {
            if (player == 1)
            {
                game.MovePaddle(game.paddle1);
            }
            if (player == 2)
            {
                game.MovePaddle(game.paddle2);
            }
            game.MoveBall(game.ball);
        }

        private void DisplayTimerHandler(object sender, EventArgs e)
        {
            UpdateDisplay();
        }


        private void UpdateDisplay()
        {
            //update ball
            Canvas.SetLeft(ballSprite, game.ball.XPos);
            Canvas.SetTop(ballSprite, game.ball.YPos);

            // udpate paddle1
            Canvas.SetLeft(paddleOneSprite, game.paddle1.XPos);
            Canvas.SetTop(paddleOneSprite, game.paddle1.YPos);

            // update paddle2
            Canvas.SetLeft(paddleTwoSprite, game.paddle2.XPos);
            Canvas.SetTop(paddleTwoSprite, game.paddle2.YPos);

            // update score labels
            playerOneScoreLabel.Content = game.playerOneScore;
            playerTwoScoreLabel.Content = game.playerTwoScore;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.Up)
            //    game.paddle2.Direction = -1;
            //else if (e.Key == Key.Down)
            //    game.paddle2.Direction = 1;
            //else if (e.Key == Key.A)
            //    game.paddle1.Direction = -1;
            //else if (e.Key == Key.Z)
            //    game.paddle1.Direction = 1;

            if (player == 1)
            {
                if (e.Key == Key.Up)
                    game.paddle2.Direction = -1;
                else
                    if (e.Key == Key.Down)
                        game.paddle2.Direction = 1;
            }
            else
                if (player == 2)
                {
                    if (e.Key == Key.Up)
                        game.paddle1.Direction = -1;
                    else
                        if (e.Key == Key.Down)
                            game.paddle1.Direction = 1;
                }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.Up || e.Key == Key.Down)
            //    game.paddle2.Direction = 0;
            //else if (e.Key == Key.A || e.Key == Key.Z)
            //    game.paddle1.Direction = 0;

            if (player == 1)
            {
                if (e.Key == Key.Up || e.Key == Key.Down)
                    game.paddle2.Direction = 0;
            }
            else
                if (player == 2)
                {
                    if (e.Key == Key.Up || e.Key == Key.Down)
                        game.paddle1.Direction = 0;
                }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            logicThread.Abort();
        }
    }
}