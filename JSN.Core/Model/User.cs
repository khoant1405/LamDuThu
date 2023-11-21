namespace JSN.Core.Model;

public class User
{
    public int Id { get; set; }

    public string UserName { get; set; } = null!;

    public bool IsActive { get; set; }

    public int? Role { get; set; }

    public byte[] PasswordHash { get; set; } = null!;

    public byte[] PasswordSalt { get; set; } = null!;

    public string? RefreshToken { get; set; }

    public DateTime? TokenCreated { get; set; }

    public DateTime? TokenExpired { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public int? ModifiedBy { get; set; }

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();
}