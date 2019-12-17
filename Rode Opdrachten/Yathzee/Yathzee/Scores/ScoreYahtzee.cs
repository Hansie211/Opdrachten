using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yathzee.Scores {
    class ScoreYahtzee : BaseScore {
        public ScoreYahtzee( string name ) : base( name ) {
        }

        public override int potentailValue() {

            int v1 = Dices.dice[0].getValue();
            for( int i = 1; i < Dices.COUNT; i++ ) {

                if ( Dices.dice[i].getValue() != v1 ) {

                    return 0;
                }
            }

            return 50;
        }

        public void addBonus() {

            setValue( value + 100 );
        }


    }
}
