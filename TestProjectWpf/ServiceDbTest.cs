using Moq;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Task8;
using System.Text.RegularExpressions;

public class ServiceDBTests
{
    private readonly Mock<IApplicationDbContext> _mockContext;
    private readonly ServiceDB _serviceDB;

    public ServiceDBTests()
    {
        _mockContext = new Mock<IApplicationDbContext>();
        _serviceDB = new ServiceDB(_mockContext.Object);
    }

    [Fact]
    public void GetAllStudents_Test()
    {
        var students = new List<Student>
        {
            new Student { Name = "John", Surname = "Doe", GroupId = 1 },
            new Student { Name = "Jane", Surname = "Smith", GroupId = 2 }
        }.AsQueryable();

        var mockSet = new Mock<DbSet<Student>>();
        mockSet.As<IQueryable<Student>>().Setup(m => m.Provider).Returns(students.Provider);
        mockSet.As<IQueryable<Student>>().Setup(m => m.Expression).Returns(students.Expression);
        mockSet.As<IQueryable<Student>>().Setup(m => m.ElementType).Returns(students.ElementType);
        mockSet.As<IQueryable<Student>>().Setup(m => m.GetEnumerator()).Returns(students.GetEnumerator());

        _mockContext.Setup(c => c.Students).Returns(mockSet.Object);

        var result = _serviceDB.GetAllStudents();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, s => s.Name == "John");
        Assert.Contains(result, s => s.Name == "Jane");
    }

    [Fact]
    public void GetStudentsByGroup_Test()
    {
        var students = new List<Student>
        {
            new Student { Name = "John", Surname = "Doe", GroupId = 1 },
            new Student { Name = "Jane", Surname = "Smith", GroupId = 2 },
            new Student { Name = "Alice", Surname = "Johnson", GroupId = 1 }
        }.AsQueryable();

        var mockSet = new Mock<DbSet<Student>>();
        mockSet.As<IQueryable<Student>>().Setup(m => m.Provider).Returns(students.Provider);
        mockSet.As<IQueryable<Student>>().Setup(m => m.Expression).Returns(students.Expression);
        mockSet.As<IQueryable<Student>>().Setup(m => m.ElementType).Returns(students.ElementType);
        mockSet.As<IQueryable<Student>>().Setup(m => m.GetEnumerator()).Returns(students.GetEnumerator());

        _mockContext.Setup(c => c.Students).Returns(mockSet.Object);

        var result = _serviceDB.GetStudentsByGroup(1);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, s => s.Name == "John");
        Assert.Contains(result, s => s.Name == "Alice");
    }

    [Fact]
    public void GetAllTeachers_Test()
    {
        var teachers = new List<Teacher>
        {
            new Teacher { Name = "Mr. Smith" },
            new Teacher { Name = "Mrs. Johnson" }
        }.AsQueryable();

        var mockSet = new Mock<DbSet<Teacher>>();
        mockSet.As<IQueryable<Teacher>>().Setup(m => m.Provider).Returns(teachers.Provider);
        mockSet.As<IQueryable<Teacher>>().Setup(m => m.Expression).Returns(teachers.Expression);
        mockSet.As<IQueryable<Teacher>>().Setup(m => m.ElementType).Returns(teachers.ElementType);
        mockSet.As<IQueryable<Teacher>>().Setup(m => m.GetEnumerator()).Returns(teachers.GetEnumerator());

        _mockContext.Setup(c => c.Teachers).Returns(mockSet.Object);

        var result = _serviceDB.GetAllTeachers();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, t => t.Name == "Mr. Smith");
        Assert.Contains(result, t => t.Name == "Mrs. Johnson");
    }

    [Fact]
    public void GetAllCourses_Test()
    {
        var courses = new List<Course>
        {
            new Course { Name = "Mathematics" },
            new Course { Name = "Physics" }
        }.AsQueryable();

        var mockSet = new Mock<DbSet<Course>>();
        mockSet.As<IQueryable<Course>>().Setup(m => m.Provider).Returns(courses.Provider);
        mockSet.As<IQueryable<Course>>().Setup(m => m.Expression).Returns(courses.Expression);
        mockSet.As<IQueryable<Course>>().Setup(m => m.ElementType).Returns(courses.ElementType);
        mockSet.As<IQueryable<Course>>().Setup(m => m.GetEnumerator()).Returns(courses.GetEnumerator());

        _mockContext.Setup(c => c.Courses).Returns(mockSet.Object);

        var result = _serviceDB.GetAllCourses();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, c => c.Name == "Mathematics");
        Assert.Contains(result, c => c.Name == "Physics");
    }

    [Fact]
    public void GetAllGroups_Test()
    {
        var groups = new List<Group>
        {
            new Group { Name = "Group A" },
            new Group { Name = "Group B" }
        }.AsQueryable();

        var mockSet = new Mock<DbSet<Group>>();
        mockSet.As<IQueryable<Group>>().Setup(m => m.Provider).Returns(groups.Provider);
        mockSet.As<IQueryable<Group>>().Setup(m => m.Expression).Returns(groups.Expression);
        mockSet.As<IQueryable<Group>>().Setup(m => m.ElementType).Returns(groups.ElementType);
        mockSet.As<IQueryable<Group>>().Setup(m => m.GetEnumerator()).Returns(groups.GetEnumerator());

        _mockContext.Setup(c => c.Groups).Returns(mockSet.Object);

        var result = _serviceDB.GetAllGroups();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, g => g.Name == "Group A");
        Assert.Contains(result, g => g.Name == "Group B");
    }

    [Fact]
    public void GetGroupsByCourse_Test()
    {
        var groups = new List<Group>
        {
            new Group { Name = "Group A", CourseId = 1 },
            new Group { Name = "Group B", CourseId = 2 },
            new Group { Name = "Group C", CourseId = 1 }
        }.AsQueryable();

        var mockSet = new Mock<DbSet<Group>>();
        mockSet.As<IQueryable<Group>>().Setup(m => m.Provider).Returns(groups.Provider);
        mockSet.As<IQueryable<Group>>().Setup(m => m.Expression).Returns(groups.Expression);
        mockSet.As<IQueryable<Group>>().Setup(m => m.ElementType).Returns(groups.ElementType);
        mockSet.As<IQueryable<Group>>().Setup(m => m.GetEnumerator()).Returns(groups.GetEnumerator());

        _mockContext.Setup(c => c.Groups).Returns(mockSet.Object);

        var result = _serviceDB.GetGroupsByCourse(1);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, g => g.Name == "Group A");
        Assert.Contains(result, g => g.Name == "Group C");
    }
}


