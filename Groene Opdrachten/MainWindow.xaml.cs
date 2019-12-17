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
        private readonly Type[] opdrachten = OpdrWindow.getSubWindows();

        public MainWindow() {

            InitializeComponent();

            App.Current.Resources["DateTimeNow"] = DateTime.Now;

            // Init the selectionbox
            for ( int i = 0; i < opdrachten.Length; i++ ) {

                string name = OpdrWindow.Opdracht( opdrachten[i] );
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
