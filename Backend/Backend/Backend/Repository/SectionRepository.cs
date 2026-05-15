using Backend.Backend.Interface.RepositoryInterface;
using Backend.Backend.Model;
using Microsoft.EntityFrameworkCore;

namespace Backend.Backend.Repository
{
    public class SectionRepository(DatabaseLibrary db) : ISectionRepository
    {
        private readonly DatabaseLibrary _db = db;

        public async Task<IEnumerable<Section>> GetAllAsync()
        {
            return await _db.Sections.Include(s => s.Course).ToListAsync();
        }

        public async Task<Section?> GetByIdAsync(int id)
        {
            return await _db.Sections.Include(s => s.Course).FirstOrDefaultAsync(s => s.Section_Id == id);
        }

        public async Task AddAsync(Section enrollment)
        {
            _db.Sections.Add(enrollment);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Section enrollment)
        {
            _db.Entry(enrollment).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Section enrollment)
        {
            _db.Sections.Remove(enrollment);
            await _db.SaveChangesAsync();
        }
    }
}