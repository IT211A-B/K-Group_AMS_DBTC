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

        public async Task<ResponseDTO<IEnumerable<GetDepartmentDTO>>> GetAllAsync()
        {
            var departments = await _departmentRepository.GetAllAsync();
            if (departments is null || !departments.Any())
                return new ResponseDTO<IEnumerable<GetDepartmentDTO>>
                {
                    Status_code= 404,
                    Data = null
                };
            var data = departments.Select(d => new GetDepartmentDTO
            {
                Department_Id = d.Department_Id,
                Name = d.Name,
                Description = d.Description
            });

            return new ResponseDTO<IEnumerable<GetDepartmentDTO>>
            {
                Status_code = 200,
                Data    = data
            };
        }

        public async Task<ResponseDTO<GetDepartmentDTO>> GetByIdAsync(int id)
        {
            var d = await _departmentRepository.GetByIdAsync(id);
            if (d == null)
                return new ResponseDTO<GetDepartmentDTO>()
                {
                    Status_code=404,
                    Data = null
                };

            var data = new GetDepartmentDTO
            {
                Department_Id = d.Department_Id,
                Name = d.Name,
                Description = d.Description
            };

            return new ResponseDTO<GetDepartmentDTO>()
            {
                Status_code = 200,
                Data = data
            };
        }

        public async Task<ResponseDTO<GetDepartmentDTO>> AddAsync(AddDepartmentDTO dto)
        {
            var department = new Department
            {
                Name = dto.Name,
                Description = dto.Description
            };

            await _departmentRepository.AddAsync(department);

            var data = new GetDepartmentDTO
            {
                Department_Id = department.Department_Id,
                Name = department.Name,
                Description = department.Description
            };

            return new ResponseDTO<GetDepartmentDTO>()
            { Status_code = 200, Data = data };
        }

        public async Task<ResponseDTO<GetDepartmentDTO>> UpdateAsync(int id, AddDepartmentDTO dto)
        {
            var existing = await _departmentRepository.GetByIdAsync(id);
            if (existing == null) return new ResponseDTO<GetDepartmentDTO>() { Status_code = 404, Data = null };

            existing.Name = dto.Name;
            existing.Description = dto.Description;

            await _departmentRepository.UpdateAsync(existing);

            var data = new GetDepartmentDTO
            {
                Department_Id = existing.Department_Id,
                Name = existing.Name,
                Description = existing.Description
            };

            return new ResponseDTO<GetDepartmentDTO>()
            {
                Status_code =200,
                Data = data
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