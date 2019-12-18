using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GroeneOpdrachten {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    using static Helper;

    public partial class MainWindow : Window {

        // Get a list of all OpdrWindows
        private readonly List<Type> opdrachten = new List<Type>( OpdrWindow.getSubWindows() );

        private static int sortOpdrachten( Type A, Type B ) {

            // Make sure Opdr10 is after Opdr1

            string titleA = A.Name;
            string titleB = B.Name;

            if ( titleB.Length == titleA.Length ) {

                return titleA.CompareTo( titleB );
            }

            if ( titleB.Length < titleA.Length ) {

                return 1;
            }

            return -1;
        }

        public MainWindow() {

            InitializeComponent();

            App.Current.Resources["DateTimeNow"] = DateTime.Now;

            opdrachten.Sort( sortOpdrachten );

            // Init the selectionbox
            for ( int i = 0; i < opdrachten.Count; i++ ) {

                string name = OpdrWindow.getName( opdrachten[i] );
                boxOpdracht.Items.Add( name );
            }

        }

        private void Button_Click( object sender, RoutedEventArgs e ) {

            int selected = boxOpdracht.SelectedIndex;
            
            if ( selected < 0 ) {
                return;
            }

            OpdrWindow w    = OpdrWindow.newInstance( opdrachten[selected] );
            w.Title         = boxOpdracht.Items[selected].ToString();

            w.reset();
            w.ShowDialog();
        }

    }



}
