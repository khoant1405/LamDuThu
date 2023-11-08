using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JSN.Core.Model;

[Table("Article")]
public class Article
{
    [Key] public int Id { get; set; }

    public string ArticleName { get; set; } = null!;

    public int Status { get; set; }

    [Column("RefURL")] [StringLength(500)] public string RefUrl { get; set; } = null!;

    [StringLength(500)] public string? ImageThumb { get; set; }

    public string? Description { get; set; }

    [Column(TypeName = "datetime")] public DateTime? CreatedOn { get; set; }

    public int? CreatedBy { get; set; }

    [Column(TypeName = "datetime")] public DateTime? ModifiedOn { get; set; }

    public int? ModifiedBy { get; set; }

    public int UserId { get; set; }

    [StringLength(50)] public string? UserName { get; set; }

    [InverseProperty("Article")] public virtual ArticleContent? ArticleContent { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Articles")]
    public virtual User User { get; set; } = null!;
}