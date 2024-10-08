using Microsoft.AspNetCore.Mvc;

namespace DoctorAppointmentWebApi.Controllers;

[Route("api")]
[ApiController]
public class RootController : ControllerBase
{
    [HttpGet(Name = nameof(GetRoot))]
    public IActionResult GetRoot()
    {
        var links = new List<Link>
        {
            new Link(
                href: Url.Action(nameof(GetRoot)),
                rel: "self",
                method: "GET"),

            new Link(
                href: Url.Action(nameof(SpecializationController.GetSpecializations), "Specialization"),
                rel: "get-specializations",
                method: "GET"),

            new Link(
                href: Url.Action(nameof(SpecializationController.CreateSpecialization), "Specialization"),
                rel: "create-specialization",
                method: "POST"),

            new Link(
                href: Url.Action(nameof(DoctorController.GetDoctors), "Doctor"),
                rel: "get-doctors",
                method: "GET"),

            new Link(
                href: Url.Action(nameof(DoctorController.CreateDoctor), "Doctor"),
                rel: "create-doctor",
                method: "POST"),

            new Link(
                href: Url.Action(nameof(AppointmentController.GetAppointments), "Appointment"),
                rel: "get-appointments",
                method: "GET"),

            new Link(
                href: Url.Action(nameof(AppointmentController.CreateAppointment), "Appointment"),
                rel: "create-appointment",
                method: "POST"),

            new Link(
                href: Url.Action(nameof(PatientController.GetPatients), "Patient"),
                rel: "get-patients",
                method: "GET"),

            new Link(
                href: Url.Action(nameof(PatientController.CreatePatient), "Patient"),
                rel: "create-patient",
                method: "POST"),

            new Link(
                href: Url.Action(nameof(DoctorScheduleController.GetSchedules), "DoctorSchedule"),
                rel: "get-doctor-schedules",
                method: "GET"),

            new Link(
                href: Url.Action(nameof(DoctorScheduleController.CreateSchedule), "DoctorSchedule"),
                rel: "create-doctor-schedule",
                method: "POST")
        };

        return Ok(links);
    }
}
