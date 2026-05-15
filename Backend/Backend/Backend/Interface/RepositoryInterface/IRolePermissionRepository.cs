using Backend.Backend.Model;

namespace Backend.Backend.Interface.RepositoryInterface
{
    public interface IRolePermissionRepository
    {
        Task<IEnumerable<RolePermission>> GetAllAsync();
        Task<RolePermission?> GetByIdAsync(int ID);
        Task AddAsync(RolePermission rolepermission);
        Task UpdateAsync(RolePermission rolepermission);
        Task DeleteAsync(RolePermission rolepermission);
    }
}