using Hospital_Management.Dto;
using Hospital_Management.Models;

namespace Hospital_Management.Services.Iservice
{
    public interface IprescriptionService
    {
        public Task<List<PrescriptionResponseDto>> GetAllPrescription();
        public Task<PrescriptionModel> AddPrescription(AddPrescriptionDto prescription);
        public Task<PrescriptionModel> UpdatePrescription(int patientId, int PrescriptionId, AddPrescriptionDto dto);
        public Task<PrescriptionModel> AddMoreMedicinInPrescription(int patientId, int PrescriptionId, AddPrescriptionDto newprescription);
        public Task<PrescriptionModel> DeletePrescription(int PatientId);
    }
}
