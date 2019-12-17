using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yathzee.Scores;

namespace Yathzee {
    class CacheScore {

        public readonly int upperTotal;
        public readonly bool hasBonus;
        public readonly int upperGrandTotal;

        public readonly int lowerTotal;
        public readonly int grandTotal;

        public readonly int[] scores;

        public static void zeroChart( int index, ref string html ) {

            string id = String.Format("%SCORE{0}-{1}%", index, "{0}");

            for ( int i = 0; i < Scorecard.SCORECOUNT; i++ ) {

                html = html.Replace( String.Format( id, i ), "" );
            }

            html = html.Replace( String.Format( id, "UPTOT" ), "" );
            html = html.Replace( String.Format( id, "UPBONUS" ), "" );
            html = html.Replace( String.Format( id, "UPGTOT" ), "" );
            html = html.Replace( String.Format( id, "LOWTOT" ), "" );
            html = html.Replace( String.Format( id, "GTOT" ), "" );
        }

        public CacheScore() {

            scores = new int[ Scorecard.SCORECOUNT ];
            for ( int i = 0; i < scores.Length; i++ ) {

                scores[ i ] = Scorecard.list[ i ].getValue();
            }

            upperTotal      = Scorecard.upperHalfScore();
            hasBonus        = upperTotal >= Scorecard.UPPER_BONUS_MIN;
            upperGrandTotal = upperTotal + ( hasBonus ? Scorecard.UPPER_BONUS : 0 );

            lowerTotal  = Scorecard.lowerHalfScore();
            grandTotal  = upperGrandTotal + lowerTotal;
        }

        public void addToChart( int index, ref string html ) {

            string id = String.Format("%SCORE{0}-{1}%", index, "{0}");

            for ( int i = 0; i < scores.Length; i++ ) {

                html = html.Replace( String.Format( id, i ), scores[ i ].ToString() );
            }

            html = html.Replace( String.Format( id, "UPTOT" ), upperTotal.ToString() );
            html = html.Replace( String.Format( id, "UPBONUS" ), hasBonus ? Scorecard.UPPER_BONUS.ToString() : "0" );
            html = html.Replace( String.Format( id, "UPGTOT" ), upperGrandTotal.ToString() );
            html = html.Replace( String.Format( id, "LOWTOT" ), lowerTotal.ToString() );
            html = html.Replace( String.Format( id, "GTOT" ), grandTotal.ToString() );
        }
    }
}
