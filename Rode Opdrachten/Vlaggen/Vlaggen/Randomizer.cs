using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Vlaggen {
    static class Randomizer {

        public static RNGCryptoServiceProvider provider;


        public static void init() {
            provider = new RNGCryptoServiceProvider();
        }

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
