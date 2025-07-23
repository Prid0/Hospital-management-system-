using Hospital_Management.Dto;

namespace Hospital_Management.Services.Iservice
{
    public interface Ipatientservice
    {
        public Task<List<GeneralResponseDto>> GetAllPatient();
        public Task<GeneralResponseDto> AddPatient(AddGeneralDto patient);
        public Task<GeneralResponseDto> UpdatePatient(int Id, AddGeneralDto patient, int UpdatedById);
        public Task<GeneralResponseDto> GetPatientBySearch(string search);
        public Task<GeneralResponseDto> GetPatientByName(string patientName);
        public Task<GeneralResponseDto> GetPatientByEmail(string patientEmail);
        public Task<PatientMedicalRecordDto> GetPatientMedicalRecord(int patientId);
        public Task<GeneralResponseDto> GetPatientByPhoneNumber(string patientPhoneNumber);
        public Task<GeneralResponseDto> DeletePatient(int patientId);
    }
}
