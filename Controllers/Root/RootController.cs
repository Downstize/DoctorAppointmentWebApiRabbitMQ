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
                rel: "specializations",
                method: "GET"),

            new Link(
                href: Url.Action(nameof(SpecializationController.CreateSpecialization), "Specialization"),
                rel: "create-specialization",
                method: "POST"),

            new Link(
                href: Url.Action(nameof(DoctorController.GetDoctors), "Doctor"),
                rel: "doctors",
                method: "GET"),
            
            new Link(
                href: Url.Action(nameof(AppointmentController.GetAppointments), "Appointment"),
                rel: "appointments",
                method: "GET")
        };

        return Ok(links);
    }
}
