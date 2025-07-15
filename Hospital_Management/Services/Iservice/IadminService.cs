using Hospital_Management.Dto;

namespace Hospital_Management.Services.Iservice
{
    public interface IadminService
    {
        public Task<List<GeneralResponseDto>> GetAllAdmin();
        public Task<GeneralResponseDto> AddAdmin(AddGeneralDto admin);
        public Task<GeneralResponseDto> UpdateAdmin(AddGeneralDto admin, int adminId, int UpdatedById);
        public Task<GeneralResponseDto> DeleteAdmin(int adminId);
    }
}
