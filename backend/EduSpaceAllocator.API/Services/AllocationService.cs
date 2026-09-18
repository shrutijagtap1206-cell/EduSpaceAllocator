using EduSpaceAllocator.API.Data;
using EduSpaceAllocator.API.DTOs;
using EduSpaceAllocator.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EduSpaceAllocator.API.Services;

public class AllocationService
{
    private readonly AppDbContext _context;

    public AllocationService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<AllocationDto>> GetAllAsync()
    {
        return await _context.Allocations
            .AsNoTracking()
            .OrderByDescending(a => a.OptimizationScore)
            .Select(a => new AllocationDto
            {
                AllocationId = a.AllocationId,
                SpaceId = a.SpaceId,
                RequestId = a.RequestId,
                MatchId = a.MatchId,
                DistanceKm = a.DistanceKm,
                MatchScore = a.MatchScore,
                CapacityUtilization = a.CapacityUtilization,
                CostEfficiency = a.CostEfficiency,
                StudentReach = a.StudentReach,
                StudentsServed = a.StudentsServed,
                OptimizationScore = a.OptimizationScore,
                Status = a.Status
            })
            .ToListAsync();
    }

    public async Task<AllocationDto?> GetByIdAsync(int id)
    {
        return await _context.Allocations
            .AsNoTracking()
            .Where(a => a.AllocationId == id)
            .Select(a => new AllocationDto
            {
                AllocationId = a.AllocationId,
                SpaceId = a.SpaceId,
                RequestId = a.RequestId,
                MatchId = a.MatchId,
                DistanceKm = a.DistanceKm,
                MatchScore = a.MatchScore,
                CapacityUtilization = a.CapacityUtilization,
                CostEfficiency = a.CostEfficiency,
                StudentReach = a.StudentReach,
                StudentsServed = a.StudentsServed,
                OptimizationScore = a.OptimizationScore,
                Status = a.Status
            })
            .FirstOrDefaultAsync();
    }

    public async Task<AllocationDto> CreateAsync(Allocation allocation)
    {
        _context.Allocations.Add(allocation);
        await _context.SaveChangesAsync();

        return new AllocationDto
        {
            AllocationId = allocation.AllocationId,
            SpaceId = allocation.SpaceId,
            RequestId = allocation.RequestId,
            MatchId = allocation.MatchId,
            DistanceKm = allocation.DistanceKm,
            MatchScore = allocation.MatchScore,
            CapacityUtilization = allocation.CapacityUtilization,
            CostEfficiency = allocation.CostEfficiency,
            StudentReach = allocation.StudentReach,
            StudentsServed = allocation.StudentsServed,
            OptimizationScore = allocation.OptimizationScore,
            Status = allocation.Status
        };
    }

    public async Task<bool> UpdateStatusAsync(
        int id,
        string status)
    {
        var allocation = await _context.Allocations
            .FirstOrDefaultAsync(a => a.AllocationId == id);

        if (allocation == null)
        {
            return false;
        }

        allocation.Status = status;

        await _context.SaveChangesAsync();

        return true;
    }
}