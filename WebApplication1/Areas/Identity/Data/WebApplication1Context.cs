using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
namespace WebApplication1.Data;
public class WebApplication1Context : IdentityDbContext<UserRegistration>
{
    
    public WebApplication1Context(DbContextOptions<WebApplication1Context> options)
        : base(options)
    {

    }
    public DbSet<Sell> Sells { get; set; }

    public DbSet<Requests> Requests { get; set; }

    public DbSet<CarDetails> CarDetails { get; set; }

    public DbSet<CarsSold> CarsSold { get; set; }

    public DbSet<Payment> Payments { get; set; }

    public DbSet<TestDrive> TestDrives { get; set; }

    public DbSet<VerificationAppointment> VerificationAppointments { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
