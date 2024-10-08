using DoctorAppointmentWebApi.DTOs;
using DoctorAppointmentWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoctorAppointmentWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DoctorScheduleController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DoctorScheduleController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet(Name = nameof(GetSchedules))]
    public async Task<ActionResult<IEnumerable<DoctorScheduleDto>>> GetSchedules()
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
    public async Task<ActionResult<DoctorScheduleDto>> CreateSchedule(DoctorScheduleDto scheduleDto)
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
    public async Task<ActionResult<DoctorScheduleDto>> GetScheduleById(Guid id)
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

    [HttpPut("{id}", Name = nameof(UpdateSchedule))]
    public async Task<IActionResult> UpdateSchedule(Guid id, DoctorScheduleDto scheduleDto)
    {
        var schedule = await _context.DoctorSchedules.FindAsync(id);
        if (schedule == null) return NotFound();

        schedule.DoctorId = scheduleDto.DoctorId;
        schedule.AvailableFrom = scheduleDto.AvailableFrom;
        schedule.AvailableTo = scheduleDto.AvailableTo;
        schedule.DayOfWeek = scheduleDto.DayOfWeek;

        await _context.SaveChangesAsync();

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

    // Вспомогательный метод для создания DTO с включенными ссылками HAL
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
