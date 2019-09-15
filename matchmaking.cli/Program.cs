using matchmaking.data;
using System;
using System.IO;
using System.Threading;

namespace matchmaking.cli
{
    class Program
    {
        static readonly MatchMaker matchMaker = new MatchMaker(1234, 20, 60);
        static readonly Random random = new Random();

        static void Main(string[] args)
        {
            Timer generateTimer = null;
            Timer consumeTimer = null;

            try
            {
                string dbName = $"matchmaking.db";
                if (File.Exists(dbName))
                {
                    File.Delete(dbName);
                }
                using (var dbContext = new MatchMakingContext())
                {
                    dbContext.Database.EnsureCreated();
                }

                //matchMaker.TestingAlgorithms(new MatchMakingContext());

                generateTimer = new Timer(GeneratePlayers, 0, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(0.1));
                consumeTimer = new Timer(StartMatches, null, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(0.1));

                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                generateTimer?.Dispose();
                consumeTimer?.Dispose();
            }
        }

        private static void GeneratePlayers(object state)
        {
            var numberOfPlayers = (int)state == 0 ? random.Next(5) : (int)state;
            matchMaker.AddRandomPlayers(new MatchMakingContext(), numberOfPlayers);
        }

        private static void StartMatches(object state)
        {
            var match = matchMaker.TryMatchmaking(new MatchMakingContext());
            if (match != null)
            {
                Console.WriteLine($"Match Started:\n{match}");
            }
        }


    }
}
