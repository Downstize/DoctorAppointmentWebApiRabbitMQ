using DoctorAppointmentWebApi.DTOs;

namespace DoctorAppointmentWebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PatientsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/patients
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PatientDto>>> GetPatients()
    {
        var patients = await _context.Patients.ToListAsync();

        var patientDtos = patients.Select(patient => new PatientDto
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
        }).ToList();

        foreach (var patientDto in patientDtos)
        {
            patientDto.Links.Add(new Link(
                href: Url.Action(nameof(GetPatientById), new { id = patientDto.PatientID }),
                rel: "self",
                method: "GET"));

            patientDto.Links.Add(new Link(
                href: Url.Action(nameof(UpdatePatient), new { id = patientDto.PatientID }),
                rel: "update",
                method: "PUT"));

            patientDto.Links.Add(new Link(
                href: Url.Action(nameof(DeletePatient), new { id = patientDto.PatientID }),
                rel: "delete",
                method: "DELETE"));
        }

        return Ok(patientDtos);
    }

    // GET: api/patients/5
    [HttpGet("{id}", Name = nameof(GetPatientById))]
    public async Task<ActionResult<PatientDto>> GetPatientById(int id)
    {
        var patient = await _context.Patients.FindAsync(id);

        if (patient == null)
        {
            return NotFound();
        }

        var patientDto = new PatientDto
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
        
        patientDto.Links.Add(new Link(
            href: Url.Action(nameof(GetPatientById), new { id = patientDto.PatientID }),
            rel: "self",
            method: "GET"));

        patientDto.Links.Add(new Link(
            href: Url.Action(nameof(UpdatePatient), new { id = patientDto.PatientID }),
            rel: "update",
            method: "PUT"));

        patientDto.Links.Add(new Link(
            href: Url.Action(nameof(DeletePatient), new { id = patientDto.PatientID }),
            rel: "delete",
            method: "DELETE"));

        return Ok(patientDto);
    }

    // PUT: api/patients/5
    [HttpPut("{id}", Name = nameof(UpdatePatient))]
    public async Task<IActionResult> UpdatePatient(int id, PatientDto patientDto)
    {
        if (id != patientDto.PatientID)
        {
            return BadRequest();
        }

        var patient = await _context.Patients.FindAsync(id);
        if (patient == null)
        {
            return NotFound();
        }
        
        patient.FirstName = patientDto.FirstName;
        patient.LastName = patientDto.LastName;
        patient.DateOfBirth = patientDto.DateOfBirth;
        patient.Gender = patientDto.Gender;
        patient.PhoneNumber = patientDto.PhoneNumber;
        patient.Email = patientDto.Email;
        patient.Address = patientDto.Address;
        patient.InsuranceNumber = patientDto.InsuranceNumber;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PatientExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // DELETE: api/patients/5
    [HttpDelete("{id}", Name = nameof(DeletePatient))]
    public async Task<IActionResult> DeletePatient(int id)
    {
        var patient = await _context.Patients.FindAsync(id);
        if (patient == null)
        {
            return NotFound();
        }

        _context.Patients.Remove(patient);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool PatientExists(int id)
    {
        return _context.Patients.Any(e => e.PatientID == id);
    }
}
