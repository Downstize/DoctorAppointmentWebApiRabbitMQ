using DoctorAppointmentWebApi.DTOs;
using DoctorAppointmentWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoctorAppointmentWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AppointmentController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAppointments()
    {
        var appointments = await _context.Appointments.Include(a => a.Doctor).Include(a => a.Patient).ToListAsync();

        var appointmentDtos = appointments.Select(appointment => new AppointmentDto
        {
            AppointmentId = appointment.AppointmentId,
            PatientId = appointment.PatientId,
            DoctorId = appointment.DoctorId,
            AppointmentDateTime = appointment.AppointmentDateTime,
            Status = appointment.Status,
            Notes = appointment.Notes
        }).ToList();

        

        foreach (var appointmentDto in appointmentDtos)
        {
            appointmentDto.Links.Add(new Link(
                href: Url.Action(nameof(GetAppointmentById), new { id = appointmentDto.AppointmentId }),
                rel: "self",
                method: "GET"));

            appointmentDto.Links.Add(new Link(
                href: Url.Action(nameof(UpdateAppointment), new { id = appointmentDto.AppointmentId }),
                rel: "update",
                method: "PUT"));

            appointmentDto.Links.Add(new Link(
                href: Url.Action(nameof(DeleteAppointment), new { id = appointmentDto.AppointmentId }),
                rel: "delete",
                method: "DELETE"));
        }
        
        return Ok(appointmentDtos);
    }

    [HttpPost("Create a new appointment", Name = nameof(CreateAppointment))]
    public async Task<ActionResult<AppointmentDto>> CreateAppointment(AppointmentDto appointmentDto)
    {
        var appointment = new Appointment
        {
            PatientId = appointmentDto.PatientId,
            DoctorId = appointmentDto.DoctorId,
            AppointmentDateTime = appointmentDto.AppointmentDateTime,
            Status = appointmentDto.Status,
            Notes = appointmentDto.Notes
        };

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        var newAppointmentDto = new AppointmentDto
        {
            PatientId = appointmentDto.PatientId,
            DoctorId = appointmentDto.DoctorId,
            AppointmentDateTime = appointmentDto.AppointmentDateTime,
            Status = appointmentDto.Status,
            Notes = appointmentDto.Notes
        };

        newAppointmentDto.Links.Add(new Link(
            href: Url.Action(nameof(GetAppointmentById), new { id = appointmentDto.AppointmentId }),
            rel: "self",
            method: "GET"
            ));
        
        newAppointmentDto.Links.Add(new Link(
            href: Url.Action(nameof(UpdateAppointment), new { id = appointmentDto.AppointmentId }),
            rel: "update",
            method: "PUT"
            ));
        
        newAppointmentDto.Links.Add(new Link(
            href: Url.Action(nameof(DeleteAppointment), new { id = appointmentDto.AppointmentId }),
            rel: "delete",
            method: "DELETE"
            ));
        

        return CreatedAtAction(nameof(GetAppointmentById), new { id = appointment.AppointmentId }, appointmentDto);
    }

    [HttpGet("{id}", Name = nameof(GetAppointmentById))]
    public async Task<ActionResult<AppointmentDto>> GetAppointmentById(Guid id)
    {
        var appointment = await _context.Appointments.Include(a => a.Doctor).Include(a => a.Patient).FirstOrDefaultAsync(a => a.AppointmentId.ToString() == id.ToString());
        if (appointment == null) return NotFound();

        var appointmentDto = new AppointmentDto
        {
            AppointmentId = appointment.AppointmentId,
            PatientId = appointment.PatientId,
            DoctorId = appointment.DoctorId,
            AppointmentDateTime = appointment.AppointmentDateTime,
            Status = appointment.Status,
            Notes = appointment.Notes
        };
        
        appointmentDto.Links.Add(new Link(
            href: Url.Action(nameof(GetAppointmentById), new { id = appointmentDto.AppointmentId }),
            rel: "self",
            method: "GET"
            ));
        
        appointmentDto.Links.Add(new Link(
            href: Url.Action(nameof(UpdateAppointment), new {id = appointmentDto.AppointmentId}),
            rel: "update",
            method: "PUT"
            ));

        appointmentDto.Links.Add(new Link(
            href:Url.Action(nameof(DeleteAppointment), new {id = appointmentDto.AppointmentId}),
            rel: "delete",
            method: "DELETE"
            ));
        
        return Ok(appointmentDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAppointment(Guid id, AppointmentDto appointmentDto)
    {
        if (id.ToString() != appointmentDto.AppointmentId.ToString()) return BadRequest();

        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null) return NotFound();

        appointment.PatientId = appointmentDto.PatientId;
        appointment.DoctorId = appointmentDto.DoctorId;
        appointment.AppointmentDateTime = appointmentDto.AppointmentDateTime;
        appointment.Status = appointmentDto.Status;
        appointment.Notes = appointmentDto.Notes;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAppointment(Guid id)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null) return NotFound();

        _context.Appointments.Remove(appointment);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
