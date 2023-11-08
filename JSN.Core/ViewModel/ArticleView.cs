namespace JSN.Core.ViewModel;

public class ArticleView
{
    public int Id { get; set; }

    public string ArticleName { get; set; } = null!;

    public int Status { get; set; }

    public string RefUrl { get; set; } = null!;

    public string? ImageThumb { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public int? ModifiedBy { get; set; }

    public int UserId { get; set; }

    public string? UserName { get; set; }
}