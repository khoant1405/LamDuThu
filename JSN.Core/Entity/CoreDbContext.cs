using JSN.Core.Model;
using Microsoft.EntityFrameworkCore;

namespace JSN.Core.Entity;

public partial class CoreDbContext : DbContext
{
    public CoreDbContext()
    {
    }

    public CoreDbContext(DbContextOptions<CoreDbContext> options) : base(options)
    {
    }

    public virtual DbSet<Article> Articles { get; set; }

    public virtual DbSet<ArticleContent> ArticleContents { get; set; }

    public virtual DbSet<EventKafka> EventKafkas { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https: //go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        =>
            optionsBuilder.UseSqlServer("Server=localhost,1434;User ID=sa;Password=1405;TrustServerCertificate=True;Initial Catalog=CoreData;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Article>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Article__3214EC076CFC0EDE");

            entity.ToTable("Article");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.ImageThumb).HasMaxLength(500);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.RefUrl).HasMaxLength(500).HasColumnName("RefURL");
            entity.Property(e => e.UserName).HasMaxLength(50);

            entity.HasOne(d => d.User).WithMany(p => p.Articles).HasForeignKey(d => d.UserId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("Fk_Article_UserId");
        });

        modelBuilder.Entity<ArticleContent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ArticleC__3214EC07BEE5FD86");

            entity.ToTable("ArticleContent");

            entity.HasIndex(e => e.ArticleId, "UQ__ArticleC__9C6270E9B275F87B").IsUnique();

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

            entity.HasOne(d => d.Article).WithOne(p => p.ArticleContent).HasForeignKey<ArticleContent>(d => d.ArticleId).OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Fk_ArticleContent_ArticleId");
        });

        modelBuilder.Entity<EventKafka>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EventKaf__3214EC070761C1B3");

            entity.ToTable("EventKafka");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC07C0C9656B");

            entity.ToTable("User");

            entity.HasIndex(e => e.UserName, "UQ__User__C9F2845641D3D0E3").IsUnique();

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.TokenCreated).HasColumnType("datetime");
            entity.Property(e => e.TokenExpired).HasColumnType("datetime");
            entity.Property(e => e.UserName).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}