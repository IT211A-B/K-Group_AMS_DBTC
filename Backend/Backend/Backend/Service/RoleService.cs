using Backend.Backend.Model;
using Backend.Backend.DTOs;
using Backend.Backend.Interface.ServiceInterface;
using Backend.Backend.Interface.RepositoryInterface;

namespace Backend.Backend.Service
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<ResponseDTO<IEnumerable<GetRoleDTO>>> GetAllAsync()
        {
            var roles = await _roleRepository.GetAllAsync();
            if (roles is null || !roles.Any())
                return new ResponseDTO<IEnumerable<GetRoleDTO>>
                {
                    Status_code = 404,
                    Data = Enumerable.Empty<GetRoleDTO>()
                };

            var data = roles.Select(r => new GetRoleDTO
            {
                Role_ID = r.Role_ID,
                Name = r.Name,
                Description = r.Description
            });

            return new ResponseDTO<IEnumerable<GetRoleDTO>>
            {
                Status_code = 200,
                Data = data
            };
        }

        public async Task<ResponseDTO<GetRoleDTO>> GetByIdAsync(int id)
        {
            var r = await _roleRepository.GetByIdAsync(id);
            if (r == null)
                return new ResponseDTO<GetRoleDTO>
                {
                    Status_code = 404,
                    Data = null,
                };

            var data= new GetRoleDTO
            {
                Role_ID = r.Role_ID,
                Name = r.Name,
                Description = r.Description
            };

            return new ResponseDTO<GetRoleDTO>
            {
                Status_code = 200,
                Data = data
            };
        }

        public async Task<ResponseDTO<GetRoleDTO>> AddAsync(AddRoleDTO dto)
        {
            var role = new Role
            {
                Name = dto.Name,
                Description = dto.Description
            };

            await _roleRepository.AddAsync(role);

            var data = new GetRoleDTO
            {
                Role_ID = role.Role_ID,
                Name = role.Name,
                Description = role.Description
            };


            return new ResponseDTO<GetRoleDTO>
            {
                Status_code = 200,
                Data = data
            };
        }

        public async Task<ResponseDTO<GetRoleDTO>> UpdateAsync(int id, AddRoleDTO dto)
        {
            var existing = await _roleRepository.GetByIdAsync(id);
            if (existing == null)
                return new ResponseDTO<GetRoleDTO>
                {
                    Status_code = 404,
                    Data = null
                };

            existing.Name = dto.Name;
            existing.Description = dto.Description;

            await _roleRepository.UpdateAsync(existing);

            var data = new GetRoleDTO
            {
                Role_ID = existing.Role_ID,
                Name = existing.Name,
                Description = existing.Description
            };

            return new ResponseDTO<GetRoleDTO>
            {
                Status_code = 200,
                Data = data
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _roleRepository.GetByIdAsync(id);
            if (existing == null) return false;

            await _roleRepository.DeleteAsync(existing);
            return true;
        }
    }
}