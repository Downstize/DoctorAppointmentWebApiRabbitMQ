using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoctorAppointmentWebApi.Models;

[Table("departments")]
public class Department
{
    [Key]
    [Column("departmentid")]
    public Guid DepartmentId { get; set; }
    
    [Column("name")]
    public string Name { get; set; }
    
    [Column("location")]
    public string Location { get; set; }
}