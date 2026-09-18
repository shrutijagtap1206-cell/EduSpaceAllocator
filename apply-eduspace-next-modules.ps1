$ErrorActionPreference = "Stop"

$root = "D:\shruti\EduSpaceAllocator"
$api = Join-Path $root "backend\EduSpaceAllocator.API"
$frontend = Join-Path $root "frontend\eduspace-client"
$models = Join-Path $api "Models"
$services = Join-Path $api "Services"
$controllers = Join-Path $api "Controllers"
$pages = Join-Path $frontend "src\pages"

Write-Host "Applying EduSpace remaining modules..." -ForegroundColor Cyan

# ---------- C# MODELS ----------
@{
"Partner.cs" = @'
using System.ComponentModel.DataAnnotations;

namespace EduSpaceAllocator.API.Models;

public class Partner
{
    [Key]
    public int PartnerId { get; set; }
    public string OrganizationName { get; set; } = string.Empty;
    public string PartnerType { get; set; } = string.Empty;
    public string ContactPerson { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
}
'@
"Course.cs" = @'
using System.ComponentModel.DataAnnotations;

namespace EduSpaceAllocator.API.Models;

public class Course
{
    [Key]
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int RequiredCapacity { get; set; }
    public int DurationHours { get; set; }
    public string SkillLevel { get; set; } = string.Empty;
}
'@
"Schedule.cs" = @'
using System.ComponentModel.DataAnnotations;

namespace EduSpaceAllocator.API.Models;

public class Schedule
{
    [Key]
    public int ScheduleId { get; set; }
    public int AllocationId { get; set; }
    public int CourseId { get; set; }
    public DateTime SessionDate { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public string Status { get; set; } = "Planned";

    public Allocation? Allocation { get; set; }
    public Course? Course { get; set; }
}
'@
"AuditLog.cs" = @'
using System.ComponentModel.DataAnnotations;

namespace EduSpaceAllocator.API.Models;

public class AuditLog
{
    [Key]
    public int AuditLogId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
'@
"SocialImpact.cs" = @'
using System.ComponentModel.DataAnnotations;

namespace EduSpaceAllocator.API.Models;

public class SocialImpact
{
    [Key]
    public int SocialImpactId { get; set; }
    public int AllocationId { get; set; }
    public int StudentsServed { get; set; }
    public int HoursDelivered { get; set; }
    public string CommunityOutcome { get; set; } = string.Empty;
    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;

    public Allocation? Allocation { get; set; }
}
'@
}.GetEnumerator() | ForEach-Object {
    Set-Content -Path (Join-Path $models $_.Key) -Value $_.Value -Encoding UTF8
}

# ---------- SERVICES ----------
@{
"PartnerService.cs" = @'
using EduSpaceAllocator.API.Data;
using EduSpaceAllocator.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EduSpaceAllocator.API.Services;

public class PartnerService
{
    private readonly AppDbContext _db;
    public PartnerService(AppDbContext db) => _db = db;

    public Task<List<Partner>> GetAllAsync() => _db.Partners.AsNoTracking().OrderBy(p => p.PartnerId).ToListAsync();
    public async Task<Partner> CreateAsync(Partner partner) { _db.Partners.Add(partner); await _db.SaveChangesAsync(); return partner; }
    public async Task<bool> DeleteAsync(int id) { var x = await _db.Partners.FindAsync(id); if (x == null) return false; _db.Partners.Remove(x); await _db.SaveChangesAsync(); return true; }
}
'@
"CourseService.cs" = @'
using EduSpaceAllocator.API.Data;
using EduSpaceAllocator.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EduSpaceAllocator.API.Services;

public class CourseService
{
    private readonly AppDbContext _db;
    public CourseService(AppDbContext db) => _db = db;

    public Task<List<Course>> GetAllAsync() => _db.Courses.AsNoTracking().OrderBy(c => c.CourseId).ToListAsync();
    public async Task<Course> CreateAsync(Course course) { _db.Courses.Add(course); await _db.SaveChangesAsync(); return course; }
    public async Task<bool> DeleteAsync(int id) { var x = await _db.Courses.FindAsync(id); if (x == null) return false; _db.Courses.Remove(x); await _db.SaveChangesAsync(); return true; }
}
'@
"ScheduleService.cs" = @'
using EduSpaceAllocator.API.Data;
using EduSpaceAllocator.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EduSpaceAllocator.API.Services;

public class ScheduleService
{
    private readonly AppDbContext _db;
    public ScheduleService(AppDbContext db) => _db = db;

    public Task<List<Schedule>> GetAllAsync() =>
        _db.Schedules.AsNoTracking()
            .Include(s => s.Course)
            .Include(s => s.Allocation)
            .OrderBy(s => s.SessionDate)
            .ToListAsync();

    public async Task<Schedule> CreateAsync(Schedule schedule)
    {
        _db.Schedules.Add(schedule);
        await _db.SaveChangesAsync();
        return schedule;
    }
}
'@
}.GetEnumerator() | ForEach-Object {
    Set-Content -Path (Join-Path $services $_.Key) -Value $_.Value -Encoding UTF8
}

# ---------- CONTROLLERS ----------
@{
"PartnersController.cs" = @'
using EduSpaceAllocator.API.Models;
using EduSpaceAllocator.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace EduSpaceAllocator.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PartnersController : ControllerBase
{
    private readonly PartnerService _service;
    public PartnersController(PartnerService service) => _service = service;

    [HttpGet] public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());
    [HttpPost] public async Task<IActionResult> Create(Partner partner) => Ok(await _service.CreateAsync(partner));
    [HttpDelete("{id:int}")] public async Task<IActionResult> Delete(int id) => await _service.DeleteAsync(id) ? NoContent() : NotFound();
}
'@
"CoursesController.cs" = @'
using EduSpaceAllocator.API.Models;
using EduSpaceAllocator.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace EduSpaceAllocator.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly CourseService _service;
    public CoursesController(CourseService service) => _service = service;

    [HttpGet] public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());
    [HttpPost] public async Task<IActionResult> Create(Course course) => Ok(await _service.CreateAsync(course));
    [HttpDelete("{id:int}")] public async Task<IActionResult> Delete(int id) => await _service.DeleteAsync(id) ? NoContent() : NotFound();
}
'@
"SchedulingController.cs" = @'
using EduSpaceAllocator.API.Models;
using EduSpaceAllocator.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace EduSpaceAllocator.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SchedulingController : ControllerBase
{
    private readonly ScheduleService _service;
    public SchedulingController(ScheduleService service) => _service = service;

    [HttpGet] public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());
    [HttpPost] public async Task<IActionResult> Create(Schedule schedule) => Ok(await _service.CreateAsync(schedule));
}
'@
"AuditController.cs" = @'
using EduSpaceAllocator.API.Data;
using EduSpaceAllocator.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduSpaceAllocator.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuditController : ControllerBase
{
    private readonly AppDbContext _db;
    public AuditController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _db.AuditLogs.AsNoTracking().OrderByDescending(x => x.CreatedAt).ToListAsync());
}
'@
"SocialImpactController.cs" = @'
using EduSpaceAllocator.API.Data;
using EduSpaceAllocator.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduSpaceAllocator.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SocialImpactController : ControllerBase
{
    private readonly AppDbContext _db;
    public SocialImpactController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _db.SocialImpacts.AsNoTracking().OrderByDescending(x => x.RecordedAt).ToListAsync());

    [HttpPost]
    public async Task<IActionResult> Create(SocialImpact impact)
    {
        _db.SocialImpacts.Add(impact);
        await _db.SaveChangesAsync();
        return Ok(impact);
    }
}
'@
}.GetEnumerator() | ForEach-Object {
    Set-Content -Path (Join-Path $controllers $_.Key) -Value $_.Value -Encoding UTF8
}

