using System.Text.Json;
using HeadToHead_Latelier.Common.Data;
using HeadToHead_Latelier.Common.Interfaces.Readers;

namespace HeadToHead_Latelier.Reader
{
    public class HeadToHeadReader : IHeadToHeadReader, ICacheManager
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

        public Player? GetPlayerById(int id)
        {
            var players = LoadPlayersFromJson();
            return players.FirstOrDefault(p => p.Id == id);
        }

        public string GetCountryWithTheBestWinRatio()
        {
            var players = LoadPlayersFromJson();

            return players
                .Where(p => p.Data.Last.Length > 0 && p.Country.Code != null)
                .GroupBy(p => p.Country.Code)
                .Select(group => 
                {
                    var totalMatches = group.Sum(p => p.Data.Last.Length);
                    var winCount = group.Sum(p => p.Data.Last.Count(result => result == 1));
                    var winRatio = totalMatches > 0 ? (double)winCount / totalMatches : 0;
                    return (group.Key, winRatio);
                })
                .OrderByDescending(x => x.winRatio)
                .Select(x => x.Key)
                .First();
        }

        public double GetAverageIMC()
        {
            var players = LoadPlayersFromJson();

            var imcValues = players
                .Where(p => p.Data.Height > 0)
                .Select(p => (double)p.Data.Weight / (p.Data.Height * p.Data.Height))
                .ToList();

            return imcValues.Count > 0 ? imcValues.Average() : 0;
        }

        public double GetMedianHeight()
        {
            var players = LoadPlayersFromJson();

            var heights = players
                .Select(p => p.Data.Height)
                .OrderBy(h => h)
                .ToList();

            if (heights.Count == 0)
            {
                return 0;
            }

            var count = heights.Count;
            if (count % 2 == 1)
            {
                return heights[count / 2];
            }

            return (heights[count / 2 - 1] + heights[count / 2]) / 2.0;
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

        public void ResetCache()
        {
            _cachedPlayers = null;
            _cachedPlayersByRank = null;
        }
    }
}
