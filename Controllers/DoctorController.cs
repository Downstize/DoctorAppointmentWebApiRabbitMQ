using DoctorAppointmentWebApi.DTOs;
using DoctorAppointmentWebApi.Models;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoctorAppointmentWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DoctorController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DoctorController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DoctorDto>>> GetDoctors()
    {
        var doctors = await _context.Doctors.Include(d => d.Specialization).Include(d => d.Department).ToListAsync();

        var doctorDtos = doctors.Select(doctor => new DoctorDto
        {
            DoctorId = doctor.DoctorId,
            FirstName = doctor.FirstName,
            LastName = doctor.LastName,
            SpecializationId = doctor.SpecializationId,
            DepartmentId = doctor.DepartmentId,
            PhoneNumber = doctor.PhoneNumber,
            Email = doctor.Email,
            RoomNumber = doctor.RoomNumber
        }).ToList();

        return Ok(doctorDtos);
    }

    [HttpPost]
    public async Task<ActionResult<DoctorDto>> CreateDoctor(DoctorDto doctorDto)
    {
        var doctor = new Doctor
        {
            DoctorId = doctorDto.DoctorId,
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

        return CreatedAtAction(nameof(GetDoctorById), new { id = doctor.DoctorId }, doctorDto);
    }

    [HttpGet("{id}", Name = nameof(GetDoctorById))]
    public async Task<ActionResult<DoctorDto>> GetDoctorById(Guid id)
    {
        var doctor = await _context.Doctors.Include(d => d.Specialization).Include(d => d.Department).FirstOrDefaultAsync(d => d.DoctorId == id);
        if (doctor == null) return NotFound();

        var doctorDto = new DoctorDto
        {
            DoctorId = doctor.DoctorId,
            FirstName = doctor.FirstName,
            LastName = doctor.LastName,
            SpecializationId = doctor.SpecializationId,
            DepartmentId = doctor.DepartmentId,
            PhoneNumber = doctor.PhoneNumber,
            Email = doctor.Email,
            RoomNumber = doctor.RoomNumber
        };

        return Ok(doctorDto);
    }

    [HttpPut("Update doctor by {id}", Name = nameof(UpdateDoctor))]
    public async Task<IActionResult> UpdateDoctor( Guid id, DoctorDto doctorDto)
    {
        if (id != doctorDto.DoctorId) return BadRequest();

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
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDoctor(int id)
    {
        var doctor = await _context.Doctors.FindAsync(id);
        if (doctor == null) return NotFound();

        _context.Doctors.Remove(doctor);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
