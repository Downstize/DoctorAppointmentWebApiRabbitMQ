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
}

