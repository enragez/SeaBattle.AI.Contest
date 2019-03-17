namespace SeaBattle.Engine.Models
{
    using System;
    using System.Linq;

    [Serializable]
    public class SerializableTurnResult
    {
        public SerializableTurnResult()
        {
            
        }

        public SerializableTurnResult(ExtendedTurnResult turnResult)
        {
            PlayerId = turnResult.ParticipantId;
            Id = turnResult.Id;

            ChangedCells = turnResult.ChangedCells.Select(kvp => new SerializableCell
                                                                 {
                                                                     Row = kvp.Key.Row,
                                                                     Column = kvp.Key.Column,
                                                                     State = kvp.Value
                                                                 }).ToArray();
        }
        
        public SerializableCell[] ChangedCells { get; set; }
        
        public int PlayerId { get; set; }
        
        public int Id { get; set; }
    }
}