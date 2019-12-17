using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Yathzee.Scores {
    static class Scorecard {

        public const int SCORECOUNT = 13;

        public const int SCORE_ONES             = 0;
        public const int SCORE_TWOS             = 1;
        public const int SCORE_THREES           = 2;
        public const int SCORE_FOURS            = 3;
        public const int SCORE_FIVES            = 4;
        public const int SCORE_SIXES            = 5;
        public const int SCORE_THREE_OF_A_KIND  = 6;
        public const int SCORE_FOUR_OF_A_KIND   = 7;
        public const int SCORE_SMALL_STRAIGHT   = 8;
        public const int SCORE_LARGE_STRAIGHT   = 9;
        public const int SCORE_FULL_HOUSE       = 10;
        public const int SCORE_YAHTZEE          = 11;
        public const int SCORE_CHANCE           = 12;

        public const int UPPER_HALF             = 6;
        public const int UPPER_BONUS            = 35;
        public const int UPPER_BONUS_MIN        = 63;

        public const double SCOREHEIGHT            = 60;
        public const double SCOREWIDTH             = 340;

        public static BaseScore[] list = new BaseScore[ SCORECOUNT ];

        static Scorecard() {

            list[ SCORE_ONES ]     = new ScoreCount( 1, "Enen" );
            list[ SCORE_TWOS ]     = new ScoreCount( 2, "Tweeën" );
            list[ SCORE_THREES ]   = new ScoreCount( 3, "Drieën" );
            list[ SCORE_FOURS ]    = new ScoreCount( 4, "Vieren" );
            list[ SCORE_FIVES ]    = new ScoreCount( 5, "Vijven" );
            list[ SCORE_SIXES ]    = new ScoreCount( 6, "Zessen" );

            list[ SCORE_THREE_OF_A_KIND ]  = new ScoreKind( 3, "3 of a kind" );
            list[ SCORE_FOUR_OF_A_KIND ]   = new ScoreKind( 4, "4 of a kind" );
            list[ SCORE_SMALL_STRAIGHT ]   = new ScoreStraight( 4, 30, "Kleine straat" );
            list[ SCORE_LARGE_STRAIGHT ]   = new ScoreStraight( 5, 40, "Grote straat" );
            list[ SCORE_FULL_HOUSE ]       = new ScoreFullhouse( "Full House" );
            list[ SCORE_CHANCE ]           = new ScoreChance( "Kans" );
            list[ SCORE_YAHTZEE ]          = new ScoreYahtzee( "Yahtzee" );
        }

        public static void reset() {

            for ( int i = 0; i < list.Length; i++ ) {

                list[ i ].reset();
            }

            updateScores();
        }

        public static void addToCanvas( Canvas canvasUpper, Canvas canvasLower ) {

            for ( int i = 0; i < SCORECOUNT; i++ ) {

                double y = ( i < UPPER_HALF ) ? i : i - UPPER_HALF;
                y *= SCOREHEIGHT;

                list[ i ].addToCanvas( ( i < UPPER_HALF ) ? canvasUpper : canvasLower, 0, y );
            }
        }

        public static void updateScores() {
            Game.window.textScoreLower.Text = String.Format( "Totaal: {0}", lowerHalfScore() );
            Game.window.textScoreUpper.Text = String.Format( "Totaal: {0}", upperHalfScore() );

            if ( hasBonus() ) {
                Game.window.textScoreUpper.Text += String.Format( " (+{0})", 35 );
            }
        }

        public static void updatePreview() {

            for ( int i = 0; i < SCORECOUNT; i++ ) {

                list[ i ].updatePreview();
            }
        }

        public static int upperHalfScore() {

            int result = 0;

            for ( int i = 0; i < UPPER_HALF; i++ ) {

                result  += list[ i ].getValue();
            }

            return result;
        }

        public static int lowerHalfScore() {
            int result = 0;

            for ( int i = UPPER_HALF; i < SCORECOUNT; i++ ) {

                result  += list[ i ].getValue();
            }

            return result;
        }

        public static bool hasBonus() {

            return upperHalfScore() >= UPPER_BONUS_MIN;
        }

        public static int totalScore() {

            int result = lowerHalfScore() + upperHalfScore();

            if ( hasBonus() ) {

                result += UPPER_BONUS;
            }

            return result;
        }

    }
}
