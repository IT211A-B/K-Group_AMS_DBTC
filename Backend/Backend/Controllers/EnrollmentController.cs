using Microsoft.AspNetCore.Mvc;
using Backend.Backend.DTOs;
using Backend.Backend.Interface.ServiceInterface;

namespace Backend.Backend.Controllers
{
    /// <summary>
    /// Handles all operations related to Enrollment management.
    /// </summary>
    [Route("AttendanceManagement/[controller]")]
    [ApiController]
    public class EnrollmentController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        /// <summary>
        /// Retrieve all enrollments
        /// </summary>
        /// <remarks>
        /// <para><b>Description:</b></para>
        /// <para>Fetches all enrollment records in the system.</para>
        ///
        /// <para><b>Inputs:</b></para>
        /// <list type="bullet">
        ///     <item><description>No input parameters required</description></item>
        /// </list>
        ///
        /// <para><b>Behavior:</b></para>
        /// <list type="bullet">
        ///     <item><description>Returns all enrollment records</description></item>
        ///     <item><description>Returns 404 if no enrollments exist</description></item>
        /// </list>
        ///
        /// <para><b>Example:</b></para>
        /// <code>
        /// GET /AttendanceManagement/Enrollment
        /// </code>
        /// </remarks>
        /// <returns>List of enrollments</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllEnrollments()
        {
            var enrollments = await _enrollmentService.GetAllAsync();
            if (!enrollments.Any())
                return NotFound("No Enrollments Found");

            return Ok(enrollments);
        }

        /// <summary>
        /// Retrieve an enrollment by ID
        /// </summary>
        /// <remarks>
        /// <para><b>Description:</b></para>
        /// <para>Fetches a specific enrollment record using its unique ID.</para>
        ///
        /// <para><b>Inputs:</b></para>
        /// <list type="bullet">
        ///     <item><description><b>id</b> - Enrollment ID</description></item>
        /// </list>
        ///
        /// <para><b>Behavior:</b></para>
        /// <list type="bullet">
        ///     <item><description>Returns the enrollment if found</description></item>
        ///     <item><description>Returns 404 if the record does not exist</description></item>
        /// </list>
        ///
        /// <para><b>Example:</b></para>
        /// <code>
        /// GET /AttendanceManagement/Enrollment/1
        /// </code>
        /// </remarks>
        /// <param name="id">Enrollment ID</param>
        /// <returns>Enrollment record</returns>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetEnrollmentById(int id)
        {
            var enrollment = await _enrollmentService.GetByIdAsync(id);
            if (enrollment == null)
                return NotFound($"#404! Enrollment with ID {id} not found");

            return Ok(enrollment);
        }

        /// <summary>
        /// Create a new enrollment
        /// </summary>
        /// <remarks>
        /// <para><b>Description:</b></para>
        /// <para>Creates a new enrollment linking a student to a schedule.</para>
        ///
        /// <para><b>Inputs:</b></para>
        /// <list type="bullet">
        ///     <item><description><b>Student_ID</b> - ID of the student</description></item>
        ///     <item><description><b>Schedule_ID</b> - ID of the schedule</description></item>
        /// </list>
        ///
        /// <para><b>Behavior:</b></para>
        /// <list type="bullet">
        ///     <item><description>Creates a new enrollment record</description></item>
        /// </list>
        ///
        /// <para><b>Example:</b></para>
        /// <code>
        /// POST /AttendanceManagement/Enrollment
        /// {
        ///   "student_ID": 1,
        ///   "schedule_ID": 5
        /// }
        /// </code>
        /// </remarks>
        /// <param name="dto">Enrollment data</param>
        /// <returns>Created enrollment</returns>
        [HttpPost]
        public async Task<IActionResult> AddEnrollment(AddEnrollmentDTO dto)
        {
            var enrollment = await _enrollmentService.AddAsync(dto);
            return Ok(enrollment);
        }

        /// <summary>
        /// Update an existing enrollment
        /// </summary>
        /// <remarks>
        /// <para><b>Description:</b></para>
        /// <para>Updates an enrollment record using its ID.</para>
        ///
        /// <para><b>Inputs:</b></para>
        /// <list type="bullet">
        ///     <item><description><b>id</b> - Enrollment ID</description></item>
        ///     <item><description><b>Student_ID</b> - Updated student ID</description></item>
        ///     <item><description><b>Schedule_ID</b> - Updated schedule ID</description></item>
        /// </list>
        ///
        /// <para><b>Behavior:</b></para>
        /// <list type="bullet">
        ///     <item><description>Updates the enrollment if it exists</description></item>
        ///     <item><description>Returns 404 if not found</description></item>
        /// </list>
        /// </remarks>
        /// <param name="id">Enrollment ID</param>
        /// <param name="dto">Updated enrollment data</param>
        /// <returns>Updated enrollment</returns>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateEnrollment(int id, AddEnrollmentDTO dto)
        {
            var updatedEnrollment = await _enrollmentService.UpdateAsync(id, dto);
            if (updatedEnrollment == null)
                return NotFound($"#404! Enrollment with ID {id} not found");

            return Ok(updatedEnrollment);
        }

        /// <summary>
        /// Delete an enrollment
        /// </summary>
        /// <remarks>
        /// <para><b>Description:</b></para>
        /// <para>Deletes an enrollment record from the system.</para>
        ///
        /// <para><b>Inputs:</b></para>
        /// <list type="bullet">
        ///     <item><description><b>id</b> - Enrollment ID to delete</description></item>
        /// </list>
        ///
        /// <para><b>Behavior:</b></para>
        /// <list type="bullet">
        ///     <item><description>Deletes the record if found</description></item>
        ///     <item><description>Returns 404 if not found</description></item>
        /// </list>
        /// </remarks>
        /// <param name="id">Enrollment ID</param>
        /// <returns>Status message</returns>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEnrollment(int id)
        {
            var success = await _enrollmentService.DeleteAsync(id);
            if (!success)
                return NotFound($"#404! Enrollment with ID {id} not found");

            return Ok($"Enrollment with ID {id} deleted successfully");
        }
    }
}