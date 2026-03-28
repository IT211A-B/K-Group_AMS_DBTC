using Attendance_Management_System.AttendanceManagementSystem.Model;
using Attendance_Management_System.AttendanceManagementSystem.DTOs;
using Attendance_Management_System.AttendanceManagementSystem.Interface.ServiceInterface;
using Attendance_Management_System.AttendanceManagementSystem.Interface.RepositoryInterface;

namespace Attendance_Management_System.AttendanceManagementSystem.Service
{
    public class AccessService : IAccessService
    {
        private readonly IAccessRepository _accessRepository;

        public AccessService(IAccessRepository accessRepository)
        {
            _accessRepository = accessRepository;
        }

        public async Task<IEnumerable<GetAccessDTO>> GetAllAsync()
        {
            var accesses = await _accessRepository.GetAllAsync();
            return accesses.Select(a => new GetAccessDTO
            {
                Access_ID = a.Access_ID,
                Name = a.Name,
                Description = a.Description
            });
        }

        public async Task<GetAccessDTO?> GetByIdAsync(int id)
        {
            var a = await _accessRepository.GetByIdAsync(id);
            if (a == null) return null;

            return new GetAccessDTO
            {
                Access_ID = a.Access_ID,
                Name = a.Name,
                Description = a.Description
            };
        }

        public async Task<GetAccessDTO> AddAsync(AddAccessDTO dto)
        {
            var access = new Access
            {
                Name = dto.Name,
                Description = dto.Description
            };

            await _accessRepository.AddAsync(access);

            return new GetAccessDTO
            {
                Access_ID = access.Access_ID,
                Name = access.Name,
                Description = access.Description
            };
        }

        public async Task<GetAccessDTO?> UpdateAsync(int id, AddAccessDTO dto)
        {
            var existing = await _accessRepository.GetByIdAsync(id);
            if (existing == null) return null;

            existing.Name = dto.Name;
            existing.Description = dto.Description;

            await _accessRepository.UpdateAsync(existing);

            return new GetAccessDTO
            {
                Access_ID = existing.Access_ID,
                Name = existing.Name,
                Description = existing.Description
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _accessRepository.GetByIdAsync(id);
            if (existing == null) return false;

            await _accessRepository.DeleteAsync(existing);
            return true;
        }
    }
}