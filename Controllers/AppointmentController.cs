using DoctorAppointmentWeb.Api.Controllers;
using DoctorAppointmentWeb.Api.Responses;
using DoctorAppointmentWebApi.DTOs;
using DoctorAppointmentWebApi.Messages;
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
            Status = "In processing", // Устанавливаем статус автоматически
            Notes = request.Notes
        };

        _context.Appointments.Add(appointment);
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
    
        PublishAppointmentToRabbitMQ(appointment);

        return CreatedAtAction(nameof(GetAppointmentById), new { id = appointment.AppointmentId }, response);
    }
    
    private void PublishAppointmentToRabbitMQ(Appointment appointment)
    {
        var patient = _context.Patients.FirstOrDefault(p => p.PatientID == appointment.PatientId);

        if (patient == null)
        {
            throw new Exception("Пациент не найден");
        }

        var message = new AppointmentMessage
        {
            AppointmentId = appointment.AppointmentId,
            PatientId = appointment.PatientId,
            DoctorId = appointment.DoctorId,
            AppointmentDateTime = appointment.AppointmentDateTime,
            Status = appointment.Status,
            PatientFullName = $"{patient.FirstName} {patient.LastName}"
        };

        _bus.PubSub.Publish(message);
        Console.WriteLine($"[x] Опубликована запись на приём: {message.AppointmentId}");
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAppointment(Guid id, AppointmentRequest request)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null) return NotFound();

        appointment.PatientId = request.PatientId;
        appointment.DoctorId = request.DoctorId;
        appointment.AppointmentDateTime = request.AppointmentDateTime;
        appointment.Status = "In processing";
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
}