# ---------- DB CONTEXT ----------
$dbctx = Join-Path $api "Data\AppDbContext.cs"
$content = Get-Content $dbctx -Raw

foreach ($line in @(
'    public DbSet<Partner> Partners => Set<Partner>();',
'    public DbSet<Course> Courses => Set<Course>();',
'    public DbSet<Schedule> Schedules => Set<Schedule>();',
'    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();',
'    public DbSet<SocialImpact> SocialImpacts => Set<SocialImpact>();'
)) {
    if ($content -notmatch [regex]::Escape($line.Trim())) {
        $content = $content -replace '(public DbSet<Allocation> Allocations => Set<Allocation>\(\);)', "`$1`r`n$line"
    }
}

if ($content -notmatch 'HasOne\(s => s\.Allocation\)') {
    $relationship = @'

        modelBuilder.Entity<Schedule>()
            .HasOne(s => s.Allocation)
            .WithMany()
            .HasForeignKey(s => s.AllocationId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Schedule>()
            .HasOne(s => s.Course)
            .WithMany()
            .HasForeignKey(s => s.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SocialImpact>()
            .HasOne(s => s.Allocation)
            .WithMany()
            .HasForeignKey(s => s.AllocationId)
            .OnDelete(DeleteBehavior.Restrict);
'@
    $content = $content -replace '\s*}\s*$', "$relationship`r`n    }`r`n}"
}
Set-Content $dbctx $content -Encoding UTF8

# ---------- PROGRAM SERVICES ----------
$program = Join-Path $api "Program.cs"
$p = Get-Content $program -Raw
$registrations = @'
builder.Services.AddScoped<PartnerService>();
builder.Services.AddScoped<CourseService>();
builder.Services.AddScoped<ScheduleService>();
'@
if ($p -notmatch 'AddScoped<PartnerService>') {
    $p = $p -replace '(builder\.Services\.AddScoped<[^;]+>;\s*)+$', "`$0`r`n$registrations"
}
Set-Content $program $p -Encoding UTF8

# ---------- FRONTEND PAGES ----------
@{
"Partners.jsx" = @'
import { useEffect, useState } from "react";
import api from "../services/api";

export default function Partners() {
  const [items, setItems] = useState([]);
  const load = async () => setItems((await api.get("/Partners")).data);
  useEffect(() => { load(); }, []);
  return <div className="page"><h1>Partners</h1><p>Organizations supporting learning-space delivery.</p>
    <div className="card"><table><thead><tr><th>ID</th><th>Organization</th><th>Type</th><th>Contact</th><th>Status</th></tr></thead>
    <tbody>{items.map(x => <tr key={x.partnerId}><td>{x.partnerId}</td><td>{x.organizationName}</td><td>{x.partnerType}</td><td>{x.contactPerson}</td><td>{x.status}</td></tr>)}</tbody></table></div>
  </div>;
}
'@
"Courses.jsx" = @'
import { useEffect, useState } from "react";
import api from "../services/api";

export default function Courses() {
  const [items, setItems] = useState([]);
  useEffect(() => { api.get("/Courses").then(r => setItems(r.data)); }, []);
  return <div className="page"><h1>Courses</h1><p>Course catalog and space requirements.</p>
    <div className="card"><table><thead><tr><th>ID</th><th>Course</th><th>Category</th><th>Capacity</th><th>Hours</th><th>Level</th></tr></thead>
    <tbody>{items.map(x => <tr key={x.courseId}><td>{x.courseId}</td><td>{x.courseName}</td><td>{x.category}</td><td>{x.requiredCapacity}</td><td>{x.durationHours}</td><td>{x.skillLevel}</td></tr>)}</tbody></table></div>
  </div>;
}
'@
"Scheduling.jsx" = @'
import { useEffect, useState } from "react";
import api from "../services/api";

export default function Scheduling() {
  const [items, setItems] = useState([]);
  useEffect(() => { api.get("/Scheduling").then(r => setItems(r.data)); }, []);
  return <div className="page"><h1>Scheduling</h1><p>Planned sessions linked to allocated learning spaces.</p>
    <div className="card"><table><thead><tr><th>ID</th><th>Course</th><th>Allocation</th><th>Date</th><th>Time</th><th>Status</th></tr></thead>
    <tbody>{items.map(x => <tr key={x.scheduleId}><td>{x.scheduleId}</td><td>{x.course?.courseName ?? x.courseId}</td><td>{x.allocationId}</td><td>{String(x.sessionDate).slice(0,10)}</td><td>{x.startTime} - {x.endTime}</td><td>{x.status}</td></tr>)}</tbody></table></div>
  </div>;
}
'@
"AuditLogs.jsx" = @'
import { useEffect, useState } from "react";
import api from "../services/api";

export default function AuditLogs() {
  const [items, setItems] = useState([]);
  useEffect(() => { api.get("/Audit").then(r => setItems(r.data)); }, []);
  return <div className="page"><h1>Audit Logs</h1><p>Traceable records of system actions.</p>
    <div className="card"><table><thead><tr><th>Time</th><th>Action</th><th>Entity</th><th>Details</th></tr></thead>
    <tbody>{items.map(x => <tr key={x.auditLogId}><td>{new Date(x.createdAt).toLocaleString()}</td><td>{x.action}</td><td>{x.entityType} {x.entityId}</td><td>{x.details}</td></tr>)}</tbody></table></div>
  </div>;
}
'@
"SocialImpact.jsx" = @'
import { useEffect, useState } from "react";
import api from "../services/api";

export default function SocialImpact() {
  const [items, setItems] = useState([]);
  useEffect(() => { api.get("/SocialImpact").then(r => setItems(r.data)); }, []);
  const students = items.reduce((a,x) => a + (x.studentsServed || 0), 0);
  const hours = items.reduce((a,x) => a + (x.hoursDelivered || 0), 0);
  return <div className="page"><h1>Social Impact</h1><p>Education outcomes produced through allocated spaces.</p>
    <div className="kpi-grid"><div className="card"><span>Students Served</span><strong>{students}</strong></div><div className="card"><span>Hours Delivered</span><strong>{hours}</strong></div><div className="card"><span>Impact Records</span><strong>{items.length}</strong></div></div>
    <div className="card"><table><thead><tr><th>Allocation</th><th>Students</th><th>Hours</th><th>Outcome</th></tr></thead>
    <tbody>{items.map(x => <tr key={x.socialImpactId}><td>{x.allocationId}</td><td>{x.studentsServed}</td><td>{x.hoursDelivered}</td><td>{x.communityOutcome}</td></tr>)}</tbody></table></div>
  </div>;
}
'@
"SDGDashboard.jsx" = @'
export default function SDGDashboard() {
  const sdgs = [
    ["SDG 4", "Quality Education", "Expands access to learning infrastructure."],
    ["SDG 8", "Decent Work", "Supports skills and employability programs."],
    ["SDG 9", "Industry, Innovation & Infrastructure", "Uses existing spaces as productive infrastructure."],
    ["SDG 10", "Reduced Inequalities", "Targets underserved communities through demand data."],
    ["SDG 11", "Sustainable Cities", "Improves utilization of underused urban spaces."],
    ["SDG 17", "Partnerships", "Connects NGOs, communities and space providers."]
  ];
  return <div className="page"><h1>SDG Dashboard</h1><p>How EduSpace Allocator supports the Sustainable Development Goals.</p>
    <div className="card-grid">{sdgs.map(([id,name,desc]) => <div className="card" key={id}><h2>{id}</h2><h3>{name}</h3><p>{desc}</p></div>)}</div>
  </div>;
}
'@
}.GetEnumerator() | ForEach-Object {
    Set-Content -Path (Join-Path $pages $_.Key) -Value $_.Value -Encoding UTF8
}

Write-Host ""
Write-Host "Remaining module source files created." -ForegroundColor Green
Write-Host "NEXT: run the following commands:" -ForegroundColor Yellow
Write-Host "cd D:\shruti\EduSpaceAllocator\backend\EduSpaceAllocator.API"
Write-Host "dotnet ef migrations add AddPartnersCoursesSchedulingAuditImpact"
Write-Host "dotnet ef database update"
Write-Host "dotnet build"
Write-Host "cd D:\shruti\EduSpaceAllocator\frontend\eduspace-client"
Write-Host "npm run build"
Write-Host ""
Write-Host "NOTE: App.jsx sidebar integration is intentionally left for the existing clean UI to avoid overwriting your working navigation." -ForegroundColor Cyan
