using Backend.Backend.DTOs;
using Backend.Backend.Helper;
using Backend.Backend.Interface.RepositoryInterface;
using Backend.Backend.Interface.ServiceInterface;
using Backend.Backend.Model;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System.ComponentModel;
using posStat = Backend.Backend.Helper.Enum.PosEnum.PosStatus;

using static Backend.Backend.Helper.Enum.PosEnum;

namespace Backend.Backend.Service
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IQrService _qrService;

        public StudentService(IStudentRepository studentRepository, IUserRepository userRepository, IQrService qrIService)
        {
            _studentRepository = studentRepository;
            _userRepository = userRepository;
            _qrService = qrIService;
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

        public async Task<ResponseDTO<GetStudentDTO>> GetByCurrentStudentAsync(string uuid)
        {
            var s = await _studentRepository.GetByUserUUIDAsync(uuid);
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

        public async Task<ResponseDTO<IEnumerable<GetStudentItsCourse>>> GetAllStudentCourse(string uuid)
        {
            var getStudentID = await _studentRepository.GetByUserUUIDAsync(uuid);
            if (getStudentID is null)
                return new ResponseDTO<IEnumerable<GetStudentItsCourse>>
                {
                    Status_code = 404,
                    Data = null,
                    Detail = $"No Operator Has Found"
                };

            var students = await _studentRepository.GetStudentCoursesAsync(getStudentID.Student_ID);
            if (!students.Any() || !students.Any())
                return new ResponseDTO<IEnumerable<GetStudentItsCourse>>
                {
                    Status_code = 404,
                    Data = Enumerable.Empty<GetStudentItsCourse>(),
                    Detail = $"No Student Has Found"
                };

            var data = students.Select(s => new GetStudentItsCourse
            {
                Course_ID = s.Course_ID,
                Title = s.Title,
                Code = s.Code,
                Full_Name = s.Full_Name
            });

            return new ResponseDTO<IEnumerable<GetStudentItsCourse>>
            {
                Status_code = 200,
                Data = data
            };
        }

        public async Task<byte[]?> getQrById(int id)
        {
            var student = await _studentRepository.GetByIdAsync(id);
            if (student == null)
                return null;

            // content inside QR
            var qrContent = student.QrToken;

            var qrBytes = _qrService.GenerateQr(qrContent);

            return qrBytes;
        }

        public async Task<byte[]?> getQrByCurrentStudent(string uuid)
        {
            var student = await _studentRepository.GetByUserUUIDAsync(uuid);
            if (student == null)
                return null;

            // content inside QR
            var qrContent = student.QrToken;

            var qrBytes = _qrService.GenerateQr(qrContent);

            return qrBytes;
        }

        public async Task<ResponseDTO<IEnumerable<GetRecordAttendanceOfCertainStudentServiceDTO>>> GetRecordAttendanceOfOneStudent(string uuid)
        {
            var getStudent = await _studentRepository.GetByUserUUIDAsync(uuid);
            if (getStudent == null)
                return new ResponseDTO<IEnumerable<GetRecordAttendanceOfCertainStudentServiceDTO>>
                {
                    Status_code = 404,
                    Data = null,
                    Detail = $"No Student Has Been Found"
                };

            var returnData = await _studentRepository.GetStudentAttendanceAsync(getStudent.Student_ID);
            if (returnData == null)
                return new ResponseDTO<IEnumerable<GetRecordAttendanceOfCertainStudentServiceDTO>>
                {
                    Status_code = 404,
                    Data = null,
                    Detail = $"No Student Has Been Found"
                };

            var data = returnData.Select(s => new GetRecordAttendanceOfCertainStudentServiceDTO
            {
                Attendance_ID = s.Attendance_ID,
                Course_Title = s.Course_Title,
                Course_Code = s.Course_Code,
                Date = s.Date,
                DayOfWeek = s.DayOfWeek,
                AttendanceStatus = AttendanceEnumToRole.ConvertEnumAttendancetoStringAttendance(s.AttendanceStatus),
            });

            return new ResponseDTO<IEnumerable<GetRecordAttendanceOfCertainStudentServiceDTO>>
            {
                Data = data,
                Status_code =200,
                Detail = ""
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

            var position = ExtractDocuSer.ExtractDataFromDocumentSeries(getUser.DocumentSeries);
            if (position.ExtractedPosition != posStat.TEA)
                return new ResponseDTO<GetStudentDTO>
                {
                    Status_code = 404,
                    Data =null,
                    Detail = $"User Is Not a Student"
                };

            var student = new Student
            {
                SectionID = dto.SectionID,
                User_ID = getUser.Id,
                DocumentSeries = DocSer,
                QrToken = _qrService.GenerateToken(),
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
            existing.LastUpdatedAt = TimeHelper.Now();
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