using Hospital_Management.Dto;

namespace Hospital_Management.Services.Iservice
{
    public interface IdoctorService
    {
        public Task<List<ResponseDoctorDto>> GetAllDoctor();
        public Task<List<ResponseDoctorDto>> GetAvailableDoctors();
        public Task<DoctorDto> AddDoctor(DoctorDto doctor);
        public Task<ResponseDoctorDto> UpdateDoctor(DoctorDto doctor, int doctorId, int UpdatedById);
        public Task<DoctorDto> DeleteDoctor(int doctorId);
    }
}
