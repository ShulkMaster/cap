using Cap.Models;
using Microsoft.EntityFrameworkCore;

namespace Cap.Data;

public class QuakeContext: DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Quake> Quakes { get; set; } = null!;
    
    public QuakeContext(DbContextOptions options) : base(options)
    {
    }
}