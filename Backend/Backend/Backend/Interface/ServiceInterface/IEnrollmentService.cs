using Backend.Backend.DTOs;

namespace Backend.Backend.Interface.ServiceInterface
{
    public interface IEnrollmentService
    {
        Task<ResponseDTO<IEnumerable<GetEnrollmentDTO>>> GetAllAsync();
        Task<ResponseDTO<GetEnrollmentDTO>> GetByIdAsync(int id);
        Task<ResponseDTO<GetEnrollmentDTO>> AddAsync(AddEnrollmentDTO dto);
        Task<ResponseDTO<GetEnrollmentDTO>> UpdateAsync(int id, AddEnrollmentDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}