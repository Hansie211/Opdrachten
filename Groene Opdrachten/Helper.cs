using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GroeneOpdrachten {
    public static class Helper {

        public const int HOURS_PER_DAY      = 24;
        public const int MINUTES_PER_HOUR   = 60;
        public const int MINUTES_PER_DAY    = HOURS_PER_DAY * MINUTES_PER_HOUR;

        public static readonly Regex regexNum = new Regex("[^0-9.-]+");

        static public int max( int a, int b ) {
            return ( a > b ) ? a : b;
        }

        static public double max( double a, double b ) {
            return ( a > b ) ? a : b;
        }

        static public DateTime max( DateTime a, DateTime b ) {

            return ( a > b ) ? a : b;
        }

        static public int min( int a, int b ) {
            return ( a < b ) ? a : b;
        }

        static public double min( double a, double b ) {
            return ( a < b ) ? a : b;
        }

        static public DateTime min( DateTime a, DateTime b ) {

            return ( a < b ) ? a : b;
        }

        static public int clamp( int i, int _min, int _max ) {

            return min( max(i, _min), _max );
        }

        static public void split( int[] input, int splitValue, out int[] resultLthen, out int[] resultGEthen ) {

            resultLthen = new int[0];
            resultGEthen = new int[0];

            for( int i = 0; i < input.Length; i++ ) {

                if ( input[i] < splitValue ) {
                    int k = resultLthen.Length;

                    Array.Resize( ref resultLthen, k + 1);
                    resultLthen[k] = input[i];
                } else {
                    int k = resultGEthen.Length;

                    Array.Resize( ref resultGEthen, k + 1 );
                    resultGEthen[k] = input[i];
                }
            }
        }

        static public int age( DateTime birthdate, DateTime current ) {

            if ( birthdate == null ) {
                return 0;
            }

            if ( current == null ) {
                return 0;
            }

            // Born in november but 'current' is february? Subtract a year!
            int result = (current.Year - birthdate.Year) - 1;

            // Add the year back if birthday passed this year
            if ( current.Month > birthdate.Month ) {

                result++;
            } else if ( ( current.Month == birthdate.Month ) && ( current.Day >= birthdate.Day ) ) {

                result++;
            }

            return result;
        }

        static public int age( DateTime birthdate ) {

            return age( birthdate, DateTime.Now );
        }

        static public Boolean splice<T>( ref T[] array, int index ) {

            if ( ( index < 0 ) || ( index > array.Length - 1 ) ) {
                return false;
            }

            // Copy the second half over the 'to be cut' index
            Array.Copy( array, index + 1, array, index, (array.Length - index) - 1 );
            // Remove the last element
            Array.Resize( ref array, array.Length - 1 );
            
            return true;
        }

        static public bool isNumeric( string text ) {

            return !regexNum.IsMatch( text );
        }

        static public double parseDouble( string text ) {

            return double.Parse( text, System.Globalization.CultureInfo.InvariantCulture );
        }

    }
}
