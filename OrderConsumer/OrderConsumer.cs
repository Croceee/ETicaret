using EFCoreData.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TrCellUI;
using System.Net.Http.Json;
using SpanJson;
using Microsoft.Extensions.Logging;

namespace OrderConsumer
{
    public class OrderConsumer
    {
        private static string queName;
        private static IConnection connection;
        private static IModel _channel;
        private ApiClientFactory apiClientFactory;
        private readonly ILogger<OrderConsumer> _logger;

        private static IModel channel => _channel ?? (_channel=CreateOrGetChannel());

        public OrderConsumer( ApiClientFactory apiClientFactory, ILogger<OrderConsumer> logger)
        {
            this.apiClientFactory = apiClientFactory;
            _logger = logger;


        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                connection = GetConnection();
                var consumerEvent = new EventingBasicConsumer(channel);
                //consumerEvent.Received += IpcMessageReceived;
                consumerEvent.Received += (ch, e) =>
                {
                    var byteArr = e.Body.ToArray();
                   // var bodyStr = Encoding.UTF8.GetString(byteArr);
                    MessageReceived(byteArr);
                };
                channel.BasicConsume("Order", true, consumerEvent);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex.ToString());
            }
        }

        private static IConnection GetConnection()
        {
            var factory = new ConnectionFactory() { HostName = "localhost", UserName = "admin", Password = "admin" };
            return  factory.CreateConnection();

        }
        private static IModel CreateOrGetChannel()
        {
            return connection.CreateModel();
        }
        private void MessageReceived(byte[] message)
        {
            var order = message;

            SendOrderToAPI(order);
        }
        private async void SendOrderToAPI(byte[] order)
        {
            try
            {
                string querystring = "";
                string query = "Order";

                //var entity = order.FromJson<Order>();
                var order_ = Encoding.UTF8.GetString(order);
                //string json = JsonConvert.SerializeObject(order, Formatting.Indented);

                Order orderModel_ = JsonConvert.DeserializeObject<Order>(order_);

                _logger.LogWarning($"RabbitMQ 'dan  {orderModel_.OrderGuid.ToString()} keyli sipariş geldi.");

                var requesturl = apiClientFactory.Instance.CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture, query), querystring);

                var response = await apiClientFactory.Instance.PostAsync<Order>(requesturl, orderModel_, "", "", 0);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} in SendOrderToAPI");

            }

        }
      

    }
}

