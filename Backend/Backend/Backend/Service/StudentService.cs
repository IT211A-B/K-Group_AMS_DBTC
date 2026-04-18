using Backend.Backend.Model;
using Backend.Backend.DTOs;
using Backend.Backend.Interface.ServiceInterface;
using Backend.Backend.Interface.RepositoryInterface;

namespace Backend.Backend.Service
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;

        public StudentService(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
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
                User_ID = s.User_ID,
                DocumentSeries = s.DocumentSeries,
                Program_ID = s.Program_ID,
                Department_ID = s.Department_ID,
                Year_Level = s.Year_Level,
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
                User_ID = s.User_ID,
                DocumentSeries = s.DocumentSeries,
                Program_ID = s.Program_ID,
                Department_ID = s.Department_ID,
                Year_Level = s.Year_Level,
            };

            return new ResponseDTO<GetStudentDTO>
            {
                Status_code = 200,
                Data = data
            };
        }

        public async Task<ResponseDTO<GetStudentDTO>> AddAsync(AddStudentDTO dto)
        {
            // Get Program
            var get_program = await _studentRepository.GetProgramByIdAsync(dto.Program_ID);

            // Check if User is Taken
            if (await _studentRepository.CheckUserIfTaken(dto.User_ID))
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

            var student = new Student
            {
                User_ID = dto.User_ID,
                DocumentSeries = DocSer,
                Program_ID = dto.Program_ID,
                Department_ID = dto.Department_ID,
                Year_Level = dto.Year_Level,
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow,
            };

            await _studentRepository.AddAsync(student);

            var data = new GetStudentDTO
            {
                User_ID = student.User_ID,
                DocumentSeries = student.DocumentSeries,
                Program_ID = student.Program_ID,
                Department_ID = student.Department_ID,
                Year_Level = student.Year_Level,
            };

            return new ResponseDTO<GetStudentDTO>
            {
                Status_code = 200,
                Data = data
            };
        }

        public async Task<ResponseDTO<GetStudentDTO>> UpdateAsync(int id, AddStudentDTO dto)
        {
            var existing = await _studentRepository.GetByIdAsync(id);
            if (existing == null)
                return new ResponseDTO<GetStudentDTO>
                {
                    Status_code = 404,
                    Data = null
                }
                ;
            existing.Program_ID = dto.Program_ID;
            existing.Department_ID = dto.Department_ID;
            existing.Year_Level = dto.Year_Level;
            existing.LastUpdatedAt = DateTime.UtcNow;

            await _studentRepository.UpdateAsync(existing);

            var data = new GetStudentDTO
            {
                User_ID = existing.User_ID,
                DocumentSeries = existing.DocumentSeries,
                Program_ID = existing.Program_ID,
                Department_ID = existing.Department_ID,
                Year_Level = existing.Year_Level,
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