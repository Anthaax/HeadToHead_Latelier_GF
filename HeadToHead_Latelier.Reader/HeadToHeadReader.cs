using System.Text.Json;
using HeadToHead_Latelier.Common.Data;
using HeadToHead_Latelier.Common.Interfaces.Readers;

namespace HeadToHead_Latelier.Reader
{
    public class HeadToHeadReader : IHeadToHeadReader
    {
        private const string JsonFilePath = "headtohead.json";
        private List<Player>? _cachedPlayers;
        private List<Player>? _cachedPlayersByRank;

        public IEnumerable<Player> GetPlayersByRank()
        {
            if (_cachedPlayersByRank is not null)
            {
                return _cachedPlayersByRank;
            }

            var players = LoadPlayersFromJson();

            _cachedPlayersByRank = players
                .OrderBy(p => p.Data.Rank)
                .ToList();

            return _cachedPlayersByRank;
        }

        private IEnumerable<Player> LoadPlayersFromJson()
        {
            if (_cachedPlayers is not null)
            {
                return _cachedPlayers;
            }

            var filePath = Path.Combine(AppContext.BaseDirectory, JsonFilePath);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Unable to find {JsonFilePath}.");
            }

            var json = File.ReadAllText(filePath);

            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;
            var playersArray = root.GetProperty("players").EnumerateArray();

            _cachedPlayers = playersArray
                .Select(element =>
                {
                    JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };
                    return JsonSerializer.Deserialize<Player>(element.GetRawText(), options);
                })
                .OfType<Player>()
                .ToList();

            return _cachedPlayers;
        }
    }
}
