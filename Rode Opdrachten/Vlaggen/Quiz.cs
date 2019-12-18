using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vlaggen {
    public class Quiz {

        public const int MAX_QUIZ_QUESTION = 10;

        public int[] questionAnswers;
        public int[] userAnswers;
        public int[] questionTypes;
        public int currentQuestion;
        public int correctAnswerCount;

        public Boolean answerQuestion( int answer ) {

            userAnswers[ currentQuestion ] = answer;

            Boolean correct = ( answer == getCurrentAnswer() );

            if ( correct ) {
                correctAnswerCount++;
            }

            return correct;
        }

        public Boolean quizFinished() {

            return currentQuestion >= MAX_QUIZ_QUESTION;
        }

        public int getCurrentAnswer() {

            return questionAnswers[ currentQuestion ];
        }

        public int nextQuestion() {

            currentQuestion++;

            if ( quizFinished() ) {
                return -1;
            }

            return getCurrentAnswer();
        }

        public Quiz() {

            correctAnswerCount  = 0;
            currentQuestion     = 0;

            questionAnswers = new int[ MAX_QUIZ_QUESTION ];
            userAnswers     = new int[ MAX_QUIZ_QUESTION ];

            // Get our 10 questions
            int[] randomCountries = CountryManager.randomCountries();

            for ( int i = 0; i < MAX_QUIZ_QUESTION; i++ ) {
                questionAnswers[ i ] = randomCountries[ i ];
            }

            // I only want 3 open questions
            questionTypes = new int[ MAX_QUIZ_QUESTION ] { 0, 0, 0, 1, 1, 1, 1, 1, 1, 1 };
            questionTypes = Randomizer.randomize( questionTypes );
        }

    }
}
