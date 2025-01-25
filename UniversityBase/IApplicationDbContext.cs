using Microsoft.EntityFrameworkCore;

public interface IApplicationDbContext
{
    DbSet<Student> Students { get; }
    DbSet<Teacher> Teachers { get; }
    DbSet<Course> Courses { get; }
    DbSet<Group> Groups { get; }
    int SaveChanges();
}
