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
    }
}
