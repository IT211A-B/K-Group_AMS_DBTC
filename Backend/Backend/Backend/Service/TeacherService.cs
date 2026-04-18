using Backend.Backend.Model;
using Backend.Backend.DTOs;
using Backend.Backend.Interface.ServiceInterface;
using Backend.Backend.Interface.RepositoryInterface;

namespace Backend.Backend.Service
{
    public class TeacherService : ITeacherService
    {
        private readonly ITeacherRepository _teacherRepository;

        public TeacherService(ITeacherRepository teacherRepository)
        {
            _teacherRepository = teacherRepository;
        }

        public async Task<ResponseDTO<IEnumerable<GetTeacherDTO>>> GetAllAsync()
        {
            var teachers = await _teacherRepository.GetAllAsync();
            if (teachers is null || !teachers.Any())
                return new ResponseDTO<IEnumerable<GetTeacherDTO>>
                {
                    Status_code= 404,
                    Data = null
                };
            var data = teachers.Select(t => new GetTeacherDTO
            {
                User_ID = t.User_ID,
                DocumentSeries = t.DocumentSeries,
                DepartmentId = t.DepartmentId,
            });
            return new ResponseDTO<IEnumerable<GetTeacherDTO>>
            {
                Status_code = 200,
                Data = data
            };
        }

        public async Task<ResponseDTO<GetTeacherDTO>> GetByIdAsync(int id)
        {
            var t = await _teacherRepository.GetByIdAsync(id);
            if (t == null)
                return new ResponseDTO<GetTeacherDTO>
                {
                    Status_code = 404,
                    Data = null
                };

            var data = new GetTeacherDTO
            {
                User_ID = t.User_ID,
                DocumentSeries = t.DocumentSeries,
                DepartmentId = t.DepartmentId,
            };

            return new ResponseDTO<GetTeacherDTO>
            {
                Status_code = 200,
                Data = data
            };
        }

        public async Task<ResponseDTO<GetTeacherDTO>> AddAsync(AddTeacherDTO dto)
        {
            // Get Department
            var getDepartment = await _teacherRepository.GetDepartmentById(dto.DepartmentId);
            // Get Acronym
            string getAcronym = Helper.ThreeFirstLetterCapital.CapitalTheFirstThreeLetterInWord(getDepartment!.Name);
            // Get Year
            int Year = DateTime.Now.Year;
            // Get Id
            long getId = await _teacherRepository.GetNextTeacherNumberAsync();

            // Generate Document Series
            string docSer = $"{getAcronym}-{Year}-{getId}";

            var teacher = new Teacher
            {
                User_ID=dto.User_ID,
                DocumentSeries = docSer,
                DepartmentId = dto.DepartmentId,
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow,
            };

            await _teacherRepository.AddAsync(teacher);

           var data = new GetTeacherDTO
            {
                User_ID = teacher.User_ID,
                DocumentSeries = teacher.DocumentSeries,
                DepartmentId = teacher.DepartmentId,
            };

            return new ResponseDTO<GetTeacherDTO>
            {
                Status_code = 200,
                Data =data
            };
        }

        public async Task<ResponseDTO<GetTeacherDTO>> UpdateAsync(int id, AddTeacherDTO dto)
        {
            var existing = await _teacherRepository.GetByIdAsync(id);
            if (existing == null)
                return new ResponseDTO<GetTeacherDTO>
                {
                    Status_code = 404,
                    Data = null
                };

            existing.DepartmentId = dto.DepartmentId;
            existing.LastUpdatedAt = DateTime.UtcNow;
            await _teacherRepository.UpdateAsync(existing);

            var data = new GetTeacherDTO
            {
                User_ID = existing.User_ID,
                DepartmentId = existing.DepartmentId,
            };

            return new ResponseDTO<GetTeacherDTO>
            {
                Status_code = 200,
                Data =data
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _teacherRepository.GetByIdAsync(id);
            if (existing == null) return false;

            await _teacherRepository.DeleteAsync(existing);
            return true;
        }
    }
}