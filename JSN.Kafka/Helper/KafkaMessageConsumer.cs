using Confluent.Kafka;

namespace JSN.Kafka.Helper;

public class KafkaMessageConsumer : IDisposable
{
    private readonly IConsumer<Ignore, string> _consumer;

    public KafkaMessageConsumer()
    {
        var consumerBuilder = new ConsumerBuilder<Ignore, string>(KafkaHelper.Instance.GetKafkaConsumerConfig());
        _consumer = consumerBuilder.Build();
    }

    public void Dispose()
    {
        _consumer.Dispose();
    }

    public void Subscribe(List<string> topics)
    {
        _consumer.Subscribe(topics);
    }

    public void StartConsuming(Action<ConsumeResult<Ignore, string>> processMessage, bool isAutoCommit = true)
    {
        try
        {
            while (true)
            {
                try
                {
                    var message = _consumer.Consume();
                    processMessage.Invoke(message);
                    if (!isAutoCommit)
                    {
                        _consumer.Commit();
                    }
                }
                catch (ConsumeException e)
                {
                    Console.WriteLine($"Error occurred: {e.Error.Reason}");
                }
            }
        }
        catch (ConsumeException)
        {
            _consumer.Close();
        }
    }
}