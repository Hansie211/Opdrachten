using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BoterKaasEieren {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public enum fieldValue { None, Cross, Circle };

    public struct moveScore {

        public int index;
        public int score;
    }


    public partial class MainWindow : Window {

        private fieldValue[] field;

        private const int fieldSize = 3;
        private const int squareMargin = 4;

        private int boardWidth, boardHeight;
        private int squareSizeX, squareSizeY;

        private const fieldValue player = fieldValue.Circle;
        private const fieldValue ai     = fieldValue.Cross;

        private readonly string backgroundColor = "#FFFFFF";
        private readonly string contentColor    = "#FFFFFF";
        private readonly string highlightColor  = "#F04436";


        public MainWindow() {
            InitializeComponent();

            // Create the playing field
            field = new fieldValue[ fieldSize * fieldSize ];
        }

        private Brush hexToColor( string hex ) {

            return (Brush)( new BrushConverter().ConvertFrom( hex ) );
        }

        private void drawLineOnBoard( int X1, int X2, int Y1, int Y2, Brush color, int thickness ) {

            Line line = new Line();

            line.Stroke = color;
            line.StrokeThickness = thickness;

            line.X1 = X1;
            line.X2 = X2;
            line.Y1 = Y1;
            line.Y2 = Y2;

            // Give the line a rounded edge
            line.StrokeStartLineCap = PenLineCap.Round;
            line.StrokeEndLineCap   = PenLineCap.Round;

            canvas.Children.Add( line );
        }

        private void drawBoard() {

            Brush color     = hexToColor(backgroundColor);
            int thickness   = 3;

            // Vertical lines
            drawLineOnBoard( squareSizeX, squareSizeX, 0, boardHeight, color, thickness );
            drawLineOnBoard( boardWidth - squareSizeX, boardWidth - squareSizeX, 0, boardHeight, color, thickness );

            // Horizontal lines
            drawLineOnBoard( 0, boardWidth, squareSizeY, squareSizeY, color, thickness );
            drawLineOnBoard( 0, boardWidth, boardHeight - squareSizeY, boardHeight - squareSizeY, color, thickness );

        }

        private void drawSquareX( Rect rect, Brush color, int thickness ) {

            drawLineOnBoard( (int)rect.Left, (int)rect.Right, (int)rect.Top, (int)rect.Bottom, color, thickness );
            drawLineOnBoard( (int)rect.Left, (int)rect.Right, (int)rect.Bottom, (int)rect.Top, color, thickness );
        }

        private void drawSquareO( Rect rect, Brush color, int thickness ) {

            Ellipse ellipse = new Ellipse();

            ellipse.Width  = rect.Width;
            ellipse.Height = rect.Height;

            ellipse.Stroke          = color;
            ellipse.StrokeThickness = thickness;

            // Move the object to it's x & y
            ellipse.SetValue( Canvas.LeftProperty, rect.Left );
            ellipse.SetValue( Canvas.TopProperty, rect.Top );

            canvas.Children.Add( ellipse );
        }

        private void drawSquare( int x, int y, Boolean active = false ) {

            // The square to draw
            int square = x + y * fieldSize;

            // Setup a rect as outline
            Rect rect = new Rect( x * squareSizeX + squareMargin, y * squareSizeY + squareMargin, squareSizeX - squareMargin * 2, squareSizeY - squareMargin * 2 );

            // Decide on the colors
            Brush color     = (active) ? hexToColor( highlightColor ) : hexToColor( contentColor );
            int thickness   = (active) ? 8 : 3;


            switch ( field[ square ] ) {

                case fieldValue.Circle:
                    drawSquareO( rect, color, thickness );
                    break;
                case fieldValue.Cross:
                    drawSquareX( rect, color, thickness );
                    break;
            }

        }

        private void drawSquare( int square, Boolean active = false ) {

            int x, y;
            toXY( square, out x, out y );

            drawSquare( x, y, active );
        }

        private void clearBoard() {

            // Clear out the canvas
            canvas.Children.Clear();

            // Set all fields back to None
            for ( int i = 0; i < field.Length; i++ ) {
                field[ i ] = fieldValue.None;
            }

            // Draw the board outline
            drawBoard();
        }

        private Boolean isThreeInARow( fieldValue[] board, int[] squares, fieldValue comp ) {

            // return True if all three squares equal the comp
            for ( int i = 0; i < squares.Length; i++ ) {
                if ( board[ squares[ i ] ] != comp ) {
                    return false;
                }
            }

            return true;
        }

        private Boolean hasWon( fieldValue val, fieldValue[] board, out int sq1, out int sq2, out int sq3 ) {
            // Basic board layout:
            // 0,1,2
            // 3,4,5
            // 6,7,8

            // Check horizontal lines
            for ( int y = 0; y < fieldSize; y++ ) {

                sq1 = y * 3 + 0;
                sq2 = y * 3 + 1;
                sq3 = y * 3 + 2;

                if ( isThreeInARow( board, new int[] { sq1, sq2, sq3 }, val ) ) {
                    return true;
                }
            }

            // Check vertical lines
            for ( int x = 0; x < fieldSize; x++ ) {

                sq1 = 0 * 3 + x;
                sq2 = 1 * 3 + x;
                sq3 = 2 * 3 + x;

                if ( isThreeInARow( board, new int[] { sq1, sq2, sq3 }, val ) ) {
                    return true;
                }
            }

            // Check diagonal lines
            sq1 = 0;
            sq2 = 4;
            sq3 = 8;
            if ( isThreeInARow( board, new int[] { sq1, sq2, sq3 }, val ) ) {
                return true;
            }

            sq1 = 2;
            sq2 = 4;
            sq3 = 6;
            if ( isThreeInARow( board, new int[] { sq1, sq2, sq3 }, val ) ) {
                return true;
            }

            return false;
        }

        private Boolean hasWon( fieldValue val, fieldValue[] board ) {

            int d1, d2, d3; // dummy
            return hasWon( val, board, out d1, out d2, out d3 );
        }

        private int getFreeSquares( fieldValue[] board ) {

            int result = 0;

            for ( int i = 0; i < board.Length; i++ ) {
                if ( board[ i ] == fieldValue.None ) {
                    result++;
                }
            }

            return result;
        }

        private int[] getAllMoves( fieldValue[] board ) {

            // Return a list of all indexes that are free on this board

            int[] result = new int[ field.Length ];

            int squareIndex = 0;
            for ( int i = 0; i < board.Length; i++ ) {

                if ( board[ i ] == fieldValue.None ) {

                    result[ squareIndex ] = i;
                    squareIndex++;
                }
            }

            // Trim the result
            Array.Resize( ref result, squareIndex );

            return result;
        }

        private int getBoardScore( fieldValue[] board ) {

            if ( hasWon( ai, board ) ) {
                return 1;
            }

            if ( hasWon( player, board ) ) {
                return -1;
            }

            return 0;
        }

        private fieldValue counterPlayer( fieldValue turn ) {

            // Switch the turn
            if ( turn == ai ) {
                return player;
            }

            return ai;
        }

        private fieldValue[] makeMove( fieldValue[] board, int move, fieldValue turn ) {

            // Create a new board
            fieldValue[] result = new fieldValue[ board.Length ];
            Array.Copy( board, 0, result, 0, board.Length );

            // Make the move on the new board
            result[ move ] = turn;

            // Return the new board
            return result;
        }


        private int getBestMove( fieldValue[] board, fieldValue turn, out int score ) {

            // Basic
            switch ( getBoardScore( board ) ) {
                case 1:
                    score = 1;
                    return -1;
                case -1:
                    score = -1;
                    return -1;
            }

            if ( getFreeSquares( board ) == 0 ) {
                score = 0; // draw  
                return -1;
            }

            // No direct result ? Next round
            int[] moves  = getAllMoves( board );
            int[] scores = new int[ moves.Length ];

            // Make all the moves
            for ( int i = 0; i < moves.Length; i++ ) {

                fieldValue[] newBoard = makeMove( board, moves[i], turn );
                // Recursion, would we win or loose by making this move ?
                getBestMove( newBoard, counterPlayer( turn ), out scores[ i ] );
            }

            int index = 0;

            // sore loser: can't win, then draw
            for ( int i = 0; i < scores.Length; i++ ) {
                if ( scores[ i ] == 0 ) {
                    index = i;
                    break;
                }
            }

            // What's the optimal play?
            for ( int i = 0; i < scores.Length; i++ ) {

                // AI wants to win
                if ( ( turn == ai ) && ( scores[ i ] > 0 ) ) {
                    index = i;
                    break;
                }

                // Player wants to win as well
                if ( ( turn == player ) && ( scores[ i ] < 0 ) ) {
                    index = i;
                    break;
                }

            }

            score = scores[ index ];
            return moves[ index ];
        }

        private void toXY( int square, out int x, out int y ) {

            x = square % fieldSize;
            y = square / fieldSize;
        }

        private void makeAIMove() {

            int bestScore;
            int bestMove = getBestMove( field, ai, out bestScore );

            if ( bestMove < 0 ) {
                return;
            }

            // Make the move
            field[ bestMove ] = ai;

            // Update the board
            int bestMoveX, bestMoveY;
            toXY( bestMove, out bestMoveX, out bestMoveY );

            drawSquare( bestMoveX, bestMoveY );
        }

        private void highlightWinner( fieldValue winner ) {

            // Get the winning squares
            int sq1, sq2, sq3;
            hasWon( winner, field, out sq1, out sq2, out sq3 );

            drawSquare( sq1, true );
            drawSquare( sq2, true );
            drawSquare( sq3, true );

            // This code is a little to much
            //int x, y;
            //int x1, x2, y1, y2;

            //toXY( sq1, out x, out y );
            //x1 = x * squareSizeX + ( squareSizeX / 3 );
            //y1 = y * squareSizeY + ( squareSizeY / 3 );

            //toXY( sq3, out x, out y );
            //x2 = x * squareSizeX + ( squareSizeX - squareSizeX / 3 );
            //y2 = y * squareSizeY + ( squareSizeY - squareSizeY / 3 );

            //drawLineOnBoard( x1, x2, y1, y2, Brushes.Red, 16 );
        }

        private void Window_Loaded( object sender, RoutedEventArgs e ) {

            // ActualWidth / ActualHeight are only useable after the canvas is loaded
            boardWidth  = (int)canvas.ActualWidth;
            boardHeight = (int)canvas.ActualHeight;

            squareSizeX = (int)boardWidth / fieldSize;
            squareSizeY = (int)boardHeight / fieldSize;

            // Setup the board for the first time
            clearBoard();
        }

        private void Canvas_MouseLeftButtonUp( object sender, MouseButtonEventArgs e ) {

            // Is the result label still over the board?
            if ( lblResult.Visibility != Visibility.Hidden ) {

                // Hide it again
                lblResult.Visibility = Visibility.Hidden;

                // Reset the game
                clearBoard();

                return;
            }

            // Get the square beneath the cursor
            Point p = e.GetPosition( canvas );

            int squareX = (int)Math.Floor( p.X / squareSizeX );
            int squareY = (int)Math.Floor( p.Y / squareSizeY );

            int square  = squareX + squareY * fieldSize;

            // Is this square taken ?
            if ( field[ square ] != fieldValue.None ) {

                return;
            }

            // Make the move
            field[ square ] = player;
            // Update the board
            drawSquare( squareX, squareY, false );

            // Game end?
            if ( hasWon( player, field ) ) {

                // This will never happen

                // Increase the displayed score
                int t;
                lblScorePlayer.Content = int.TryParse( lblScorePlayer.Content.ToString(), out t );
                lblScorePlayer.Content = ( t + 1 ).ToString();

                // Show a message
                lblResult.Content = "YOU WIN";
                lblResult.Visibility = Visibility.Visible;

                highlightWinner( player );

                return;
            }

            // Player did not win, so it is AI's turn
            makeAIMove();

            // Game end?
            if ( hasWon( ai, field ) ) {

                // Increase the displayed score
                int t;
                lblScoreAI.Content = int.TryParse( lblScoreAI.Content.ToString(), out t );
                lblScoreAI.Content = ( t+1 ).ToString();

                // Show a message
                lblResult.Content = "YOU LOSE";
                lblResult.Visibility = Visibility.Visible;

                highlightWinner( ai );

                return;
            }

            // Draw?
            if ( getFreeSquares( field ) == 0 ) {

                // No one wins
                lblResult.Content = "DRAW";
                lblResult.Visibility = Visibility.Visible;
            }

        }
    }
}
