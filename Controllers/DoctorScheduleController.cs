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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DoctorScheduleDto>>> GetSchedules()
    {
        var schedules = await _context.DoctorSchedules.Include(ds => ds.Doctor).ToListAsync();

        var scheduleDtos = schedules.Select(schedule => new DoctorScheduleDto
        {
            ScheduleId = schedule.ScheduleId,
            DoctorId = schedule.DoctorId,
            AvailableFrom = schedule.AvailableFrom,
            AvailableTo = schedule.AvailableTo,
            DayOfWeek = schedule.DayOfWeek
        }).ToList();

        return Ok(scheduleDtos);
    }

    [HttpPost]
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

        return CreatedAtAction(nameof(GetScheduleById), new { id = schedule.ScheduleId }, scheduleDto);
    }

    [HttpGet("{id}", Name = nameof(GetScheduleById))]
    public async Task<ActionResult<DoctorScheduleDto>> GetScheduleById(int id)
    {
        var schedule = await _context.DoctorSchedules.Include(ds => ds.Doctor).FirstOrDefaultAsync(ds => ds.ScheduleId.ToString() == id.ToString());
        if (schedule == null) return NotFound();

        var scheduleDto = new DoctorScheduleDto
        {
            ScheduleId = schedule.ScheduleId,
            DoctorId = schedule.DoctorId,
            AvailableFrom = schedule.AvailableFrom,
            AvailableTo = schedule.AvailableTo,
            DayOfWeek = schedule.DayOfWeek
        };

        return Ok(scheduleDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSchedule(int id, DoctorScheduleDto scheduleDto)
    {
        if (id.ToString() != scheduleDto.ScheduleId.ToString()) return BadRequest();

        var schedule = await _context.DoctorSchedules.FindAsync(id);
        if (schedule == null) return NotFound();

        schedule.DoctorId = scheduleDto.DoctorId;
        schedule.AvailableFrom = scheduleDto.AvailableFrom;
        schedule.AvailableTo = scheduleDto.AvailableTo;
        schedule.DayOfWeek = scheduleDto.DayOfWeek;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSchedule(int id)
    {
        var schedule = await _context.DoctorSchedules.FindAsync(id);
        if (schedule == null) return NotFound();

        _context.DoctorSchedules.Remove(schedule);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
