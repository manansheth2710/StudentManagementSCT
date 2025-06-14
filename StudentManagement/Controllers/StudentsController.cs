using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Data;
using StudentManagement.Data.Models;
using System.Net;

namespace StudentManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StudentsController : Controller
    {
        private readonly StudentDbContext _context;

        public StudentsController(StudentDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<List<Student>> GetAllStudents() => await _context.Students.ToListAsync();

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudentDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,PhoneNumber")] Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return Ok(student);
            }
            return StatusCode((int)HttpStatusCode.InternalServerError);
        }

        [HttpPut]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,PhoneNumber")] Student student)
        {
            if (id != student.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        return StatusCode((int)HttpStatusCode.InternalServerError);
                    }
                }
            }
            return Ok(student);
        }

        [HttpDelete, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
            }

            await _context.SaveChangesAsync();
            return StatusCode((int)HttpStatusCode.OK);
        }

        [HttpPost("{studentId}")]
        public async Task<IActionResult> AssignCoursesToStudent(int studentId, [FromBody]List<string> courses)
        {
            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.Id == studentId);
            if (student == null)
            {
                return NotFound();
            }
            else
            {
                try
                {
                    student.Courses = string.Join(",",courses);
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        return StatusCode((int)HttpStatusCode.InternalServerError);
                    }
                }
            }
            return Ok(student);
        }

        [NonAction]
        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.Id == id);
        }
    }
}
