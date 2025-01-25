using System.Collections.Generic;
using System.Linq;

namespace Task8
{
    public class ServiceDB
    {
        private readonly IApplicationDbContext _context;

        public ServiceDB(IApplicationDbContext context)
        {
            _context = context;
        }

        // Студенти
        public List<Student> GetAllStudents()
        {
            return _context.Students.ToList();
        }

        public List<Student> GetStudentsByGroup(int groupId)
        {
            return _context.Students.Where(s => s.GroupId == groupId).ToList();
        }

        public void AddStudent(Student student)
        {
            _context.Students.Add(student);
            _context.SaveChanges();
        }

        public void DeleteStudent(Student student)
        {
            _context.Students.Remove(student);
            _context.SaveChanges();
        }

        public void UpdateStudent(Student student)
        {
            var existingStudent = _context.Students.Find(student.Id);
            if (existingStudent != null)
            {
                existingStudent.Name = student.Name;
                existingStudent.Surname = student.Surname;
                existingStudent.GroupId = student.GroupId;
                _context.SaveChanges();
            }
        }

        // Викладачі
        public List<Teacher> GetAllTeachers()
        {
            return _context.Teachers.ToList();
        }

        public void AddTeacher(Teacher teacher)
        {
            _context.Teachers.Add(teacher);
            _context.SaveChanges();
        }

        public void DeleteTeacher(Teacher teacher)
        {
            _context.Teachers.Remove(teacher);
            _context.SaveChanges();
        }

        public void UpdateTeacher(Teacher teacher)
        {
            var existingTeacher = _context.Teachers.Find(teacher.Id);
            if (existingTeacher != null)
            {
                existingTeacher.Name = teacher.Name;
                existingTeacher.Surname = teacher.Surname;
                _context.SaveChanges();
            }
        }

        // Курси
        public List<Course> GetAllCourses()
        {
            return _context.Courses.ToList();
        }

        public void AddCourse(Course course)
        {
            _context.Courses.Add(course);
            _context.SaveChanges();
        }

        public void DeleteCourse(Course course)
        {
            _context.Courses.Remove(course);
            _context.SaveChanges();
        }

        public void UpdateCourse(Course course)
        {
            var existingCourse = _context.Courses.Find(course.Id);
            if (existingCourse != null)
            {
                existingCourse.Name = course.Name;
                _context.SaveChanges();
            }
        }

        // Групи
        public List<Group> GetAllGroups()
        {
            return _context.Groups.ToList();
        }

        public List<Group> GetGroupsByCourse(int courseId)
        {
            return _context.Groups.Where(g => g.CourseId == courseId).ToList();
        }

        public void AddGroup(Group group)
        {
            _context.Groups.Add(group);
            _context.SaveChanges();
        }

        public void DeleteGroup(Group group)
        {
            _context.Groups.Remove(group);
            _context.SaveChanges();
        }

        public void UpdateGroup(Group group)
        {
            var existingGroup = _context.Groups.Find(group.Id);
            if (existingGroup != null)
            {
                existingGroup.Name = group.Name;
                existingGroup.TeacherId = group.TeacherId;
                existingGroup.CourseId = group.CourseId;
                _context.SaveChanges();
            }
        }
    }
}
