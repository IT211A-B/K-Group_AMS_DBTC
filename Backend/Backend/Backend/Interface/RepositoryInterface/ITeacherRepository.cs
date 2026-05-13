using Backend.Backend.Model;

namespace Backend.Backend.Interface.RepositoryInterface
{
    public interface ITeacherRepository
    {
        Task<IEnumerable<Teacher>> GetAllAsync();
        Task<Teacher?> GetByIdAsync(int ID);
        Task AddAsync(Teacher teacher);
        Task<Teacher?> GetByUUIDAsync(string id);
        Task<Teacher?> GetByQrToken(string qrToken);
        Task UpdateAsync(Teacher teacher);
        Task DeleteAsync(Teacher teacher);
        Task<Department?> GetDepartmentById(int ID);
        Task<long> GetNextTeacherNumberAsync();
    }
}