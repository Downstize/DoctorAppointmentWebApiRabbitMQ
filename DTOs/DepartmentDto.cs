using System.Text.Json.Serialization;

namespace DoctorAppointmentWebApi.DTOs;

public class DepartmentDto
{
    [JsonIgnore]
    public Guid DepartmentId { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }

    public List<Link> Links { get; set; } = new List<Link>();
}