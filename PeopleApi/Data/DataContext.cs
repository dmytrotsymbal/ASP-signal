using Microsoft.EntityFrameworkCore;
using PeopleApi.Models;

namespace PeopleApi.Data;

public class DataContext : DbContext
{
    public DbSet<Person> People { get; set; }

    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>().HasKey(p => p.Id);
    }
}