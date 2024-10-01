namespace DoctorAppointmentWebApi.DTOs;

public class DoctorScheduleDto
{
    public Guid ScheduleId { get; set; }
    
    public Guid DoctorId { get; set; }
    
    public TimeSpan AvailableFrom { get; set; }
    
    public TimeSpan AvailableTo { get; set; }
    
    public string DayOfWeek { get; set; }

    
    public List<Link> Links { get; set; } = new List<Link>();
}