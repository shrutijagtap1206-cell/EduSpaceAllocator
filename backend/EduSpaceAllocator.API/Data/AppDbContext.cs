using Microsoft.EntityFrameworkCore;
using EduSpaceAllocator.API.Models;

namespace EduSpaceAllocator.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Space> Spaces => Set<Space>();

    public DbSet<LearningRequest> LearningRequests => Set<LearningRequest>();
}
