using Backend.Backend.Model;

namespace Backend.Backend.Interface.RepositoryInterface
{
    public interface IStudentRepository
    {
        Task<IEnumerable<Student>> GetAllAsync();
        Task<Student?> GetByIdAsync(int ID);
        Task AddAsync(Student student);
        Task UpdateAsync(Student student);
        Task DeleteAsync(Student student);
        Task<Program_?> GetProgramByIdAsync(int Id);
        Task<Student?> GetByUUIDAsync(string id);
        Task<long> GetNextStudentNumber();
        Task<bool> CheckUserIfTaken(string uId);
    }
}
