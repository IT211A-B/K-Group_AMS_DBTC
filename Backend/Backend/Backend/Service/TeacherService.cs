using Backend.Backend.Model;
using Backend.Backend.DTOs;
using Backend.Backend.Interface.ServiceInterface;
using Backend.Backend.Interface.RepositoryInterface;

namespace Backend.Backend.Service
{
    public class TeacherService : ITeacherService
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly IUserRepository _userRepository;

        public TeacherService(ITeacherRepository teacherRepository, IUserRepository userRepository)
        {
            _teacherRepository = teacherRepository;
            _userRepository = userRepository;
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
                UserDocumentSeries = t.User.DocumentSeries,
                DocumentSeries = t.DocumentSeries,
                DepartmentId = t.DepartmentId,
                CreatedAt = t.CreatedAt,
                CreatedBy = t.CreatedBy,
                LastUpdatedAt = t.LastUpdatedAt,
                LastUpdatedBy = t.LastUpdatedBy,
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
                UserDocumentSeries = t.User.DocumentSeries,
                DocumentSeries = t.DocumentSeries,
                DepartmentId = t.DepartmentId,
                CreatedAt = t.CreatedAt,
                CreatedBy = t.CreatedBy,
                LastUpdatedAt = t.LastUpdatedAt,
                LastUpdatedBy = t.LastUpdatedBy,
            };

            return new ResponseDTO<GetTeacherDTO>
            {
                Status_code = 200,
                Data = data
            };
        }

        public async Task<ResponseDTO<GetTeacherDTO>> AddAsync(AddTeacherDTO dto, string uuid)
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

            //Get Operator
            var getOperator = await _userRepository.GetByUUIDAsync(uuid);

            // Get User
            var getUser = await _userRepository.GetByIdAsync(dto.User_ID);
            if (getUser == null)
                throw new Exception($"User No.{dto.User_ID} Does Not Exist");

            var teacher = new Teacher
            {
                User_ID = getUser.Id,
                DocumentSeries = docSer,
                DepartmentId = dto.DepartmentId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = getOperator?.Full_Name ?? "Admin",
                LastUpdatedAt = DateTime.UtcNow,
                LastUpdatedBy = getOperator?.Full_Name ?? "Admin",
            };

            await _teacherRepository.AddAsync(teacher);

           var data = new GetTeacherDTO
            {
               UserDocumentSeries = teacher.User.DocumentSeries,
               DocumentSeries = teacher.DocumentSeries,
               DepartmentId = teacher.DepartmentId,
               CreatedAt = teacher.CreatedAt,
               CreatedBy = teacher.CreatedBy,
               LastUpdatedBy = teacher.LastUpdatedBy,
               LastUpdatedAt = teacher.LastUpdatedAt,
            };

            return new ResponseDTO<GetTeacherDTO>
            {
                Status_code = 200,
                Data =data
            };
        }

        public async Task<ResponseDTO<GetTeacherDTO>> UpdateAsync(int id, AddTeacherDTO dto, string uuid)
        {
            var existing = await _teacherRepository.GetByIdAsync(id);
            if (existing == null)
                return new ResponseDTO<GetTeacherDTO>
                {
                    Status_code = 404,
                    Data = null
                };

            //Get Operator
            var getOperator = await _userRepository.GetByUUIDAsync(uuid);

            // Get User
            var getUser = await _userRepository.GetByIdAsync(dto.User_ID);
            if (getUser == null)
                throw new Exception($"User Id {dto.User_ID} Does not Exist");

            existing.DepartmentId = dto.DepartmentId;
            existing.LastUpdatedAt = DateTime.UtcNow;
            existing.LastUpdatedBy = getOperator?.Full_Name ?? "Admin";

            await _teacherRepository.UpdateAsync(existing);

            var data = new GetTeacherDTO
            {
                UserDocumentSeries = existing.User.DocumentSeries,
                DocumentSeries = existing.DocumentSeries,
                DepartmentId = existing.DepartmentId,
                CreatedAt = existing.CreatedAt,
                CreatedBy = existing.CreatedBy,
                LastUpdatedBy = existing.LastUpdatedBy,
                LastUpdatedAt = existing.LastUpdatedAt,

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