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

        [HttpGet("by-rank", Name = "GetPlayersByRank")]
        /// <summary>
        /// Gets a list of players sorted by their rank (Best rank first).
        /// </summary>
        /// <returns>A list of players sorted by rank.</returns>
        public ActionResult<IEnumerable<Player>> GetPlayersByRank()
        {
            try
            {
                var playersByRank = _headToHeadService.GetPlayers();
                return Ok(playersByRank);
            }
            catch (Exception e)
            {
                return ExceptionHandler.HandleException(e);
            }
        }
    }
}
