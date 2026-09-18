using EduSpaceAllocator.API.Data;
using EduSpaceAllocator.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EduSpaceAllocator.API.Services;

public class PartnerService
{
    private readonly AppDbContext _db;

    public PartnerService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Partner>> GetAllAsync()
    {
        return await _db.Partners
            .AsNoTracking()
            .OrderBy(p => p.PartnerId)
            .ToListAsync();
    }

    public async Task<Partner> CreateAsync(Partner partner)
    {
        Validate(partner);

        partner.Status = string.IsNullOrWhiteSpace(partner.Status)
            ? "Active"
            : partner.Status;

        _db.Partners.Add(partner);
        await _db.SaveChangesAsync();

        return partner;
    }

    public async Task<Partner?> UpdateAsync(int id, Partner updated)
    {
        Validate(updated);

        var partner = await _db.Partners.FindAsync(id);

        if (partner == null)
            return null;

        partner.OrganizationName = updated.OrganizationName;
        partner.PartnerType = updated.PartnerType;
        partner.ContactPerson = updated.ContactPerson;
        partner.Email = updated.Email;
        partner.Phone = updated.Phone;
        partner.Status = string.IsNullOrWhiteSpace(updated.Status)
            ? "Active"
            : updated.Status;

        await _db.SaveChangesAsync();

        return partner;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var partner = await _db.Partners.FindAsync(id);

        if (partner == null)
            return false;

        _db.Partners.Remove(partner);
        await _db.SaveChangesAsync();

        return true;
    }

    private static void Validate(Partner partner)
    {
        if (string.IsNullOrWhiteSpace(partner.OrganizationName))
            throw new ArgumentException("Organization name is required.");

        if (string.IsNullOrWhiteSpace(partner.PartnerType))
            throw new ArgumentException("Partner type is required.");

        if (string.IsNullOrWhiteSpace(partner.ContactPerson))
            throw new ArgumentException("Contact person is required.");

        if (string.IsNullOrWhiteSpace(partner.Email))
            throw new ArgumentException("Email is required.");
    }
}
