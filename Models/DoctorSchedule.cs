using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoctorAppointmentWebApi.Models;

[Table("doctorschedules")]
public class DoctorSchedule
{
    [Key]
    [Column("scheduleid")]
    public Guid ScheduleId { get; set; }
    
    [Column("doctorid")]
    public Guid DoctorId { get; set; }
    
    [Column("availablefrom")]
    public TimeSpan AvailableFrom { get; set; }
    
    [Column("availableto")]
    public TimeSpan AvailableTo { get; set; }
    
    [Column("dayofweek")]
    public string DayOfWeek { get; set; }

    
    public Doctor Doctor { get; set; }
}