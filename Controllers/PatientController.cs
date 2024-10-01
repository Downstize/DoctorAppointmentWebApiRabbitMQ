using DoctorAppointmentWebApi.DTOs;
using DoctorAppointmentWebApi.Models;

namespace DoctorAppointmentWebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class PatientController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PatientController(ApplicationDbContext context)
    {
        _context = context;
    }
    
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

    [HttpPost("Create a new patient", Name = (nameof(CreatePatient)))]
    public async Task<ActionResult<PatientDto>> CreatePatient(PatientDto patientDto)
    {
        var patient = new Patient
        {
            PatientID = patientDto.PatientID,
            FirstName = patientDto.FirstName,
            Address = patientDto.Address,
            DateOfBirth = patientDto.DateOfBirth,
            Email = patientDto.Email,
            Gender = patientDto.Gender,
            InsuranceNumber = patientDto.InsuranceNumber,
            LastName = patientDto.LastName,
            PhoneNumber = patientDto.PhoneNumber,
        };
        
        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        var createdPatient = new PatientDto
        {
            PatientID = patientDto.PatientID,
            FirstName = patientDto.FirstName,
            Address = patientDto.Address,
            DateOfBirth = patientDto.DateOfBirth,
            Email = patientDto.Email,
            Gender = patientDto.Gender,
            InsuranceNumber = patientDto.InsuranceNumber,
            LastName = patientDto.LastName,
            PhoneNumber = patientDto.PhoneNumber,
        };
        
        createdPatient.Links.Add(new Link(
            href: Url.Action(nameof(GetPatientById), new { id = createdPatient.PatientID }),
            rel: "self",
            method: "GET"));

        createdPatient.Links.Add(new Link(
            href: Url.Action(nameof(UpdatePatient), new { id = createdPatient.PatientID }),
            rel: "update",
            method: "PUT"));

        createdPatient.Links.Add(new Link(
            href: Url.Action(nameof(DeletePatient), new { id = createdPatient.PatientID }),
            rel: "delete",
            method: "DELETE"));
        
        
        return Ok(createdPatient);
    }
    
    [HttpGet("{id}", Name = nameof(GetPatientById))]
    public async Task<ActionResult<PatientDto>> GetPatientById(Guid id)
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
            href: Url.Action(nameof(GetPatientById), new { id = patientDto.PatientID })!,
            rel: "self",
            method: "GET"));

        patientDto.Links.Add(new Link(
            href: Url.Action(nameof(UpdatePatient), new { id = patientDto.PatientID })!,
            rel: "update",
            method: "PUT"));

        patientDto.Links.Add(new Link(
            href: Url.Action(nameof(DeletePatient), new { id = patientDto.PatientID })!,
            rel: "delete",
            method: "DELETE"));

        return Ok(patientDto);
    }
    
    [HttpPut("{id}", Name = nameof(UpdatePatient))]
    public async Task<IActionResult> UpdatePatient(Guid id, PatientDto patientDto)
    {
        if (id.ToString() != patientDto.PatientID.ToString())
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

        await _context.SaveChangesAsync();

        return NoContent();
    }
    
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
    
}
