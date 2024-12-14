using DoctorAppointmentWebApi.Messages;
using DoctorAppointmentWebApi.Models;

namespace DoctorAppointmentWebApi.RabbitMQ;

public static class RabbitMQ
{
    public static DoctorReportData ToDoctorReportData(this Doctor doctor, DateTime startDate, DateTime endDate,
        List<Appointment> appointments)
    {
        var reportData = new DoctorReportData
        {
            DoctorName = $"{doctor.FirstName} {doctor.LastName}",
            Specialization = doctor.Specialization?.SpecializationName,
            Period = $"{startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}",
            TotalPatients = appointments.Count,
            PatientDetails = appointments.Select(a => new PatientInfo
            {
                FirstName = a.Patient.FirstName,
                LastName = a.Patient.LastName,
                DateOfBirth = a.Patient.DateOfBirth ?? default,
                AppointmentDate = a.AppointmentDateTime,
                Symptoms = a.Notes
            }).ToList()
        };
        return reportData;
    }
}


