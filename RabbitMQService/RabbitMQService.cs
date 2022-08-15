using Microsoft.Extensions.ObjectPool;
using RabbitMQ.Client;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQService
{
    public class ChannelPool : ObjectPool<IModel>
    {
        public ChannelPool(Func<IModel> valueFactory) : base(valueFactory) { }
    }
    public class RabbitMQService
    {
        //private readonly RabbitMQSetting _setting;

        private IConnection _connection;
        private ChannelPool _channelPool;

        private readonly ConcurrentDictionary<IIpcModel, bool> _declareQueues = new();
        private readonly ConcurrentDictionary<IIpcModel, AsyncEventingBasicConsumer> _consumers = new();
        private Task _connectionTask;
        private volatile bool _disposedValue;
        private CancellationTokenSource cancellationTokenSource = new();

        public RabbitMQService(/*ILogger<RabbitMQService> logger, IPCServiceSetting ipcServiceSetting*/)
        {
           /* _logger = logger;
            _setting = ipcServiceSetting.RabbitMQSetting;*/
        }
        public void Start()
        {
            _connectionTask = Task.Run(async () =>
            {
                try
                {
                    //_logger.LogInformation("Starting RabbitMQService..");

                    if (_connection == null)
                        await ConnectAsync();
                }
                catch (Exception ex)
                {
                    //_logger.LogError(ex.ToString());
                    throw;
                }
            });
        }
        private async Task ConnectAsync()
        {
            int retryCount = 0;

            while (!CancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    var factory = new ConnectionFactory() { HostName = "192.168.0.74", UserName = "admin", Password = "admin" };

                    factory.ClientProvidedName = $"{Assembly.GetEntryAssembly()?.GetName().Name ?? nameof(RabbitMQService)} ({nameof(RabbitMQService)})";
                    factory.AutomaticRecoveryEnabled = true;
                    factory.DispatchConsumersAsync = true;
                    factory.Port = "15672";

                    _connection = factory.CreateConnection();
                    _channelPool = new(() =>
                    {
                        var model = _connection.CreateModel();

                        if (_setting.FetchCount.HasValue)
                            model.BasicQos(0, _setting.FetchCount.Value, false);

                        model.CallbackException += (s, e) =>
                        {
                            _logger.LogWarning(e.Exception.ToString());
                        };

                        model.ModelShutdown += (s, e) =>
                        {
                            _logger.LogWarning("RabbitMQ ModelShutdown : Cause:{c}, Initiator:{i}, ReplyCode:{rc}, ReplyText:{rt}", e.Cause, e.Initiator.ToString(), e.ReplyCode, e.ReplyText);
                        };

                        return model;
                    });

                    #region RabbitMq Events
                    _connection.ConnectionBlocked += (s, e) =>
                    {
                        //_logger.LogWarning($"RabbitMQ connection blocked! reason: {e.Reason}");
                    };
                    _connection.ConnectionUnblocked += (s, e) =>
                    {
                        //_logger.LogWarning("RabbitMQ connection unblocked.");
                    };
                    _connection.ConnectionShutdown += (s, e) =>
                    {
                        //_logger.LogWarning($"RabbitMQ connection shutdown by {e.Initiator}. Cause : {e.Cause}");
                    };
                    #endregion

                    //_logger.LogInformation("RabbitMQ Service connection succeeded.");

                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    //_logger.LogError($"RabbitMQ Service connection error : {ex.Message}\nRetryCount : {retryCount} Will try again in 2 seconds...");
                    await Task.Delay(2000);
                }
            }
        }


        private static SemaphoreSlim _sem = new(1);
        private async Task DeclareQueueExchange(IModel channel, RabbitMqModel rabbitMqModel)
        {
            try
            {
                var result = await IsDeclaredQueue(rabbitMqModel);

                if (result)
                {
                    channel.ExchangeDeclare(rabbitMqModel.ExchangeName, rabbitMqModel.ExchangeType, true, false);

                    if (!string.IsNullOrEmpty(rabbitMqModel.Queue))
                    {
                        channel.QueueDeclare(rabbitMqModel.Queue, true, false, false, null);
                        channel.QueueBind(rabbitMqModel.Queue, rabbitMqModel.ExchangeName, rabbitMqModel.RouteKey, null);
                    }

                    foreach (var queueName in rabbitMqModel.OtherQueues)
                    {
                        try
                        {
                            var routeKey = GetRouteKey(queueName);

                            channel.QueueDeclare(queueName, true, false, false, null);
                            channel.QueueBind(queueName, rabbitMqModel.ExchangeName, routeKey, null);

                            _logger.LogDebug($"{rabbitMqModel} => {queueName}");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex.ToString());
                        }
                    }

                    _logger.LogInformation($"RabbitMqModel declared for {rabbitMqModel}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in {rabbitMqModel} : {ex}");

                _declareQueues.TryRemove(rabbitMqModel, out _);
            }

            await Task.CompletedTask;
        }

        private async Task<bool> IsDeclaredQueue(RabbitMqModel rabbitMqModel)
        {
            bool result = false;
            try
            {
                await _sem.WaitAsync();
                result = _declareQueues.TryAdd(rabbitMqModel, true);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _sem.Release();
            }

            return result;
        }

        private RabbitMqModel GetRabbitMqModel(QueueModel queueModel)
        {
            try
            {
                string routeKey = $"message.{queueModel.RouteKey}.route".ToLowerInvariant();
                string exchangeType = string.IsNullOrEmpty(queueModel.ExchangeType) ? Shared.ValueObjects.ExchangeType.Topic : queueModel.ExchangeType;

                if (queueModel.Queues != null && queueModel.Queues.Length > 0)
                {
                    routeKey = $"message.{string.Join(".", queueModel.Queues)}.{queueModel.Queue}".ToLowerInvariant();
                }

                var rabbitMqModel = new RabbitMqModel(exchangeType, $"{queueModel.Category}Exc",
                                                      routeKey, queueModel.Queue, queueModel.Queues);

                rabbitMqModel.InstanceId = queueModel.InstanceId;

                rabbitMqModel.Persistent = queueModel.Persistent;

                return rabbitMqModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return null;
            }
        }

        private static string GetRouteKey(string otherQueue)
        {
            // message.rhm.power8 => main queue
            // message.rhm.hello => main queue
            // *.rhm.* => other queue

            //message.rhm.createdlocation.power8

            return $"#.{otherQueue}.#".ToLowerInvariant();
        }

        #region Sender
        public async Task SendAsync(QueueModel queueModel, byte[] buffer)
        {
            var rabbitMqModel = GetRabbitMqModel(queueModel);

            await SendMessageAsync(rabbitMqModel, buffer);
        }

        public async Task SendAsync<T>(QueueModel queueModel, T sendObject)
        {
            var rabbitMqModel = GetRabbitMqModel(queueModel);

            var buffer = sendObject.GetBytesFromObject();

            await SendMessageAsync(rabbitMqModel, buffer);
        }

        public async Task SendAsync(QueueModel queueModel, object sendObject)
        {
            var rabbitMqModel = GetRabbitMqModel(queueModel);

            var buffer = sendObject.GetBytesFromObject();

            await SendMessageAsync(rabbitMqModel, buffer);
        }

        public async Task ReSendAsync<T>(IIpcModel ipcModel, T sendObject)
        {
            var buffer = sendObject.GetBytesFromObject();

            await SendMessageAsync((RabbitMqModel)ipcModel, buffer, true);
        }

        public async Task ReSendAsync<T>(QueueModel queueModel, T sendObject)
        {
            var rabbitMqModel = GetRabbitMqModel(queueModel);

            var buffer = sendObject.GetBytesFromObject();

            await SendMessageAsync(rabbitMqModel, buffer, true);
        }

        private async Task SendMessageAsync(RabbitMqModel rabbitMqModel, byte[] buffer, bool withoutExchange = false)
        {
            _connectionTask.Wait(cancellationTokenSource.Token);

            var channel = _channelPool.Take();
            try
            {
                if (channel?.IsOpen ?? false)
                {
                    await DeclareQueueExchange(channel, rabbitMqModel);

                    var props = channel.CreateBasicProperties();
                    props.Timestamp = DateTime.Now.ToAmqpTimestamp();
                    props.Persistent = rabbitMqModel.Persistent;

                    if (withoutExchange)
                        channel.BasicPublish("", rabbitMqModel.Queue, props, buffer);
                    else
                        channel.BasicPublish(rabbitMqModel.ExchangeName, rabbitMqModel.RouteKey, props, buffer);
                }
                else
                {
                    _logger.LogWarning($"RabbitMQ Channel is close! ->{rabbitMqModel}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("SendMessageAsync: " + ex.ToString());
            }
            finally
            {
                if ((channel?.IsClosed ?? false) == false)
                    _channelPool.Return(channel);
                else if (channel != null)
                    channel.Dispose();
            }
        }
        #endregion

        #region Listener
        public async Task StartListeningAsync(QueueModel queueModel)
        {
            _connectionTask.Wait(cancellationTokenSource.Token);
            var channel = _channelPool.Take();

            try
            {
                var rabbitMqModel = GetRabbitMqModel(queueModel);

                if (channel != null)
                {
                    await DeclareQueueExchange(channel, rabbitMqModel);

                    var consumer = _consumers.GetOrAdd(rabbitMqModel, new AsyncEventingBasicConsumer(channel));
                    if (consumer.ConsumerTags.Length == 0)
                        await StartConsumingQueueAsync(consumer, rabbitMqModel, channel);
                }
                else
                {
                    _logger.LogWarning($"RabbitMQ Consumer failed to start for {rabbitMqModel}");

                    await Task.Delay(2000, cancellationTokenSource.Token);
                    await StartListeningAsync(queueModel);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        public async Task StopListeningAsync(QueueModel queueModel)
        {
            _connectionTask.Wait(cancellationTokenSource.Token);

            try
            {
                var rabbitMqModel = GetRabbitMqModel(queueModel);

                if (_consumers.TryGetValue(rabbitMqModel, out AsyncEventingBasicConsumer consumer) && consumer != null)
                {
                    _logger.LogInformation("StopListeningAsync ConsumerTags : " + string.Join(" | ", consumer.ConsumerTags));
                    _consumers.TryRemove(rabbitMqModel, out _);

                    if (consumer.ConsumerTags.Length > 0)
                    {
                        consumer.Model.BasicCancel(consumer.ConsumerTags.First());
                        //_channel.BasicCancel(consumer.ConsumerTags.First());

                        _logger.LogWarning($"{rabbitMqModel} consumption is stopped...");
                        await Task.Delay(10);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            finally
            {
                await Task.Delay(10);
            }
        }

        private async Task StartConsumingQueueAsync(AsyncEventingBasicConsumer consumer, RabbitMqModel rabbitMqModel, IModel channel)
        {
            try
            {
                consumer.Received += async (model, ea) =>
                {
                    try
                    {
                        var message = ea.Body.ToArray();

                        if (IpcMessageReceived != null)
                        {
                            var args = new QueueMessageEventArgs(rabbitMqModel, message, ea.BasicProperties.Timestamp.ToDateTime());
                            await IpcMessageReceived.Invoke(this, args);

                            if (args.Nack == false)
                                channel.BasicAck(ea.DeliveryTag, false);
                            else
                                channel.BasicNack(ea.DeliveryTag, false, true);
                        }
                        else
                        {
                            channel.BasicNack(ea.DeliveryTag, false, true);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.ToString());

                        channel.BasicAck(ea.DeliveryTag, false);
                    }
                };

                consumer.ConsumerCancelled += async (sender, ea) =>
                {
                    _logger.LogWarning($"{rabbitMqModel} consumption cancelled.");
                    await Task.CompletedTask;
                };

                consumer.Shutdown += async (sender, ea) =>
                {
                    _logger.LogWarning($"{rabbitMqModel} consumer shutdown.");
                    await Task.CompletedTask;
                };

                consumer.Unregistered += async (sender, ea) =>
                {
                    _logger.LogWarning($"{rabbitMqModel} consumer unregistered.");
                    await Task.CompletedTask;
                };

                var consumerTag = consumer.Model.BasicConsume(queue: rabbitMqModel.Queue, autoAck: false, consumer: consumer);

                _logger.LogInformation($"RabbitMQ Consumer started successfully for : {rabbitMqModel}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }

            await Task.CompletedTask;
        }

        #endregion

        #region Dispose
        public void Dispose()
        {
            if (!_disposedValue)
            {
                _disposedValue = true;

                cancellationTokenSource.Cancel();

                try
                {
                    if (_connection != null && _connection.IsOpen) _connection.Abort();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                }
                finally
                {
                    if (_connection != null && _connection.IsOpen) _connection.Close();

                    _logger.LogInformation("RabbitMQService was disposed.");
                }
            }

            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
