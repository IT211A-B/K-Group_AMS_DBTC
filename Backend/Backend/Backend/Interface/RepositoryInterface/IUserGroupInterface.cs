using Backend.Backend.Model;

namespace Backend.Backend.Interface.RepositoryInterface
{
    public interface IUserGroupRepository
    {
        Task<IEnumerable<UserGroup>> GetAllAsync();   // Get all groups

        Task<UserGroup?> GetByIdAsync(int ID);        // Get single group

        Task AddAsync(UserGroup userGroup);           // Insert

        Task UpdateAsync(UserGroup userGroup);        // Update

        Task DeleteAsync(UserGroup userGroup);        // Delete
    }
}