using DoctorAppointmentWeb.Api.Controllers;
using DoctorAppointmentWeb.Api.Responses;
using DoctorAppointmentWebApi.DTOs;
using DoctorAppointmentWebApi.Models;
using DoctorAppointmentWebApi.RabbitMQ;
using EasyNetQ;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace DoctorAppointmentWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentController : ControllerBase, IAppointmentApi
{
    private readonly ApplicationDbContext _context;
    private readonly IBus _bus;

    public AppointmentController(ApplicationDbContext context, IBus bus)
    {
        _context = context;
        _bus = bus;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Получить список всех приемов", Description = "Возвращает список всех записей на прием.")]
    [SwaggerResponse(200, "Список приемов успешно возвращен", typeof(IEnumerable<AppointmentResponse>))]
    [SwaggerResponse(500, "Внутренняя ошибка сервера")]
    public async Task<ActionResult<IEnumerable<AppointmentResponse>>> GetAppointments()
    {
        var appointments = await _context.Appointments
                                         .Include(a => a.Doctor)
                                         .Include(a => a.Patient)
                                         .ToListAsync();

        var responses = appointments.Select(appointment => new AppointmentResponse
        {
            AppointmentId = appointment.AppointmentId,
            PatientId = appointment.PatientId,
            DoctorId = appointment.DoctorId,
            AppointmentDateTime = appointment.AppointmentDateTime,
            Status = appointment.Status,
            Notes = appointment.Notes
        }).ToList();

        return Ok(responses);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AppointmentResponse>> GetAppointmentById(Guid id)
    {
        var appointment = await _context.Appointments
                                        .Include(a => a.Doctor)
                                        .Include(a => a.Patient)
                                        .FirstOrDefaultAsync(a => a.AppointmentId == id);
        if (appointment == null) return NotFound();

        var response = new AppointmentResponse
        {
            AppointmentId = appointment.AppointmentId,
            PatientId = appointment.PatientId,
            DoctorId = appointment.DoctorId,
            AppointmentDateTime = appointment.AppointmentDateTime,
            Status = appointment.Status,
            Notes = appointment.Notes
        };

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<AppointmentResponse>> CreateAppointment(AppointmentRequest request)
    {
        var appointment = new Appointment
        {
            PatientId = request.PatientId,
            DoctorId = request.DoctorId,
            AppointmentDateTime = request.AppointmentDateTime,
            Status = request.Status,
            Notes = request.Notes
        };

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();
        await PublishNewAppointment(appointment);

        var response = new AppointmentResponse
        {
            AppointmentId = appointment.AppointmentId,
            PatientId = appointment.PatientId,
            DoctorId = appointment.DoctorId,
            AppointmentDateTime = appointment.AppointmentDateTime,
            Status = appointment.Status,
            Notes = appointment.Notes
        };

        return CreatedAtAction(nameof(GetAppointmentById), new { id = appointment.AppointmentId }, response);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAppointment(Guid id, AppointmentRequest request)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null) return NotFound();

        appointment.PatientId = request.PatientId;
        appointment.DoctorId = request.DoctorId;
        appointment.AppointmentDateTime = request.AppointmentDateTime;
        appointment.Status = request.Status;
        appointment.Notes = request.Notes;

        await _context.SaveChangesAsync();

        var response = new AppointmentResponse
        {
            AppointmentId = appointment.AppointmentId,
            PatientId = appointment.PatientId,
            DoctorId = appointment.DoctorId,
            AppointmentDateTime = appointment.AppointmentDateTime,
            Status = appointment.Status,
            Notes = appointment.Notes
        };

        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAppointment(Guid id)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null) return NotFound();

        _context.Appointments.Remove(appointment);
        await _context.SaveChangesAsync();

        return Ok(new { message = $"Appointment {appointment.AppointmentId} deleted" });
    }

    private async Task PublishNewAppointment(Appointment appointment)
    {
        var message = appointment.ToNewAppointmentMessage();
        await _bus.PubSub.PublishAsync(message);
    }
}