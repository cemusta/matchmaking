using System;
using System.Collections.Generic;
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
        public ActionResult<string> Get([FromQuery]string name, float skill, int remoteness)
        {
            var player = new Player() { Name = name, Skill = skill, Remoteness = remoteness };
            _matchmakingContext.Add(player);
            _matchmakingContext.SaveChanges();
            return $"added: {player}";
        }

        [HttpGet("Random")]
        public ActionResult<string> AddRandomUser()
        {
            var player = _matchMaker.AddRandomPlayer(_matchmakingContext);
            //var player = _matchMaker.GenerateRandomPlayer();
            //_matchmakingContext.Add(player);
            //_matchmakingContext.SaveChanges();
            return $"added random: {player}";
        }

        [HttpGet("Matches")]
        public ActionResult<string> Matches()
        {
            var mCount = _matchmakingContext.Matches.Count();
            var result = $"{mCount} match(es) started\n";
            foreach (var match in _matchmakingContext.Matches)
            {
                result += $"{match}\n";
            }
            return result;
        }



    }
}
