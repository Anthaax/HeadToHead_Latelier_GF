using HeadToHead_Latelier.Common.Data;

namespace HeadToHead_Latelier.Common.Interfaces.Services
{
    public interface IHeadToHeadService
    {
        IEnumerable<Player> GetPlayers();
        Player? GetPlayerById(int id);
        GeneralStats GetGeneralStats();
        Player InsertPlayer(Player player);
    }
}
