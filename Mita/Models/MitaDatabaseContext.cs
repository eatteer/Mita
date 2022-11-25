using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Mita.Models
{
    public partial class MitaDatabaseContext : DbContext
    {
        public MitaDatabaseContext()
        {
        }

        public MitaDatabaseContext(DbContextOptions<MitaDatabaseContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Comment> Comments { get; set; } = null!;
        public virtual DbSet<Manga> Mangas { get; set; } = null!;
        public virtual DbSet<Review> Reviews { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=DESKTOP-HGQ9TVJ\\SQLEXPRESS;Database=MitaDatabase;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("comments");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Body)
                    .HasColumnType("ntext")
                    .HasColumnName("body");

                entity.Property(e => e.ReviewId).HasColumnName("review_id");

                entity.Property(e => e.Title)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("title");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Review)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.ReviewId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_review_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_comment_user_id");
            });

            modelBuilder.Entity<Manga>(entity =>
            {
                entity.ToTable("mangas");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Chapters).HasColumnName("chapters");

                entity.Property(e => e.MalUri)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("mal_uri");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("name");

                entity.Property(e => e.Score)
                    .HasColumnType("decimal(2, 0)")
                    .HasColumnName("score");

                entity.Property(e => e.Status)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("status");

                entity.Property(e => e.Synopsis)
                    .HasColumnType("ntext")
                    .HasColumnName("synopsis");

                entity.Property(e => e.Volumes).HasColumnName("volumes");
            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.ToTable("reviews");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Body)
                    .HasColumnType("ntext")
                    .HasColumnName("body");

                entity.Property(e => e.MangaId).HasColumnName("manga_id");

                entity.Property(e => e.Title)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("title");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Manga)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.MangaId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_manga_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_user_id");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.PasswordHash)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("password_hash");

                entity.Property(e => e.PasswordSalt)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("password_salt");

                entity.Property(e => e.Role)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("role");

                entity.Property(e => e.Username)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("username");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
