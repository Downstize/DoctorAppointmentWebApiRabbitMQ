namespace DoctorAppointmentWebApi.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
[Table("specializations")]
public class Specializations
{
    [Key]
    [Column("specializationid")]
    public int SpecializationId { get; set; }
    
    [Column("name")]
    public string? SpecializationName { get; set; }
}