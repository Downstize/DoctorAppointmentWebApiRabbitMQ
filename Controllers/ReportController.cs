using Microsoft.AspNetCore.Mvc;
using DoctorAppointmentWebApi.Models;
using DoctorAppointmentWebApi.Messages;
using DoctorAppointmentWebApi.RabbitMQ;
using EasyNetQ;
using Microsoft.EntityFrameworkCore;

namespace DoctorAppointmentWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IBus _bus;

        public ReportController(ApplicationDbContext context, IBus bus)
        {
            _context = context;
            _bus = bus;
        }

        [HttpPost("create")]
        public async Task<IActionResult> GenerateDoctorAttendanceReport([FromBody] DoctorReportRequest request)
        {
            if (request == null || request.DoctorId == Guid.Empty || request.StartDate == default || request.EndDate == default)
            {
                return BadRequest("Необходимо указать DoctorId, StartDate и EndDate.");
            }
            
            var doctor = await _context.Doctors
                .Include(d => d.Specialization)
                .FirstOrDefaultAsync(d => d.DoctorId == request.DoctorId);

            if (doctor == null)
            {
                return NotFound($"Врач с ID {request.DoctorId} не найден.");
            }
            
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == request.DoctorId &&
                            a.AppointmentDateTime >= request.StartDate &&
                            a.AppointmentDateTime <= request.EndDate)
                .ToListAsync();
            
            var reportData = doctor.ToDoctorReportData(request.StartDate, request.EndDate, appointments);
            
            await _bus.PubSub.PublishAsync(reportData);

            return Ok("Запрос на генерацию отчёта отправлен.");
        }

    }
}



