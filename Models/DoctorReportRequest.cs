namespace DoctorAppointmentWebApi.Models;

public class DoctorReportRequest
{
    public Guid DoctorId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}