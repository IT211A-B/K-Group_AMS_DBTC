using Backend.Backend.Model;
using Backend.Backend.DTOs;
using Backend.Backend.Interface.ServiceInterface;
using Backend.Backend.Interface.RepositoryInterface;

namespace Backend.Backend.Service
{
    public class AccessService : IAccessService
    {
        private readonly IAccessRepository _accessRepository;

        public AccessService(IAccessRepository accessRepository)
        {
            _accessRepository = accessRepository;
        }

        public async Task<ResponseDTO<IEnumerable<GetAccessDTO>>> GetAllAsync()
        {
            var accesses = await _accessRepository.GetAllAsync();
            if (accesses is null || !accesses.Any())
                return new ResponseDTO<IEnumerable<GetAccessDTO>>
                {
                    Status_code= 404,
                    Data = null
                };

            var data = accesses.Select(a => new GetAccessDTO
            {
                Access_ID = a.Access_ID,
                Name = a.Name,
                Description = a.Description
            });

            return new ResponseDTO<IEnumerable<GetAccessDTO>>
            {
                Status_code = 200,
                Data = data
            };
        }

        public async Task<ResponseDTO<GetAccessDTO>> GetByIdAsync(int id)
        {
            var a = await _accessRepository.GetByIdAsync(id);
            if (a == null)
                return new ResponseDTO<GetAccessDTO>
                {
                    Status_code = 404,
                    Data = null
                };

            var data = new GetAccessDTO
            {
                Access_ID = a.Access_ID,
                Name = a.Name,
                Description = a.Description
            };

            return new ResponseDTO<GetAccessDTO>
            {
                Status_code = 200,
                Data = data
            };
        }

        public async Task<ResponseDTO<GetAccessDTO>> AddAsync(AddAccessDTO dto)
        {
            var access = new Access
            {
                Name = dto.Name,
                Description = dto.Description
            };

            await _accessRepository.AddAsync(access);

            var data = new GetAccessDTO
            {
                Access_ID = access.Access_ID,
                Name = access.Name,
                Description = access.Description
            };

            return new ResponseDTO<GetAccessDTO>
            {
                Status_code=200,
                Data = data
            };
        }

        public async Task<ResponseDTO<GetAccessDTO>> UpdateAsync(int id, AddAccessDTO dto)
        {
            var existing = await _accessRepository.GetByIdAsync(id);
            if (existing == null)
                return new ResponseDTO<GetAccessDTO>()
                {
                    Status_code = 404,
                    Data = null
                };

            existing.Name = dto.Name;
            existing.Description = dto.Description;

            await _accessRepository.UpdateAsync(existing);

            var data = new GetAccessDTO
            {
                Access_ID = existing.Access_ID,
                Name = existing.Name,
                Description = existing.Description
            };

            return new ResponseDTO<GetAccessDTO>()
            {
                Status_code = 200,
                Data = data
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