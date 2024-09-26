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
                href: Url.Action(nameof(SpecializationsController.GetSpecializations), "Specializations"),
                rel: "specializations",
                method: "GET"),

            new Link(
                href: Url.Action(nameof(SpecializationsController.CreateSpecialization), "Specializations"),
                rel: "create-specialization",
                method: "POST"),

            // new Link(
            //     href: Url.Action(nameof(DoctorsController.GetDoctors), "Doctors"),
            //     rel: "doctors",
            //     method: "GET"),
            //
            // new Link(
            //     href: Url.Action(nameof(AppointmentsController.GetAppointments), "Appointments"),
            //     rel: "appointments",
            //     method: "GET")
        };

        return Ok(links);
    }
}
