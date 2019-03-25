namespace SeaBattle.Engine.Models.Serializable
{
    using System;
    using System.Linq;
    using Field;
    using Utils;

    [Serializable]
    internal class SerializableGameResult
    {
        public SerializableGameResult()
        {
            
        }

        public SerializableGameResult(GameResult result)
        {
            StartTime = result.StartTime;
            EndTime = result.EndTime;
            WinnerId = result.Winner.Id;
            Participant1 = result.Participant1;
            Participant2 = result.Participant2;
            TurnsHistory = result.TurnsHistory.Select(tr => new SerializableTurnResult(tr)).ToArray();
            Participant1StartField = ConvertField(result.Participant1StartField);
            Participant2StartField = ConvertField(result.Participant2StartField);
        }
        
        public DateTime StartTime { get; set; }
        
        public DateTime EndTime { get; set; }
        
        public int WinnerId { get; set; }
        
        public ParticipantModel Participant1 { get; set; }
        
        public ParticipantModel Participant2 { get; set; }
      
        public SerializableTurnResult[] TurnsHistory { get; set; }
        
        public Row[] Participant1StartField { get; set; }
        
        public Row[] Participant2StartField { get; set; }

        private Row[] ConvertField(Field field)
        {
            var result = new Row[10];

            for (var row = 0; row < 10; row++)
            {
                var rowModel = new Row
                               {
                                   Cells = new Column[10]
                               };

                for (var column = 0; column < 10; column++)
                {
                    rowModel.Cells[column] = new Column
                                             {
                                                 Name = ColumnNamesHelper.ColumnNameByNumber[column],
                                                 Number = column,
                                                 State = field.Cells[row, column].State
                                             };
                }

                result[row] = rowModel;
            }
            
            return result;
        }
    }
}