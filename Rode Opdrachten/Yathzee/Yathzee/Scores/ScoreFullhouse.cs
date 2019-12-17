using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yathzee.Scores {
    class ScoreFullhouse : BaseScore {

        public ScoreFullhouse( string name ) : base( name ) {
        }

        public override int potentailValue() {

            bool found2 = false;
            bool found3 = false;

            for ( int i = Dice.MIN; i < Dice.MAX + 1; i++ ) {

                int c = Dices.getCountOf(i);

                switch ( c ) {

                    case 2:
                        found2 = true;
                        break;
                    case 3:
                        found3 = true;
                        break;
                }

                if ( ( found2 ) && ( found3 ) ) {

                    return 25;
                }

            }
            return 0;
        }
    }
}
