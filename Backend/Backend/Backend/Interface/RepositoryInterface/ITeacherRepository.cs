using Backend.Backend.Model;

namespace Backend.Backend.Interface.RepositoryInterface
{
    public interface ITeacherRepository
    {
        Task<IEnumerable<Teacher>> GetAllAsync();
        Task<Teacher?> GetByIdAsync(int ID);
        Task AddAsync(Teacher teacher);
        Task UpdateAsync(Teacher teacher);
        Task DeleteAsync(Teacher teacher);
        Task<Department?> GetDepartmentById(int ID);
        Task<long> GetNextTeacherNumberAsync();
    }
}