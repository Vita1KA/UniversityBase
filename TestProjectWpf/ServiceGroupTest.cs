using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xceed.Words.NET;
using Xunit;

public class ServiceGroupTests
{
    private readonly Mock<IApplicationDbContext> _mockContext;
    private readonly ServiceGroup _serviceGroup;

    public ServiceGroupTests()
    {
        _mockContext = new Mock<IApplicationDbContext>();
        _serviceGroup = new ServiceGroup(_mockContext.Object);
    }

    [Fact]
    public void GenerateDoc_ValidGroup_CreatesDocFile()
    {
        var students = new List<Student>
        {
            new Student { Name = "John", Surname = "Doe" },
            new Student { Name = "Jane", Surname = "Smith" }
        };

        var group = new Group
        {
            Id = 1,
            Name = "Group A",
            CourseId = 1,
            Students = students
        };

        var course = new Course { Id = 1, Name = "Course 101", Groups = new List<Group> { group } };

        var mockGroupSet = new Mock<DbSet<Group>>();
        var mockCourseSet = new Mock<DbSet<Course>>();

        var groupsList = new List<Group> { group };
        mockGroupSet.As<IQueryable<Group>>().Setup(m => m.Provider).Returns(groupsList.AsQueryable().Provider);
        mockGroupSet.As<IQueryable<Group>>().Setup(m => m.Expression).Returns(groupsList.AsQueryable().Expression);
        mockGroupSet.As<IQueryable<Group>>().Setup(m => m.ElementType).Returns(groupsList.AsQueryable().ElementType);
        mockGroupSet.As<IQueryable<Group>>().Setup(m => m.GetEnumerator()).Returns(groupsList.AsQueryable().GetEnumerator());

        var coursesList = new List<Course> { course };
        mockCourseSet.As<IQueryable<Course>>().Setup(m => m.Provider).Returns(coursesList.AsQueryable().Provider);
        mockCourseSet.As<IQueryable<Course>>().Setup(m => m.Expression).Returns(coursesList.AsQueryable().Expression);
        mockCourseSet.As<IQueryable<Course>>().Setup(m => m.ElementType).Returns(coursesList.AsQueryable().ElementType);
        mockCourseSet.As<IQueryable<Course>>().Setup(m => m.GetEnumerator()).Returns(coursesList.AsQueryable().GetEnumerator());

        _mockContext.Setup(m => m.Groups).Returns(mockGroupSet.Object);
        _mockContext.Setup(m => m.Courses).Returns(mockCourseSet.Object);

        var filePath = Path.Combine(Path.GetTempPath(), "GroupA.docx");
        _serviceGroup.GenerateDoc(1, filePath);

        Assert.True(File.Exists(filePath));

        string docContent;
        using (var document = DocX.Load(filePath))
        {
            docContent = document.Text;
        }

        Assert.Contains("Group: Group A", docContent);
        Assert.Contains("John Doe", docContent);
        Assert.Contains("Jane Smith", docContent);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void GeneratePdf_ValidGroup_CreatesPdfFile()
    {
        int groupId = 1;
        var students = new List<Student>
        {
            new Student { Name = "John", Surname = "Doe" },
            new Student { Name = "Jane", Surname = "Smith" }
        };

        var group = new Group { Id = groupId, Name = "Group A", Students = students, CourseId = 1 };

        var mockCourseSet = new Mock<DbSet<Course>>();
        var coursesList = new List<Course> { new Course { Id = 1, Name = "Mathematics" } };

        mockCourseSet.As<IQueryable<Course>>().Setup(m => m.Provider).Returns(coursesList.AsQueryable().Provider);
        mockCourseSet.As<IQueryable<Course>>().Setup(m => m.Expression).Returns(coursesList.AsQueryable().Expression);
        mockCourseSet.As<IQueryable<Course>>().Setup(m => m.ElementType).Returns(coursesList.AsQueryable().ElementType);
        mockCourseSet.As<IQueryable<Course>>().Setup(m => m.GetEnumerator()).Returns(coursesList.AsQueryable().GetEnumerator());

        var mockGroupSet = new Mock<DbSet<Group>>();
        var groupsList = new List<Group> { group };

        mockGroupSet.As<IQueryable<Group>>().Setup(m => m.Provider).Returns(groupsList.AsQueryable().Provider);
        mockGroupSet.As<IQueryable<Group>>().Setup(m => m.Expression).Returns(groupsList.AsQueryable().Expression);
        mockGroupSet.As<IQueryable<Group>>().Setup(m => m.ElementType).Returns(groupsList.AsQueryable().ElementType);
        mockGroupSet.As<IQueryable<Group>>().Setup(m => m.GetEnumerator()).Returns(groupsList.AsQueryable().GetEnumerator());

        _mockContext.Setup(m => m.Groups).Returns(mockGroupSet.Object);
        _mockContext.Setup(m => m.Courses).Returns(mockCourseSet.Object);

        string filePath = Path.Combine(Path.GetTempPath(), "group.pdf");
        _serviceGroup.GeneratePdf(groupId, filePath);

        Assert.True(File.Exists(filePath));
        var fileContent = File.ReadAllText(filePath);
        Assert.Contains("Group: Group A", fileContent);
        Assert.Contains("Course: Mathematics", fileContent);
        Assert.Contains("1. John Doe", fileContent);
        Assert.Contains("2. Jane Smith", fileContent);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void ExportStudentsToCsv_ValidGroup_ExportsToCsv()
    {
        var students = new List<Student>
        {
            new Student { Name = "John", Surname = "Doe" },
            new Student { Name = "Jane", Surname = "Smith" }
        };

        var group = new Group
        {
            Id = 1,
            Students = students
        };

        var groupsList = new List<Group> { group };

        var mockGroupSet = new Mock<DbSet<Group>>();
        mockGroupSet.As<IQueryable<Group>>().Setup(m => m.Provider).Returns(groupsList.AsQueryable().Provider);
        mockGroupSet.As<IQueryable<Group>>().Setup(m => m.Expression).Returns(groupsList.AsQueryable().Expression);
        mockGroupSet.As<IQueryable<Group>>().Setup(m => m.ElementType).Returns(groupsList.AsQueryable().ElementType);
        mockGroupSet.As<IQueryable<Group>>().Setup(m => m.GetEnumerator()).Returns(groupsList.AsQueryable().GetEnumerator());

        _mockContext.Setup(m => m.Groups).Returns(mockGroupSet.Object);

        var filePath = Path.Combine(Path.GetTempPath(), "students.csv");
        _serviceGroup.ExportStudentsToCsv(1, filePath);

        Assert.True(File.Exists(filePath));

        var csvContent = File.ReadAllLines(filePath);
        Assert.Equal(3, csvContent.Length);
        Assert.Equal("Name,Surname", csvContent[0]);
        Assert.Equal("John,Doe", csvContent[1]);
        Assert.Equal("Jane,Smith", csvContent[2]);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void ImportStudentsFromCsv_ValidFile_AddsStudentsToGroup()
    {
        int groupId = 1;
        var group = new Group { Id = groupId, Students = new List<Student>() };

        var mockSet = new Mock<DbSet<Group>>();
        var groupsList = new List<Group> { group };

        mockSet.As<IQueryable<Group>>().Setup(m => m.Provider).Returns(groupsList.AsQueryable().Provider);
        mockSet.As<IQueryable<Group>>().Setup(m => m.Expression).Returns(groupsList.AsQueryable().Expression);
        mockSet.As<IQueryable<Group>>().Setup(m => m.ElementType).Returns(groupsList.AsQueryable().ElementType);
        mockSet.As<IQueryable<Group>>().Setup(m => m.GetEnumerator()).Returns(groupsList.AsQueryable().GetEnumerator());

        _mockContext.Setup(m => m.Groups).Returns(mockSet.Object);

        mockSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns((object[] id) => groupsList.FirstOrDefault(g => g.Id == (int)id[0]));

        string csvContent = "John,Doe\nJane,Smith";
        string filePath = Path.Combine(Path.GetTempPath(), "test.csv");

        File.WriteAllText(filePath, csvContent);

        _serviceGroup.ImportStudentsFromCsv(groupId, filePath);

        Assert.Equal(2, group.Students.Count);
        Assert.Contains(group.Students, s => s.Name == "John" && s.Surname == "Doe");
        Assert.Contains(group.Students, s => s.Name == "Jane" && s.Surname == "Smith");

        File.Delete(filePath);
    }
}
