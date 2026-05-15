using Backend.Backend.DTOs;
using Backend.Backend.Interface.RepositoryInterface;
using Backend.Backend.Interface.ServiceInterface;
using Backend.Backend.Model;
using Backend.Backend.Repository;
using Microsoft.AspNetCore.Identity;
using System.Data;

namespace Backend.Backend.Service
{
    public class RolePermissionService : IRolePermissionService
    {
        private readonly IRolePermissionRepository _rolepermissionRepository;

        public RolePermissionService(IRolePermissionRepository _rolepermissionRepository)
        {
            this._rolepermissionRepository = _rolepermissionRepository;
        }

        public async Task<ResponseDTO<IEnumerable<GetRolePermissionDTO>>> GetAllAsync()
        {
            var rolepermission = await _rolepermissionRepository.GetAllAsync();
            if (rolepermission is null || !rolepermission.Any())
                return new ResponseDTO<IEnumerable<GetRolePermissionDTO>>
                {
                    Status_code= 404,
                    Data = null
                };

            var data = rolepermission.Select(rp => new GetRolePermissionDTO
            {
                Role_ID = rp.Role_ID,
                Permission_ID = rp.Permission_ID,
                Role = rp.Role,
                Permission = rp.Permission_Entity,
            });

            return new ResponseDTO<IEnumerable<GetRolePermissionDTO>>
            {
                Status_code = 200,
                Data = data
            };
        }

        public async Task<ResponseDTO<GetRolePermissionDTO>> GetByIdAsync(int id)
        {
            var rp = await _rolepermissionRepository.GetByIdAsync(id);
            if (rp is null)
                return new ResponseDTO<GetRolePermissionDTO>
                {
                    Status_code = 404,
                    Data = null
                };

            var data = new GetRolePermissionDTO
            {
                Role_ID = rp.Role_ID,
                Permission_ID = rp.Permission_ID,
                Role = rp.Role,
                Permission = rp.Permission_Entity,
            };

            return new ResponseDTO<GetRolePermissionDTO>
            {
                Status_code = 200,
                Data = data
            };
        }

        public async Task<ResponseDTO<GetRolePermissionDTO>> AddAsync(AddRolePermissionDTO dto)
        {
            var rolepermission = new RolePermission
            {
                Role_ID = dto.Role_ID,
                Permission_ID = dto.Permission_ID,
                Role = dto.Role,
                Permission_Entity = dto.Permission,
            };

            await _rolepermissionRepository.AddAsync(rolepermission);

            var data = new GetRolePermissionDTO
            {
                Role_ID = dto.Role_ID,
                Permission_ID = dto.Permission_ID,
                Role = dto.Role,
                Permission = dto.Permission,
            };

            return new ResponseDTO<GetRolePermissionDTO>
            {
                Status_code=200,
                Data = data
            };
        }

        public async Task<ResponseDTO<GetRolePermissionDTO>> UpdateAsync(int id, AddRolePermissionDTO dto)
        {
            var existing = await _rolepermissionRepository.GetByIdAsync(id);
            if (existing == null)
                return new ResponseDTO<GetRolePermissionDTO>()
                {
                    Status_code = 404,
                    Data = null
                };

            existing.Role_ID = dto.Role_ID;
            existing.Permission_ID = dto.Permission_ID;
            existing.Role = dto.Role;
            existing.Permission_Entity = dto.Permission;

            await _rolepermissionRepository.UpdateAsync(existing);

            var data = new GetRolePermissionDTO
            {
                Role_ID = existing.Role_ID,
                Permission_ID = existing.Permission_ID,
                Role = existing.Role,
                Permission = existing.Permission_Entity,
            };

            return new ResponseDTO<GetRolePermissionDTO>()
            {
                Status_code = 200,
                Data = data
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _rolepermissionRepository.GetByIdAsync(id);
            if (existing == null) return false;

            await _rolepermissionRepository.DeleteAsync(existing);
            return true;
        }
    }
}