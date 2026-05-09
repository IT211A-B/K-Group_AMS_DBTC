using Backend.Backend.DTOs;
using Backend.Backend.Interface.ServiceInterface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backend.Backend.Controllers
{
    /// <summary>
    /// Handles all operations related to Attendance management.
    /// </summary>
    [Authorize(Roles = "Admin,Teacher")]
    [Route("AttendanceManagement/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;

        public AttendanceController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        /// <summary>
        /// Retrieve all attendance records
        /// </summary>
        /// <remarks>
        /// <para><b>Description:</b></para>
        /// <para>Fetches all attendance records stored in the system.</para>
        ///
        /// <para><b>Inputs:</b></para>
        /// <list type="bullet">
        ///     <item><description>No input parameters required</description></item>
        /// </list>
        ///
        /// <para><b>Behavior:</b></para>
        /// <list type="bullet">
        ///     <item><description>Returns all attendance records</description></item>
        ///     <item><description>Returns 404 if no records exist</description></item>
        /// </list>
        ///
        /// <para><b>Example:</b></para>
        /// <code>
        /// GET /AttendanceManagement/Attendance
        /// </code>
        /// </remarks>
        /// <returns>List of attendance records</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllAttendances()
        {
            try { 
                var attendances = await _attendanceService.GetAllAsync();
                if (!(attendances?.Data?.Any() ?? false))
                    return NotFound("No Attendance Records Found");

                return Ok(attendances);
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }

        /// <summary>
        /// Retrieve an attendance record by ID
        /// </summary>
        /// <remarks>
        /// <para><b>Description:</b></para>
        /// <para>Fetches a specific attendance record using its unique ID.</para>
        ///
        /// <para><b>Inputs:</b></para>
        /// <list type="bullet">
        ///     <item><description><b>id</b> - Attendance record ID</description></item>
        /// </list>
        ///
        /// <para><b>Behavior:</b></para>
        /// <list type="bullet">
        ///     <item><description>Returns the record if found</description></item>
        ///     <item><description>Returns 404 if the record does not exist</description></item>
        /// </list>
        ///
        /// <para><b>Example:</b></para>
        /// <code>
        /// GET /AttendanceManagement/Attendance/1
        /// </code>
        /// </remarks>
        /// <param name="id">Attendance ID</param>
        /// <returns>Attendance record</returns>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAttendanceById(int id)
        {
            try {
                var attendance = await _attendanceService.GetByIdAsync(id);
                if (attendance == null)
                    return NotFound($"#404! Attendance with ID {id} not found");

                return Ok(attendance);
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }

        /// <summary>
        /// Create a new attendance record
        /// </summary>
        /// <remarks>
        /// <para><b>Description:</b></para>
        /// <para>Creates a new attendance entry for a student.</para>
        ///
        /// <para><b>Inputs:</b></para>
        /// <list type="bullet">
        ///     <item><description><b>Enrollment_ID</b> - Associated enrollment</description></item>
        ///     <item><description><b>Date</b> - Attendance date</description></item>
        ///     <item><description><b>Status</b> - Attendance status (Present, Absent, Late)</description></item>
        /// </list>
        ///
        /// <para><b>Behavior:</b></para>
        /// <list type="bullet">
        ///     <item><description>Creates a new attendance record</description></item>
        /// </list>
        ///
        /// <para><b>Example:</b></para>
        /// <code>
        /// POST /AttendanceManagement/Attendance
        /// {
        ///   "enrollment_ID": 1,
        ///   "date": "2026-03-21T00:00:00",
        ///   "status": "Present"
        /// }
        /// </code>
        /// </remarks>
        /// <param name="dto">Attendance data</param>
        /// <returns>Created attendance record</returns>
        [HttpPost]
        public async Task<IActionResult> AddAttendance(AddAttendanceDTO dto)
        {
            try {
                string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userId == null)
                    throw new Exception("The Operator is Not Found");

                var attendance = await _attendanceService.AddAsync(dto, userId);
                return Ok(attendance);
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }

        /// <summary>
        /// Update an existing attendance record
        /// </summary>
        /// <remarks>
        /// <para><b>Description:</b></para>
        /// <para>Updates an attendance record using its ID.</para>
        ///
        /// <para><b>Inputs:</b></para>
        /// <list type="bullet">
        ///     <item><description><b>id</b> - Attendance ID</description></item>
        ///     <item><description><b>Enrollment_ID</b> - Updated enrollment ID</description></item>
        ///     <item><description><b>Date</b> - Updated date</description></item>
        ///     <item><description><b>Status</b> - Updated status</description></item>
        /// </list>
        ///
        /// <para><b>Behavior:</b></para>
        /// <list type="bullet">
        ///     <item><description>Updates the record if it exists</description></item>
        ///     <item><description>Returns 404 if not found</description></item>
        /// </list>
        /// </remarks>
        /// <param name="id">Attendance ID</param>
        /// <param name="dto">Updated attendance data</param>
        /// <returns>Updated attendance record</returns>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAttendance(int id, AddAttendanceDTO dto)
        {
            try { 
                var updatedAttendance = await _attendanceService.UpdateAsync(id, dto);
                if (updatedAttendance == null)
                    return NotFound($"#404! Attendance with ID {id} not found");

                return Ok(updatedAttendance);
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }

        /// <summary>
        /// Delete an attendance record
        /// </summary>
        /// <remarks>
        /// <para><b>Description:</b></para>
        /// <para>Deletes an attendance record from the system.</para>
        ///
        /// <para><b>Inputs:</b></para>
        /// <list type="bullet">
        ///     <item><description><b>id</b> - Attendance ID to delete</description></item>
        /// </list>
        ///
        /// <para><b>Behavior:</b></para>
        /// <list type="bullet">
        ///     <item><description>Deletes the record if found</description></item>
        ///     <item><description>Returns 404 if not found</description></item>
        /// </list>
        /// </remarks>
        /// <param name="id">Attendance ID</param>
        /// <returns>Status message</returns>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAttendance(int id)
        {
            try { 
                var success = await _attendanceService.DeleteAsync(id);
                if (!success)
                    return NotFound($"#404! Attendance with ID {id} not found");

                return Ok($"Attendance with ID {id} deleted successfully");
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }
    }
}