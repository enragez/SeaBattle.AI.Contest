namespace SeaBattle.Server.Models
{
    using System;

    public class StrategyCompilationException : Exception
    {
        public StrategyCompilationException(string message) 
            : base(message)
        {
        }
    }
}