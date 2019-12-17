using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml;

namespace Opdracht2 {
    struct Country {

        public string name;
        public BitmapImage flag;
    }

    static class CountryManager {

        public static Country[] countries;

        public static void init() {

            XmlDocument XML = new XmlDocument();
            XML.Load( "DB\\database.xml" );

            // Get elements
            XmlNodeList xmlCountries = XML.GetElementsByTagName("country");
            countries = new Country[xmlCountries.Count];

            for ( int i = 0; i < xmlCountries.Count; i++ ) {

                countries[i].name   = xmlCountries[i].Attributes["name"].Value;
                string flagFilename = xmlCountries[i].Attributes["file"].Value;

                countries[i].flag = new BitmapImage( new Uri( AppDomain.CurrentDomain.BaseDirectory + flagFilename, UriKind.Absolute ) );
            }
        }

        public static int randomCountry() {

            return Randomizer.randomNumber( countries.Length );
        }

        public static int[] randomCountries() {

            return Randomizer.randomizeIndex( countries );
        }
    }
}
