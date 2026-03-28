using Attendance_Management_System.AttendanceManagementSystem.Model;
using Attendance_Management_System.AttendanceManagementSystem.DTOs;
using Attendance_Management_System.AttendanceManagementSystem.Interface.ServiceInterface;
using Attendance_Management_System.AttendanceManagementSystem.Interface.RepositoryInterface;

namespace Attendance_Management_System.AttendanceManagementSystem.Service
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;

        public PermissionService(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        public async Task<IEnumerable<GetPermissionDTO>> GetAllAsync()
        {
            var permissions = await _permissionRepository.GetAllAsync();
            return permissions.Select(p => new GetPermissionDTO
            {
                Permission_ID = p.Permission_ID,
                Permission_Description = p.Permission_Description,
                Access_ID = p.Access_ID
            });
        }

        public async Task<GetPermissionDTO?> GetByIdAsync(int id)
        {
            var p = await _permissionRepository.GetByIdAsync(id);
            if (p == null) return null;

            return new GetPermissionDTO
            {
                Permission_ID = p.Permission_ID,
                Permission_Description = p.Permission_Description,
                Access_ID = p.Access_ID
            };
        }

        public async Task<GetPermissionDTO> AddAsync(AddPermissionDTO dto)
        {
            var permission = new Permission
            {
                Permission_Description = dto.Permission_Description,
                Access_ID = dto.Access_ID
            };

            await _permissionRepository.AddAsync(permission);

            return new GetPermissionDTO
            {
                Permission_ID = permission.Permission_ID,
                Permission_Description = permission.Permission_Description,
                Access_ID = permission.Access_ID
            };
        }

        public async Task<GetPermissionDTO?> UpdateAsync(int id, AddPermissionDTO dto)
        {
            var existing = await _permissionRepository.GetByIdAsync(id);
            if (existing == null) return null;

            existing.Permission_Description = dto.Permission_Description;
            existing.Access_ID = dto.Access_ID;

            await _permissionRepository.UpdateAsync(existing);

            return new GetPermissionDTO
            {
                Permission_ID = existing.Permission_ID,
                Permission_Description = existing.Permission_Description,
                Access_ID = existing.Access_ID
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _permissionRepository.GetByIdAsync(id);
            if (existing == null) return false;

            await _permissionRepository.DeleteAsync(existing);
            return true;
        }
    }
}