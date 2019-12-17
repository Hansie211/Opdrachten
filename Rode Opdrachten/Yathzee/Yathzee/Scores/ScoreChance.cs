using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yathzee.Scores {
    class ScoreChance : BaseScore {
        public ScoreChance( string name ) : base( name ) {
        }

        public override int potentailValue() {

            return Dices.getSum();
        }
    }
}
