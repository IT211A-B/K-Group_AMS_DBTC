using Backend.Backend.DTOs;

namespace Backend.Backend.Interface.ServiceInterface
{
    public interface IDepartmentService
    {
        Task<ResponseDTO<IEnumerable<GetDepartmentDTO>>> GetAllAsync();
        Task<ResponseDTO<GetDepartmentDTO>> GetByIdAsync(int id);
        Task<ResponseDTO<GetDepartmentDTO>> AddAsync(AddDepartmentDTO dto);
        Task<ResponseDTO<GetDepartmentDTO>> UpdateAsync(int id, AddDepartmentDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}