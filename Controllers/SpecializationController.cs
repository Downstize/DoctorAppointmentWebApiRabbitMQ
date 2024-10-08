using DoctorAppointmentWebApi.DTOs;
using DoctorAppointmentWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoctorAppointmentWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SpecializationController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SpecializationController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet(Name = nameof(GetSpecializations))]
    public async Task<ActionResult<IEnumerable<SpecializationDto>>> GetSpecializations()
    {
        var specializations = await _context.Specializations.ToListAsync();

        var specializationDtos = specializations.Select(specialization => CreateSpecializationDtoWithLinks(specialization)).ToList();

        var response = new
        {
            _links = new
            {
                self = new Link(Url.Action(nameof(GetSpecializations))!, "self", "GET"),
                create = new Link(Url.Action(nameof(CreateSpecialization))!, "create-specialization", "POST")
            },
            specializations = specializationDtos
        };

        return Ok(response);
    }

    [HttpGet("{id}", Name = nameof(GetSpecializationById))]
    public async Task<ActionResult<SpecializationDto>> GetSpecializationById(Guid id)
    {
        var specialization = await _context.Specializations.FindAsync(id);

        if (specialization == null) return NotFound();

        var specializationDto = CreateSpecializationDtoWithLinks(specialization);
        return Ok(new
        {
            specialization = specializationDto,
            _links = new
            {
                self = new Link(Url.Action(nameof(GetSpecializationById), new { id = specialization.SpecializationId })!, "self", "GET"),
                update = new Link(Url.Action(nameof(UpdateSpecialization), new { id = specialization.SpecializationId })!, "update", "PUT"),
                delete = new Link(Url.Action(nameof(DeleteSpecialization), new { id = specialization.SpecializationId })!, "delete", "DELETE")
            }
        });
    }

    [HttpPost(Name = nameof(CreateSpecialization))]
    public async Task<ActionResult<SpecializationDto>> CreateSpecialization(SpecializationDto specializationDto)
    {
        var specialization = new Specialization
        {
            SpecializationName = specializationDto.SpecializationName
        };

        _context.Specializations.Add(specialization);
        await _context.SaveChangesAsync();

        var createdSpecializationDto = CreateSpecializationDtoWithLinks(specialization);

        return CreatedAtAction(nameof(GetSpecializationById), new { id = specialization.SpecializationId }, new
        {
            specialization = createdSpecializationDto,
            _links = new
            {
                self = new Link(Url.Action(nameof(GetSpecializationById), new { id = createdSpecializationDto.SpecializationId })!, "self", "GET"),
                update = new Link(Url.Action(nameof(UpdateSpecialization), new { id = createdSpecializationDto.SpecializationId })!, "update", "PUT"),
                delete = new Link(Url.Action(nameof(DeleteSpecialization), new { id = createdSpecializationDto.SpecializationId })!, "delete", "DELETE")
            }
        });
    }

    [HttpPut("{id}", Name = nameof(UpdateSpecialization))]
    public async Task<IActionResult> UpdateSpecialization(Guid id, SpecializationDto specializationDto)
    {
        var specialization = await _context.Specializations.FindAsync(id);
        if (specialization == null) return NotFound();

        specialization.SpecializationName = specializationDto.SpecializationName;

        await _context.SaveChangesAsync();

        var updatedSpecializationDto = CreateSpecializationDtoWithLinks(specialization);
        return Ok(new
        {
            specialization = updatedSpecializationDto,
            _links = new
            {
                self = new Link(Url.Action(nameof(GetSpecializationById), new { id = specialization.SpecializationId })!, "self", "GET"),
                update = new Link(Url.Action(nameof(UpdateSpecialization), new { id = specialization.SpecializationId })!, "update", "PUT"),
                delete = new Link(Url.Action(nameof(DeleteSpecialization), new { id = specialization.SpecializationId })!, "delete", "DELETE")
            }
        });
    }

    [HttpDelete("{id}", Name = nameof(DeleteSpecialization))]
    public async Task<IActionResult> DeleteSpecialization(Guid id)
    {
        var specialization = await _context.Specializations.FindAsync(id);
        if (specialization == null) return NotFound();

        _context.Specializations.Remove(specialization);
        await _context.SaveChangesAsync();

        var response = new
        {
            message = $"Specialization {specialization.SpecializationId} deleted",
            _links = new
            {
                getAllSpecializations = new Link(Url.Action(nameof(GetSpecializations))!, "get-all-specializations", "GET"),
                createSpecialization = new Link(Url.Action(nameof(CreateSpecialization))!, "create-specialization", "POST")
            }
        };

        return Ok(response);
    }

    // Вспомогательный метод для создания DTO с включенными ссылками HAL
    private SpecializationDto CreateSpecializationDtoWithLinks(Specialization specialization)
    {
        var specializationDto = new SpecializationDto
        {
            SpecializationId = specialization.SpecializationId,
            SpecializationName = specialization.SpecializationName,
            Links = new List<Link>
            {
                new Link(Url.Action(nameof(GetSpecializationById), new { id = specialization.SpecializationId })!, "self", "GET"),
                new Link(Url.Action(nameof(UpdateSpecialization), new { id = specialization.SpecializationId })!, "update", "PUT"),
                new Link(Url.Action(nameof(DeleteSpecialization), new { id = specialization.SpecializationId })!, "delete", "DELETE")
            }
        };

        return specializationDto;
    }
}
