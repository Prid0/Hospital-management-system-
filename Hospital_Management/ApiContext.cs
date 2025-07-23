using Hospital_Management.Models;
using Microsoft.EntityFrameworkCore;

namespace Hospital_Management
{
    public class ApiContext : DbContext
    {
        public ApiContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<UsersModel> users { get; set; }
        public DbSet<PatientModel> patients { get; set; }
        public DbSet<DoctorModel> doctors { get; set; }
        public DbSet<DepartmentModel> departments { get; set; }
        public DbSet<AppointmentModel> appointments { get; set; }
        public DbSet<PrescriptionModel> prescriptions { get; set; }
        public DbSet<PrescribedMedicineModel> prescribedMedicines { get; set; }
        public DbSet<DoctorLeaveModel> doctorOnLeaves { get; set; }
        public DbSet<AdminModel> admins { get; set; }
        public DbSet<ReceptionistModel> receptionists { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // Prevent cascade delete from Patient → Prescription
            modelBuilder.Entity<PrescriptionModel>()
                .HasOne(p => p.Patient)
                .WithMany(pat => pat.Prescriptions)
                .HasForeignKey(p => p.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Prevent cascade delete from Doctor → Prescription
            modelBuilder.Entity<PrescriptionModel>()
                .HasOne(p => p.Doctor)
                .WithMany(d => d.Prescriptions)
                .HasForeignKey(p => p.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Prevent cascade delete from Patient → Appointment
            modelBuilder.Entity<AppointmentModel>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Prevent cascade delete from Doctor → Appointment
            modelBuilder.Entity<AppointmentModel>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);
        }


    }
}
