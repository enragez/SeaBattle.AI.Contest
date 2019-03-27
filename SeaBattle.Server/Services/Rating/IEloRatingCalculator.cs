namespace SeaBattle.Server.Services.Rating
{
    public interface IEloRatingCalculator
    {
        (double, double) Calculate(double ratingA, double ratingB, double scoreA, double scoreB);
    }
}