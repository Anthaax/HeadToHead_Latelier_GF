using HeadToHead_Latelier.API.Exceptions;
using HeadToHead_Latelier.Common.Data;
using HeadToHead_Latelier.Common.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace HeadToHead_Latelier.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HeadToHeadController : ControllerBase
    {
        private readonly IHeadToHeadService _headToHeadService;

        public HeadToHeadController(IHeadToHeadService headToHeadService)
        {
            _headToHeadService = headToHeadService;
        }

        [HttpGet(Name = "GetPlayers")]
        /// <summary>
        /// Gets a list of players sorted by their results (Better players first).
        /// </summary>
        /// <returns>A list of players.</returns>
        public ActionResult<IEnumerable<Player>> GetPlayers()
        {
            try
            {
                var players = _headToHeadService.GetPlayers();
                return Ok(players);
            }
            catch (Exception e)
            {
                return ExceptionHandler.HandleException(e);
            }
        }

        [HttpGet("{id}", Name = "GetPlayerById")]
        /// <summary>
        /// Gets a player by their Id.
        /// </summary>
        /// <param name="id">The player's Id.</param>
        /// <returns>The player if found, otherwise a 404 Not Found response.</returns>
        public ActionResult<Player> GetPlayerById(int id)
        {
            try
            {
                var player = _headToHeadService.GetPlayerById(id);
                if (player == null)
                {
                    return NotFound();
                }
                return Ok(player);
            }
            catch (Exception e)
            {
                return ExceptionHandler.HandleException(e);
            }
        }

        [HttpGet("stats/general", Name = "GetGeneralStats")]
        /// <summary>
        /// Gets general statistics about players including the country with the best win ratio, average IMC, and median height.
        /// </summary>
        /// <returns>GeneralStats object containing statistics.</returns>
        public ActionResult<GeneralStats> GetGeneralStats()
        {
            try
            {
                var stats = _headToHeadService.GetGeneralStats();
                return Ok(stats);
            }
            catch (Exception e)
            {
                return ExceptionHandler.HandleException(e);
            }
        }

        [HttpPut(Name = "InsertPlayer")]
        /// <summary>
        /// Inserts a new player.
        /// </summary>
        /// <param name="player">The player to insert.</param>
        /// <returns>The inserted player.</returns>
        public ActionResult<Player> InsertPlayer([FromBody] Player player)
        {
            try
            {
                var insertedPlayer = _headToHeadService.InsertPlayer(player);
                return CreatedAtAction(nameof(GetPlayerById), new { id = insertedPlayer.Id }, insertedPlayer);
            }
            catch (Exception e)
            {
                return ExceptionHandler.HandleException(e);
            }
        }
    }
}
