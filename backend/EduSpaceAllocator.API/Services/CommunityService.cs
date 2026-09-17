using EduSpaceAllocator.API.Data;
using EduSpaceAllocator.API.DTOs;
using EduSpaceAllocator.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EduSpaceAllocator.API.Services;

public class CommunityService
{
    private readonly AppDbContext _context;

    public CommunityService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CommunityDto>> GetAllAsync()
    {
        return await _context.Communities
            .AsNoTracking()
            .Select(c => new CommunityDto
            {
                CommunityId = c.CommunityId,
                Population = c.Population,
                StudentCount = c.StudentCount,
                LiteracyRate = c.LiteracyRate,
                IncomeGroup = c.IncomeGroup
            })
            .ToListAsync();
    }

    public async Task<CommunityDto?> GetByIdAsync(int id)
    {
        var community = await _context.Communities.FindAsync(id);

        if (community == null)
            return null;

        return new CommunityDto
        {
            CommunityId = community.CommunityId,
            Population = community.Population,
            StudentCount = community.StudentCount,
            LiteracyRate = community.LiteracyRate,
            IncomeGroup = community.IncomeGroup
        };
    }

    public async Task<CommunityDto> CreateAsync(CommunityDto dto)
    {
        var community = new Community
        {
            Population = dto.Population,
            StudentCount = dto.StudentCount,
            LiteracyRate = dto.LiteracyRate,
            IncomeGroup = dto.IncomeGroup
        };

        _context.Communities.Add(community);
        await _context.SaveChangesAsync();

        dto.CommunityId = community.CommunityId;

        return dto;
    }

    public async Task<bool> UpdateAsync(int id, CommunityDto dto)
    {
        var community = await _context.Communities.FindAsync(id);

        if (community == null)
            return false;

        community.Population = dto.Population;
        community.StudentCount = dto.StudentCount;
        community.LiteracyRate = dto.LiteracyRate;
        community.IncomeGroup = dto.IncomeGroup;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var community = await _context.Communities.FindAsync(id);

        if (community == null)
            return false;

        _context.Communities.Remove(community);
        await _context.SaveChangesAsync();

        return true;
    }
}
