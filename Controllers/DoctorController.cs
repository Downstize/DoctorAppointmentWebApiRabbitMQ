using DoctorAppointmentWeb.Api.Controllers;
using DoctorAppointmentWeb.Api.Responses;
using DoctorAppointmentWebApi.DTOs;
using DoctorAppointmentWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoctorAppointmentWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DoctorController : ControllerBase, IDoctorApi
{
    private readonly ApplicationDbContext _context;

    public DoctorController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet(Name = nameof(GetDoctors))]
    public async Task<ActionResult<IEnumerable<DoctorResponse>>> GetDoctors()
    {
        var doctors = await _context.Doctors.Include(d => d.Specialization).Include(d => d.Department).ToListAsync();

        var doctorDtos = doctors.Select(doctor => CreateDoctorDtoWithLinks(doctor)).ToList();

        var response = new
        {
            _links = new
            {
                self = new Link(Url.Action(nameof(GetDoctors))!, "self", "GET"),
                create = new Link(Url.Action(nameof(CreateDoctor))!, "create-doctor", "POST")
            },
            doctors = doctorDtos
        };

        return Ok(response);
    }

    [HttpPost(Name = nameof(CreateDoctor))]
    public async Task<ActionResult<DoctorResponse>> CreateDoctor(DoctorRequest doctorDto)
    {
        var doctor = new Doctor
        {
            FirstName = doctorDto.FirstName,
            LastName = doctorDto.LastName,
            SpecializationId = doctorDto.SpecializationId,
            DepartmentId = doctorDto.DepartmentId,
            PhoneNumber = doctorDto.PhoneNumber,
            Email = doctorDto.Email,
            RoomNumber = doctorDto.RoomNumber
        };

        _context.Doctors.Add(doctor);
        await _context.SaveChangesAsync();

        var newDoctorDto = CreateDoctorDtoWithLinks(doctor);

        return CreatedAtAction(nameof(GetDoctorById), new { id = doctor.DoctorId }, new
        {
            doctor = newDoctorDto,
            _links = new
            {
                self = new Link(Url.Action(nameof(GetDoctorById), new { id = doctor.DoctorId })!, "self", "GET"),
                update = new Link(Url.Action(nameof(UpdateDoctor), new { id = doctor.DoctorId })!, "update", "PUT"),
                delete = new Link(Url.Action(nameof(DeleteDoctor), new { id = doctor.DoctorId })!, "delete", "DELETE")
            }
        });
    }

    [HttpGet("{id}", Name = nameof(GetDoctorById))]
    public async Task<ActionResult<DoctorResponse>> GetDoctorById(Guid id)
    {
        var doctor = await _context.Doctors.Include(d => d.Specialization).Include(d => d.Department).FirstOrDefaultAsync(d => d.DoctorId == id);
        if (doctor == null) return NotFound();

        var doctorDto = CreateDoctorDtoWithLinks(doctor);

        var response = new
        {
            doctor = doctorDto,
            _links = new
            {
                self = new Link(Url.Action(nameof(GetDoctorById), new { id = doctor.DoctorId })!, "self", "GET"),
                update = new Link(Url.Action(nameof(UpdateDoctor), new { id = doctor.DoctorId })!, "update", "PUT"),
                delete = new Link(Url.Action(nameof(DeleteDoctor), new { id = doctor.DoctorId })!, "delete", "DELETE")
            }
        };

        return Ok(response);
    }

    [HttpPut("{id}", Name = nameof(UpdateDoctor))]
    public async Task<IActionResult> UpdateDoctor(Guid id, DoctorRequest doctorDto)
    {
        var doctor = await _context.Doctors.FindAsync(id);
        if (doctor == null) return NotFound();

        doctor.FirstName = doctorDto.FirstName;
        doctor.LastName = doctorDto.LastName;
        doctor.SpecializationId = doctorDto.SpecializationId;
        doctor.DepartmentId = doctorDto.DepartmentId;
        doctor.PhoneNumber = doctorDto.PhoneNumber;
        doctor.Email = doctorDto.Email;
        doctor.RoomNumber = doctorDto.RoomNumber;

        await _context.SaveChangesAsync();

        var updatedDoctorDto = CreateDoctorDtoWithLinks(doctor);
        var response = new
        {
            doctor = updatedDoctorDto,
            _links = new
            {
                self = new Link(Url.Action(nameof(GetDoctorById), new { id = doctor.DoctorId })!, "self", "GET"),
                update = new Link(Url.Action(nameof(UpdateDoctor), new { id = doctor.DoctorId })!, "update", "PUT"),
                delete = new Link(Url.Action(nameof(DeleteDoctor), new { id = doctor.DoctorId })!, "delete", "DELETE")
            }
        };

        return Ok(response);
    }

    [HttpDelete("{id}", Name = nameof(DeleteDoctor))]
    public async Task<IActionResult> DeleteDoctor(Guid id)
    {
        var doctor = await _context.Doctors.FindAsync(id);
        if (doctor == null) return NotFound();

        _context.Doctors.Remove(doctor);
        await _context.SaveChangesAsync();

        var response = new
        {
            message = $"Doctor {doctor.DoctorId} deleted",
            _links = new
            {
                getAllDoctors = new Link(Url.Action(nameof(GetDoctors))!, "get-all-doctors", "GET"),
                createDoctor = new Link(Url.Action(nameof(CreateDoctor))!, "create-doctor", "POST")
            }
        };

        return Ok(response);
    }

    // Вспомогательный метод для создания DTO с включенными ссылками HAL
    private DoctorDto CreateDoctorDtoWithLinks(Doctor doctor)
    {
        var doctorDto = new DoctorDto
        {
            DoctorId = doctor.DoctorId,
            FirstName = doctor.FirstName,
            LastName = doctor.LastName,
            SpecializationId = doctor.SpecializationId,
            DepartmentId = doctor.DepartmentId,
            PhoneNumber = doctor.PhoneNumber,
            Email = doctor.Email,
            RoomNumber = doctor.RoomNumber,
            Links = new List<Link>
            {
                new Link(Url.Action(nameof(GetDoctorById), new { id = doctor.DoctorId })!, "self", "GET"),
                new Link(Url.Action(nameof(UpdateDoctor), new { id = doctor.DoctorId })!, "update", "PUT"),
                new Link(Url.Action(nameof(DeleteDoctor), new { id = doctor.DoctorId })!, "delete", "DELETE")
            }
        };

        return doctorDto;
    }
}
