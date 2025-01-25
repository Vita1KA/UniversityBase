using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System.IO;
using System.Text;
using Xceed.Words.NET;

public class ServiceGroup
{
    private readonly IApplicationDbContext _context;

    public ServiceGroup(IApplicationDbContext context)
    {
        _context = context;
    }

    public void ImportStudentsFromCsv(int groupId, string filePath)
    {
        var group = _context.Groups.Find(groupId)
            ?? throw new ArgumentException("Group not found");

        var csvLines = File.ReadAllLines(filePath);
        foreach (var line in csvLines)
        {
            var parts = line.Split(',');
            if (parts.Length != 2) continue;

            var student = new Student
            {
                Name = parts[0],
                Surname = parts[1],
                GroupId = groupId
            };
            group.Students.Add(student);
        }
        _context.SaveChanges();
    }

    public void ExportStudentsToCsv(int groupId, string filePath)
    {
        var group = _context.Groups
            .Include(g => g.Students)
            .FirstOrDefault(g => g.Id == groupId)
            ?? throw new FileNotFoundException("Group not found.");

        using var writer = new StreamWriter(filePath);
        writer.WriteLine("Name,Surname");
        foreach (var student in group.Students)
        {
            writer.WriteLine($"{student.Name},{student.Surname}");
        }
    }

    public void GenerateDoc(int groupId, string filePath)
    {
        var group = _context.Groups
            .Include(g => g.Students)
            .Include(g => g.Course)
            .Include(g => g.Teacher)
            .FirstOrDefault(g => g.Id == groupId)
            ?? throw new FileNotFoundException("Group not found.");

        using var document = DocX.Create(filePath);
        document.InsertParagraph($"Group: {group.Name}").FontSize(20).Bold();

        var courseName = group.Course?.Name ?? "N/A";
        document.InsertParagraph($"Course: {courseName}").FontSize(16);

        var curatorInfo = group.Teacher != null
            ? $"{group.Teacher.Name} {group.Teacher.Surname}"
            : "N/A";
        document.InsertParagraph($"Curator: {curatorInfo}").FontSize(16);

        document.InsertParagraph();
        var studentsParagraph = document.InsertParagraph("Students:").Bold();

        int index = 1;
        foreach (var student in group.Students)
        {
            studentsParagraph.AppendLine($"{index}. {student.Name} {student.Surname}");
            index++;
        }
        document.Save();
    }

    public void GeneratePdf(int groupId, string filePath)
    {
        var group = _context.Groups
            .Include(g => g.Students)
            .Include(g => g.Course)
            .Include(g => g.Teacher)
            .FirstOrDefault(g => g.Id == groupId)
            ?? throw new FileNotFoundException("Group not found.");

        var content = new StringBuilder();
        content.AppendLine($"Group: {group.Name}");

        var courseName = group.Course?.Name ?? "N/A";
        content.AppendLine($"Course: {courseName}");

        var curatorInfo = group.Teacher != null
            ? $"{group.Teacher.Name} {group.Teacher.Surname}"
            : "N/A";
        content.AppendLine($"Curator: {curatorInfo}");

        content.AppendLine();
        content.AppendLine("Students:");

        int index = 1;
        foreach (var student in group.Students)
        {
            content.AppendLine($"{index}. {student.Name} {student.Surname}");
            index++;
        }

        File.WriteAllText(filePath, content.ToString());
    }
}

