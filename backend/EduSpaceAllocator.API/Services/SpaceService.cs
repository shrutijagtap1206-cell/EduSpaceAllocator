using EduSpaceAllocator.API.Data;
using EduSpaceAllocator.API.DTOs;
using EduSpaceAllocator.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EduSpaceAllocator.API.Services;

public class SpaceService
{
    private readonly AppDbContext _context;

    public SpaceService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<SpaceDto>> GetAllAsync()
    {
        return await _context.Spaces
            .Select(space => new SpaceDto
            {
                SpaceId = space.SpaceId,
                BuildingName = space.BuildingName,
                Address = space.Address,
                City = space.City,
                Latitude = space.Latitude,
                Longitude = space.Longitude,
                FloorArea = space.FloorArea,
                Capacity = space.Capacity,
                RentalCost = space.RentalCost,
                Availability = space.Availability
            })
            .ToListAsync();
    }

    public async Task<SpaceDto?> GetByIdAsync(int id)
    {
        return await _context.Spaces
            .Where(space => space.SpaceId == id)
            .Select(space => new SpaceDto
            {
                SpaceId = space.SpaceId,
                BuildingName = space.BuildingName,
                Address = space.Address,
                City = space.City,
                Latitude = space.Latitude,
                Longitude = space.Longitude,
                FloorArea = space.FloorArea,
                Capacity = space.Capacity,
                RentalCost = space.RentalCost,
                Availability = space.Availability
            })
            .FirstOrDefaultAsync();
    }

    public async Task<SpaceDto> CreateAsync(SpaceDto dto)
    {
        var space = new Space
        {
            BuildingName = dto.BuildingName,
            Address = dto.Address,
            City = dto.City,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            FloorArea = dto.FloorArea,
            Capacity = dto.Capacity,
            RentalCost = dto.RentalCost,
            Availability = dto.Availability
        };

        _context.Spaces.Add(space);
        await _context.SaveChangesAsync();

        dto.SpaceId = space.SpaceId;

        return dto;
    }

    public async Task<bool> UpdateAsync(int id, SpaceDto dto)
    {
        var space = await _context.Spaces.FindAsync(id);

        if (space == null)
        {
            return false;
        }

        space.BuildingName = dto.BuildingName;
        space.Address = dto.Address;
        space.City = dto.City;
        space.Latitude = dto.Latitude;
        space.Longitude = dto.Longitude;
        space.FloorArea = dto.FloorArea;
        space.Capacity = dto.Capacity;
        space.RentalCost = dto.RentalCost;
        space.Availability = dto.Availability;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var space = await _context.Spaces.FindAsync(id);

        if (space == null)
        {
            return false;
        }

        _context.Spaces.Remove(space);
        await _context.SaveChangesAsync();

        return true;
    }
}
