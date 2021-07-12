using Newtonsoft.Json;
using System.IO;

namespace Battlefield.Services
{
    class LevelReaderService
    {
        private string _levelFileDirectory = "Levels/";
        private string _levelFileTempalte = "{0}level-{1}.json";

        public int[,] GetLevelPoints(int level)
        {
            var fileName = string.Format(_levelFileTempalte, _levelFileDirectory, level);
            var jsonData = File.ReadAllText(fileName);

            return JsonConvert.DeserializeObject<int[,]>(jsonData);
        }
    }
}
