using Backend.Backend.DTOs;
using Backend.Backend.Interface.ServiceInterface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backend.Backend.Controllers
{
    /// <summary>
    /// Handles all operations related to AttendanceStudent management.
    /// </summary>
    [Route("AttendanceStudentManagement/[controller]")]
    [ApiController]
    public class AttendanceStudentController : ControllerBase
    {
        private readonly IAttendanceStudentService _attendanceService;

        public AttendanceStudentController(IAttendanceStudentService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        /// <summary>
        /// Retrieve all attendancestudent records
        /// </summary>
        /// <remarks>
        /// <para><b>Description:</b></para>
        /// <para>Fetches all attendancestudent records stored in the system.</para>
        ///
        /// <para><b>Inputs:</b></para>
        /// <list type="bullet">
        ///     <item><description>No input parameters required</description></item>
        /// </list>
        ///
        /// <para><b>Behavior:</b></para>
        /// <list type="bullet">
        ///     <item><description>Returns all attendancestudent records</description></item>
        ///     <item><description>Returns 404 if no records exist</description></item>
        /// </list>
        ///
        /// <para><b>Example:</b></para>
        /// <code>
        /// GET /AttendanceStudentManagement/AttendanceStudent
        /// </code>
        /// </remarks>
        /// <returns>List of attendancestudent records</returns>
        [Authorize(Roles = "Admin,Teacher,Student")]
        [HttpGet]
        public async Task<IActionResult> GetAllAttendanceStudents()
        {
            try { 
                var attendances = await _attendanceService.GetAllAsync();
                if (!(attendances?.Data?.Any() ?? false))
                    return NotFound("No Attendance Student Records Found");

                return Ok(attendances);
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }


        //[HttpGet("{id:int}")]
        //public async Task<IActionResult> GetAttendanceStudentById(int id)
        //{
        //    try {
        //        var attendancestudent = await _attendanceService.GetByIdAsync(id);
        //        if (attendancestudent == null)
        //            return NotFound($"#404! AttendanceStudent with ID {id} not found");

        //        return Ok(attendancestudent);
        //    }
        //    catch (Exception x)
        //    {
        //        // Internal Error
        //        return BadRequest($"An Error \"{x}\" Occured");
        //    }
        //}

        /// <summary>
        /// Create a new attendancestudent record
        /// </summary>
        /// <remarks>
        /// <para><b>Description:</b></para>
        /// <para>Creates a new attendancestudent entry for a student.</para>
        ///
        /// <para><b>Inputs:</b></para>
        /// <list type="bullet">
        ///     <item><description><b>Enrollment_ID</b> - Associated enrollment</description></item>
        ///     <item><description><b>Date</b> - AttendanceStudent date</description></item>
        ///     <item><description><b>Status</b> - AttendanceStudent status (Present, Absent, Late)</description></item>
        /// </list>
        ///
        /// <para><b>Behavior:</b></para>
        /// <list type="bullet">
        ///     <item><description>Creates a new attendancestudent record</description></item>
        /// </list>
        ///
        /// <para><b>Example:</b></para>
        /// <code>
        /// POST /AttendanceStudentManagement/AttendanceStudent
        /// {
        ///   "enrollment_ID": 1,
        ///   "date": "2026-03-21T00:00:00",
        ///   "status": "Present"
        /// }
        /// </code>
        /// </remarks>
        /// <returns>Created attendancestudent record</returns>
        [Authorize(Roles = "Admin,Teacher,Student")]
        [HttpPost]
        public async Task<IActionResult> AddAttendanceStudent()
        {
            try {
                string? uuid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(uuid))
                    throw new Exception("No Operator has been found");

                var attendancestudent = await _attendanceService.AddAsync(uuid);
                return Ok(attendancestudent);
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }


        //[HttpPut("{id:int}")]
        //public async Task<IActionResult> UpdateAttendanceStudent(int id, AddAttendanceStudentDTO dto)
        //{
        //    try { 
        //        var updatedAttendanceStudent = await _attendanceService.UpdateAsync(id, dto);
        //        if (updatedAttendanceStudent == null)
        //            return NotFound($"#404! AttendanceStudent with ID {id} not found");

        //        return Ok(updatedAttendanceStudent);
        //    }
        //    catch (Exception x)
        //    {
        //        // Internal Error
        //        return BadRequest($"An Error \"{x}\" Occured");
        //    }
        //}

        //[HttpDelete("{id:int}")]
        //public async Task<IActionResult> DeleteAttendanceStudent(int id)
        //{
        //    try { 
        //        var success = await _attendanceService.DeleteAsync(id);
        //        if (!success)
        //            return NotFound($"#404! AttendanceStudent with ID {id} not found");

        //        return Ok($"AttendanceStudent with ID {id} deleted successfully");
        //    }
        //    catch (Exception x)
        //    {
        //        // Internal Error
        //        return BadRequest($"An Error \"{x}\" Occured");
        //    }
        //}
    }
}