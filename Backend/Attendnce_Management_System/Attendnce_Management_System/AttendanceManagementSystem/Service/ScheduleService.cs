using Attendance_Management_System.AttendanceManagementSystem.Model;
using Attendance_Management_System.AttendanceManagementSystem.Interface.ServiceInterface;
using Attendance_Management_System.AttendanceManagementSystem.Interface.RepositoryInterface;

namespace Attendance_Management_System.AttendanceManagementSystem.Service
{
    public class ScheduleService : IScheduleService
    {
        private readonly IScheduleRepository _scheduleRepository;

        public ScheduleService(IScheduleRepository scheduleRepository)
        {
            _scheduleRepository = scheduleRepository;
        }

        public async Task<IEnumerable<Schedule>> GetAllAsync()
        {
            return await _scheduleRepository.GetAllAsync();
        }

        public async Task<Schedule?> GetByIdAsync(int id)
        {
            return await _scheduleRepository.GetByIdAsync(id);
        }

        public async Task<Schedule> AddAsync(Schedule schedule)
        {
            await _scheduleRepository.AddAsync(schedule);
            return schedule;
        }

        public async Task<Schedule?> UpdateAsync(int id, Schedule schedule)
        {
            var existing = await _scheduleRepository.GetByIdAsync(id);
            if (existing == null) return null;

            existing.Course_ID = schedule.Course_ID;
            existing.Course_Year = schedule.Course_Year;
            existing.Department_ID = schedule.Department_ID;
            existing.Program_ID = schedule.Program_ID;
            existing.DayOfWeek = schedule.DayOfWeek;
            existing.StartTime = schedule.StartTime;
            existing.EndTime = schedule.EndTime;

            await _scheduleRepository.UpdateAsync(existing);
            return existing;
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