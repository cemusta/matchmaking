using matchmaking.data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace matchmaking.web.Services
{
    internal class MatchMakingService : IHostedService, IDisposable
    {
        private readonly IServiceProvider _services;
        private readonly MatchMaker _matchMaker;
        private readonly ILogger _logger;
        private Timer _timer;

        public MatchMakingService(IServiceProvider services, MatchMaker matchMaker, ILoggerFactory loggerFactory)
        {
            _services = services;
            _matchMaker = matchMaker;
            _logger = loggerFactory.CreateLogger<MatchMakingService>();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("MatchMaking Background Service is starting.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(2));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            _logger.LogInformation("Timed Background Service is working.");

            using (var scope = _services.CreateScope())
            {
                var dbcontext = scope.ServiceProvider.GetRequiredService<MatchMakingContext>();

                var result = _matchMaker.TryMatchmaking(dbcontext);

                _logger.LogInformation(result);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("MatchMaking Background Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}

