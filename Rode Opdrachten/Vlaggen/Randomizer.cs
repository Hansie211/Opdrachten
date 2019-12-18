namespace Vlaggen {
    using System;
    using System.Security.Cryptography;

    /// <summary>
    /// Defines the <see cref="Randomizer" />
    /// </summary>
    internal static class Randomizer {
        /// <summary>
        /// Defines the provider
        /// </summary>
        public static RNGCryptoServiceProvider provider;

        /// <summary>
        /// The init
        /// </summary>
        public static void init() {
            provider = new RNGCryptoServiceProvider();
        }

        /// <summary>
        /// The randomNumber
        /// </summary>
        /// <param name="max">The max<see cref="int"/></param>
        /// <returns>The <see cref="int"/></returns>
        public static int randomNumber( int max = 0x7FFFFFFF ) {

            byte[] bytes = new byte[4];
            provider.GetBytes( bytes );

            int result = (int)(
                ( bytes[0] << 0 ) |
                ( bytes[1] << 8 ) |
                ( bytes[2] << 16 ) |
                ( bytes[3] << 24 )
               );

            if ( result < 0 ) {
                result *= -1;
            }

            return (int)( result % max );
        }

        /// <summary>
        /// The randomize
        /// </summary>
        /// <param name="list">The list<see cref="int[]"/></param>
        /// <returns>The <see cref="int[]"/></returns>
        public static int[] randomize( int[] list ) {

            for ( int i = 0; i < list.Length; i++ ) {

                int j = randomNumber(list.Length);

                // swap
                int tmp = list[i];
                list[ i ] = list[ j ];
                list[ j ] = tmp;
            }

            return list;
        }

        /// <summary>
        /// The randomizeIndex
        /// </summary>
        /// <param name="list">The list<see cref="Array"/></param>
        /// <returns>The <see cref="int[]"/></returns>
        public static int[] randomizeIndex( Array list ) {

            // Create a pool of indexes
            int[] pool = new int[ list.Length ];
            for ( int i = 0; i < pool.Length; i++ ) {
                pool[ i ] = i;
            }

            // Randomize the pool
            return randomize( pool );
        }
    }
}
