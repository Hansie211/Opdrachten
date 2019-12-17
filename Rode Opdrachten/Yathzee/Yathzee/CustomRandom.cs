using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Yathzee {
    class CustomRandom {

        private RNGCryptoServiceProvider Generator;

        public CustomRandom() {

            Generator = new RNGCryptoServiceProvider();
        }

        public int Random( int min, int max ) {

            if ( min > max ) {

                throw new Exception( "'min' cannot be larger than 'max'." );
            }

            if ( min == max ) {
                return 0;
            }

            uint span = (uint)(max + (min * -1) + 1);

            byte[] bytes = new byte[4];
            Generator.GetBytes( bytes );

            uint result = BitConverter.ToUInt32( bytes, 0 );

            result %= span;

            return (int)( result + min );
        }
    }
}
