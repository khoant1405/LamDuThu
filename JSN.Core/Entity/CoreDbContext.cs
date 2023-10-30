using JSN.Core.Model;
using JSN.Shared.Setting;
using Microsoft.EntityFrameworkCore;

namespace JSN.Core.Entity;

public class CoreDbContext : DbContext
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
    {
        optionsBuilder.UseSqlServer(AppSettings.DefaultSqlSetting?.ConnectString);
    }

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

    protected static void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        throw new NotImplementedException();
    }
}