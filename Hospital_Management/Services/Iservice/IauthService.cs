using Hospital_Management.Dto;

namespace Hospital_Management.Services.Iservice
{
    public interface IauthService
    {
        public Task<string> Login(AuthDto auth);
    }
}
