using Backend.Backend.DTOs;

namespace Backend.Backend.Interface.ServiceInterface
{
    public interface ICourseService
    {
        Task<IEnumerable<GetCourseDTO>> GetAllAsync();
        Task<GetCourseDTO?> GetByIdAsync(int id);
        Task<GetCourseDTO> AddAsync(AddCourseDTO dto);
        Task<GetCourseDTO?> UpdateAsync(int id, AddCourseDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}