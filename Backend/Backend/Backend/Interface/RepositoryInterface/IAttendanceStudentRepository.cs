using Backend.Backend.Model;

namespace Backend.Backend.Interface.RepositoryInterface
{
    public interface IAttendanceStudentRepository
    {
        Task<IEnumerable<AttendanceStudent>> GetAllAsync();
        Task AddAsync(AttendanceStudent schedule);
        Task UpdateAsync(AttendanceStudent schedule);
        Task DeleteAsync(AttendanceStudent schedule);
    }
}