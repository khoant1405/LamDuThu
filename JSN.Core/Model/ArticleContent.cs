namespace JSN.Core.Model;

public class ArticleContent
{
    public int Id { get; set; }

    public int ArticleId { get; set; }

    public string? Content { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public int? ModifiedBy { get; set; }

    public virtual Article Article { get; set; } = null!;
}