using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toren_van_Hanoi {
    public class HighscoreEntry {

        public string Name { get; set; }
        public int Score { get; set; }
        public Difficulty Difficulty { get; set; }

        public HighscoreEntry( string Name, int Score, Difficulty Difficulty ) {

            this.Name       = Name;
            this.Score      = Score;
            this.Difficulty = Difficulty;
        }
    }

}
