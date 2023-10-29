using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace JSN.Core.Model
{
    [Table("ArticleContent")]
    [Index("ArticleId", Name = "UQ__ArticleC__9C6270E921CC6257", IsUnique = true)]
    public class ArticleContent
    {
        [Key]
        public int Id { get; set; }

        public int ArticleId { get; set; }

        public string? Content { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreatedOn { get; set; }

        public int? CreatedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ModifiedOn { get; set; }

        public int? ModifiedBy { get; set; }

        [ForeignKey("ArticleId")]
        [InverseProperty("ArticleContent")]
        public virtual Article Article { get; set; } = null!;
    }
}
