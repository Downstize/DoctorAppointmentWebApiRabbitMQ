namespace DoctorAppointmentWebApi.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
[Table("specializations")]
public class Specialization
{
    [Key]
    [Column("specializationid")]
    public Guid SpecializationId { get; set; }
    
    [Column("name")]
    public string? SpecializationName { get; set; }
}