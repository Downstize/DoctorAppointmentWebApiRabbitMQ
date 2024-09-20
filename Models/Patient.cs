namespace DoctorAppointmentWebApi.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("patients")]
public class Patient
{
    [Key]
    [Column("patientid")]
    public int PatientID { get; set; }

    [Column("firstname")]
    public string FirstName { get; set; }

    [Column("lastname")]
    public string LastName { get; set; }

    [Column("dateofbirth")]
    public DateTime? DateOfBirth { get; set; }

    [Column("gender")]
    public string Gender { get; set; }

    [Column("phonenumber")]
    public string PhoneNumber { get; set; }

    [Column("email")]
    public string Email { get; set; }

    [Column("address")]
    public string Address { get; set; }

    [Column("insurancenumber")]
    public string InsuranceNumber { get; set; }
}

