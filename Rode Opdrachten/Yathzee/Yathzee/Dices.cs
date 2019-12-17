using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yathzee {
    static class Dices {

        public const int COUNT             = 5;
        public static readonly Dice[] dice = new Dice[ COUNT ];

        static Dices() {

            HoldManager.init( COUNT );

            for ( int i = 0; i < dice.Length; i++ ) {

                int x = 95;
                int y = 50;

                switch ( i ) {

                    case 0:
                        //x += 0;
                        //y += 0;
                        break;
                    case 1:
                        x += 240;
                        //y += 0;
                        break;
                    case 2:
                        x += 120;
                        y += 100;
                        break;
                    case 3:
                        //x += 0;
                        y += 200;
                        break;
                    case 4:
                        x += 240;
                        y += 200;
                        break;
                }

                dice[ i ] = new Dice( i, x, y );
            }
        }

        public static int getCountOf( int eyes ) {

            int result = 0;

            for ( int i = 0; i < dice.Length; i++ ) {

                if ( dice[ i ].getValue() != eyes ) {

                    continue;
                }

                result++;
            }

            return result;
        }

        public static void roll() {

            // Lets roll them all
            for ( int i = 0; i < dice.Length; i++ ) {

                dice[ i ].roll();
            }
        }

        public static bool hasValue( int eyes ) {

            for ( int i = 0; i < COUNT; i++ ) {

                if ( dice[ i ].getValue() == eyes ) {
                    return true;
                }
            }

            return false;
        }

        public static int getSum() {

            int result = 0;
            for ( int i = 0; i < COUNT; i++ ) {

                result += dice[ i ].getValue();
            }

            return result;
        }
    }
}
