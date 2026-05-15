using Backend.Backend.DTOs;
using Backend.Backend.Interface.RepositoryInterface;
using Backend.Backend.Interface.ServiceInterface;
using Backend.Backend.Model;
using Backend.Backend.Helper;

namespace Backend.Backend.Service
{
    public class ScheduleService : IScheduleService
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IStudentRepository _studentRepository;

        public ScheduleService(IScheduleRepository scheduleRepository, ICourseRepository courseRepository, IStudentRepository studentRepository)
        {
            _scheduleRepository = scheduleRepository;
            _courseRepository = courseRepository;
            _studentRepository = studentRepository;
        }

        public async Task<ResponseDTO<IEnumerable<GetScheduleDTO>>> GetAllAsync()
        {
            var schedule = await _scheduleRepository.GetAllAsync();
            if (schedule is null || !schedule.Any()) 
                return new ResponseDTO<IEnumerable<GetScheduleDTO>>
                {
                    Status_code = 404,
                    Data = Enumerable.Empty<GetScheduleDTO>()
                };
            var data = schedule.Select(s => new GetScheduleDTO
            {
                Schedule_Id = s.Schedule_Id,
                Section_ID = s.Section_ID,
                Course_ID  = s.Course_ID,
                DayOfWeek  = s.DayOfWeek,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                AcademicYear = s.AcademicYear,
                Semester = s.Semester,
            });

            return new ResponseDTO<IEnumerable<GetScheduleDTO>>
            {
                Status_code = 200,
                Data = data
            };
        }

        public async Task<ResponseDTO<GetScheduleDTO>> GetByIdAsync(int id)
        {
            var schedule = await _scheduleRepository.GetByIdAsync(id);
            if (schedule is null)
                return new ResponseDTO<GetScheduleDTO>
                {
                    Status_code = 200,
                    Data = null
                };

            var data = new GetScheduleDTO
            {
                Schedule_Id = schedule.Schedule_Id,
                Section_ID = schedule.Section_ID,
                Course_ID  = schedule.Course_ID,
                DayOfWeek  = schedule.DayOfWeek,
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime,
                AcademicYear = schedule.AcademicYear,
                Semester = schedule.Semester,
            };

            return new ResponseDTO<GetScheduleDTO>
            {
                Status_code = 200,
                Data = data
            };
        }

        public async Task<ResponseDTO<IEnumerable<GetStudentSchedule>>> GetCurrentStudentAttendance(string uuid, string dayOfWeek)
        {
            var getStudent = await _studentRepository.GetByUserUUIDAsync(uuid);
            Console.WriteLine("1");
            if (getStudent is null)
                return new ResponseDTO<IEnumerable<GetStudentSchedule>>
                {
                    Data = null,
                    Detail = $"No Student Found",
                    Status_code = 404,
                };

            DayOfWeek dayofweek_ = StringToDayOfWeek.ConvertFromStringToDayOfWeek(dayOfWeek);
            Console.WriteLine("2");

            var getSched = await _scheduleRepository.GetStudentSchedulesAsync(getStudent.Student_ID, dayofweek_);
            Console.WriteLine("3");
            if (getSched is null)
                return new ResponseDTO<IEnumerable<GetStudentSchedule>>
                {
                    Data = null,
                    Detail = $"No Schedule of Student {getStudent.DocumentSeries} Has Been Found",
                    Status_code = 404,
                };
            Console.WriteLine("4");


            var data = getSched.Select(s => new GetStudentSchedule
            {
                Title = s.Title,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
            });
            Console.WriteLine("5");
            Console.WriteLine(data);

            return new ResponseDTO<IEnumerable<GetStudentSchedule>>
            {
                Status_code=200,
                Data = data,
            };
        }

        public async Task<ResponseDTO<GetScheduleDTO>> AddAsync(AddScheduleDTO schedule)
        {
            // check course row's existence shesh
            var course = await _courseRepository.GetByIdAsync(schedule.Course_ID);
            if (course is null)
                return new ResponseDTO<GetScheduleDTO>
                {
                    Status_code = 404,
                    Data = null
                };

            if (await _scheduleRepository.HasConflictingScheduleAsync(schedule.Course_ID, schedule.AcademicYear, schedule.StartTime, schedule.EndTime, schedule.Section_ID))
                throw new Exception("Course is already Taken");

            var sched = new Schedule
            {
                Section_ID = schedule.Section_ID,
                Course_ID  = schedule.Course_ID,
                DayOfWeek  = schedule.DayOfWeek,
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime,
                AcademicYear = schedule.AcademicYear,
                Semester = schedule.Semester,
            };


            await _scheduleRepository.AddAsync(sched);

            var data = new GetScheduleDTO
            {
                Schedule_Id = sched.Schedule_Id,
                Section_ID = sched.Section_ID,
                Course_ID  = sched.Course_ID,
                DayOfWeek  = sched.DayOfWeek,
                StartTime = sched.StartTime,
                EndTime = sched.EndTime,
                AcademicYear = sched.AcademicYear,
                Semester = sched.Semester,
            };

            return new ResponseDTO<GetScheduleDTO>
            {
                Status_code = 200,
                Data = data
            };
        }

        public async Task<ResponseDTO<GetScheduleDTO>> UpdateAsync(int id, AddScheduleDTO schedule)
        {
            var existing = await _scheduleRepository.GetByIdAsync(id);
            if (existing == null)
                return new ResponseDTO<GetScheduleDTO>
                {
                    Status_code = 404,
                    Data = null
                };

            existing.Course_ID = schedule.Course_ID;
            existing.Section_ID = schedule.Section_ID;
            existing.AcademicYear = schedule.AcademicYear;
            existing.Semester = schedule.Semester;
            existing.DayOfWeek = schedule.DayOfWeek;
            existing.StartTime = schedule.StartTime;
            existing.EndTime = schedule.EndTime;

            await _scheduleRepository.UpdateAsync(existing);

            var data = new GetScheduleDTO
            {
                Course_ID = existing.Course_ID,
                Section_ID = existing.Section_ID,
                AcademicYear = existing.AcademicYear,
                Semester = existing.Semester,
                DayOfWeek = existing.DayOfWeek,
                StartTime = existing.StartTime,
                EndTime = existing.EndTime,
            };
            return new ResponseDTO<GetScheduleDTO>
            {
                Status_code = 200,
                Data = data
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _scheduleRepository.GetByIdAsync(id);
            if (existing == null) return false;

            await _scheduleRepository.DeleteAsync(existing);
            return true;
        }
    }
}