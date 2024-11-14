using DoctorAppointmentWebApi.Messages;
using DoctorAppointmentWebApi.Models;

namespace DoctorAppointmentWebApi.RabbitMQ;

public static class RabbitMQ
{
    public static NewPatientMessage ToNewPatientMessage(this Patient patient)
    {
        var message = new NewPatientMessage()
        {
            PatientID = patient.PatientID,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            DateOfBirth = patient.DateOfBirth,
            Gender = patient.Gender,
            PhoneNumber = patient.PhoneNumber,
            Email = patient.Email,
            Address = patient.Address,
            InsuranceNumber = patient.InsuranceNumber
        };
        return message;
    }
    
        public static AppointmentMessage ToNewAppointmentMessage(this Appointment appointment)
        {
            var message = new AppointmentMessage()
            {
                AppointmentId = appointment.AppointmentId,
                PatientId = $"{appointment.Patient.FirstName} {appointment.Patient.LastName}",
                DoctorId = $"{appointment.Doctor.FirstName} {appointment.Doctor.LastName}",
                AppointmentDateTime = appointment.AppointmentDateTime,
                Status = appointment.Status,
                Notes = appointment.Notes
            };
            return message;
        }
        
        public static DoctorScheduleUpdatedMessage ToUpdatedScheduleMessage(this DoctorSchedule schedule)
        {
            if (schedule.Doctor == null)
            {
                throw new InvalidOperationException("Произошла ошибка при получении сущности Доктора!");
            }

            var message = new DoctorScheduleUpdatedMessage()
            {
                DoctorName = schedule.Doctor.FirstName ?? "Неизвестно",
                DoctorLastName = schedule.Doctor.LastName ?? "Неизсвестно",
                AvailableFrom = schedule.AvailableFrom,
                AvailableTo = schedule.AvailableTo,
                DayOfWeek = schedule.DayOfWeek,
            };
            return message;
        }

        
        public static ToPatientAboutUpdatedDoctorSchedule ToPatientAboutUpdatedDoctorSchedule(this DoctorSchedule schedule, Doctor doctor, Patient patient)
        {
            var message = new ToPatientAboutUpdatedDoctorSchedule()
            {
                PatientFirstName = patient.FirstName,
                PatientLastName = patient.LastName,
                DoctorFirstName = doctor.FirstName,
                DoctorLastName = doctor.LastName,
                AvailableFrom = schedule.AvailableFrom,
                AvailableTo = schedule.AvailableTo,
                DayOfWeek = schedule.DayOfWeek,
            };
            return message;
        } 
        
        public static DoctorReportData ToDoctorReportData(this Doctor doctor, DateTime startDate, DateTime endDate, List<Appointment> appointments)
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
