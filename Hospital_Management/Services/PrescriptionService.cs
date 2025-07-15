using Hospital_Management.Dto;
using Hospital_Management.Models;
using Hospital_Management.Services.Iservice;
using Microsoft.EntityFrameworkCore;

namespace Hospital_Management.Services
{
    public class PrescriptionService : IprescriptionService
    {
        private readonly ApiContext _db;
        public PrescriptionService(ApiContext db)
        {
            _db = db;
        }
        public async Task<List<PrescriptionResponseDto>> GetAllPrescription()
        {
            var prescriptions = await _db.prescriptions.Include(p => p.Medicines).Include(p => p.Patient)
              .Include(p => p.Doctor)
              .Select(p => new PrescriptionResponseDto
              {
                  CreatedDate = p.CreatedDate,
                  UpdatedDate = p.UpdatedDate,
                  PrescriptionId = p.PrescriptionId,
                  PatientId = p.PatientId,
                  PatientName = p.Patient.FullName,
                  PatientEmail = p.Patient.Email,
                  PatientPhone = p.Patient.PhoneNumber,
                  DoctorId = p.DoctorId,
                  DoctorName = p.Doctor.FullName,
                  DoctorEmail = p.Doctor.Email,
                  Decease = p.Decease,
                  Medicines = p.Medicines
                          .Select(m => new PrescribedMedicineDto
                          {
                              MedicineName = m.MedicineName,
                              TimesPerDay = m.TimesPerDay,
                              StartDate = m.StartDate,
                              EndDate = m.EndDate,
                          })
                          .ToList()
              })
                  .ToListAsync();

            if (prescriptions != null & prescriptions.Count > 0)
            {

                return prescriptions;
            }
            return null;
        }
        public async Task<PrescriptionModel> AddPrescription(AddPrescriptionDto prescription)
        {
            if (prescription == null)
            {
                throw new MissingFieldException();
            }
            var Addprescription = new PrescriptionModel
            {
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow,
                DoctorId = prescription.DoctorId,
                PatientId = prescription.PatientId,
                Decease = prescription.Decease,
                Medicines = prescription.Medicines.Select(m => new PrescribedMedicineModel
                {
                    MedicineName = m.MedicineName,
                    TimesPerDay = m.TimesPerDay,
                    StartDate = m.StartDate,
                    EndDate = m.EndDate,
                }).ToList()
            };

            _db.prescriptions.Add(Addprescription);
            await _db.SaveChangesAsync();
            return Addprescription;

        }
        public async Task<PrescriptionModel> AddMoreMedicinInPrescription(int patientId, int PrescriptionId, AddPrescriptionDto newprescription)
        {
            var oldPrescripton = await _db.prescriptions.FirstOrDefaultAsync(p => p.PatientId == patientId & p.PrescriptionId == PrescriptionId);
            if (oldPrescripton == null)
            {
                return null;
            }
            oldPrescripton.UpdatedDate = DateTime.UtcNow;
            oldPrescripton.DoctorId = newprescription.DoctorId;
            oldPrescripton.PatientId = newprescription.PatientId;
            oldPrescripton.Decease = string.IsNullOrEmpty(newprescription.Decease) ? oldPrescripton.Decease : newprescription.Decease;
            oldPrescripton.Medicines = newprescription.Medicines.Select(m => new PrescribedMedicineModel
            {
                MedicineName = m.MedicineName,
                TimesPerDay = m.TimesPerDay,
                StartDate = m.StartDate,
                EndDate = m.EndDate,
            }).ToList();
            await _db.SaveChangesAsync();
            return oldPrescripton;

        }

        public async Task<PrescriptionModel> UpdatePrescription(int patientId, int prescriptionId, AddPrescriptionDto newprescription)
        {
            var oldPrescription = await _db.prescriptions
                .Include(p => p.Medicines)
                .FirstOrDefaultAsync(p => p.PatientId == patientId && p.PrescriptionId == prescriptionId);

            if (oldPrescription == null)
                return null;

            // 1. Remove old medicine entries
            _db.prescribedMedicines.RemoveRange(oldPrescription.Medicines);

            // 2. Add updated ones
            oldPrescription.Medicines = newprescription.Medicines.Select(m => new PrescribedMedicineModel
            {
                MedicineName = m.MedicineName,
                TimesPerDay = m.TimesPerDay,
                StartDate = m.StartDate,
                EndDate = m.EndDate
            }).ToList();

            // 3. Update other fields
            oldPrescription.UpdatedDate = DateTime.UtcNow;
            oldPrescription.Decease = string.IsNullOrEmpty(newprescription.Decease) ? oldPrescription.Decease : newprescription.Decease;
            oldPrescription.DoctorId = newprescription.DoctorId;
            oldPrescription.PatientId = newprescription.PatientId;

            await _db.SaveChangesAsync();
            return oldPrescription;
        }


        public async Task<PrescriptionModel> DeletePrescription(int PatientId)
        {
            var oldPrescripton = await _db.prescriptions.FirstOrDefaultAsync(p => p.PatientId == PatientId);
            if (oldPrescripton == null)
            {
                return null;
            }
            _db.prescribedMedicines.RemoveRange(oldPrescripton.Medicines);
            _db.prescriptions.Remove(oldPrescripton);
            await _db.SaveChangesAsync();
            return oldPrescripton;
        }
    }
}
