using matchmaking.data;
using matchmaking.data.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace matchmaking.web.Services
{
    internal class MatchMakingService : IHostedService, IDisposable
    {
        private readonly IServiceProvider _services;
        private readonly ILogger _logger;
        private Timer _timer;
        private MatchMaker matchMaker = new MatchMaker();
        // TODO: move to config
        private int _matchSize = 20;
        private int _timeout = 30;

        public MatchMakingService(IServiceProvider services,
            ILoggerFactory loggerFactory)
        {
            _services = services;
            _logger = loggerFactory.CreateLogger<MatchMakingService>();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is starting.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            _logger.LogInformation("Timed Background Service is working.");

            using (var scope = _services.CreateScope())
            {
                var dbcontext = scope.ServiceProvider.GetRequiredService<MatchMakingContext>();

                //using (var dbContext = new MatchMakingContext())
                //{
                //    var players = dbContext.Players.ToList();
                //    if (!matchMaker.ShouldStartMatch(players, _matchSize, _timeout))
                //    {
                //        return;
                //    }

                //    var selectedPlayers = matchMaker.NearestToAverage(players, _matchSize);
                //    var newMatch = new Match(selectedPlayers);

                //    dbContext.Players.RemoveRange(selectedPlayers);
                //    dbContext.Matches.Add(newMatch);
                //    dbContext.SaveChanges();

                //    _logger.LogInformation($"Match started ({selectedPlayers.Count}), {dbContext.Players.Count()} player(s) in the queue.");
                //}
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}

