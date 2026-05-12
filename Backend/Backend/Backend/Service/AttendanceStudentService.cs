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
        private readonly IUserRepository _userRepository;

        public AttendanceStudentService(IAttendanceStudentRepository attendancestudentRepository, IStudentRepository studentRepository, IAttendanceRepository attendanceRepository, IUserRepository userRepository)
        {
            _attendancestudentRepository = attendancestudentRepository;
            _studentRepository = studentRepository;
            _attendanceRepository=attendanceRepository;
            _userRepository = userRepository;
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

        public async Task<ResponseDTO<GetAttendanceStudentDTO>> AddAsync(string uuid)
        {
            // get student
            var getStudent = await _studentRepository.GetByUserUUIDAsync(uuid);
            if (getStudent is null)
                throw new Exception($"Student Does Not Exist");

            // get day of the week
            DateOnly thisday = DateOnly.FromDateTime(DateTime.Now);
            DayOfWeek dayOfThisWeek = thisday.DayOfWeek;

            // get current time for validations
            TimeOnly now = TimeOnly.FromDateTime(DateTime.Now); 

            var getAttendance = await _attendanceRepository.GetAttendanceIfExist(getStudent.Student_ID,thisday,now);
            if (getAttendance is null)
                throw new Exception($"Attendance Does Not Exist, Please wait for your teacher to comply");

            // Get schedule
            var getSchedule = getAttendance.Schedule;

            // Limit Time that can be tracked
            TimeOnly attendanceStarted = getSchedule.StartTime.AddMinutes(-30);

            // Get time started
            TimeOnly started = getSchedule.StartTime;

            // set late limitation
            TimeOnly lateChecker = started.AddMinutes(15);

            // set attendance status, initialize to absent
            attStat stat = attStat.Unassigned;

            // Validation and Status Assignment
            // Check if todays is the recorded day for schedule
            if (getSchedule.DayOfWeek != dayOfThisWeek)
                throw new Exception($"Course is Only available at {getSchedule.DayOfWeek} Not {dayOfThisWeek}");

            // Status
            if (now <= started && attendanceStarted >= now)
                stat = attStat.Present;
            else if (now <= lateChecker)
                stat = attStat.Late;
            else if (now > lateChecker)
                stat = attStat.Absent;
            else
                stat = attStat.Unassigned;

            var attendancestudent = new AttendanceStudent
            {
                Student_Id = getStudent.Student_ID,
                StudentAttendance = stat,
                Attendance_Id = getAttendance.Attendance_ID,
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