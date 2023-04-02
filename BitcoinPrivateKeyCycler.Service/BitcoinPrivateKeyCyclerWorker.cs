using System.Diagnostics;
using System.Reactive.Subjects;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDbQueueService;

namespace BitcoinPrivateKeyCycler.Service
{
    public class BitcoinPrivateKeyCyclerWorker : BackgroundService
    {
        private readonly ILogger<BitcoinPrivateKeyCyclerWorker> _logger;
        private ISubscriber _subscriber;
        private IPublisher _publisher;
        
        private Subject<PrivateKeyAddress> _onNewPrivateKeyArray;

        public BitcoinPrivateKeyCyclerWorker(ILogger<BitcoinPrivateKeyCyclerWorker> logger)
        {
            this._logger = logger;

            this._onNewPrivateKeyArray = new Subject<PrivateKeyAddress>();

            var debugMode = false;

#if DEBUG
            debugMode = true;
#endif
            this._logger.LogInformation($"DebuggerMode: {debugMode}");
            this._subscriber = new Subscriber(debugMode);
            this._publisher = new Publisher(debugMode);
        }


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this._logger.LogInformation("PrivateKeyCycler started...");

            this._subscriber
                .SubscribeQueueCollection<PrivateKeyAddress>(stoppingToken)
                .Subscribe(x => 
                {
                    var stopWatch = Stopwatch.StartNew();

                    this.Cycle(x.Payload.PrivateKeyBytes);
                    x.ProcessSucessful = true;

                    stopWatch.Stop();
                    this._logger.LogInformation("Process private key took: {0}", stopWatch.Elapsed);
                });

            this._onNewPrivateKeyArray
                .Subscribe(async x => 
                {
                    await this._publisher.SendAsync<PrivateKeyAddress>(x);
                });

            return Task.CompletedTask;
        }

        private void Cycle(byte[] sourceArray)
        {
            for (var i = 0; i < sourceArray.Length; i++)
            {
                var newArray = new byte[sourceArray.Length];
                Array.Copy(sourceArray, newArray, sourceArray.Length);

                for (var cycler = 0; cycler <= 255; cycler ++)
                {
                    newArray[i] = (byte)cycler;
                    this._onNewPrivateKeyArray?.OnNext(new PrivateKeyAddress { PrivateKeyBytes = newArray });

                    this._logger.LogInformation($"[{string.Join(",", newArray)}]");
                }
            }
        }
    }
}