namespace SeaBattle.Engine.Exceptions
{
    using System;

    public class CheatDetectedException : Exception
    {
        public CheatDetectedException(string message)
            : base(message)
        {
        }
    }
}