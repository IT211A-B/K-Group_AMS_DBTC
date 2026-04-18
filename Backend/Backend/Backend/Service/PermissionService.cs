using Backend.Backend.Model;
using Backend.Backend.DTOs;
using Backend.Backend.Interface.ServiceInterface;
using Backend.Backend.Interface.RepositoryInterface;

namespace Backend.Backend.Service
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;

        public PermissionService(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        public async Task<ResponseDTO<IEnumerable<GetPermissionDTO>>> GetAllAsync()
        {
            var permissions = await _permissionRepository.GetAllAsync();
            if (permissions is null || !permissions.Any())
                return new ResponseDTO<IEnumerable<GetPermissionDTO>>
                {
                    Status_code= 404,
                    Data = Enumerable.Empty<GetPermissionDTO>()
                };

            var data = permissions.Select(p => new GetPermissionDTO
            {
                Permission_ID = p.Permission_ID,
                Permission_Description = p.Permission_Description,
                Access_ID = p.Access_ID
            });

            return new ResponseDTO<IEnumerable<GetPermissionDTO>>
            {
                Status_code = 200,
                Data = data
            };
        }

        public async Task<ResponseDTO<GetPermissionDTO>> GetByIdAsync(int id)
        {
            var p = await _permissionRepository.GetByIdAsync(id);
            if (p == null)
                return new ResponseDTO<GetPermissionDTO>
                {
                    Status_code = 404,
                    Data=null
                };

            var data = new GetPermissionDTO
            {
                Permission_ID = p.Permission_ID,
                Permission_Description = p.Permission_Description,
                Access_ID = p.Access_ID
            };

            return new ResponseDTO<GetPermissionDTO>
            {
                Status_code = 200,
                Data = data
            };
        }

        public async Task<ResponseDTO<GetPermissionDTO>> AddAsync(AddPermissionDTO dto)
        {
            var permission = new Permission
            {
                Permission_Description = dto.Permission_Description,
                Access_ID = dto.Access_ID
            };

            await _permissionRepository.AddAsync(permission);

            var data = new GetPermissionDTO
            {
                Permission_ID = permission.Permission_ID,
                Permission_Description = permission.Permission_Description,
                Access_ID = permission.Access_ID
            };

            return new ResponseDTO<GetPermissionDTO>
            {
                Status_code = 200,
                Data = data
            };
        }

        public async Task<ResponseDTO<GetPermissionDTO>> UpdateAsync(int id, AddPermissionDTO dto)
        {
            var existing = await _permissionRepository.GetByIdAsync(id);
            if (existing == null)
                return new ResponseDTO<GetPermissionDTO>
                {
                    Status_code= 404,
                    Data = null
                };

            existing.Permission_Description = dto.Permission_Description;
            existing.Access_ID = dto.Access_ID;

            await _permissionRepository.UpdateAsync(existing);

            var data = new GetPermissionDTO
            {
                Permission_ID = existing.Permission_ID,
                Permission_Description = existing.Permission_Description,
                Access_ID = existing.Access_ID
            };

            return new ResponseDTO<GetPermissionDTO>
                { Status_code = 200,
                Data = data };
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