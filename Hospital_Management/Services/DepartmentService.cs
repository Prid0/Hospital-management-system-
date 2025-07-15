using Hospital_Management.Dto;
using Hospital_Management.Models;
using Hospital_Management.Services.Iservice;
using Microsoft.EntityFrameworkCore;

namespace Hospital_Management.Services
{
    public class DepartmentService : IdepartmentService
    {
        private readonly ApiContext _db;
        public DepartmentService(ApiContext db)
        {
            _db = db;
        }
        public async Task<List<DepartmentModel>> GetAllDepartment()
        {
            var departments = await _db.departments.Include(x => x.Doctors).ToListAsync();
            if (departments == null || departments.Count <= 0)
            {
                return null;
            }
            return departments;
        }
        public async Task<DepartmentModel> AddDepartment(DepartmentDto department)
        {
            if (department == null)
            {
                return null;
            }

            var NewDepartment = new DepartmentModel { DepartmentName = department.DeparmentName };
            await _db.departments.AddAsync(NewDepartment);
            await _db.SaveChangesAsync();

            return NewDepartment;

        }
        public async Task<DepartmentDto> UpdateDepartment(DepartmentDto department, int departmentId)
        {
            var existingDepartment = await _db.departments.FindAsync(departmentId);
            if (existingDepartment == null)
            {
                return null;
            }
            existingDepartment.DepartmentName = string.IsNullOrEmpty(department.DeparmentName) ? existingDepartment.DepartmentName : department.DeparmentName;
            await _db.SaveChangesAsync();
            return (new DepartmentDto { DeparmentName = department.DeparmentName });

        }
        public async Task<DepartmentModel> DeleteDepartment(int departmentId)
        {
            var existingDepartment = await _db.departments.FindAsync(departmentId);
            if (existingDepartment == null)
            {
                return null;
            }
            _db.departments.Remove(existingDepartment);
            await _db.SaveChangesAsync();
            return (existingDepartment);
        }
    }
}
