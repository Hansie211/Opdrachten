using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yathzee.Scores {
    class ScoreCount : BaseScore {

        private int num;

        public ScoreCount( int num, string name ) : base( name ) {
            this.num = num;
        }

        public override int potentailValue() {

            int count = Dices.getCountOf( num );

            return count * num;
        }
    }
}
