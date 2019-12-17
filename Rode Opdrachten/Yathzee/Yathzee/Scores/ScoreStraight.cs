using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yathzee.Scores {
    class ScoreStraight : BaseScore {

        private int rowsize;
        private int points;

        public ScoreStraight( int rowsize, int points, string name ) : base( name ) {

            this.rowsize = rowsize;
            this.points = points;
        }

        protected bool getStraightEyes() {

            int minvalue = Dice.MAX - ( rowsize - 1 );

            for ( int i = Dice.MIN; i < minvalue + 1; i++ ) {

                if ( !Dices.hasValue( i ) ) {

                    continue;
                }

                bool straight = true;

                for ( int j = 1; j < rowsize; j++ ) {

                    if ( Dices.hasValue( i + j ) ) {

                        continue;
                    }

                    straight = false;
                    i = i + j; // skip this number, it does not exist
                    break;
                }

                if ( straight ) {

                    return true;
                }
            }

            return false;
        }

        public override int potentailValue() {

            if ( !getStraightEyes() ) {

                return 0;
            }

            return points;
        }

    }
}
