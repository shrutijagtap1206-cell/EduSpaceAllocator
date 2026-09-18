using Microsoft.EntityFrameworkCore;
using EduSpaceAllocator.API.Models;

namespace EduSpaceAllocator.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Space> Spaces => Set<Space>();

    public DbSet<LearningRequest> LearningRequests => Set<LearningRequest>();

    public DbSet<Community> Communities => Set<Community>();

    public DbSet<Allocation> Allocations => Set<Allocation>();

    public DbSet<Partner> Partners => Set<Partner>();

    public DbSet<Course> Courses => Set<Course>();

    public DbSet<Schedule> Schedules => Set<Schedule>();

    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    public DbSet<SocialImpact> SocialImpacts => Set<SocialImpact>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Learning Request → Community
        modelBuilder.Entity<LearningRequest>()
            .HasOne(r => r.Community)
            .WithMany(c => c.LearningRequests)
            .HasForeignKey(r => r.CommunityId)
            .OnDelete(DeleteBehavior.Restrict);

        // Allocation → Space
        modelBuilder.Entity<Allocation>()
            .HasOne(a => a.Space)
            .WithMany()
            .HasForeignKey(a => a.SpaceId)
            .OnDelete(DeleteBehavior.Restrict);

        // Allocation → Learning Request
        modelBuilder.Entity<Allocation>()
            .HasOne(a => a.LearningRequest)
            .WithMany()
            .HasForeignKey(a => a.RequestId)
            .OnDelete(DeleteBehavior.Restrict);

        // Schedule → Allocation
        modelBuilder.Entity<Schedule>()
            .HasOne(s => s.Allocation)
            .WithMany()
            .HasForeignKey(s => s.AllocationId)
            .OnDelete(DeleteBehavior.Restrict);

        // Schedule → Course
        modelBuilder.Entity<Schedule>()
            .HasOne(s => s.Course)
            .WithMany()
            .HasForeignKey(s => s.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        // Social Impact → Allocation
        modelBuilder.Entity<SocialImpact>()
            .HasOne(s => s.Allocation)
            .WithMany()
            .HasForeignKey(s => s.AllocationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}