using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using Microsoft.Win32;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Yathzee.Scores;
using System.Drawing;

namespace Yathzee {
    /// <summary>
    /// Interaction logic for ScoreForm.xaml
    /// </summary>
    public partial class ScoreForm : Window {

        private System.Windows.Forms.WebBrowser subweb = new System.Windows.Forms.WebBrowser();
        private string saveFile;

        public static void Execute() {

            ScoreForm window = new ScoreForm();

            string html = Yathzee.Resources.scoreblock;

            for ( int i = 0; i < Game.cache.Length; i++ ) {

                if ( Game.cache[ i ] == null ) {

                    CacheScore.zeroChart( i, ref html );
                } else {

                    Game.cache[ i ].addToChart( i, ref html );
                }
            }

            window.browser.NavigateToString( html );
            window.ShowDialog();

            Game.start();
        }


        public ScoreForm() {
            InitializeComponent();

            subweb.Width  = (int)browser.Width;
            subweb.Height = (int)browser.Height;
            subweb.ScrollBarsEnabled = false;
            subweb.DocumentCompleted += webBrowser_DocumentCompleted;
        }

        private void click_btnNext( object sender, MouseButtonEventArgs e ) {

            DialogResult = true;
            Close();
        }

        private void webBrowser_DocumentCompleted( object sender, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e ) {

            using ( Bitmap bmp = new Bitmap( subweb.Width, subweb.Height ) ) {

                subweb.DrawToBitmap( bmp, new System.Drawing.Rectangle( 0, 0, bmp.Width, bmp.Height ) );
                bmp.Save( saveFile, System.Drawing.Imaging.ImageFormat.Png );

            }
        }

        private void click_btnSave( object sender, MouseButtonEventArgs e ) {


            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "Afbeelding (*.png)|*.png";
            save.FileName = "yahtzee.png";

            if ( save.ShowDialog() != true ) {

                return;
            }

            saveFile = save.FileName;

            dynamic doc = browser.Document;
            string html = doc.documentElement.InnerHtml;
            subweb.DocumentText = html;
        }
    }
}
