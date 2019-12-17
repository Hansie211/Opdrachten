using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Toren_van_Hanoi {
    static class HighscoreManager {

        public static ObservableCollection<HighscoreEntry> highscores = new ObservableCollection<HighscoreEntry>();

        public static void addScore( string Name, int Score, Difficulty difficulty ) {

            if ( getCount(difficulty) >= 10 ) {

                int x = getLowestIndex(difficulty );
                highscores.RemoveAt( x );
            }

            highscores.Add( new HighscoreEntry( Name, Score, difficulty ) );
        }

        public static int getLowestIndex( Difficulty difficulty ) {

            // Actually it's highest, but lowest score = highscore

            int score = -1;
            int index = -1;

            for( int i = 0; i < highscores.Count; i++ ) {

                if ( highscores[i].Difficulty != difficulty ) {
                    
                    continue;
                }

                if ( highscores[i].Score > score ) {

                    score = highscores[i].Score;
                    index = i;
                }
            }

            return index;
        }

        public static int getCount( Difficulty difficulty ) {

            int result = 0;

            for( int i = 0; i < highscores.Count; i++ ) {

                if ( highscores[i].Difficulty != difficulty ) {

                    continue;
                }

                result++;
            }

            return result;
        }

        public static void saveToFile( String filename ) {

            XElement xml = new XElement("Scores", highscores.Select( x =>

                new XElement("score", new XAttribute("Name", x.Name ), new XAttribute("Score", x.Score ), new XAttribute("Difficulty", x.Difficulty ) )
            ));

            xml.Save( filename );
        }

        private static void addElementToList( XElement elem ) {
            try {

                highscores.Add( new HighscoreEntry( elem.Attribute( "Name" ).Value, int.Parse( elem.Attribute( "Score" ).Value ), ( Difficulty)Enum.Parse( typeof(Difficulty), elem.Attribute( "Difficulty" ).Value) ) );
            } catch ( Exception e ) {

                return;
            }
        }

        public static void loadFromFile( String filename ) {

            XElement xml;

            try {
                
                xml = XElement.Load( filename );
            } catch ( Exception e ) {

                // Somehow we cannot read this XML file
                // Add default values
                highscores.Add( new HighscoreEntry( "Hans", 15, Difficulty.Easy ) );
                highscores.Add( new HighscoreEntry( "Hans", 400, Difficulty.Medium ) );

                return;
            }

            List<XElement> childElements = (from item in xml.Elements("score") select item).ToList();

            foreach ( XElement elem in childElements ) {
                addElementToList( elem );
            }
        }
    }
}
