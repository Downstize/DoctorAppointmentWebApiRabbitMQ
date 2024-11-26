using DoctorAppointmentWeb.Api.Controllers;
using DoctorAppointmentWeb.Api.Responses;
using DoctorAppointmentWebApi.DTOs;
using DoctorAppointmentWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoctorAppointmentWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentController : ControllerBase, IDepartmentApi
{
    private readonly ApplicationDbContext _context;

    public DepartmentController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet(Name = nameof(GetDepartments))]
    public async Task<ActionResult<IEnumerable<DepartmentResponse>>> GetDepartments()
    {
        var departments = await _context.Departments.ToListAsync();
        var departmentDtos = departments.Select(department => CreateDepartmentDtoWithLinks(department)).ToList();

        var response = new
        {
            _links = new
            {
                self = new Link(Url.Action(nameof(GetDepartments))!, "self", "GET"),
                create = new Link(Url.Action(nameof(CreateDepartment))!, "create-department", "POST")
            },
            departments = departmentDtos
        };

        return Ok(response);
    }

    [HttpGet("{id}", Name = nameof(GetDepartmentById))]
    public async Task<ActionResult<DepartmentResponse>> GetDepartmentById(Guid id)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department == null) return NotFound();

        var departmentDto = CreateDepartmentDtoWithLinks(department);
        var response = new
        {
            department = departmentDto,
            _links = new
            {
                self = new Link(Url.Action(nameof(GetDepartmentById), new { id = department.DepartmentId })!, "self", "GET"),
                update = new Link(Url.Action(nameof(UpdateDepartment), new { id = department.DepartmentId })!, "update", "PUT"),
                delete = new Link(Url.Action(nameof(DeleteDepartment), new { id = department.DepartmentId })!, "delete", "DELETE")
            }
        };

        return Ok(response);
    }

    [HttpPost(Name = nameof(CreateDepartment))]
    public async Task<ActionResult<DepartmentResponse>> CreateDepartment(DepartmentRequest departmentDto)
    {
        var department = new Department
        {
            DepartmentId = Guid.NewGuid(),
            Name = departmentDto.Name,
            Location = departmentDto.Location
        };

        _context.Departments.Add(department);
        await _context.SaveChangesAsync();

        var newDepartmentDto = CreateDepartmentDtoWithLinks(department);
        var response = new
        {
            department = newDepartmentDto,
            _links = new
            {
                self = new Link(Url.Action(nameof(GetDepartmentById), new { id = department.DepartmentId })!, "self", "GET"),
                update = new Link(Url.Action(nameof(UpdateDepartment), new { id = department.DepartmentId })!, "update", "PUT"),
                delete = new Link(Url.Action(nameof(DeleteDepartment), new { id = department.DepartmentId })!, "delete", "DELETE")
            }
        };

        return CreatedAtAction(nameof(GetDepartmentById), new { id = department.DepartmentId }, response);
    }

    [HttpPut("{id}", Name = nameof(UpdateDepartment))]
    public async Task<IActionResult> UpdateDepartment(Guid id, DepartmentRequest departmentDto)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department == null) return NotFound();

        department.Name = departmentDto.Name;
        department.Location = departmentDto.Location;

        await _context.SaveChangesAsync();

        var updatedDepartmentDto = CreateDepartmentDtoWithLinks(department);
        var response = new
        {
            department = updatedDepartmentDto,
            _links = new
            {
                self = new Link(Url.Action(nameof(GetDepartmentById), new { id = department.DepartmentId })!, "self", "GET"),
                update = new Link(Url.Action(nameof(UpdateDepartment), new { id = department.DepartmentId })!, "update", "PUT"),
                delete = new Link(Url.Action(nameof(DeleteDepartment), new { id = department.DepartmentId })!, "delete", "DELETE")
            }
        };

        return Ok(response);
    }

    [HttpDelete("{id}", Name = nameof(DeleteDepartment))]
    public async Task<IActionResult> DeleteDepartment(Guid id)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department == null) return NotFound();

        _context.Departments.Remove(department);
        await _context.SaveChangesAsync();

        var response = new
        {
            message = $"Department {department.DepartmentId} deleted",
            _links = new
            {
                getAllDepartments = new Link(Url.Action(nameof(GetDepartments))!, "get-all-departments", "GET"),
                createDepartment = new Link(Url.Action(nameof(CreateDepartment))!, "create-department", "POST")
            }
        };

        return Ok(response);
    }

    
    /// <summary>
    /// DepartmentResponse
    /// </summary>
    /// <param name="department"></param>
    /// <returns>smth</returns>
    
    // Вспомогательный метод для создания DTO с включенными ссылками HAL
    private DepartmentDto CreateDepartmentDtoWithLinks(Department department) 
    {
        var departmentDto = new DepartmentDto
        {
            DepartmentId = department.DepartmentId,
            Name = department.Name,
            Location = department.Location,
            Links = new List<Link>
            {
                new Link(Url.Action(nameof(GetDepartmentById), new { id = department.DepartmentId })!, "self", "GET"),
                new Link(Url.Action(nameof(UpdateDepartment), new { id = department.DepartmentId })!, "update", "PUT"),
                new Link(Url.Action(nameof(DeleteDepartment), new { id = department.DepartmentId })!, "delete", "DELETE")
            }
        };

        return departmentDto;
    }
}
