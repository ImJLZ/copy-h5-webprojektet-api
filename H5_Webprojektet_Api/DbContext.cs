using H5_Webprojektet_Api.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace H5_Webprojektet_Api;

public class DbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbContext(DbContextOptions<DbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<AppUser>(model =>
        {
            model.HasKey(x => x.Id);
            model.Property(x => x.Email).HasMaxLength(254);
            model.HasMany(x => x.TodoEntries)
                .WithOne(x => x.User);
        });
        builder.Entity<TodoEntry>(model =>
        {
            model.HasKey(x => x.Id);
            model.HasOne(x => x.User)
                .WithMany(x => x.TodoEntries);
        });
        base.OnModelCreating(builder);
    }

    public DbSet<AppUser> Users { get; set; } = null!;
    public DbSet<TodoEntry> TodoEntries { get; set; } = null!;
}