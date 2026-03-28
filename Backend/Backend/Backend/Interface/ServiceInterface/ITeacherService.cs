using Backend.Backend.DTOs;

namespace Backend.Backend.Interface.ServiceInterface
{
    public interface ITeacherService
    {
        Task<IEnumerable<GetTeacherDTO>> GetAllAsync();
        Task<GetTeacherDTO?> GetByIdAsync(int id);
        Task<GetTeacherDTO> AddAsync(AddTeacherDTO dto);
        Task<GetTeacherDTO?> UpdateAsync(int id, AddTeacherDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}