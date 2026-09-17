using EduSpaceAllocator.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EduSpaceAllocator.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Space> Spaces => Set<Space>();
}
