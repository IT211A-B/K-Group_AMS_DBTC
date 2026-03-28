using Attendance_Management_System.AttendanceManagementSystem.DTO;

namespace Attendance_Management_System.AttendanceManagementSystem.Interface.ServiceInterface
{
    public interface IUserService
    {
        Task<IEnumerable<GetUserDTO>> GetAllAsync();
        Task<GetUserDTO?> GetByIdAsync(int id);
        Task<GetUserDTO> AddAsync(AddUserDTO userDto);
        Task<LoginResult> LoginAsync(LoginUserDto login);
        //Task<GetUserDTO?> UpdateAsync(int id, AddUserDTO userDto);
        Task<bool> DeleteAsync(int id);
    }
}