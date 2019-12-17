using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Toren_van_Hanoi {

    public static class Game {

        public static GameWindow window;

        public static int score;
        public static int time;

        public static int diskCount;
        public static int stickCount;

        public static int boardWidth;
        public static int boardHeight;
        public static int baselineHeight;

        public static int maxTime;

        public static bool won;
        public static bool stopped;

        public static Difficulty difficulty;


        public static Disk[] disks;
        public static Stick[] sticks;

        private static DispatcherTimer timer = new DispatcherTimer();

        static Game() {

            timer.Tick += Game.onTick;
            timer.Interval = new TimeSpan( 0, 0, 1 );
        }

        private static void initDisks() {

            Game.disks = new Disk[Game.diskCount];
            for ( int i = 0; i < Game.diskCount; i++ ) {

                disks[i] = new Disk( i );
                disks[i].onClick += Game.window.onClickDisk;

                disks[i].addToCanvas( window.canvas );
            }

        }

        private static void initSticks() {
            Game.sticks = new Stick[Game.stickCount];
            for ( int i = 0; i < Game.stickCount; i++ ) {

                sticks[i] = new Stick( i );
                window.canvas.Children.Add( sticks[i].shape );
            }
        }

        private static Line drawLine( int startX, int startY, int endX, int endY, int thickness = 2 ) {

            Line line = new Line();

            line.Stroke = Brushes.Black;

            line.X1 = startX;
            line.Y1 = startY;
            line.X2 = endX;
            line.Y2 = endY;

            line.StrokeThickness = thickness;
            return line;
        }

        public static void New( Difficulty difficulty ) {

            Game.difficulty = difficulty;

            switch ( Game.difficulty ) {
                case Difficulty.Easy:
                    Game.New( 1, 2, 30 );
                    break;
                case Difficulty.Medium:
                    Game.New( 4, 3, 60 * 3 );
                    break;
                case Difficulty.Hard:
                    Game.New( 8, 3, 60 * 8 );
                    break;
                case Difficulty.Expert:
                    Game.New( 12, 4, 60 * 20 );
                    break;

                default:
                    throw new Exception( String.Format( "Unknown difficulty '{0}'.", difficulty ) );
            }

        }

        private static void New( int diskCount, int stickCount, int maxTime ) {

            Game.diskCount  = diskCount;
            Game.stickCount = stickCount;
            Game.maxTime    = maxTime;

            Game.score  = 0;
            Game.time   = 0;
            Game.won     = false;
            Game.stopped = false;

            Game.boardWidth     = Disk.getSize( Game.diskCount ) * Game.stickCount + 150;
            Game.boardHeight    = 480;
            Game.baselineHeight = Game.boardHeight - 30;
            Stick.calculateProportions();

            Game.window = new GameWindow();
            Game.window.Closed += onWindowClosed;

            // Draw the baseline
            Game.window.canvas.Children.Add( drawLine( 0, Game.baselineHeight, Game.boardWidth, Game.baselineHeight ) );

            Game.initSticks();
            Game.initDisks();

            // Place the disks
            for ( int i = Game.diskCount - 1; i > -1; i-- ) {

                // Start at stick 0
                Game.disks[i].moveToStick( 0 );
            }


            timer.Start();

            window.Show();
            Application.Current.MainWindow.Hide();
        }

        public static bool finished() {

            for ( int i = 0; i < diskCount; i++ ) {

                if ( sticks[stickCount - 1].disks[i] < 0 ) {

                    // Not all disks are on the last stick
                    return false;
                }
            }

            return true;
        }

        public static void Win() {

            timer.Stop();
            window.labelFinish.Content = "Win!";
            window.labelFinish.Visibility = Visibility.Visible;

            Game.won = true;
        }

        public static void Loose() {

            timer.Stop();
            window.labelFinish.Content = "Loose!";
            window.labelFinish.Visibility = Visibility.Visible;
        }

        private static void addHighscore() {
            
            int index = HighscoreManager.getLowestIndex( Game.difficulty );

            if (( index > -1 ) && ( HighscoreManager.highscores[index].Score < Game.score )) {

                return; // not good enough
            }

            HighscoreWindow.Execute( Game.score, Game.difficulty );
        }

        public static void Stop() {

            timer.Stop();
            window.Close();

            // Add time to the score
            Game.score += Game.time;

            if ( won ) {

                addHighscore();
            }

            Application.Current.MainWindow.Show();
        }

        public static void increaseScore() {

            Game.score += 10;
            window.labelMoves.Content = String.Format( "Score: {0}", Game.score );
        }

        private static void onTick( object sender, EventArgs e ) {

            Game.time++;

            if ( Game.time > Game.maxTime ) {
                Game.Loose();
            }

            String str;
            if ( time < 60 ) {

                str = String.Format( "Time: {0}s", time );
            } else {

                int min = time / 60;
                int sec = time - ( min * 60 );

                str = String.Format( "Time: {0}m {1}s", min, sec );
            }

            window.labelTime.Content = str;
        }

        private static void onWindowClosed( object sender, EventArgs e ) {

            if ( !Game.stopped ) {

                Game.Stop();
            }
        }
    }

    public enum Difficulty { Easy, Medium, Hard, Expert };
}
