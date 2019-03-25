namespace SeaBattle.Console
{
    using System.IO;
    using Engine.Models;
    using Engine.Models.Serializable;
    using Newtonsoft.Json;

    public class GameResultSaver
    {
        public string SaveGameResult(GameResult result)
        {
            var serializable = new SerializableGameResult(result);

            var json = JsonConvert.SerializeObject(serializable);

            var filePath = Path.ChangeExtension(Path.GetTempFileName(), ".json");
            
            File.WriteAllText(filePath, json);

            return filePath;
        }
    }
}