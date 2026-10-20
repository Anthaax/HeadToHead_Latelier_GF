using HeadToHead_Latelier.Common.Data;
using HeadToHead_Latelier.Common.Interfaces.Readers;
using HeadToHead_Latelier.Common.Interfaces.Services;

namespace HeadToHead_Latelier.API.Services
{
    public class HeadToHeadService(IHeadToHeadReader reader) : IHeadToHeadService
    {
        /// <summary>
        /// Gets a list of players sorted by their results (Better players first).
        /// </summary>
        /// <returns>A list of players.</returns>
        public IEnumerable<Player> GetPlayers()
        {
            return reader.GetPlayersByRank();
        }

        /// <summary>
        /// Gets a player by their Id.
        /// </summary>
        /// <param name="id">The player's Id.</param>
        /// <returns>The player or null if not found.</returns>
        public Player? GetPlayerById(int id)
        {
            return reader.GetPlayerById(id);
        }

        /// <summary>
        /// Gets general statistics about players.
        /// </summary>
        /// <returns>A GeneralStats object containing statistics.</returns>
        public GeneralStats GetGeneralStats()
        {
            return new GeneralStats
            {
                CountryWithBestWinRatio = reader.GetCountryWithTheBestWinRatio(),
                AverageIMC = reader.GetAverageIMC(),
                MedianHeight = reader.GetMedianHeight()
            };
        }
    }
}
