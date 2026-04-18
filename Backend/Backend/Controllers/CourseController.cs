using Microsoft.AspNetCore.Mvc;
using Backend.Backend.DTOs;
using Backend.Backend.Interface.ServiceInterface;

namespace Backend.Backend.Controllers
{
    /// <summary>
    /// Handles all operations related to Course management.
    /// </summary>
    [Route("AttendanceManagement/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        /// <summary>
        /// Retrieve all courses
        /// </summary>
        /// <remarks>
        /// <para><b>Description:</b></para>
        /// <para>Fetches all courses available in the system.</para>
        ///
        /// <para><b>Inputs:</b></para>
        /// <list type="bullet">
        ///     <item><description>No input parameters required</description></item>
        /// </list>
        ///
        /// <para><b>Behavior:</b></para>
        /// <list type="bullet">
        ///     <item><description>Returns all course records</description></item>
        ///     <item><description>Returns 404 if no courses exist</description></item>
        /// </list>
        ///
        /// <para><b>Example:</b></para>
        /// <code>
        /// GET /AttendanceManagement/Course
        /// </code>
        /// </remarks>
        /// <returns>List of courses</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllCourses()
        {
            try { 
                var courses = await _courseService.GetAllAsync();
                if (!(courses?.Data?.Any() ?? false))
                    return NotFound("No Courses Found");

                return Ok(courses);
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }

        /// <summary>
        /// Retrieve a course by ID
        /// </summary>
        /// <remarks>
        /// <para><b>Description:</b></para>
        /// <para>Fetches a specific course using its unique identifier.</para>
        ///
        /// <para><b>Inputs:</b></para>
        /// <list type="bullet">
        ///     <item><description><b>id</b> - Course ID</description></item>
        /// </list>
        ///
        /// <para><b>Behavior:</b></para>
        /// <list type="bullet">
        ///     <item><description>Returns the course if found</description></item>
        ///     <item><description>Returns 404 if the course does not exist</description></item>
        /// </list>
        ///
        /// <para><b>Example:</b></para>
        /// <code>
        /// GET /AttendanceManagement/Course/1
        /// </code>
        /// </remarks>
        /// <param name="id">Course ID</param>
        /// <returns>Course details</returns>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCourseById(int id)
        {
            try { 
                var course = await _courseService.GetByIdAsync(id);
                if (course == null)
                    return NotFound($"#404! Course with ID {id} not found");

                return Ok(course);
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }

        /// <summary>
        /// Create a new course
        /// </summary>
        /// <remarks>
        /// <para><b>Description:</b></para>
        /// <para>Creates a new course record in the system.</para>
        ///
        /// <para><b>Inputs:</b></para>
        /// <list type="bullet">
        ///     <item><description><b>Title</b> - Course title</description></item>
        ///     <item><description><b>Code</b> - Course code</description></item>
        ///     <item><description><b>Description</b> - Optional description</description></item>
        ///     <item><description><b>Teacher_ID</b> - Assigned teacher</description></item>
        /// </list>
        ///
        /// <para><b>Behavior:</b></para>
        /// <list type="bullet">
        ///     <item><description>Creates a new course record</description></item>
        /// </list>
        ///
        /// <para><b>Example:</b></para>
        /// <code>
        /// POST /AttendanceManagement/Course
        /// {
        ///   "title": "Database Systems",
        ///   "code": "CS101",
        ///   "description": "Introduction to databases",
        ///   "teacher_ID": 1
        /// }
        /// </code>
        /// </remarks>
        /// <param name="dto">Course data</param>
        /// <returns>Created course</returns>
        [HttpPost]
        public async Task<IActionResult> AddCourse(AddCourseDTO dto)
        {
            try { 
                var course = await _courseService.AddAsync(dto);
                return Ok(course);
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }

        /// <summary>
        /// Update an existing course
        /// </summary>
        /// <remarks>
        /// <para><b>Description:</b></para>
        /// <para>Updates a course record using its ID.</para>
        ///
        /// <para><b>Inputs:</b></para>
        /// <list type="bullet">
        ///     <item><description><b>id</b> - Course ID</description></item>
        ///     <item><description><b>Title</b> - Updated title</description></item>
        ///     <item><description><b>Code</b> - Updated course code</description></item>
        ///     <item><description><b>Description</b> - Updated description</description></item>
        ///     <item><description><b>Teacher_ID</b> - Updated teacher ID</description></item>
        /// </list>
        ///
        /// <para><b>Behavior:</b></para>
        /// <list type="bullet">
        ///     <item><description>Updates the course if it exists</description></item>
        ///     <item><description>Returns 404 if not found</description></item>
        /// </list>
        /// </remarks>
        /// <param name="id">Course ID</param>
        /// <param name="dto">Updated course data</param>
        /// <returns>Updated course</returns>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCourse(int id, AddCourseDTO dto)
        {
            try { 
                var updatedCourse = await _courseService.UpdateAsync(id, dto);
                if (updatedCourse == null)
                    return NotFound($"#404! Course with ID {id} not found");

                return Ok(updatedCourse);
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }

        /// <summary>
        /// Delete a course
        /// </summary>
        /// <remarks>
        /// <para><b>Description:</b></para>
        /// <para>Deletes a course record from the system.</para>
        ///
        /// <para><b>Inputs:</b></para>
        /// <list type="bullet">
        ///     <item><description><b>id</b> - Course ID to delete</description></item>
        /// </list>
        ///
        /// <para><b>Behavior:</b></para>
        /// <list type="bullet">
        ///     <item><description>Deletes the course if found</description></item>
        ///     <item><description>Returns 404 if not found</description></item>
        /// </list>
        /// </remarks>
        /// <param name="id">Course ID</param>
        /// <returns>Status message</returns>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            try { 
                var success = await _courseService.DeleteAsync(id);
                if (!success)
                    return NotFound($"#404! Course with ID {id} not found");

                return Ok($"Course with ID {id} deleted successfully");
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }
    }
}