using HeadToHead_Latelier.Common.Data;

namespace HeadToHead_Latelier.Common.Interfaces.Readers
{
    public interface IHeadToHeadReader
    {
        IEnumerable<Player> GetPlayersByRank();
    }
}
