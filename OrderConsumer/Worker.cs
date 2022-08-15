using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OrderConsumer
{
    public class Worker : BackgroundService
    {
        private readonly OrderConsumer _orderConsumer;

        public Worker(OrderConsumer orderConsumer/*ILogger<Worker> logger,
                     DataMergedService dataMergedService,
                     IServiceProvider serviceProvider,
                     SignalRSetting settings*/)
        {
            /* _logger = logger;
             _dataMergedService = dataMergedService;
             _serviceProvider = serviceProvider;
             _settings = settings;*/
            _orderConsumer = orderConsumer;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await StartServicesAsync(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                //_logger.LogInformation("DataTransfer running at: {time}", DateTimeOffset.Now.ToDateString());
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
       
        private async Task StartServicesAsync(CancellationToken stoppingToken)
        {
            await _orderConsumer.StartAsync(stoppingToken);
        }

        }
}
