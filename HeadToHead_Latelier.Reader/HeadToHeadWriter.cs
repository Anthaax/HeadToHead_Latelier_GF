using System.Text.Json;
using HeadToHead_Latelier.Common.Data;
using HeadToHead_Latelier.Common.Interfaces.Readers;
using HeadToHead_Latelier.Common.Interfaces.Writers;

namespace HeadToHead_Latelier.Reader
{
    public class HeadToHeadWriter : IHeadToHeadWriter
    {
        private const string JsonFilePath = "headtohead.json";
        private readonly ICacheManager _cacheManager;

        public HeadToHeadWriter(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        public Player InsertPlayer(Player player)
        {
            var filePath = Path.Combine(AppContext.BaseDirectory, JsonFilePath);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Unable to find {JsonFilePath}.");
            }

            var json = File.ReadAllText(filePath);
            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;

            var playersArray = root.GetProperty("players").EnumerateArray();
            var playersData = playersArray.Select(element =>
            {
                JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<Player>(element.GetRawText(), options);
            }).OfType<Player>().ToList();

            player.Id = playersData.Count > 0 ? playersData.Max(p => p.Id) + 1 : 1;
            playersData.Add(player);

            var updatedJson = JsonSerializer.Serialize(
                new { players = playersData },
                new JsonSerializerOptions { WriteIndented = true, PropertyNamingPolicy = null }
            );

            File.WriteAllText(filePath, updatedJson);

            if (_cacheManager is HeadToHeadReader reader)
            {
                reader.ResetCache();
            }

            return player;
        }
    }
}
