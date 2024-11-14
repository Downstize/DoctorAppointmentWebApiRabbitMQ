using DoctorAppointmentWebApi.DTOs;
using DoctorAppointmentWebApi.Models;
using DoctorAppointmentWebApi.RabbitMQ;
using EasyNetQ;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoctorAppointmentWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IBus _bus;

    public AppointmentController(ApplicationDbContext context, IBus bus)
    {
        _context = context;
        _bus = bus;
    }

    [HttpGet(Name = nameof(GetAppointments))]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAppointments()
    {
        var appointments = await _context.Appointments
                                         .Include(a => a.Doctor)
                                         .Include(a => a.Patient)
                                         .ToListAsync();

        var appointmentDtos = appointments.Select(appointment => CreateAppointmentDtoWithLinks(appointment)).ToList();

        var response = new
        {
            _links = new
            {
                self = new Link(Url.Action(nameof(GetAppointments))!, "self", "GET"),
                create = new Link(Url.Action(nameof(CreateAppointment))!, "create-appointment", "POST")
            },
            appointments = appointmentDtos
        };

        return Ok(response);
    }

    [HttpGet("{id}", Name = nameof(GetAppointmentById))]
    public async Task<ActionResult<AppointmentDto>> GetAppointmentById(Guid id)
    {
        var appointment = await _context.Appointments
                                        .Include(a => a.Doctor)
                                        .Include(a => a.Patient)
                                        .FirstOrDefaultAsync(a => a.AppointmentId == id);
        if (appointment == null) return NotFound();

        var appointmentDto = CreateAppointmentDtoWithLinks(appointment);
        return Ok(new
        {
            appointment = appointmentDto,
            _links = new
            {
                self = new Link(Url.Action(nameof(GetAppointmentById), new { id = appointment.AppointmentId })!, "self", "GET"),
                update = new Link(Url.Action(nameof(UpdateAppointment), new { id = appointment.AppointmentId })!, "update", "PUT"),
                delete = new Link(Url.Action(nameof(DeleteAppointment), new { id = appointment.AppointmentId })!, "delete", "DELETE")
            }
        });
    }
    
    private async Task PublishNewAppointment(Appointment appointment)
    {
        var loadedAppointment = await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .FirstOrDefaultAsync(a => a.AppointmentId == appointment.AppointmentId);

        if (loadedAppointment == null || loadedAppointment.Patient == null || loadedAppointment.Doctor == null)
        {
            throw new InvalidOperationException("Ошибочка!");
        }

        var message = loadedAppointment.ToNewAppointmentMessage();
        await _bus.PubSub.PublishAsync(message);
    }


    [HttpPost(Name = nameof(CreateAppointment))]
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
        await PublishNewAppointment(appointment);

        var newAppointmentDto = CreateAppointmentDtoWithLinks(appointment);

        return CreatedAtAction(nameof(GetAppointmentById), new { id = appointment.AppointmentId }, new
        {
            appointment = newAppointmentDto,
            _links = new
            {
                self = new Link(Url.Action(nameof(GetAppointmentById), new { id = appointment.AppointmentId })!, "self", "GET"),
                update = new Link(Url.Action(nameof(UpdateAppointment), new { id = appointment.AppointmentId })!, "update", "PUT"),
                delete = new Link(Url.Action(nameof(DeleteAppointment), new { id = appointment.AppointmentId })!, "delete", "DELETE")
            }
        });
    }

    [HttpPut("{id}", Name = nameof(UpdateAppointment))]
    public async Task<IActionResult> UpdateAppointment(Guid id, AppointmentDto appointmentDto)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null) return NotFound();

        appointment.PatientId = appointmentDto.PatientId;
        appointment.DoctorId = appointmentDto.DoctorId;
        appointment.AppointmentDateTime = appointmentDto.AppointmentDateTime;
        appointment.Status = appointmentDto.Status;
        appointment.Notes = appointmentDto.Notes;

        await _context.SaveChangesAsync();

        var updatedAppointmentDto = CreateAppointmentDtoWithLinks(appointment);
        return Ok(new
        {
            appointment = updatedAppointmentDto,
            _links = new
            {
                self = new Link(Url.Action(nameof(GetAppointmentById), new { id = appointment.AppointmentId })!, "self", "GET"),
                update = new Link(Url.Action(nameof(UpdateAppointment), new { id = appointment.AppointmentId })!, "update", "PUT"),
                delete = new Link(Url.Action(nameof(DeleteAppointment), new { id = appointment.AppointmentId })!, "delete", "DELETE")
            }
        });
    }

    [HttpDelete("{id}", Name = nameof(DeleteAppointment))]
    public async Task<IActionResult> DeleteAppointment(Guid id)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null) return NotFound();

        _context.Appointments.Remove(appointment);
        await _context.SaveChangesAsync();

        var response = new
        {
            message = $"Appointment {appointment.AppointmentId} deleted",
            _links = new
            {
                getAllAppointments = new Link(Url.Action(nameof(GetAppointments))!, "get-all-appointments", "GET"),
                createAppointment = new Link(Url.Action(nameof(CreateAppointment))!, "create-appointment", "POST")
            }
        };

        return Ok(response);
    }

    // Вспомогательный метод для создания DTO с включенными ссылками HAL
    private AppointmentDto CreateAppointmentDtoWithLinks(Appointment appointment)
    {
        var appointmentDto = new AppointmentDto
        {
            AppointmentId = appointment.AppointmentId,
            PatientId = appointment.PatientId,
            DoctorId = appointment.DoctorId,
            AppointmentDateTime = appointment.AppointmentDateTime,
            Status = appointment.Status,
            Notes = appointment.Notes,
            Links = new List<Link>
            {
                new Link(Url.Action(nameof(GetAppointmentById), new { id = appointment.AppointmentId })!, "self", "GET"),
                new Link(Url.Action(nameof(UpdateAppointment), new { id = appointment.AppointmentId })!, "update", "PUT"),
                new Link(Url.Action(nameof(DeleteAppointment), new { id = appointment.AppointmentId })!, "delete", "DELETE"),
                new Link(Url.Action("GetDoctorById", "Doctor", new { id = appointment.DoctorId })!, "doctor", "GET"),
                new Link(Url.Action("GetPatientById", "Patient", new { id = appointment.PatientId })!, "patient", "GET")
            }
        };

        return appointmentDto;
    }
}
