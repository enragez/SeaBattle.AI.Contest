namespace SeaWars.Engine.Exceptions
{
    using System;

    public class CheatDetectedException : Exception
    {
        public CheatDetectedException()
        {
        }
        
        public CheatDetectedException(string message)
            : base(message)
        {
        }
    }
}