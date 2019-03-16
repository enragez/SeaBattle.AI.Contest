namespace SeaWars.Engine.Models.Ships
{
    using System;
    using System.Collections.Generic;
    using Exceptions;

    public abstract class Ship
    {
        protected abstract int Length { get; }
        
        public Coordinate StartLocation { get; set; }

        public Coordinate EndLocation { get; set; }
        
        public List<Coordinate> Coordinates { get; set; }

        protected Ship()
        {
            Coordinates = new List<Coordinate>();
        }
        
        public void Validate()
        {
            if (Length == 1)
            {
                return;
            }
            
            if (StartLocation.Row == EndLocation.Row)
            {
                if (StartLocation.Column == EndLocation.Column)
                {
                    throw new CheatDetectedException("Как эта хуйня может плавать?");
                }

                if (Math.Abs(StartLocation.Column - EndLocation.Column) != Length - 1)
                {
                    throw new CheatDetectedException("Как эта хуйня может плавать?");
                }

                return;
            }
            
            if (StartLocation.Column == EndLocation.Column)
            {
                if (Math.Abs(StartLocation.Row - EndLocation.Row) != Length - 1)
                {
                    throw new CheatDetectedException("Как эта хуйня может плавать?");
                }

                return;
            }
            
            throw new CheatDetectedException("Как эта хуйня может плавать?");
        }
    }
}