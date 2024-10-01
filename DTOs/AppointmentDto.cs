namespace DoctorAppointmentWebApi.DTOs;

public class AppointmentDto
{
    public Guid AppointmentId { get; set; }
    
    public Guid PatientId { get; set; }
    
    public Guid DoctorId { get; set; }
    
    public DateTime AppointmentDateTime { get; set; }
    
    public string Status { get; set; }
    
    public string Notes { get; set; }

    
    public List<Link> Links { get; set; } = new List<Link>();
}