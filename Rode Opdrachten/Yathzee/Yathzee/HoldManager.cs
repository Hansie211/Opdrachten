using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Yathzee {
    static class HoldManager {

        public static Point[] pos;
        public static Dice[] location;
        private static Rectangle[] shape;

        private const int MARGIN    = 10;
        private const double LEFT   = (double)(Game.BOARDWIDTH - (( Dices.COUNT ) * 64 + (Dices.COUNT - 1) * MARGIN )) / 2;

        public static void init( int count ) {

            pos         = new Point[ count ];
            location    = new Dice[ count ];

            shape       = new Rectangle[ count ];
            for ( int i = 0; i < count; i++ ) {

                // Zero out the location
                location[ i ] = null;

                // Set the pos
                pos[ i ].X = LEFT + i * ( Dice.WIDTH + MARGIN );
                pos[ i ].Y = Game.BOARDHEIGHT - 200;

                // Create the outline
                shape[ i ] = new Rectangle();

                shape[ i ].Width    = Dice.WIDTH;
                shape[ i ].Height   = Dice.HEIGHT;

                shape[ i ].Stroke = Brushes.White;
                shape[ i ].StrokeThickness = 2;
                shape[ i ].StrokeDashArray = new DoubleCollection( new double[] { 4, 4 } );
                shape[ i ].RadiusX = 10;
                shape[ i ].RadiusY = 10;

                Canvas.SetLeft( shape[ i ], pos[ i ].X );
                Canvas.SetTop( shape[ i ], pos[ i ].Y );
            }
        }

        public static void addToCanvas( Canvas canvas ) {

            for ( int i = 0; i < shape.Length; i++ ) {

                canvas.Children.Add( shape[ i ] );
            }
        }

        private static int getEmptySlot() {

            for ( int i = 0; i < location.Length; i++ ) {

                if ( location[ i ] != null ) {

                    continue;
                }

                return i;
            }

            return -1;
        }

        private static int getDicePos( Dice dice ) {

            for ( int i = 0; i < location.Length; i++ ) {

                if ( location[ i ] == null ) {
                    continue;
                }

                if ( location[ i ].index != dice.index ) {
                    continue;
                }

                return i;
            }

            return -1;
        }

        public static int takeSlot( Dice dice ) {

            int slot = getEmptySlot();

            location[ slot ] = dice;
            dice.moveToHoldPos( slot );

            return slot;
        }

        public static void freeSlot( Dice dice ) {

            int slot = getDicePos( dice );
            if ( slot < 0 ) {
                return;
            }

            location[ slot ] = null;

            //for ( int i = slot; i < location.Length - 1; i++ ) {

            //    location[ i ] = location[ i + 1 ];

            //    if ( location[ i ] == null ) {
            //        break;
            //    }

            //    location[ i ].moveToHoldPos( i );
            //}

            // location[ location.Length - 1 ] = null;
        }
    }
}
