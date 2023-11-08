using Confluent.Kafka;
using Confluent.Kafka.Admin;
using JSN.Shared.Config;
using JSN.Shared.Utilities;
using Newtonsoft.Json;

namespace JSN.Kafka.Helper;

public sealed class KafkaHelper
{
    #region Set Kakfa config

    public void SetKafkaConfig()
    {
        // Consumer config
        _consumerConfig = new ConsumerConfig
        {
            BootstrapServers = _kafka.KafkaIp,
            GroupId = _kafka.GroupId,

            // ID duy nhất cho Consumer. Điều này giúp dõi các Consumer. 
            ClientId = _kafka.ClientId,

            // Nếu = false, Consumer sẽ không tự động xác nhận việc đọc message từ Kafka broker. Bạn sẽ phải tự quản lý xác nhận (commit) khi bạn đã xử lý thành công các message.
            EnableAutoCommit = false,

            // Khoảng thời gian giữa các lần gửi thông tin thống kê từ Consumer đến Kafka broker, để theo dõi hiệu suất và hoạt động của Consumer.
            StatisticsIntervalMs = 5000,

            // Thời gian chờ cho phiên làm việc của Consumer. Nếu Consumer không gửi tin hiệu hoạt động đến Kafka broker trong khoảng thời gian này, nó có thể bị coi là đã ngừng hoạt động.
            // Kafka broker sẽ gán lại các partition mà Consumer đang đọc cho một Consumer khác trong cùng nhóm.
            SessionTimeoutMs = 6000,

            // Xác định hành vi của Consumer khi không có offset lưu trữ cho các partition mà nó muốn đọc.
            // Giá trị "Latest" sẽ làm Consumer đọc message mới nhất, trong khi "Earliest" sẽ đọc từ đầu (các message cũ).
            AutoOffsetReset = AutoOffsetReset.Latest,

            // Kích thước tối đa mà Consumer có thể yêu cầu từ một lần fetch.
            // Điều này ảnh hưởng đến kích thước của các gói dữ liệu mà Consumer yêu cầu từ Kafka broker mỗi khi nó lấy dữ liệu mới.
            FetchMaxBytes = 104857600,

            // Thời gian tối đa giữa các lần poll. Polling là hoạt động mà Consumer sử dụng để lấy dữ liệu từ Kafka broker.
            // Poll là một hoạt động quan trọng trong việc lấy và xử lý dữ liệu từ Kafka broker. Nó cho phép Consumer lấy dữ liệu từ các partition và thực hiện các xử lý cần thiết.
            // Thời gian giữa các lần poll quyết định tốc độ mà Consumer đọc dữ liệu từ Kafka và cũng quan trọng để Kafka broker có thể theo dõi trạng thái hoạt động của Consumer.
            MaxPollIntervalMs = JsnStatic.DefaultMaxPollIntervalMs
        };

        // Using for debug only
        // Theo dõi và ghi lại các sự kiện quan trọng liên quan đến hoạt động của Consumer và giao tiếp với Kafka broker.
        if (_kafka.IsKafkaMonitor)
        {
            // consumer: Ghi lại các sự kiện liên quan đến hoạt động của Consumer.
            // cgrp: Ghi lại các sự kiện liên quan đến quản lý của Consumer Group.
            // topic: Ghi lại các sự kiện liên quan đến các chủ đề (topics) mà Consumer đang theo dõi.
            // fetch: Ghi lại các sự kiện liên quan đến việc lấy dữ liệu từ Kafka broker.
            _consumerConfig.Debug = "consumer,cgrp,topic,fetch";
        }

        // producer config
        _producerConfig = new ProducerConfig
        {
            BootstrapServers = _kafka.KafkaIp,

            // Thuộc tính này xác định kích thước tối đa của một message Kafka mà Producer có thể gửi. Kích thước này thường được đo bằng byte.
            // Ví dụ: 104857600 byte tương đương với khoảng 100 MB.
            MessageMaxBytes = 104857600,

            // Đây là kích thước tối đa cho một batch (gói) của các message Kafka mà Producer sẽ gửi trong một lần gửi dữ liệu đến Kafka broker.
            // Batch là một tập hợp của các message được gửi cùng nhau để giảm độ trễ và tối ưu hóa hiệu suất gửi. 
            BatchSize = 104857600
        };

        if (_kafkaProducer.BatchNumMessages > 0)
        {
            // Số lượng message tối đa được gửi trong 1 batch
            _producerConfig.BatchNumMessages = _kafkaProducer.BatchNumMessages;
        }

        if (_kafkaProducer.LingerMs > 0)
        {
            // Thời gian chờ giữa các lần gửi batch đi. Càng cao số message trong 1 batch càng nhiều
            _producerConfig.LingerMs = _kafkaProducer.LingerMs;
        }

        if (_kafkaProducer.MessageSendMaxRetries > 0)
        {
            // Số lần cố gắng gửi lại một message nếu nó không thể được gửi thành công lần đầu tiên
            _producerConfig.MessageSendMaxRetries = _kafkaProducer.MessageSendMaxRetries;
        }

        if (_kafkaProducer.MessageTimeoutMs > 0)
        {
            // Xác định thời gian mà một message cố gắng chờ để được gửi thành công.
            // Nếu trong khoảng thời gian này message không thể được gửi thành công, nó có thể bị coi là gửi không thành công và có thể gây ra lỗi
            // Nếu được đặt thành 0, thì thời gian chờ là vô hạn, nghĩa là message sẽ chờ mãi cho đến khi nó được gửi thành công hoặc xảy ra lỗi.
            // Nếu bạn đã cấu hình transactional.id, thì thời gian chờ của message (MessageTimeoutMs) có thể được tự động điều chỉnh để phù hợp với transaction.timeout.ms.
            _producerConfig.MessageTimeoutMs = _kafkaProducer.MessageTimeoutMs;
        }

        if (_kafkaProducer.RequestTimeoutMs > 0)
        {
            // Nếu được đặt thành 10000 (10 giây), thì Kafka Producer sẽ gửi message đến broker và chờ đợi tối đa 10 giây để message này được xử lý và có kết quả trả về từ broker.
            // Nếu trong thời gian 10 giây đó, message không được xử lý và trả về kết quả, Kafka Producer có thể xem xét message này là thất bại do vượt quá thời gian chờ.
            // Chỉ áp dụng khi request.required.acks != 0.
            _producerConfig.RequestTimeoutMs = _kafkaProducer.RequestTimeoutMs;
        }

        // Using for debug only
        if (_kafka.IsKafkaMonitor)
        {
            // broker: Bật ghi log liên quan đến thông tin về các kết nối và giao tiếp với các broker Kafka.
            // topic: Bật ghi log liên quan đến thông tin về các sự kiện liên quan đến các chủ đề (topics).
            // msg: Bật ghi log liên quan đến thông tin về các tin nhắn (messages) được gửi và nhận.
            _producerConfig.Debug = "broker,topic,msg";
        }

        // Lấy thông tin của broker, topic...
        using var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = _kafka.KafkaIp }).Build();
        try
        {
            _kafkaMetadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
        }
        catch (Exception ex)
        {
            //KvException.WriteException(new LogObject(GetType().Name, $"{GetType().Name}.{nameof(SetKafkaConfig)}"), ex);
        }
    }

    #endregion

    #region Topic

    public async Task SetTopic(string topic, int size)
    {
        using (var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = _kafka.KafkaIp }).Build())
        {
            try
            {
                if (!_kafkaMetadata.Topics.Exists(x => x.Topic == topic))
                {
                    await adminClient.CreateTopicsAsync(new[]
                    {
                        new TopicSpecification { Name = topic, ReplicationFactor = _kafka.Replica, NumPartitions = size * _kafka.PartitionSize }
                    });
                }
            }
            catch (CreateTopicsException e)
            {
                //KvException.WriteException(new LogObject(GetType().Name, $"{GetType().Name}.{nameof(SetTopic)}"), e);
                Console.WriteLine($"An error occured creating topic {e.Results[0].Topic}: {e.Results[0].Error.Reason}");
            }
        }
    }

    #endregion

    #region Consumer

    public void SetOnHandle(ConsumerBuilder<Ignore, string> consumer, int indexConsumer = -1)
    {
        consumer.SetErrorHandler((_, e) =>
        {
            // method apply for Kafka only
            KvException.WriteException(new LogObject(GetType().Name, $"{GetType().Name}.SetErrorHandler"), new KafkaException(e), null,
                $"Kafka Consumer Error: {JsonConvert.SerializeObject(e)}");
        }).SetLogHandler((_, e) =>
        {
            if (KvAppConfig.ServiceMode.Equals("console"))
            {
                Console.WriteLine($"Kafka consumer Log Handler, name: {e.Name}, {e.Facility}, {e.Level}, message: {e.Message}");
            }

            if (KvAppConfig.IsKafkaMonitor)
            {
                KvException.WriteInfo($"Kafka consumer Log Handler, name: {e.Name}, {e.Facility}, {e.Level}, message: {e.Message}", KvLogType.Kafka);
            }
        }).SetStatisticsHandler((_, json) =>
        {
            if (KvAppConfig.IsKafkaMonitor)
            {
                KvException.WriteInfo($"Kafka statistics: {json}", KvLogType.Kafka);
            }
        }).SetPartitionsAssignedHandler((c, partitions) =>
        {
            if (KvAppConfig.ServiceMode.Equals("console"))
            {
                Console.WriteLine($"Assigned partitions: [{string.Join(", ", partitions)}]");
            }

            if (KvAppConfig.IsKafkaMonitor)
            {
                KvException.WriteInfo($"Assigned partitions: [{string.Join(", ", partitions)}]", KvLogType.Kafka);
            }

            if ((KvAppConfig.IsMonitorThreadImprove || KvAppConfig.IsMonitorRecoveryFromMongodb) && indexConsumer > -1)
            {
                KvException.WriteInfo(
                    $"Assigned partitions: IndexConsumer: {indexConsumer} | Topic: {partitions.FirstOrDefault()?.Topic} | Partitions: [{string.Join(", ", partitions.Select(p => p.Partition.Value).ToList())}]",
                    KvLogType.Kafka);
            }
        }).SetPartitionsRevokedHandler((c, partitions) =>
        {
            if (KvAppConfig.ServiceMode.Equals("console"))
            {
                Console.WriteLine($"Revoking assignment: [{string.Join(", ", partitions)}]");
            }

            if (KvAppConfig.IsKafkaMonitor)
            {
                KvException.WriteInfo($"Revoking assignment: [{string.Join(", ", partitions)}]", KvLogType.Kafka);
            }
        });
    }

    #endregion

    #region Constructor

    public static KafkaHelper Instance => Nested.instance;

    private static class Nested
    {
        // ReSharper disable once InconsistentNaming
        internal static readonly KafkaHelper instance = new();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static Nested()
        {
        }
    }

    private KafkaHelper()
    {
    }

    // build config
    private ConsumerConfig _consumerConfig;

    private ProducerConfig _producerConfig;

    // config _kafka from app.json
    private readonly KafkaConfig _kafka = AppConfig.KafkaConfig;
    private readonly KafkaProducerConfig _kafkaProducer = AppConfig.KafkaProducerConfig;
    private Metadata _kafkaMetadata;
    private IProducer<string, string> _producer;

    #endregion

    #region Get Kafka config

    public ConsumerConfig GetKafkaConsumerConfig()
    {
        return _consumerConfig;
    }

    public KafkaConfig GetKafka()
    {
        return _kafka;
    }

    #endregion

    #region Producer

    public void InitProducer()
    {
        var producerBuilder = new ProducerBuilder<string, string>(_producerConfig);
        // set error handle
        producerBuilder.SetErrorHandler((_, e) =>
        {
            KvException.WriteException(new LogObject(GetType().Name, $"{GetType().Name}.{nameof(InitProducer)}"), new KafkaException(e), null,
                $"Kafka Producer Error: {JsonConvert.SerializeObject(e)}");
        });
        // set log handle
        if (KvAppConfig.IsKafkaMonitor)
        {
            producerBuilder.SetLogHandler((_, logMessage) => { KvException.WriteInfo($"Kafka log: {logMessage.ToJson()}", KvLogType.Kafka); });
        }

        // build producer
        _producer = new ProducerBuilder<string, string>(_producerConfig).Build();
    }

    public void PublishMessage(string topic, string key, string jsonData, bool isLog = true)
    {
        try
        {
            if (KvAppConfig.KafkaIsUseProduceAsync)
            {
                var deliveryResult = _producer.ProduceAsync(topic, new Message<string, string> { Key = key, Value = jsonData });
                deliveryResult.ContinueWith(task =>
                {
                    if (!isLog)
                    {
                        return;
                    }

                    if (task.IsFaulted)
                    {
                        KvException.WriteException(new LogObject(GetType().Name, $"{GetType().Name}.{nameof(PublishMessage)}"),
                            new KvLogException($"Kafka producer sent msg failed: Topic: {topic} | Key: {key} | Data: {jsonData} | Exception: {task.Exception.ToJson()}"));
                    }
                });
            }
            else
            {
                // async producer
                // sync will kill performance
                // using result callback instead of sync to wait result
                void Handler(DeliveryReport<string, string> dr)
                {
                    if (!isLog || dr.Error.Code == ErrorCode.NoError)
                    {
                        return;
                    }

                    var msg = dr.Message.Value;
                    KvException.WriteException(new LogObject(GetType().Name, $"{GetType().Name}.{nameof(PublishMessage)}"),
                        new KvLogException($"Kafka producer sent msg failed: Topic: {topic} | Key: {key} | Data: {msg} | Exception: {dr.Error.ToJson()}"));
                }

                _producer.Produce(topic, new Message<string, string> { Key = key, Value = jsonData }, Handler);
            }
        }
        catch (Exception ex)
        {
            KvException.WriteException(new LogObject(GetType().Name, $"{GetType().Name}.{nameof(PublishMessage)}"), ex);
        }
    }

    #endregion
}