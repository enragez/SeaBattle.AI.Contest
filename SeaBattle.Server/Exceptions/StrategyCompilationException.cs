namespace SeaBattle.Server.Exceptions
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