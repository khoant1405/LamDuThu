namespace JSN.Core.Model;

public class KafkaEvent
{
    public int Id { get; set; }

    public int ArticleId { get; set; }

    public int? CategoryId { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? CreatedBy { get; set; }
}