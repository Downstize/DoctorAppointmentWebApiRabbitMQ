using DoctorAppointmentWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DoctorAppointmentWebApi;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        
    }
    
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Specialization> Specializations { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<DoctorSchedule> DoctorSchedules { get; set; }
    public DbSet<Department> Departments { get; set; }
}


