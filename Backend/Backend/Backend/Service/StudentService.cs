using Backend.Backend.Model;
using Backend.Backend.DTOs;
using Backend.Backend.Interface.ServiceInterface;
using Backend.Backend.Interface.RepositoryInterface;
using System.ComponentModel;

namespace Backend.Backend.Service
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IUserRepository _userRepository;

        public StudentService(IStudentRepository studentRepository, IUserRepository userRepository)
        {
            _studentRepository = studentRepository;
            _userRepository = userRepository;
        }

        public async Task<ResponseDTO<IEnumerable<GetStudentDTO>>> GetAllAsync()
        {
            var students = await _studentRepository.GetAllAsync();
            if (!students.Any() || !students.Any())
                return new ResponseDTO<IEnumerable<GetStudentDTO>>
                {
                    Status_code = 404,
                    Data = Enumerable.Empty<GetStudentDTO>()
                };

            var data = students.Select(s => new GetStudentDTO
            {
                UserDocumentSeries = s.User.DocumentSeries,
                SectionID = s.SectionID,
                DocumentSeries = s.DocumentSeries,
                Program_ID = s.Program_ID,
                Department_ID = s.Department_ID,
                Year_Level = s.Year_Level,
                CreatedAt = s.CreatedAt,
                CreatedBy = s.CreatedBy,
                LastUpdatedAt = s.LastUpdatedAt,
                LastUpdatedBy = s.LastUpdatedBy,
            });

            return new ResponseDTO<IEnumerable<GetStudentDTO>>
            {
                Status_code = 200,
                Data = data
            };
        }

        public async Task<ResponseDTO<GetStudentDTO>> GetByIdAsync(int id)
        {
            var s = await _studentRepository.GetByIdAsync(id);
            if (s == null)
                return new ResponseDTO<GetStudentDTO>
                {
                    Status_code = 404,
                    Data = null
                };

            var data = new GetStudentDTO
            {
                UserDocumentSeries = s.User.DocumentSeries,
                SectionID = s.SectionID,
                DocumentSeries = s.DocumentSeries,
                Program_ID = s.Program_ID,
                Department_ID = s.Department_ID,
                Year_Level = s.Year_Level,
                CreatedAt = s.CreatedAt,
                CreatedBy = s.CreatedBy,
                LastUpdatedAt = s.LastUpdatedAt,
                LastUpdatedBy = s.LastUpdatedBy,
            };

            return new ResponseDTO<GetStudentDTO>
            {
                Status_code = 200,
                Data = data
            };
        }

        public async Task<ResponseDTO<GetStudentDTO>> AddAsync(AddStudentDTO dto, string uuid)
        {

            // Get User Doc Series not UUID
            var getUser = await _userRepository.GetByIdAsync(dto.User_ID);
            if (getUser is null)
                throw new Exception($"Id {dto.User_ID} Does not exist");

            // Get Program
            var get_program = await _studentRepository.GetProgramByIdAsync(dto.Program_ID);

            // Check if User is Taken
            if (await _studentRepository.CheckUserIfTaken(getUser.Id))
                return new ResponseDTO<GetStudentDTO>
                { 
                    Status_code = 409,
                    Data = null
                };

            // Get Student Program's Ackronym
            string getStudentProgram = Helper.GetAcronym.GetAllCapitalLettersPerWord(get_program!.Name);
            // check if the there are program or THE PROGRAM NAME IS NOT CAPITAL 
            if (getStudentProgram is null)
            {
                return new ResponseDTO<GetStudentDTO>
                {
                    Status_code = 503,
                    Data = null
                };
            }
            // Get Year
            int getYear = DateTime.Now.Year;
            // Get Student Id
            long getId = await _studentRepository.GetNextStudentNumber();
            // Generate Document Series
            string DocSer = $"{getStudentProgram}-{getYear}-{getId}";

            //Get Operator
            var getOperator = await _userRepository.GetByUUIDAsync(uuid);

            var student = new Student
            {
                SectionID = dto.SectionID,
                User_ID = getUser.Id,
                DocumentSeries = DocSer,
                Program_ID = dto.Program_ID,
                Department_ID = dto.Department_ID,
                Year_Level = dto.Year_Level,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = getOperator?.Full_Name ?? "Admin",
                LastUpdatedAt = DateTime.UtcNow,
                LastUpdatedBy = getOperator?.Full_Name ?? "Admin",
            };

            await _studentRepository.AddAsync(student);

            var data = new GetStudentDTO
            {
                UserDocumentSeries = student.User.DocumentSeries,
                SectionID = student.SectionID,
                DocumentSeries = student.DocumentSeries,
                Program_ID = student.Program_ID,
                Department_ID = student.Department_ID,
                Year_Level = student.Year_Level,
                CreatedAt= student.CreatedAt,
                CreatedBy = student.CreatedBy,
                LastUpdatedAt = student.LastUpdatedAt,
                LastUpdatedBy = student.LastUpdatedBy,
            };

            return new ResponseDTO<GetStudentDTO>
            {
                Status_code = 200,
                Data = data
            };
        }

        public async Task<ResponseDTO<GetStudentDTO>> UpdateAsync(int id, AddStudentDTO dto, string uuid)
        {
            var existing = await _studentRepository.GetByIdAsync(id);
            if (existing == null)
                return new ResponseDTO<GetStudentDTO>
                {
                    Status_code = 404,
                    Data = null
                };

            // Get Operator
            var userOperator = await _userRepository.GetByUUIDAsync(uuid);

            // Get User
            var userStudent = await _userRepository.GetByIdAsync(dto.User_ID);
            if (userStudent == null)
                throw new Exception($"Id {dto.User_ID} does not Exist");

            existing.Program_ID = dto.Program_ID;
            existing.User_ID = userStudent.DocumentSeries;
            existing.Department_ID = dto.Department_ID;
            existing.Year_Level = dto.Year_Level;
            existing.LastUpdatedAt = DateTime.UtcNow;
            existing.LastUpdatedBy = userOperator?.Full_Name ?? "Admin";

            await _studentRepository.UpdateAsync(existing);

            var data = new GetStudentDTO
            {
                UserDocumentSeries = existing.User.DocumentSeries,
                SectionID = existing.SectionID,
                DocumentSeries = existing.DocumentSeries,
                Program_ID = existing.Program_ID,
                Department_ID = existing.Department_ID,
                Year_Level = existing.Year_Level,
                LastUpdatedAt = existing.LastUpdatedAt,
                LastUpdatedBy = existing.LastUpdatedBy,
            };

            return new ResponseDTO<GetStudentDTO>
            {
                Status_code = 200,
                Data= data
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _studentRepository.GetByIdAsync(id);
            if (existing == null) return false;

            await _studentRepository.DeleteAsync(existing);
            return true;
        }
    }
}