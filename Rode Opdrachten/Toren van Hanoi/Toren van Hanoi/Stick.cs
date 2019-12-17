using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Toren_van_Hanoi {
    public class Stick {

        public static int width     = 7;
        public static int height;

        private static int space;
        private static int delta = width / 2;

        public static void calculateProportions() {

            Stick.height    = (int)( Game.boardHeight * 0.75 );
            Stick.space     = Game.boardWidth / ( Game.stickCount + 1 );
            Stick.width     = 7;
            Stick.delta     = Stick.width / 2;
        }

        public int[] disks;
        public int index;

        public int x;

        public Line shape;

        public Stick( int index ) {

            this.index = index;

            this.disks = new int[Game.diskCount];
            for ( int i = 0; i < Game.diskCount; i++ ) {

                this.disks[i] = -1;
            }

            this.x = ( this.index + 1 ) * space - delta;

            this.shape = new Line();

            this.shape.X1 = this.x;
            this.shape.Y1 = Game.baselineHeight;
            this.shape.X2 = this.x;
            this.shape.Y2 = Game.baselineHeight - height;

            this.shape.StrokeThickness = width;
            this.shape.Stroke = Brushes.Black;
        }

        public int getLowestPos() {

            for( int i = 0; i < Game.diskCount; i++ ) {

                if ( disks[i] < 0 ) {

                    return i;
                }
            }

            return -1;
        }

        public bool canAdd( int size ) {

            int x = getLowestPos();

            if ( x < 0 ) {
                return false; // stick is full
            }

            if ( x == 0 ) {

                return true; // stick is empty
            }

            return disks[x - 1] > size;
        }
    }
}