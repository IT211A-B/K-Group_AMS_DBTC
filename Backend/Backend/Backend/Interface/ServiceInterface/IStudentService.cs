using Backend.Backend.DTOs;

namespace Backend.Backend.Interface.ServiceInterface
{
    public interface IStudentService
    {
        Task<IEnumerable<GetStudentDTO>> GetAllAsync();
        Task<GetStudentDTO?> GetByIdAsync(int id);
        Task<GetStudentDTO> AddAsync(AddStudentDTO dto);
        Task<GetStudentDTO?> UpdateAsync(int id, AddStudentDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}