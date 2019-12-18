using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Vlaggen {
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    /// 

    public partial class WindowResults : Window {

        private Quiz quiz;

        private int currentQuestion = 0;

        private void displayQuestion() {

            Title = String.Format( "{0} / {1}", currentQuestion+1, Quiz.MAX_QUIZ_QUESTION );

            imgFlagCorrect.Source       = CountryManager.countries[ quiz.questionAnswers[ currentQuestion ] ].flag;
            lblAnswerCorrect.Content    = CountryManager.countries[ quiz.questionAnswers[ currentQuestion ] ].name;

            imgFlagUser.Source    = CountryManager.countries[ quiz.userAnswers[ currentQuestion ] ].flag;
            lblAnswerUser.Content = CountryManager.countries[ quiz.userAnswers[ currentQuestion ] ].name;

            Boolean correct = ( quiz.userAnswers[currentQuestion] == quiz.questionAnswers[currentQuestion] );

            lblStatus.Content       = ( correct ) ? "CORRECT" : "FOUT";
            lblStatus.Foreground    = ( correct ) ? Brushes.Green : Brushes.Red;
        }

        public WindowResults( Quiz quiz ) {

            InitializeComponent();

            this.quiz       = quiz;
            currentQuestion = 0;

            displayQuestion();
        }

        private void BtnNext_Click( object sender, RoutedEventArgs e ) {

            currentQuestion++;

            if ( currentQuestion >= Quiz.MAX_QUIZ_QUESTION ) {

                MessageBox.Show( "Dit waren alle antwoorden." );
                return;
            }

            displayQuestion();
        }

        private void BtnPrev_Click( object sender, RoutedEventArgs e ) {

            if ( currentQuestion <= 0 ) {
                return;
            }

            currentQuestion--;
            displayQuestion();
        }

        private void Window_Closed( object sender, EventArgs e ) {
            App.Current.MainWindow.Show();
        }
    }
}
