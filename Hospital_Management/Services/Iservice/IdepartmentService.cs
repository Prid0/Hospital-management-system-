using Hospital_Management.Dto;
using Hospital_Management.Models;

namespace Hospital_Management.Services.Iservice
{
    public interface IdepartmentService
    {
        public Task<List<DepartmentModel>> GetAllDepartment();
        public Task<DepartmentModel> AddDepartment(DepartmentDto department);
        public Task<DepartmentDto> UpdateDepartment(DepartmentDto department, int departmentId);
        public Task<DepartmentModel> DeleteDepartment(int departmentId);
    }
}
