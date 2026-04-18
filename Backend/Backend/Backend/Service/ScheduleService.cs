using Backend.Backend.DTOs;
using Backend.Backend.Interface.RepositoryInterface;
using Backend.Backend.Interface.ServiceInterface;
using Backend.Backend.Model;

namespace Backend.Backend.Service
{
    public class ScheduleService : IScheduleService
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly ICourseRepository _courseRepository;

        public ScheduleService(IScheduleRepository scheduleRepository, ICourseRepository courseRepository)
        {
            _scheduleRepository = scheduleRepository;
            _courseRepository = courseRepository;
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
                Course_ID  = s.Course_ID,
                Course_Year = s.Course_Year,
                Department_ID = s.Department_ID,
                Program_ID = s.Program_ID,
                DayOfWeek  = s.DayOfWeek,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
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
                Course_ID  = schedule.Course_ID,
                Course_Year = schedule.Course_Year,
                Department_ID = schedule.Department_ID,
                Program_ID = schedule.Program_ID,
                DayOfWeek  = schedule.DayOfWeek,
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime,
            };

            return new ResponseDTO<GetScheduleDTO>
            {
                Status_code = 200,
                Data = data
            };
        }

        public async Task<ResponseDTO<GetScheduleDTO>> AddAsync(AddScheduleDTO schedule)
        {
            // check course row's existence shesh
            var course = _courseRepository.GetByIdAsync(schedule.Course_ID);
            if (course is null)
                return new ResponseDTO<GetScheduleDTO>
                {
                    Status_code = 404,
                    Data = null
                };

            var sched = new Schedule
            {
                Course_ID = schedule.Course_ID,
                Course_Year= schedule.Course_Year,
                Department_ID = schedule.Department_ID,
                Program_ID = schedule.Program_ID,
                DayOfWeek = schedule.DayOfWeek,
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime,
            };


            await _scheduleRepository.AddAsync(sched);

            var data = new GetScheduleDTO
            {
                Schedule_Id = sched.Schedule_Id,
                Course_ID = sched.Course_ID,
                Course_Year = sched.Course_Year,
                Department_ID = sched.Department_ID,
                Program_ID = sched.Program_ID,
                DayOfWeek = sched.DayOfWeek,
                StartTime = sched.StartTime,
                EndTime = sched.EndTime,
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
            existing.Course_Year = schedule.Course_Year;
            existing.Department_ID = schedule.Department_ID;
            existing.Program_ID = schedule.Program_ID;
            existing.DayOfWeek = schedule.DayOfWeek;
            existing.StartTime = schedule.StartTime;
            existing.EndTime = schedule.EndTime;

            await _scheduleRepository.UpdateAsync(existing);

            var data = new GetScheduleDTO
            {
                Course_ID = existing.Course_ID,
                Course_Year = existing.Course_Year,
                Department_ID = existing.Department_ID,
                Program_ID = existing.Program_ID,
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