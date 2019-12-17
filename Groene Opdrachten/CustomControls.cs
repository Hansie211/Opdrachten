using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GroeneOpdrachten {

    using static Helper;

    public class CustomScrollBar : System.Windows.Controls.Primitives.ScrollBar {

        public int direction;
        private const int basevalue = 100;

        public CustomScrollBar() : base() {
            Loaded += ScrollBar_Loaded;
            Scroll += ScrollBar_Scroll;
        }

        private void ScrollBar_Loaded( object sender, RoutedEventArgs e ) {

            this.Maximum = 200;
            this.Minimum = 0;

            this.SmallChange = 0.1;
            this.LargeChange = 0.1;

            this.Value = basevalue;
        }

        private void ScrollBar_Scroll( object sender, System.Windows.Controls.Primitives.ScrollEventArgs e ) {

            direction = 0;

            //if ( this.Value == 100.0 ) {
            //    goto end;
            //}

            if ( this.Value < basevalue ) {
                direction = 1;
                goto end;

            }

            direction = -1;

            end:
            this.Value = basevalue;
        }

    }

    public class CustomComboxBox : System.Windows.Controls.ComboBox {

        private static int MINUTE_INTERVAL = 5;

        public CustomComboxBox() : base() {

            for ( int i = 0; i < HOURS_PER_DAY; i++ ) {

                for ( int j = 0; j < MINUTES_PER_HOUR; j += MINUTE_INTERVAL ) {

                    this.Items.Add( String.Format( "{0}:{1,2:00}", i, j ) );
                }
            }

            this.SelectedIndex = 0;
        }

        public void SetTime( int Hour, int Minute ) {

            Hour    = clamp( Hour, 0, HOURS_PER_DAY - 1 );
            Minute  = clamp( Minute, 0, MINUTES_PER_HOUR - 1 );

            this.SelectedIndex = Hour * ( MINUTES_PER_HOUR / MINUTE_INTERVAL ) + (Minute / MINUTE_INTERVAL);
        }

    }

    public class OpdrWindow: Window {

        public static Type[] getSubWindows() {

            Type me = typeof( OpdrWindow );

            // Get all types from the Assembly where the type is a subclass of this type
            return System.Reflection.Assembly.GetExecutingAssembly().GetTypes().Where( tmp => tmp.IsSubclassOf( me ) ).ToArray();
        }

        static public string Opdracht( Type subClass ) { 
        
            return (string)subClass.GetMethod( "Opdracht" ).Invoke( null, null );
        }

        static public OpdrWindow newInstance( Type subClass ) {

            return (OpdrWindow)subClass.GetMethod( "newInstance" ).Invoke( null, null );
        }

        // Placeholders
        static public string Opdracht(){

            throw new Exception( "Cannot call 'Opdracht' of class 'OpdrWindow' " );
        }

        static public OpdrWindow newInstance() {

            throw new Exception( "Cannot call 'newInstance' of class 'OpdrWindow' " );
        }

        public virtual void reset() {

            return;
        }

    }
}
