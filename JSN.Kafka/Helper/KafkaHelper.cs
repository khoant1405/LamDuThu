//using Confluent.Kafka;
//using Confluent.Kafka.Admin;
//using JSN.Shared.Setting;
//using Newtonsoft.Json;
////using KiotViet.Util.Entity;
////using KiotViet.Util.Logging;

//namespace JSN.Kafka.Helper;

//public sealed class KafkaHelper
//{
//    #region Set Kakfa config

//    public void SetKafkaConfig(KafkaConfig kafka)
//    {
//        _kafka = kafka;
//        // consumer config
//        _consumerConfig = new ConsumerConfig
//        {
//            BootstrapServers = kafka.KafkaIp,
//            GroupId = kafka.GroupId,
//            ClientId = kafka.ClientId,
//            EnableAutoCommit = false,
//            StatisticsIntervalMs = 5000,
//            SessionTimeoutMs = 6000,
//            // The auto.offset.reset config kicks in ONLY if your consumer group does not have a valid offset committed somewhere
//            // earliest: automatically reset the offset to the earliest offset
//            // latest: automatically reset the offset to the latest offset
//            // Latest should be the best for Tracking
//            AutoOffsetReset = AutoOffsetReset.Latest,
//            FetchMaxBytes = 104857600,
//            MaxPollIntervalMs = KvStatic.DefaultMaxPollIntervalMs
//        };
//        // using for debug only
//        if (KvAppConfig.IsKafkaMonitor)
//        {
//            _consumerConfig.Debug = "consumer,cgrp,topic,fetch";
//        }

//        // producer config
//        _producerConfig = new ProducerConfig
//        {
//            BootstrapServers = kafka.KafkaIp,
//            // Maximum Kafka protocol request message size.
//            // Due to differing framing overhead between protocol versions the producer is unable to reliably enforce a strict max message limit at produce time and may exceed the maximum size by one message in protocol ProduceRequests, the broker will enforce the the topic's `max.message.bytes` limit.
//            MessageMaxBytes = 104857600,
//            // Maximum size (in bytes) of all messages batched in one MessageSet, including protocol framing overhead.
//            // This limit is applied after the first message has been added to the batch, regardless of the first message's size, this is to ensure that messages that exceed batch.size are produced.
//            // The total MessageSet size is also limited by batch.num.messages and message.max.bytes.
//            BatchSize = 104857600
//        };
//        if (KvAppConfig.KafkaProducerBatchNumMessages > 0)
//        {
//            // Maximum number of messages batched in one MessageSet. The total MessageSet size is also limited by batch.size and message.max.bytes.
//            _producerConfig.BatchNumMessages = KvAppConfig.KafkaProducerBatchNumMessages;
//        }

//        if (KvAppConfig.KafkaProducerLingerMs > 0)
//        {
//            // Delay in milliseconds to wait for messages in the producer queue to accumulate before constructing message batches (MessageSets) to transmit to brokers.
//            // A higher value allows larger and more effective (less overhead, improved compression) batches of messages to accumulate at the expense of increased message delivery latency.
//            _producerConfig.LingerMs = KvAppConfig.KafkaProducerLingerMs;
//        }

//        if (KvAppConfig.KafkaProducerMessageSendMaxRetries > 0)
//        {
//            // How many times to retry sending a failing Message. **Note:** retrying may cause reordering unless `enable.idempotence` is set to true.
//            _producerConfig.MessageSendMaxRetries = KvAppConfig.KafkaProducerMessageSendMaxRetries;
//        }

//        if (KvAppConfig.KafkaProducerMessageTimeoutMs > 0)
//        {
//            // Local message timeout. This value is only enforced locally and limits the time a produced message waits for successful delivery.
//            // A time of 0 is infinite. This is the maximum time librdkafka may use to deliver a message (including retries).
//            // Delivery error occurs when either the retry count or the message timeout are exceeded.
//            // The message timeout is automatically adjusted to `transaction.timeout.ms` if `transactional.id` is configured.
//            _producerConfig.MessageTimeoutMs = KvAppConfig.KafkaProducerMessageTimeoutMs;
//        }

