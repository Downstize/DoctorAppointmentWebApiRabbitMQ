using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoctorAppointmentWebApi.Models;

[Table("appointments")]
public class Appointment
{
    [Key]
    [Column("appointmentid")]
    public Guid AppointmentId { get; set; }
    
    [Column("patientid")]
    public Guid PatientId { get; set; }
    
    [Column("doctorid")]
    public Guid DoctorId { get; set; }
    
    [Column("appointmentdatetime")]
    public DateTime AppointmentDateTime { get; set; }
    
    [Column("status")]
    public string Status { get; set; }
    
    [Column("notes")]
    public string Notes { get; set; }

    
    public Patient Patient { get; set; }
    
    public Doctor Doctor { get; set; }
}