using Hospital_Management.Dto;

namespace Hospital_Management.Services.Iservice
{
    public interface IreceptionistService
    {
        public Task<List<GeneralResponseDto>> GetAllReceptionist();
        public Task<GeneralResponseDto> AddReceptionist(AddGeneralDto receptionist);
        public Task<GeneralResponseDto> UpdateReceptionist(AddGeneralDto receptionist, int receptionistId, int UpdatedById);
        public Task<GeneralResponseDto> DeleteReceptionist(int receptionistId);
    }
}