//        if (KvAppConfig.KafkaProducerRequestTimeoutMs > 0)
//        {
//            // The ack timeout of the producer request in milliseconds. This value is only enforced by the broker and relies on `request.required.acks` being != 0.
//            _producerConfig.RequestTimeoutMs = KvAppConfig.KafkaProducerRequestTimeoutMs;
//        }

//        // using for debug only
//        if (KvAppConfig.IsKafkaMonitor)
//        {
//            _producerConfig.Debug = "broker,topic,msg";
//        }

//        // init metadata
//        using (var adminClient =
//               new AdminClientBuilder(new AdminClientConfig { BootstrapServers = kafka.KafkaIp }).Build())
//        {
//            try
//            {
//                _kafkaMetadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
//            }
//            catch (Exception ex)
//            {
//                KvException.WriteException(new LogObject(GetType().Name, $"{GetType().Name}.{nameof(SetKafkaConfig)}"),
//                    ex);
//            }
//        }
//    }

//    #endregion

//    #region Topic

//    public async Task SetTopic(string topic, int size)
//    {
//        using (var adminClient =
//               new AdminClientBuilder(new AdminClientConfig { BootstrapServers = _kafka.KafkaIp }).Build())
//        {
//            try
//            {
//                if (!_kafkaMetadata.Topics.Exists(x => x.Topic == topic))
//                {
//                    await adminClient.CreateTopicsAsync(new[]
//                    {
//                        new TopicSpecification
//                        {
//                            Name = topic, ReplicationFactor = _kafka.Replica,
//                            NumPartitions = size * _kafka.PartitionSize
//                        }
//                    });
//                }
//            }
//            catch (CreateTopicsException e)
//            {
//                KvException.WriteException(new LogObject(GetType().Name, $"{GetType().Name}.{nameof(SetTopic)}"), e);
//                Console.WriteLine($"An error occured creating topic {e.Results[0].Topic}: {e.Results[0].Error.Reason}");
//            }
//        }
//    }

//    #endregion

//    #region Consumer

//    public void SetOnHandle(ConsumerBuilder<Ignore, string> consumer, int indexConsumer = -1)
//    {
//        consumer.SetErrorHandler((_, e) =>
//            {
//                // method apply for Kafka only
//                KvException.WriteException(new LogObject(GetType().Name, $"{GetType().Name}.SetErrorHandler"),
//                    new KafkaException(e), null, $"Kafka Consumer Error: {JsonConvert.SerializeObject(e)}");
//            })
//            .SetLogHandler((_, e) =>
//            {
//                if (KvAppConfig.ServiceMode.Equals("console"))
//                {
//                    Console.WriteLine(
//                        $"Kafka consumer Log Handler, name: {e.Name}, {e.Facility}, {e.Level}, message: {e.Message}");
//                }

//                if (KvAppConfig.IsKafkaMonitor)
//                {
//                    KvException.WriteInfo(
//                        $"Kafka consumer Log Handler, name: {e.Name}, {e.Facility}, {e.Level}, message: {e.Message}",
//                        KvLogType.Kafka);
//                }
//            })
//            .SetStatisticsHandler((_, json) =>
//            {
//                if (KvAppConfig.IsKafkaMonitor)
//                {
//                    KvException.WriteInfo($"Kafka statistics: {json}", KvLogType.Kafka);
//                }
//            }).SetPartitionsAssignedHandler((c, partitions) =>
//            {
//                if (KvAppConfig.ServiceMode.Equals("console"))
//                {
//                    Console.WriteLine($"Assigned partitions: [{string.Join(", ", partitions)}]");
//                }

//                if (KvAppConfig.IsKafkaMonitor)
//                {
//                    KvException.WriteInfo($"Assigned partitions: [{string.Join(", ", partitions)}]", KvLogType.Kafka);
//                }

//                if ((KvAppConfig.IsMonitorThreadImprove || KvAppConfig.IsMonitorRecoveryFromMongodb) &&
//                    indexConsumer > -1)
//                {
//                    KvException.WriteInfo(
//                        $"Assigned partitions: IndexConsumer: {indexConsumer} | Topic: {partitions.FirstOrDefault()?.Topic} | Partitions: [{string.Join(", ", partitions.Select(p => p.Partition.Value).ToList())}]",
//                        KvLogType.Kafka);
//                }
//            })
//            .SetPartitionsRevokedHandler((c, partitions) =>
//            {
//                if (KvAppConfig.ServiceMode.Equals("console"))
//                {
//                    Console.WriteLine($"Revoking assignment: [{string.Join(", ", partitions)}]");
//                }

