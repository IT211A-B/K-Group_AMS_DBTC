using Backend.Backend.Model;
using Backend.Backend.DTOs;
using Backend.Backend.Interface.ServiceInterface;
using Backend.Backend.Interface.RepositoryInterface;

namespace Backend.Backend.Service
{
    public class AttendanceStudentService : IAttendanceStudentService
    {
        private readonly IAttendanceStudentRepository _attendancestudentRepository;
        private readonly IStudentRepository _studentRepository;

        public AttendanceStudentService(IAttendanceStudentRepository attendancestudentRepository, IStudentRepository studentRepository)
        {
            _attendancestudentRepository = attendancestudentRepository;
            _studentRepository = studentRepository;
        }

        public async Task<ResponseDTO<IEnumerable<GetAttendanceStudentDTO>>> GetAllAsync()
        {
            var attendancestudents = await _attendancestudentRepository.GetAllAsync();
            if (attendancestudents is null || !attendancestudents.Any())
                return new ResponseDTO<IEnumerable<GetAttendanceStudentDTO>>
                {
                    Status_code= 404,
                    Data = null
                };
            var data = attendancestudents.Select(e => new GetAttendanceStudentDTO
            {
                StudentAttendanceStatus = e.StudentAttendance,
                StudentDocumentSeries = e.Student.DocumentSeries,
                Attendance_Id = e.Attendance_Id,

            });

            return new ResponseDTO<IEnumerable<GetAttendanceStudentDTO>>
            { Status_code = 200, Data = data };
        }

        //public async Task<ResponseDTO<GetAttendanceStudentDTO>> GetByIdAsync(int id)
        //{
        //    var e = await _attendancestudentRepository.GetByIdAsync(id);
        //    if (e == null)
        //        return new ResponseDTO<GetAttendanceStudentDTO>()
        //        {
        //            Status_code = 404,
        //            Data = null
        //        };

        //    var data = new GetAttendanceStudentDTO
        //    {
        //        AttendanceStudent_Code = e.AttendanceStudent_Code
        //    };

        //    return new ResponseDTO<GetAttendanceStudentDTO>
        //    {
        //        Status_code = 200,
        //        Data = data
        //    };
        //}

        public async Task<ResponseDTO<GetAttendanceStudentDTO>> AddAsync(AddAttendanceStudentDTO dto)
        {
            // get student
            var getStudent = await _studentRepository.GetByIdAsync(dto.Student_Id);
            if (getStudent is null)
                throw new Exception($"Id {dto.Student_Id} does not Exist");

            var attendancestudent = new AttendanceStudent
            {
                Student_Id = getStudent.Student_ID,
                StudentAttendance = dto.StudentAttendanceStatus,
                Attendance_Id = dto.Attendance_Id,
            };

            await _attendancestudentRepository.AddAsync(attendancestudent);

            var data = new GetAttendanceStudentDTO
            {
                StudentAttendanceStatus = attendancestudent.StudentAttendance,
                StudentDocumentSeries = attendancestudent.Student.DocumentSeries,
                Attendance_Id = attendancestudent.Attendance_Id,
            };

            return new ResponseDTO<GetAttendanceStudentDTO>
            {
                Status_code = 200,
                Data = data
            };
        }

        //public async Task<ResponseDTO<GetAttendanceStudentDTO>> UpdateAsync(int id, AddAttendanceStudentDTO dto)
        //{
        //    var existing = await _attendancestudentRepository.GetByIdAsync(id);
        //    if (existing == null) return new ResponseDTO<GetAttendanceStudentDTO>() { Status_code = 404, Data = null };

        //    existing.AttendanceStudent_Code = dto.AttendanceStudent_Code;

        //    await _attendancestudentRepository.UpdateAsync(existing);

        //    var data = new GetAttendanceStudentDTO
        //    {
        //        AttendanceStudent_Code = dto.AttendanceStudent_Code,
        //    };

        //    return new ResponseDTO<GetAttendanceStudentDTO>
        //    {
        //        Status_code = 200,
        //        Data = data
        //    };
        //}

        //public async Task<bool> DeleteAsync(int id)
        //{
        //    var existing = await _attendancestudentRepository.GetByIdAsync(id);
        //    if (existing == null) return false;

        //    await _attendancestudentRepository.DeleteAsync(existing);
        //    return true;
        //}
    }
}