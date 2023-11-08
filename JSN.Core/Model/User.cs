using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace JSN.Core.Model;

[Table("User")]
[Index("UserName", Name = "UQ__User__C9F2845605BBAAF4", IsUnique = true)]
public class User
{
    [Key] public int Id { get; set; }

    [StringLength(50)] public string UserName { get; set; } = null!;

    public bool IsActive { get; set; }

    public int? Role { get; set; }

    public byte[] PasswordHash { get; set; } = null!;

    public byte[] PasswordSalt { get; set; } = null!;

    public string? RefreshToken { get; set; }

    [Column(TypeName = "datetime")] public DateTime? TokenCreated { get; set; }

    [Column(TypeName = "datetime")] public DateTime? TokenExpired { get; set; }

    [Column(TypeName = "datetime")] public DateTime? CreatedOn { get; set; }

    public int? CreatedBy { get; set; }

    [Column(TypeName = "datetime")] public DateTime? ModifiedOn { get; set; }

    public int? ModifiedBy { get; set; }

    [InverseProperty("User")] public virtual ICollection<Article> Articles { get; } = new List<Article>();
}
