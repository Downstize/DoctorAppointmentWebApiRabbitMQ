using DoctorAppointmentWebApi.DTOs;
using DoctorAppointmentWebApi.Models;
using DoctorAppointmentWebApi.RabbitMQ;
using EasyNetQ;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoctorAppointmentWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IBus _bus;

    public PatientController(ApplicationDbContext context, IBus bus)
    {
        _context = context;
        _bus = bus;
    }
    
    [HttpGet(Name = nameof(GetPatients))]
    public async Task<ActionResult<IEnumerable<PatientDto>>> GetPatients()
    {
        var patients = await _context.Patients.ToListAsync();

        var patientDtos = patients.Select(patient => CreatePatientDtoWithLinks(patient)).ToList();

        var response = new
        {
            _links = new
            {
                self = new Link(Url.Action(nameof(GetPatients))!, "self", "GET"),
                create = new Link(Url.Action(nameof(CreatePatient))!, "create-patient", "POST")
            },
            patients = patientDtos
        };

        return Ok(response);
    }

    private async Task PublishNewPatient(Patient patient)
    {
        var message = patient.ToNewPatientMessage();
        await _bus.PubSub.PublishAsync(message);
    }

    [HttpPost(Name = nameof(CreatePatient))]
    public async Task<ActionResult<PatientDto>> CreatePatient(PatientDto patientDto)
    {
        var patient = new Patient
        {
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
        await PublishNewPatient(patient);
        var createdPatientDto = CreatePatientDtoWithLinks(patient);

        var response = new
        {
            patient = createdPatientDto,
            _links = new
            {
                self = new Link(Url.Action(nameof(GetPatientById), new { id = createdPatientDto.PatientID })!, "self", "GET"),
                update = new Link(Url.Action(nameof(UpdatePatient), new { id = createdPatientDto.PatientID })!, "update", "PUT"),
                delete = new Link(Url.Action(nameof(DeletePatient), new { id = createdPatientDto.PatientID })!, "delete", "DELETE")
            }
        };

        return CreatedAtAction(nameof(GetPatientById), new { id = patient.PatientID }, response);
    }

    [HttpGet("{id}", Name = nameof(GetPatientById))]
    public async Task<ActionResult<PatientDto>> GetPatientById(Guid id)
    {
        var patient = await _context.Patients.FindAsync(id);

        if (patient == null)
        {
            return NotFound();
        }

        var patientDto = CreatePatientDtoWithLinks(patient);

        var response = new
        {
            patient = patientDto,
            _links = new
            {
                self = new Link(Url.Action(nameof(GetPatientById), new { id = patientDto.PatientID })!, "self", "GET"),
                update = new Link(Url.Action(nameof(UpdatePatient), new { id = patientDto.PatientID })!, "update", "PUT"),
                delete = new Link(Url.Action(nameof(DeletePatient), new { id = patientDto.PatientID })!, "delete", "DELETE")
            }
        };

        return Ok(response);
    }
    
    [HttpPut("{id}", Name = nameof(UpdatePatient))]
    public async Task<IActionResult> UpdatePatient(Guid id, PatientDto patientDto)
    {
        var patient = await _context.Patients.FindAsync(id);
        if (patient == null) return NotFound();

        patient.FirstName = patientDto.FirstName;
        patient.LastName = patientDto.LastName;
        patient.DateOfBirth = patientDto.DateOfBirth;
        patient.Gender = patientDto.Gender;
        patient.PhoneNumber = patientDto.PhoneNumber;
        patient.Email = patientDto.Email;
        patient.Address = patientDto.Address;
        patient.InsuranceNumber = patientDto.InsuranceNumber;

        await _context.SaveChangesAsync();

        var updatedPatientDto = CreatePatientDtoWithLinks(patient);

        var response = new
        {
            patient = updatedPatientDto,
            _links = new
            {
                self = new Link(Url.Action(nameof(GetPatientById), new { id = patient.PatientID })!, "self", "GET"),
                update = new Link(Url.Action(nameof(UpdatePatient), new { id = patient.PatientID })!, "update", "PUT"),
                delete = new Link(Url.Action(nameof(DeletePatient), new { id = patient.PatientID })!, "delete", "DELETE")
            }
        };

        return Ok(response);
    }
    
    [HttpDelete("{id}", Name = nameof(DeletePatient))]
    public async Task<IActionResult> DeletePatient(Guid id)
    {
        var patient = await _context.Patients.FindAsync(id);
        if (patient == null)
        {
            return NotFound();
        }

        _context.Patients.Remove(patient);
        await _context.SaveChangesAsync();

        var response = new
        {
            message = $"Patient {patient.PatientID} deleted",
            _links = new
            {
                getAllPatients = new Link(Url.Action(nameof(GetPatients))!, "get-all-patients", "GET"),
                createPatient = new Link(Url.Action(nameof(CreatePatient))!, "create-patient", "POST")
            }
        };

        return Ok(response);
    }
    
    private PatientDto CreatePatientDtoWithLinks(Patient patient)
    {
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
            InsuranceNumber = patient.InsuranceNumber,
            Links = new List<Link>
            {
                new Link(Url.Action(nameof(GetPatientById), new { id = patient.PatientID })!, "self", "GET"),
                new Link(Url.Action(nameof(UpdatePatient), new { id = patient.PatientID })!, "update", "PUT"),
                new Link(Url.Action(nameof(DeletePatient), new { id = patient.PatientID })!, "delete", "DELETE")
            }
        };

        return patientDto;
    }
}
