namespace DoctorAppointmentWebApi.DTOs;

public class SpecializationsDto
{
    public int SpecializationId { get; set; }
    
    public string? SpecializationName { get; set; }
    
    public List<Link> Links { get; set; } = new List<Link>();
}