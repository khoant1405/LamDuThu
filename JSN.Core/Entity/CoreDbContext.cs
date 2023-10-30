using JSN.Core.Model;
using Microsoft.EntityFrameworkCore;

namespace JSN.Core.Entity;

public partial class CoreDbContext : DbContext
{
    public CoreDbContext()
    {
    }

    public CoreDbContext(DbContextOptions<CoreDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Article> Articles { get; set; }

    public virtual DbSet<ArticleContent> ArticleContents { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https: //go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer(
            "Server=localhost,1434;User ID=sa;Password=1405;TrustServerCertificate=True;Initial Catalog=CoreData;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Article>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Article__3214EC07F185A066");

            entity.HasOne(d => d.User).WithMany(p => p.Articles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Fk_Article_UserId");
        });

        modelBuilder.Entity<ArticleContent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ArticleC__3214EC07400C0F3C");

            entity.HasOne(d => d.Article).WithOne(p => p.ArticleContent)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Fk_ArticleContent_ArticleId");
        });

        modelBuilder.Entity<User>(entity => { entity.HasKey(e => e.Id).HasName("PK__User__3214EC07BCDECF13"); });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}