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

        public async Task<IEnumerable<GetRoleDTO>> GetAllAsync()
        {
            var roles = await _roleRepository.GetAllAsync();
            return roles.Select(r => new GetRoleDTO
            {
                Role_ID = r.Role_ID,
                Name = r.Name,
                Description = r.Description
            });
        }

        public async Task<GetRoleDTO?> GetByIdAsync(int id)
        {
            var r = await _roleRepository.GetByIdAsync(id);
            if (r == null) return null;

            return new GetRoleDTO
            {
                Role_ID = r.Role_ID,
                Name = r.Name,
                Description = r.Description
            };
        }

        public async Task<GetRoleDTO> AddAsync(AddRoleDTO dto)
        {
            var role = new Role
            {
                Name = dto.Name,
                Description = dto.Description
            };

            await _roleRepository.AddAsync(role);

            return new GetRoleDTO
            {
                Role_ID = role.Role_ID,
                Name = role.Name,
                Description = role.Description
            };
        }

        public async Task<GetRoleDTO?> UpdateAsync(int id, AddRoleDTO dto)
        {
            var existing = await _roleRepository.GetByIdAsync(id);
            if (existing == null) return null;

            existing.Name = dto.Name;
            existing.Description = dto.Description;

            await _roleRepository.UpdateAsync(existing);

            return new GetRoleDTO
            {
                Role_ID = existing.Role_ID,
                Name = existing.Name,
                Description = existing.Description
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