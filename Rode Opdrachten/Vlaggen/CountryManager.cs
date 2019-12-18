namespace Vlaggen {
    using System;
    using System.Windows.Media.Imaging;
    using System.Xml;
    using Bitmap = System.Drawing.Bitmap;

    /// <summary>
    /// Defines the <see cref="Country" />
    /// </summary>
    struct Country {
        /// <summary>
        /// Defines the name
        /// </summary>
        public string name;

        /// <summary>
        /// Defines the flag
        /// </summary>
        public BitmapSource flag;
    }

    /// <summary>
    /// Defines the <see cref="CountryManager" />
    /// </summary>
    internal static class CountryManager {
        /// <summary>
        /// Defines the countries
        /// </summary>
        public static Country[] countries;

        /// <summary>
        /// The init
        /// </summary>
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

        /// <summary>
        /// The randomCountry
        /// </summary>
        /// <returns>The <see cref="int"/></returns>
        public static int randomCountry() {

            return Randomizer.randomNumber( countries.Length );
        }

        /// <summary>
        /// The randomCountries
        /// </summary>
        /// <returns>The <see cref="int[]"/></returns>
        public static int[] randomCountries() {

            return Randomizer.randomizeIndex( countries );
        }
    }
}
