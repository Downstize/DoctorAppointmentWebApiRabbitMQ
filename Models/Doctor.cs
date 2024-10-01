using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MassTransit;

namespace DoctorAppointmentWebApi.Models;

[Table("doctors")]
public class Doctor
{
    [Key]
    [Column("doctorid")]
    public Guid DoctorId { get; set; }
    
    [Column("firstname")]
    public string FirstName { get; set; }
    
    [Column("lastname")]
    public string LastName { get; set; }
    
    [Column("specializationid")]
    public Guid SpecializationId { get; set; }
    
    [Column("departmentid")]
    public Guid DepartmentId { get; set; }
    
    [Column("phonenumber")]
    public string PhoneNumber { get; set; }
    
    [Column("email")]
    public string Email { get; set; }
    
    [Column("roomnumber")]
    public string RoomNumber { get; set; }
    

    public Specialization Specialization { get; set; }
    
    public Department Department { get; set; }
}