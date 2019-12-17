using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yathzee.Scores {
    class ScoreKind : BaseScore {

        private int rowsize;

        public ScoreKind( int rowsize, string name ) : base ( name ) {

            this.rowsize = rowsize;
        }

        public override int potentailValue() {

            for( int i = Dice.MIN; i < Dice.MAX + 1; i++ ) {

                if ( Dices.getCountOf(i) < rowsize ) {

                    continue;
                }

                return Dices.getSum();
            }

            return 0;
        }
    }
}
