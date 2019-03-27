namespace SeaBattle.Server.Services.Rating
{
    using System;

    public class EloRatingCalculator : IEloRatingCalculator
    {
        private const int Kfactor = 25;
        
        public (double, double) Calculate(double ratingA, double ratingB, double scoreA, double scoreB)
        {
            var (expectedScore1, expectedScore2) = GetExpectedScores(ratingA, ratingB);
            
            var newRatingA = ratingA + Kfactor * (scoreA - expectedScore1);
            var newRatingB = ratingB + Kfactor * (scoreB - expectedScore2);

            return (newRatingA, newRatingB);
        }

        private (double, double) GetExpectedScores(double ratingA, double ratingB)
        {
            var expectedScoreA = 1 / (1 + Math.Pow(10, (ratingB - ratingA) / 400));
            var expectedScoreB = 1 / (1 + Math.Pow(10, (ratingA - ratingB) / 400));

            return (expectedScoreA, expectedScoreB);
        }
    }
}