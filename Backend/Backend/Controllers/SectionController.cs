using Microsoft.AspNetCore.Mvc;
using Backend.Backend.DTOs;
using Microsoft.AspNetCore.Authorization;
using Backend.Backend.Interface.ServiceInterface;

namespace Backend.Backend.Controllers
{
    /// <summary>
    /// Handles all operations related to Section management.
    /// </summary>
    [Route("AttendanceManagement/[controller]")]
    [ApiController]
    public class SectionController : ControllerBase
    {
        private readonly ISectionService _sectionService;

        public SectionController(ISectionService sectionService)
        {
            _sectionService = sectionService;
        }

		/// <summary>
		/// Retrieve all sections
		/// </summary>
		/// <remarks>
		/// <para><b>Description:</b></para>
		/// <para>Fetches all section records in the system.</para>
		///
		/// <para><b>Inputs:</b></para>
		/// <list type="bullet">
		///     <item><description>No input parameters required</description></item>
		/// </list>
		///
		/// <para><b>Behavior:</b></para>
		/// <list type="bullet">
		///     <item><description>Returns all section records</description></item>
		///     <item><description>Returns 404 if no sections exist</description></item>
		/// </list>
		///
		/// <para><b>Example:</b></para>
		/// <code>
		/// GET /AttendanceManagement/Section
		/// </code>
		/// </remarks>
		/// <returns>List of sections</returns>
		[Authorize(Roles = "Admin,Student")]
		[HttpGet]
        public async Task<IActionResult> GetAllSections()
        {
            try { 
                var sections = await _sectionService.GetAllAsync();
                if (!(sections?.Data?.Any() ?? false))
                    return NotFound("No Sections Found");

                return Ok(sections);
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }

        /// <summary>
        /// Retrieve an section by ID
        /// </summary>
        /// <remarks>
        /// <para><b>Description:</b></para>
        /// <para>Fetches a specific section record using its unique ID.</para>
        ///
        /// <para><b>Inputs:</b></para>
        /// <list type="bullet">
        ///     <item><description><b>id</b> - Section ID</description></item>
        /// </list>
        ///
        /// <para><b>Behavior:</b></para>
        /// <list type="bullet">
        ///     <item><description>Returns the section if found</description></item>
        ///     <item><description>Returns 404 if the record does not exist</description></item>
        /// </list>
        ///
        /// <para><b>Example:</b></para>
        /// <code>
        /// GET /AttendanceManagement/Section/1
        /// </code>
        /// </remarks>
        /// <param name="id">Section ID</param>
        /// <returns>Section record</returns>
        [HttpGet("{id:int}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> GetSectionById(int id)
        {
            try { 
                var section = await _sectionService.GetByIdAsync(id);
                if (section == null)
                    return NotFound($"#404! Section with ID {id} not found");

                return Ok(section);
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }

        /// <summary>
        /// Create a new section
        /// </summary>
        /// <remarks>
        /// <para><b>Description:</b></para>
        /// <para>Creates a new section linking a student to a schedule.</para>
        ///
        /// <para><b>Inputs:</b></para>
        /// <list type="bullet">
        ///     <item><description><b>Student_ID</b> - ID of the student</description></item>
        ///     <item><description><b>Schedule_ID</b> - ID of the schedule</description></item>
        /// </list>
        ///
        /// <para><b>Behavior:</b></para>
        /// <list type="bullet">
        ///     <item><description>Creates a new section record</description></item>
        /// </list>
        ///
        /// <para><b>Example:</b></para>
        /// <code>
        /// POST /AttendanceManagement/Section
        /// {
        ///   "student_ID": 1,
        ///   "schedule_ID": 5
        /// }
        /// </code>
        /// </remarks>
        /// <param name="dto">Section data</param>
        /// <returns>Created section</returns>
        [HttpPost]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> AddSection(AddSectionDTO dto)
        {
            try { 
                var section = await _sectionService.AddAsync(dto);
                return Ok(section);
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }

        /// <summary>
        /// Update an existing section
        /// </summary>
        /// <remarks>
        /// <para><b>Description:</b></para>
        /// <para>Updates an section record using its ID.</para>
        ///
        /// <para><b>Inputs:</b></para>
        /// <list type="bullet">
        ///     <item><description><b>id</b> - Section ID</description></item>
        ///     <item><description><b>Student_ID</b> - Updated student ID</description></item>
        ///     <item><description><b>Schedule_ID</b> - Updated schedule ID</description></item>
        /// </list>
        ///
        /// <para><b>Behavior:</b></para>
        /// <list type="bullet">
        ///     <item><description>Updates the section if it exists</description></item>
        ///     <item><description>Returns 404 if not found</description></item>
        /// </list>
        /// </remarks>
        /// <param name="id">Section ID</param>
        /// <param name="dto">Updated section data</param>
        /// <returns>Updated section</returns>
        [HttpPut("{id:int}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> UpdateSection(int id, AddSectionDTO dto)
        {
            try { 
                var updatedSection = await _sectionService.UpdateAsync(id, dto);
                if (updatedSection == null)
                    return NotFound($"#404! Section with ID {id} not found");

                return Ok(updatedSection);
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }

        /// <summary>
        /// Delete an section
        /// </summary>
        /// <remarks>
        /// <para><b>Description:</b></para>
        /// <para>Deletes an section record from the system.</para>
        ///
        /// <para><b>Inputs:</b></para>
        /// <list type="bullet">
        ///     <item><description><b>id</b> - Section ID to delete</description></item>
        /// </list>
        ///
        /// <para><b>Behavior:</b></para>
        /// <list type="bullet">
        ///     <item><description>Deletes the record if found</description></item>
        ///     <item><description>Returns 404 if not found</description></item>
        /// </list>
        /// </remarks>
        /// <param name="id">Section ID</param>
        /// <returns>Status message</returns>
        [HttpDelete("{id:int}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> DeleteSection(int id)
        {
            try { 
                var success = await _sectionService.DeleteAsync(id);
                if (!success)
                    return NotFound($"#404! Section with ID {id} not found");

                return Ok($"Section with ID {id} deleted successfully");
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }
    }
}