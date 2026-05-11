using Backend.Backend.DTOs;
using Backend.Backend.Interface.RepositoryInterface;
using Backend.Backend.Interface.ServiceInterface;
using Backend.Backend.Model;
using Backend.Backend.Repository;
using attStat = Backend.Backend.Helper.Enum.AttendanceEnum.AttStatus;

namespace Backend.Backend.Service
{
    public class AttendanceStudentService : IAttendanceStudentService
    {
        private readonly IAttendanceStudentRepository _attendancestudentRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IAttendanceRepository _attendanceRepository;

        public AttendanceStudentService(IAttendanceStudentRepository attendancestudentRepository, IStudentRepository studentRepository, IAttendanceRepository attendanceRepository)
        {
            _attendancestudentRepository = attendancestudentRepository;
            _studentRepository = studentRepository;
            _attendanceRepository=attendanceRepository;
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

            // Get Attendance
            var getAttendance = await _attendanceRepository.GetByIdAsync(dto.Attendance_Id);
            if (getAttendance is null)
                throw new Exception($"Attendance {dto.Attendance_Id} Does Not Exist");

            // Get schedule
            var getSchedule = getAttendance.Schedule;

            // get current time for validations
            TimeOnly validationTimeAttendanceStatus = TimeOnly.FromDateTime(DateTime.UtcNow);

            // Get time started
            TimeOnly started = getSchedule!.StartTime;

            // set late limitation
            TimeOnly lateChecker = validationTimeAttendanceStatus.AddMinutes(15);

            // set attendance status, initialize to absent
            attStat stat = attStat.Absent;

            // get day of the week
            DateTime thisday = DateTime.UtcNow;
            DayOfWeek dayOfThisWeek = thisday.DayOfWeek;

            // Validation and Status Assignment
            // Check if todays is the recorded day for schedule
            if (getSchedule.DayOfWeek != dayOfThisWeek)
                throw new Exception($"Course is Only available at {getSchedule.DayOfWeek} Not {dayOfThisWeek}");

            // Present
            if (validationTimeAttendanceStatus < started)
                stat = attStat.Present;

            if (validationTimeAttendanceStatus <=  lateChecker && validationTimeAttendanceStatus > started)
                stat = attStat.Late;

            if (validationTimeAttendanceStatus > lateChecker)
                stat = attStat.Absent;

            var attendancestudent = new AttendanceStudent
            {
                Student_Id = getStudent.Student_ID,
                StudentAttendance = stat,
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