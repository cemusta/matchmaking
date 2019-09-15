using System.Linq;
using matchmaking.data;
using matchmaking.data.Models;
using Microsoft.AspNetCore.Mvc;

namespace matchmaking.web.Controllers
{
    [Route("api/queue")]
    [ApiController]
    public class QueueController : ControllerBase
    {

        private readonly MatchMakingContext _matchmakingContext;
        private readonly MatchMaker _matchMaker;

        public QueueController(MatchMakingContext matchmakingContext, MatchMaker matchMaker)
        {
            _matchmakingContext = matchmakingContext;
            _matchMaker = matchMaker;
        }

        //private readonly MatchMaker _matchMaker = new MatchMaker();

        // GET api/queue
        [HttpGet]
        public ActionResult<string> Get()
        {
            var playerCount = _matchmakingContext.Players.Count();
            var result = $"There is {playerCount} player(s) in queue\n";
            foreach (var player in _matchmakingContext.Players)
            {
                result += $"{player}\n";
            }
            return result;
        }

        // GET api/queue/AddUser?name=[name]&skill=[0.0-1.0]&remoteness=[0-100]
        [HttpGet("AddUser")]
        public ActionResult<string> Get([FromQuery]string name="", double skill=-1, int remoteness=-1)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("name is incorrect");
            }
            if (skill > 1 || skill < 0)
            {
                return BadRequest("skill index is incorrect");
            }
            if (remoteness > 100 || remoteness < 0)
            {
                return BadRequest("remoteness index is incorrect");
            }

            var player = new Player() { Name = name, Skill = skill, Remoteness = remoteness };
            _matchmakingContext.Add(player);
            _matchmakingContext.SaveChanges();
            return $"added: {player}";
        }

        [HttpGet("Random")]
        public ActionResult<string> AddRandomUser()
        {
            var player = _matchMaker.AddRandomPlayer(_matchmakingContext);
            return $"added random: {player}";
        }

        [HttpGet("Matches")]
        public ActionResult<string> Matches()
        {
            var mCount = _matchmakingContext.Matches.Count();
            var result = $"{mCount} match(es) started\n";
            result += $"Id\t#ofPlayer\tSkill\t\t\tRemoteness\t\tWaitTime\n";
            foreach (var match in _matchmakingContext.Matches)
            {
                result += $"{match.ToString()}\n";
            }
            return result;
        }



    }
}