//                if (KvAppConfig.IsKafkaMonitor)
//                {
//                    KvException.WriteInfo($"Revoking assignment: [{string.Join(", ", partitions)}]", KvLogType.Kafka);
//                }
//            });
//    }

//    #endregion

//    #region Constructor

//    public static KafkaHelper Instance => Nested.instance;

//    private static class Nested
//    {
//        // ReSharper disable once InconsistentNaming
//        internal static readonly KafkaHelper instance = new();

//        // Explicit static constructor to tell C# compiler
//        // not to mark type as beforefieldinit
//        static Nested()
//        {
//        }
//    }

//    private KafkaHelper()
//    {
//    }

//    // build config
//    private ConsumerConfig _consumerConfig;

//    private ProducerConfig _producerConfig;

//    // config kafka from app.json
//    private KafkaConfig _kafka;
//    private Metadata _kafkaMetadata;
//    private IProducer<string, string> _producer;

//    #endregion

//    #region Get Kafka config

//    public ConsumerConfig GetKafkaConsumerConfig()
//    {
//        return _consumerConfig;
//    }

//    public Kafka GetKafka()
//    {
//        return _kafka;
//    }

//    #endregion

//    #region Producer

//    public void InitProducer()
//    {
//        var producerBuilder = new ProducerBuilder<string, string>(_producerConfig);
//        // set error handle
//        producerBuilder.SetErrorHandler((_, e) =>
//        {
//            KvException.WriteException(new LogObject(GetType().Name, $"{GetType().Name}.{nameof(InitProducer)}"),
//                new KafkaException(e), null, $"Kafka Producer Error: {JsonConvert.SerializeObject(e)}");
//        });
//        // set log handle
//        if (KvAppConfig.IsKafkaMonitor)
//        {
//            producerBuilder.SetLogHandler((_, logMessage) =>
//            {
//                KvException.WriteInfo($"Kafka log: {logMessage.ToJson()}", KvLogType.Kafka);
//            });
//        }

//        // build producer
//        _producer = new ProducerBuilder<string, string>(_producerConfig).Build();
//    }

//    public void PublishMessage(string topic, string key, string jsonData, bool isLog = true)
//    {
//        try
//        {
//            if (KvAppConfig.KafkaIsUseProduceAsync)
//            {
//                var deliveryResult =
//                    _producer.ProduceAsync(topic, new Message<string, string> { Key = key, Value = jsonData });
//                deliveryResult.ContinueWith(task =>
//                {
//                    if (!isLog)
//                    {
//                        return;
//                    }

//                    if (task.IsFaulted)
//                    {
//                        KvException.WriteException(
//                            new LogObject(GetType().Name, $"{GetType().Name}.{nameof(PublishMessage)}"),
//                            new KvLogException(
//                                $"Kafka producer sent msg failed: Topic: {topic} | Key: {key} | Data: {jsonData} | Exception: {task.Exception.ToJson()}"));
//                    }
//                });
//            }
//            else
//            {
//                // async producer
//                // sync will kill performance
//                // using result callback instead of sync to wait result
//                void Handler(DeliveryReport<string, string> dr)
//                {
//                    if (!isLog || dr.Error.Code == ErrorCode.NoError)
//                    {
//                        return;
//                    }

//                    var msg = dr.Message.Value;
//                    KvException.WriteException(
//                        new LogObject(GetType().Name, $"{GetType().Name}.{nameof(PublishMessage)}"),
//                        new KvLogException(
//                            $"Kafka producer sent msg failed: Topic: {topic} | Key: {key} | Data: {msg} | Exception: {dr.Error.ToJson()}"));
//                }

//                _producer.Produce(topic, new Message<string, string> { Key = key, Value = jsonData }, Handler);
//            }
//        }
//        catch (Exception ex)
//        {
//            KvException.WriteException(new LogObject(GetType().Name, $"{GetType().Name}.{nameof(PublishMessage)}"), ex);
//        }
//    }

//    #endregion
//}