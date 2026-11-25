using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Artist> Artists { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(user =>
        {
            user.ToTable("users");

            user.HasKey(u => u.UserId);

            user.Property(u => u.UserId).HasColumnName("pk_user_id");
            user.Property(u => u.FirstName).HasColumnName("name");
            user.Property(u => u.LastName).HasColumnName("surname");
            user.Property(u => u.Email).HasColumnName("email_address");
            user.Property(u => u.PhoneNumber).HasColumnName("phone_number");
            user.Property(u => u.PasswordHash).HasColumnName("password_hash");
            user.Property(u => u.Role).HasColumnName("role").HasDefaultValue("USER");
            user.Property(u => u.CreatedAt).HasColumnName("joined_at");

            user.HasIndex(u => u.Email).IsUnique();
        });
    }
}
