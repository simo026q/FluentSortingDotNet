using Microsoft.EntityFrameworkCore;

namespace FluentSortingDotNet.Testing;

public sealed class PersonDbContext(DbContextOptions<PersonDbContext> options) : DbContext(options)
{
    public DbSet<Person> People { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>().HasKey(p => p.Id);
        modelBuilder.Entity<Person>().Property(p => p.Name).IsRequired();
        modelBuilder.Entity<Person>().Property(p => p.DateOfBirth).IsRequired();
    }
}