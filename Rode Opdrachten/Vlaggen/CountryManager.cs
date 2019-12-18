using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml;

using Bitmap = System.Drawing.Bitmap;

namespace Vlaggen {
    struct Country {

        public string name;
        public BitmapSource flag;
    }

    static class CountryManager {

        public static Country[] countries;

        public static void init() {

            XmlDocument XML = new XmlDocument();
            XML.LoadXml( Resources.database );

            // Get elements
            XmlNodeList xmlCountries = XML.GetElementsByTagName("country");
            countries = new Country[ xmlCountries.Count ];

            for ( int i = 0; i < xmlCountries.Count; i++ ) {

                countries[ i ].name = xmlCountries[ i ].Attributes[ "name" ].Value;
                string flagFilename = xmlCountries[i].Attributes["file"].Value;

                Bitmap bmp = (Bitmap)Resources.ResourceManager.GetObject( flagFilename );
                BitmapSource source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                           bmp.GetHbitmap(),
                           IntPtr.Zero,
                           System.Windows.Int32Rect.Empty,
                           BitmapSizeOptions.FromEmptyOptions() );

                countries[ i ].flag = source;
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
