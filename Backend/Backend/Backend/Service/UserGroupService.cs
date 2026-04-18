using Backend.Backend.Model;
using Backend.Backend.DTOs;
using Backend.Backend.Interface.ServiceInterface;
using Backend.Backend.Interface.RepositoryInterface;

namespace Backend.Backend.Service
{
    public class UserGroupService : IUserGroupService
    {
        private readonly IUserGroupRepository _userGroupRepository;

        public UserGroupService(IUserGroupRepository userGroupRepository)
        {
            _userGroupRepository = userGroupRepository;
        }

        public async Task<ResponseDTO<IEnumerable<GetUserGroupDTO>>> GetAllAsync()
        {
            var groups = await _userGroupRepository.GetAllAsync();
            if (groups is null || !groups.Any())
                return new ResponseDTO<IEnumerable<GetUserGroupDTO>>
                {
                    Status_code= 404,
                    Data = null
                };
            var data = groups.Select(g => new GetUserGroupDTO
            {
                Group_ID = g.Group_ID,
                Group_Name = g.Group_Name,
                Group_Description = g.Group_Description,
                Group_Created = g.Group_Created,
                Role_ID = g.Role_ID,
                Permission_ID = g.Permission_ID
            });

            return new ResponseDTO<IEnumerable<GetUserGroupDTO>>
            {
                Status_code = 200,
                Data = data
            };
        }

        public async Task<ResponseDTO<GetUserGroupDTO>> GetByIdAsync(int id)
        {
            var g = await _userGroupRepository.GetByIdAsync(id);
            if (g == null)
                return new ResponseDTO<GetUserGroupDTO>
                {
                    Status_code = 404,
                    Data = null
                };

            var data = new GetUserGroupDTO
            {
                Group_ID = g.Group_ID,
                Group_Name = g.Group_Name,
                Group_Description = g.Group_Description,
                Group_Created = g.Group_Created,
                Role_ID = g.Role_ID,
                Permission_ID = g.Permission_ID
            };

            return new ResponseDTO<GetUserGroupDTO>
            {
                Status_code = 200,
                Data =data
            };
        }

        public async Task<ResponseDTO<GetUserGroupDTO>> AddAsync(AddUserGroupDTO dto)
        {
            var group = new UserGroup
            {
                Group_Name = dto.Group_Name,
                Group_Description = dto.Group_Description,
                Group_Created = DateTime.UtcNow,
                Role_ID = dto.Role_ID,
                Permission_ID = dto.Permission_ID
            };

            await _userGroupRepository.AddAsync(group);

            var data = new GetUserGroupDTO
            {
                Group_ID = group.Group_ID,
                Group_Name = group.Group_Name,
                Group_Description = group.Group_Description,
                Group_Created = group.Group_Created,
                Role_ID = group.Role_ID,
                Permission_ID = group.Permission_ID
            };

            return new ResponseDTO<GetUserGroupDTO>
            {
                Status_code = 200,
                Data = data
            };
        }

        public async Task<ResponseDTO<GetUserGroupDTO>> UpdateAsync(int id, AddUserGroupDTO dto)
        {
            var existing = await _userGroupRepository.GetByIdAsync(id);
            if (existing == null)
                return new ResponseDTO<GetUserGroupDTO>
                {
                    Status_code = 404,
                    Data = null
                };

            existing.Group_Name = dto.Group_Name;
            existing.Group_Description = dto.Group_Description;
            existing.Role_ID = dto.Role_ID;
            existing.Permission_ID = dto.Permission_ID;

            await _userGroupRepository.UpdateAsync(existing);

            var data = new GetUserGroupDTO
            {
                Group_ID = existing.Group_ID,
                Group_Name = existing.Group_Name,
                Group_Description = existing.Group_Description,
                Group_Created = existing.Group_Created,
                Role_ID = existing.Role_ID,
                Permission_ID = existing.Permission_ID
            };

            return new ResponseDTO<GetUserGroupDTO>
            {
                Status_code=200,
                Data = data
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _userGroupRepository.GetByIdAsync(id);
            if (existing == null) return false;

            await _userGroupRepository.DeleteAsync(existing);
            return true;
        }
    }
}