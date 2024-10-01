namespace DoctorAppointmentWebApi.DTOs;

public class SpecializationDto
{
    public Guid SpecializationId { get; set; }
    
    public string? SpecializationName { get; set; }
    
    public List<Link> Links { get; set; } = new List<Link>();
}