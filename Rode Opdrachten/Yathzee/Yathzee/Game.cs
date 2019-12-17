using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using Yathzee.Scores;

namespace Yathzee {

    static class Game {

        public static CacheScore[] cache = new CacheScore[5];

        private static int rollcount    = 0;
        private const int MAX_ROLLCOUNT = 3;

        private static int turn     = 0;
        private const int MAX_TURN  = 13;

        public static MainWindow window;

        public const double BOARDHEIGHT    = 600;
        public const double BOARDWIDTH     = 500;

        public static void init( MainWindow window ) {

            Game.window = window;
            for ( int i = 0; i < Dices.COUNT; i++ ) {

                Dices.dice[ i ].addToCanvas( window.canvasBoard );
            }
            HoldManager.addToCanvas( window.canvasBoard );

            Scorecard.addToCanvas( window.canvasScoreUpper, window.canvasScoreLower );
            for ( int i = 0; i < Scorecard.SCORECOUNT; i++ ) {

                Scorecard.list[ i ].onClick += onClickScore;
            }

            setBtnEnabled( true );
        }

        private static void resetRolls() {

            setBtnEnabled( true );

            for ( int i = 0; i < Dices.COUNT; i++ ) {

                Dices.dice[ i ].setHeld( false );
            }

            rollcount = 0;
            roll();
        }

        private static void finish() {

            int spot = -1;

            // find leftmost
            for ( int i = 0; i < cache.Length; i++ ) {

                if ( cache[ i ] == null ) {

                    spot = i;
                    break;
                }
            }

            if ( spot < 0 ) { // Make room

                for ( int i = 1; i < cache.Length; i++ ) {

                    cache[ i-1 ] = cache[ i ];
                }

                spot = cache.Length - 1;
            }

            cache[ spot ] = new CacheScore();
            ScoreForm.Execute();
        }

        private static void setBtnEnabled( bool enabled ) {

            window.btnRoll.IsEnabled    = enabled;
            window.btnRoll.Foreground   = ( enabled ) ? Brushes.White : new SolidColorBrush( Color.FromArgb( 0xFF, 0x2D, 0x91, 0x68 ) );
        }

        private static void onClickScore( object sender, MouseButtonEventArgs e ) {

            BaseScore score = (BaseScore)sender;

            // Extra yahtzee ?
            if ( ( Scorecard.list[ Scorecard.SCORE_YAHTZEE ].potentailValue() > 0 ) && ( !Scorecard.list[ Scorecard.SCORE_YAHTZEE ].isOpen() ) ) {

                ( (ScoreYahtzee)Scorecard.list[ Scorecard.SCORE_YAHTZEE ] ).addBonus();
            }

            score.activate();
            Scorecard.updateScores();
            if ( ( window.textBonus.Visibility == System.Windows.Visibility.Visible ) && ( Scorecard.hasBonus() ) ) {
                window.textBonus.Visibility = System.Windows.Visibility.Hidden;
            }

            turn++;
            if ( turn >= MAX_TURN ) { // End of game?

                finish();
                return;
            }

            window.textTurn.Text    = String.Format( "Beurt: {0}", turn + 1 );
            window.textScore.Text   = String.Format( "Score: {0}", Scorecard.totalScore() );

            resetRolls();
        }

        public static void start() {

            turn = 0;
            Scorecard.reset();

            window.textTurn.Text    = String.Format( "Beurt: {0}", turn + 1 );
            window.textScore.Text   = String.Format( "Score: {0}", Scorecard.totalScore() );
            window.textBonus.Visibility = System.Windows.Visibility.Visible;

            resetRolls();
        }

        public static void roll() {

            rollcount++;
            Dices.roll();

            if ( rollcount >= MAX_ROLLCOUNT ) {

                setBtnEnabled( false );
            }

            Scorecard.updatePreview(); // update the scores
            window.textRolls.Text = String.Format( "Worpen over: {0}", MAX_ROLLCOUNT - rollcount );
        }
    }
}
