namespace SeaBattle.Server.Services.Rating
{
    using System;
    using Microsoft.Extensions.Options;
    using Models;

    public class EloRatingCalculator : IEloRatingCalculator
    {
        private readonly EloConfiguration _config;
        public EloRatingCalculator(IOptions<EloConfiguration> config)
        {
            _config = config.Value;
        }
        
        public (double, double) Calculate(double ratingA, double ratingB, double scoreA, double scoreB)
        {
            var (expectedScore1, expectedScore2) = GetExpectedScores(ratingA, ratingB);
            
            var newRatingA = ratingA + _config.Kfactor * (scoreA - expectedScore1);
            var newRatingB = ratingB + _config.Kfactor * (scoreB - expectedScore2);

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