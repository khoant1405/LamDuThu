namespace JSN.Core.Model;

public class EventKafka
{
    public int Id { get; set; }

    public int ArticleId { get; set; }

    public int? CategoryId { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? CreatedBy { get; set; }
}