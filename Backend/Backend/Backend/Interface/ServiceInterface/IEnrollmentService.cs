using Backend.Backend.DTOs;

namespace Backend.Backend.Interface.ServiceInterface
{
    public interface IEnrollmentService
    {
        Task<IEnumerable<GetEnrollmentDTO>> GetAllAsync();
        Task<GetEnrollmentDTO?> GetByIdAsync(int id);
        Task<GetEnrollmentDTO> AddAsync(AddEnrollmentDTO dto);
        Task<GetEnrollmentDTO?> UpdateAsync(int id, AddEnrollmentDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}