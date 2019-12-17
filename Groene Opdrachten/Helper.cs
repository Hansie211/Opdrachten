﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroeneOpdrachten {
    public static class Helper {

        public const int HOURS_PER_DAY      = 24;
        public const int MINUTES_PER_HOUR   = 60;
        public const int MINUTES_PER_DAY    = HOURS_PER_DAY * MINUTES_PER_HOUR;


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

            // Reversed
            return min( max(i, _min), _max );
        }

        static public void split( out int[] l1, out int[] l2, int turn, int[] input ) {

            l1 = new int[0];
            l2 = new int[0];

            for( int i = 0; i < input.Length; i++ ) {

                if ( input[i] < turn ) {
                    int k = l1.Length;

                    Array.Resize( ref l1, k + 1);
                    l1[k] = input[i];
                } else {
                    int k = l2.Length;

                    Array.Resize( ref l2, k + 1 );
                    l2[k] = input[i];
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


            int result = current.Year - birthdate.Year;

            // Born in november but 'current' is february? Subtract a year!
            DateTime tmp;

            // February 29...
            try { 
                tmp = new DateTime( current.Year, birthdate.Month, birthdate.Day );
            } catch {
                tmp = new DateTime( current.Year, birthdate.Month, birthdate.Day - 1);
            }

            if ( tmp > current ) {
                result--;
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
    }
}
