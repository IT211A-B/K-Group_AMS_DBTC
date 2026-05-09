using Backend.Backend.DTOs;

namespace Backend.Backend.Interface.ServiceInterface
{
    public interface ICourseService
    {
        Task<ResponseDTO<IEnumerable<GetCourseDTO>>> GetAllAsync();
        Task<ResponseDTO<GetCourseDTO>> GetByIdAsync(int id);
        Task<ResponseDTO<GetCourseDTO>> AddAsync(AddCourseDTO dto);
        Task<ResponseDTO<GetCourseDTO>> UpdateAsync(int id, AddCourseDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}