using DoctorAppointmentWebApi.DTOs;
using DoctorAppointmentWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoctorAppointmentWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DepartmentController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetDepartments()
    {
        var departments = await _context.Departments.ToListAsync();
        var departmentDtos = departments.Select(department => new DepartmentDto
        {
            DepartmentId = department.DepartmentId,
            Name = department.Name,
            Location = department.Location
        }).ToList();

        return Ok(departmentDtos);
    }

    [HttpPost]
    public async Task<ActionResult<DepartmentDto>> CreateDepartment(DepartmentDto departmentDto)
    {
        var department = new Department
        {
            Name = departmentDto.Name,
            Location = departmentDto.Location
        };

        _context.Departments.Add(department);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDepartmentById), new { id = department.DepartmentId }, departmentDto);
    }

    [HttpGet("{id}", Name = nameof(GetDepartmentById))]
    public async Task<ActionResult<DepartmentDto>> GetDepartmentById(int id)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department == null) return NotFound();

        var departmentDto = new DepartmentDto
        {
            DepartmentId = department.DepartmentId,
            Name = department.Name,
            Location = department.Location
        };

        return Ok(departmentDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDepartment(int id, DepartmentDto departmentDto)
    {
        if (id.ToString() != departmentDto.DepartmentId.ToString()) return BadRequest();

        var department = await _context.Departments.FindAsync(id);
        if (department == null) return NotFound();

        department.Name = departmentDto.Name;
        department.Location = departmentDto.Location;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDepartment(int id)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department == null) return NotFound();

        _context.Departments.Remove(department);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
