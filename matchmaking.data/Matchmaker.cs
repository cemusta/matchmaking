using Combinatorics.Collections;
using Faker;
using matchmaking.data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace matchmaking.data
{
    public class MatchMaker
    {
        private readonly Random _random;
        private readonly int _matchSize;
        private readonly int _timeout;
        private readonly object _lock = new object();

        public MatchMaker(int seed = 0, int matchSize = 20, int timeout = 60)
        {
            _random = new Random(seed);
            _matchSize = matchSize;
            _timeout = timeout;
        }

        private Player GenerateRandomPlayer()
        {
            Player randomPlayer = new Player()
            {
                Name = Name.First(),
                Skill = Math.Round(_random.NextDouble(), 3),
                Remoteness = _random.Next(0, 101)
            };

            return randomPlayer;
        }

        private double CalculateDistance(IList<Player> players)
        {
            var avgRemoteness = players.Average(x => x.Remoteness);
            var avgSkill = players.Average(x => x.Skill);

            var totalRemotenessDistance = players.Sum(x => Math.Abs(x.Remoteness - avgRemoteness));
            var totalSkillDistance = players.Sum(x => Math.Abs(x.Skill - avgSkill));
            
            //TODO: here we can use configurable weights for fitness, currently weight is 1.0 for each.
            var fitness = (totalSkillDistance * 100) + totalRemotenessDistance;

            return fitness;
        }

        // Brute force is too slow.
        private IList<Player> BruteForce(IList<Player> potentialPlayers, int matchSize=20)
        {
            Combinations<Player> combinations = new Combinations<Player>(potentialPlayers, matchSize);

            IList<Player> bestCombination = null;
            double best = -1;
            int index = 0;
            
            Console.WriteLine($"Combinations of {potentialPlayers.Count()} choose {matchSize}: varietions = {combinations.Count}");
            foreach (IList<Player> playerCombination in combinations)
            {
                var currentFitness = CalculateDistance(playerCombination);
                index++;
                if (best > currentFitness)
                {
                    Console.WriteLine($"({index})improved: {best} to {currentFitness}");
                    best = currentFitness;
                } else if (best == -1)
                {   
                    best = currentFitness;
                }
            }

            return bestCombination;
        }

        private IList<Player> NearestToFirst(IList<Player> potentialPlayers, int matchSize = 20)
        {
            if (potentialPlayers.Count < matchSize)
            {
                matchSize = potentialPlayers.Count;
            }

            Player firstPlayer = potentialPlayers.OrderBy(x=>x.DateTimeAdded).First();
            var selectedPlayers = potentialPlayers.OrderBy(x => x.GetDistance(firstPlayer)).Take(matchSize).ToList();
            Console.WriteLine($"Overall fitness is: {CalculateDistance(selectedPlayers)}");
            return selectedPlayers;
        }

        private IList<Player> NearestToAverage(IList<Player> potentialPlayers, int matchSize = 20)
        {
            if (potentialPlayers.Count < matchSize)
            {
                matchSize = potentialPlayers.Count;
            }

            Player firstPlayer = potentialPlayers.OrderBy(x => x.DateTimeAdded).First();
            IList<Player> selectedPlayers = new List<Player> {firstPlayer};
            potentialPlayers.Remove(firstPlayer);

            while (selectedPlayers.Count < matchSize)
            {
                var avgRemoteness = selectedPlayers.Average(x => x.Remoteness);
                var avgSkill = selectedPlayers.Average(x => x.Skill);

                var nextPlayer = potentialPlayers.OrderBy(x => x.GetDistance(avgSkill, avgRemoteness)).First();
                selectedPlayers.Add(nextPlayer);
                potentialPlayers.Remove(nextPlayer);
            }

            Console.WriteLine($"Overall fitness is: {CalculateDistance(selectedPlayers)}");
            return selectedPlayers;
        }

        private bool ShouldStartMatch(IList<Player> players, int matchSize = 20, int timeout = 60)
        {
            if(!players.Any())
            {
                return false;
            }

            if(players.Count > (matchSize * 2))
            {
                return true;
            }
   
            int longestWait = (int)players.Max(x => (DateTime.UtcNow - x.DateTimeAdded).TotalSeconds);
            if(longestWait > timeout)
            {
                Console.WriteLine($"staring a new game due to long wait time (>{timeout}secs)");
                return true;
            }

            return false;
        }

        public Match TryMatchmaking(MatchMakingContext dbContext)
        {
            lock (_lock)
            {
                using (dbContext)
                {
                    var players = dbContext.Players.ToList();
                    if (!ShouldStartMatch(players, _matchSize, _timeout))
                    {
                        return null;
                    }

                    var selectedPlayers = NearestToAverage(players, _matchSize);
                    var newMatch = new Match(selectedPlayers);

                    dbContext.Players.RemoveRange(selectedPlayers);
                    dbContext.Matches.Add(newMatch);
                    dbContext.SaveChanges();

                    return newMatch;
                }
            }
        }

        public Player AddRandomPlayer(MatchMakingContext dbContext)
        {
            lock (_lock)
            {
                using (dbContext)
                {
                    var player = GenerateRandomPlayer();

                    dbContext.Players.Add(player);
                    dbContext.SaveChanges();
                    return player;
                }
            }
        }

        public void AddRandomPlayers(MatchMakingContext dbContext, int numberOfPlayers)
        {
            lock (_lock)
            {
                using (dbContext)
                {
                    var playerList = new List<Player>();
                    for (int i = 0; i < numberOfPlayers; i++)
                    {
                        playerList.Add(GenerateRandomPlayer());
                    }

                    dbContext.Players.AddRange(playerList);
                    dbContext.SaveChanges();
              
                    Console.WriteLine($"{numberOfPlayers} new players added, {dbContext.Players.Count()} player(s) in the queue.");
                }
            }
        }

        public void TestingAlgorithms(MatchMakingContext dbContext)
        {
            using (dbContext)
            {
                dbContext.Players.Add(new Player() { Name = "Cem", Skill = 0.5, Remoteness = 15 });
                dbContext.SaveChanges();

                Thread.Sleep(1000);
                AddRandomPlayers(dbContext, 999);
                dbContext.SaveChanges();

                var players1 = NearestToAverage(dbContext.Players.Take(1000).ToList(), 20);
                var match1 = new Match(players1);
                Console.WriteLine(match1);

                var players2 = NearestToFirst(dbContext.Players.Take(1000).ToList(), 20);
                var match2 = new Match(players2);
                Console.WriteLine(match1);
            }
        }
    }
}
