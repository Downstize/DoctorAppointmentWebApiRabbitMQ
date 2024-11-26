using DoctorAppointmentWeb.Api.Controllers;
using DoctorAppointmentWeb.Api.Responses;
using DoctorAppointmentWebApi.DTOs;
using DoctorAppointmentWebApi.Messages;
using DoctorAppointmentWebApi.Models;
using DoctorAppointmentWebApi.RabbitMQ;
using EasyNetQ;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoctorAppointmentWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DoctorScheduleController : ControllerBase, IDoctorScheduleApi
{
    private readonly ApplicationDbContext _context;
    private readonly IBus _bus;

    public DoctorScheduleController(ApplicationDbContext context, IBus bus)
    {
        _context = context;
        _bus = bus;
    }

    [HttpGet(Name = nameof(GetSchedules))]
    public async Task<ActionResult<IEnumerable<DoctorScheduleResponse>>> GetSchedules()
    {
        var schedules = await _context.DoctorSchedules.Include(ds => ds.Doctor).ToListAsync();

        var scheduleDtos = schedules.Select(schedule => CreateDoctorScheduleDtoWithLinks(schedule)).ToList();

        var response = new
        {
            _links = new
            {
                self = new Link(Url.Action(nameof(GetSchedules))!, "self", "GET"),
                create = new Link(Url.Action(nameof(CreateSchedule))!, "create-schedule", "POST")
            },
            schedules = scheduleDtos
        };

        return Ok(response);
    }

    [HttpPost(Name = nameof(CreateSchedule))]
    public async Task<ActionResult<DoctorScheduleResponse>> CreateSchedule(DoctorScheduleRequest scheduleDto)
    {
        var schedule = new DoctorSchedule
        {
            DoctorId = scheduleDto.DoctorId,
            AvailableFrom = scheduleDto.AvailableFrom,
            AvailableTo = scheduleDto.AvailableTo,
            DayOfWeek = scheduleDto.DayOfWeek
        };

        _context.DoctorSchedules.Add(schedule);
        await _context.SaveChangesAsync();

        var newScheduleDto = CreateDoctorScheduleDtoWithLinks(schedule);

        var response = new
        {
            schedule = newScheduleDto,
            _links = new
            {
                self = new Link(Url.Action(nameof(GetScheduleById), new { id = schedule.ScheduleId })!, "self", "GET"),
                update = new Link(Url.Action(nameof(UpdateSchedule), new { id = schedule.ScheduleId })!, "update", "PUT"),
                delete = new Link(Url.Action(nameof(DeleteSchedule), new { id = schedule.ScheduleId })!, "delete", "DELETE")
            }
        };

        return CreatedAtAction(nameof(GetScheduleById), new { id = schedule.ScheduleId }, response);
    }

    [HttpGet("{id}", Name = nameof(GetScheduleById))]
    public async Task<ActionResult<DoctorScheduleResponse>> GetScheduleById(Guid id)
    {
        var schedule = await _context.DoctorSchedules.Include(ds => ds.Doctor).FirstOrDefaultAsync(ds => ds.ScheduleId == id);
        if (schedule == null) return NotFound();

        var scheduleDto = CreateDoctorScheduleDtoWithLinks(schedule);

        var response = new
        {
            schedule = scheduleDto,
            _links = new
            {
                self = new Link(Url.Action(nameof(GetScheduleById), new { id = schedule.ScheduleId })!, "self", "GET"),
                update = new Link(Url.Action(nameof(UpdateSchedule), new { id = schedule.ScheduleId })!, "update", "PUT"),
                delete = new Link(Url.Action(nameof(DeleteSchedule), new { id = schedule.ScheduleId })!, "delete", "DELETE")
            }
        };

        return Ok(response);
    }
    
    private async Task PublishUpdatedDoctorSchedule(DoctorSchedule doctorSchedule)
    {
        var message = doctorSchedule.ToUpdatedScheduleMessage();
        await _bus.PubSub.PublishAsync(message);
    }
    
    private async Task NotifyPatientsAboutUpdatedSchedule(DoctorSchedule schedule)
    {
        var appointments = await _context.Appointments
            .Where(a => a.DoctorId == schedule.DoctorId)
            .Include(a => a.Patient) 
            .ToListAsync();
        
        foreach (var appointment in appointments)
        {
            var message = schedule.ToPatientAboutUpdatedDoctorSchedule(schedule.Doctor, appointment.Patient);
            await _bus.PubSub.PublishAsync(message);
        }
    }



    [HttpPut("{id}", Name = nameof(UpdateSchedule))]
    public async Task<IActionResult> UpdateSchedule(Guid id, DoctorScheduleRequest scheduleDto)
    {
        var schedule = await _context.DoctorSchedules
            .Include(s => s.Doctor) 
            .FirstOrDefaultAsync(s => s.ScheduleId == id);
        if (schedule == null) return NotFound();

        schedule.DoctorId = scheduleDto.DoctorId;
        schedule.AvailableFrom = scheduleDto.AvailableFrom;
        schedule.AvailableTo = scheduleDto.AvailableTo;
        schedule.DayOfWeek = scheduleDto.DayOfWeek;

        await _context.SaveChangesAsync();
        await PublishUpdatedDoctorSchedule(schedule);
        await NotifyPatientsAboutUpdatedSchedule(schedule);

        var updatedScheduleDto = CreateDoctorScheduleDtoWithLinks(schedule);

        var response = new
        {
            schedule = updatedScheduleDto,
            _links = new
            {
                self = new Link(Url.Action(nameof(GetScheduleById), new { id = schedule.ScheduleId })!, "self", "GET"),
                update = new Link(Url.Action(nameof(UpdateSchedule), new { id = schedule.ScheduleId })!, "update", "PUT"),
                delete = new Link(Url.Action(nameof(DeleteSchedule), new { id = schedule.ScheduleId })!, "delete", "DELETE")
            }
        };

        return Ok(response);
    }


    [HttpDelete("{id}", Name = nameof(DeleteSchedule))]
    public async Task<IActionResult> DeleteSchedule(Guid id)
    {
        var schedule = await _context.DoctorSchedules.FindAsync(id);
        if (schedule == null) return NotFound();

        _context.DoctorSchedules.Remove(schedule);
        await _context.SaveChangesAsync();

        var response = new
        {
            message = $"Doctor schedule {schedule.ScheduleId} deleted",
            _links = new
            {
                getAllSchedules = new Link(Url.Action(nameof(GetSchedules))!, "get-all-schedules", "GET"),
                createSchedule = new Link(Url.Action(nameof(CreateSchedule))!, "create-schedule", "POST")
            }
        };

        return Ok(response);
    }
    
    private DoctorScheduleDto CreateDoctorScheduleDtoWithLinks(DoctorSchedule schedule)
    {
        var scheduleDto = new DoctorScheduleDto
        {
            ScheduleId = schedule.ScheduleId,
            DoctorId = schedule.DoctorId,
            AvailableFrom = schedule.AvailableFrom,
            AvailableTo = schedule.AvailableTo,
            DayOfWeek = schedule.DayOfWeek,
            Links = new List<Link>
            {
                new Link(Url.Action(nameof(GetScheduleById), new { id = schedule.ScheduleId })!, "self", "GET"),
                new Link(Url.Action(nameof(UpdateSchedule), new { id = schedule.ScheduleId })!, "update", "PUT"),
                new Link(Url.Action(nameof(DeleteSchedule), new { id = schedule.ScheduleId })!, "delete", "DELETE")
            }
        };

        return scheduleDto;
    }
}
