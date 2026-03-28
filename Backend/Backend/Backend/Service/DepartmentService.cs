using Backend.Backend.Model;
using Backend.Backend.DTOs;
using Backend.Backend.Interface.ServiceInterface;
using Backend.Backend.Interface.RepositoryInterface;

namespace Backend.Backend.Service
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;

        public DepartmentService(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        public async Task<IEnumerable<GetDepartmentDTO>> GetAllAsync()
        {
            var departments = await _departmentRepository.GetAllAsync();
            return departments.Select(d => new GetDepartmentDTO
            {
                Department_Id = d.Department_Id,
                Name = d.Name,
                Description = d.Description
            });
        }

        public async Task<GetDepartmentDTO?> GetByIdAsync(int id)
        {
            var d = await _departmentRepository.GetByIdAsync(id);
            if (d == null) return null;

            return new GetDepartmentDTO
            {
                Department_Id = d.Department_Id,
                Name = d.Name,
                Description = d.Description
            };
        }

        public async Task<GetDepartmentDTO> AddAsync(AddDepartmentDTO dto)
        {
            var department = new Department
            {
                Name = dto.Name,
                Description = dto.Description
            };

            await _departmentRepository.AddAsync(department);

            return new GetDepartmentDTO
            {
                Department_Id = department.Department_Id,
                Name = department.Name,
                Description = department.Description
            };
        }

        public async Task<GetDepartmentDTO?> UpdateAsync(int id, AddDepartmentDTO dto)
        {
            var existing = await _departmentRepository.GetByIdAsync(id);
            if (existing == null) return null;

            existing.Name = dto.Name;
            existing.Description = dto.Description;

            await _departmentRepository.UpdateAsync(existing);

            return new GetDepartmentDTO
            {
                Department_Id = existing.Department_Id,
                Name = existing.Name,
                Description = existing.Description
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _departmentRepository.GetByIdAsync(id);
            if (existing == null) return false;

            await _departmentRepository.DeleteAsync(existing);
            return true;
        }
    }
}