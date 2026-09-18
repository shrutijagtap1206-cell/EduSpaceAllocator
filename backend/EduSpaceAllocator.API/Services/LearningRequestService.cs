using EduSpaceAllocator.API.Data;
using EduSpaceAllocator.API.DTOs;
using EduSpaceAllocator.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EduSpaceAllocator.API.Services;

public class LearningRequestService
{
    private readonly AppDbContext _context;

    public LearningRequestService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<LearningRequestDto>> GetAllAsync()
    {
        return await _context.LearningRequests
            .AsNoTracking()
            .Select(r => new LearningRequestDto
            {
                RequestId = r.RequestId,
                Organization = r.Organization,
                ProgramType = r.ProgramType,
                StudentCapacity = r.StudentCapacity,
                Budget = r.Budget,
                PreferredLocation = r.PreferredLocation,
                PreferredLatitude = r.PreferredLatitude,
                PreferredLongitude = r.PreferredLongitude,
                CommunityId = r.CommunityId
            })
            .ToListAsync();
    }

    public async Task<LearningRequestDto?> GetByIdAsync(int id)
    {
        var request = await _context.LearningRequests.FindAsync(id);

        if (request == null)
            return null;

        return new LearningRequestDto
        {
            RequestId = request.RequestId,
            Organization = request.Organization,
            ProgramType = request.ProgramType,
            StudentCapacity = request.StudentCapacity,
            Budget = request.Budget,
            PreferredLocation = request.PreferredLocation,
            PreferredLatitude = request.PreferredLatitude,
            PreferredLongitude = request.PreferredLongitude,
            CommunityId = request.CommunityId
        };
    }

    public async Task<LearningRequestDto> CreateAsync(LearningRequestDto dto)
    {
        var request = new LearningRequest
        {
            Organization = dto.Organization,
            ProgramType = dto.ProgramType,
            StudentCapacity = dto.StudentCapacity,
            Budget = dto.Budget,
            PreferredLocation = dto.PreferredLocation,
            PreferredLatitude = dto.PreferredLatitude,
            PreferredLongitude = dto.PreferredLongitude,
            CommunityId = dto.CommunityId
        };

        _context.LearningRequests.Add(request);

        await _context.SaveChangesAsync();

        dto.RequestId = request.RequestId;

        return dto;
    }

    public async Task<bool> UpdateAsync(int id, LearningRequestDto dto)
    {
        var request = await _context.LearningRequests.FindAsync(id);

        if (request == null)
            return false;

        request.Organization = dto.Organization;
        request.ProgramType = dto.ProgramType;
        request.StudentCapacity = dto.StudentCapacity;
        request.Budget = dto.Budget;
        request.PreferredLocation = dto.PreferredLocation;
        request.PreferredLatitude = dto.PreferredLatitude;
        request.PreferredLongitude = dto.PreferredLongitude;
        request.CommunityId = dto.CommunityId;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var request = await _context.LearningRequests.FindAsync(id);

        if (request == null)
            return false;

        _context.LearningRequests.Remove(request);

        await _context.SaveChangesAsync();

        return true;
    }
}
