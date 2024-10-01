namespace DoctorAppointmentWebApi.DTOs;

public class DepartmentDto
{
    public Guid DepartmentId { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }

    public List<Link> Links { get; set; } = new List<Link>();
}