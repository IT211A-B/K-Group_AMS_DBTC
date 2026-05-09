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
                String_Name = p.String_Name,
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
                String_Name = p.String_Name,
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
                String_Name = dto.String_Name,
            };

            await _permissionRepository.AddAsync(permission);

            var data = new GetPermissionDTO
            {
                Permission_ID = permission.Permission_ID,
                String_Name = permission.String_Name,

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

            existing.String_Name = dto.String_Name;

            await _permissionRepository.UpdateAsync(existing);

            var data = new GetPermissionDTO
            {
                Permission_ID = existing.Permission_ID,
                String_Name = existing.String_Name,
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