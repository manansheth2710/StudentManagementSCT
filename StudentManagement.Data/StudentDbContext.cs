using Microsoft.EntityFrameworkCore;
using StudentManagement.Data.Models;

namespace StudentManagement.Data
{
    public class StudentDbContext(DbContextOptions<StudentDbContext> options) : DbContext(options)
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
    }
}
