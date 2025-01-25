using System.Windows;
using System.Windows.Controls;

namespace Task8
{
    public partial class MainWindow : Window
    {
        private readonly ApplicationDbContext _context;

        public MainWindow()
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
            LoadCourses();
            SetButtonVisibility(showStudentButtons: false, showTeacherButtons: false);
            ManageGroupsButton.Visibility = Visibility.Collapsed;
        }

        private void ManageGroupsButton_Click(object sender, RoutedEventArgs e)
        {
            EditGroupWindow manageGroupsWindow = new EditGroupWindow();
            manageGroupsWindow.ShowDialog();
        }

        private void DataListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (HeaderText.Text == "Groups")
            {
                var selectedGroupName = (string)DataListBox.SelectedItem;
                var selectedGroup = GetAllGroups().FirstOrDefault(g => g.Name == selectedGroupName);
                if (selectedGroup != null)
                {
                    LoadStudents(selectedGroup.Id);
                    SetButtonVisibility(showStudentButtons: true, showTeacherButtons: false);
                }
            }
            else if (HeaderText.Text == "Courses")
            {
                var selectedCourseName = (string)DataListBox.SelectedItem;
                var selectedCourse = GetAllCourses().FirstOrDefault(c => c.Name == selectedCourseName);
                if (selectedCourse != null)
                {
                    LoadGroups(selectedCourse.Id);
                    SetButtonVisibility(showStudentButtons: false, showTeacherButtons: false);
                    ManageGroupsButton.Visibility = Visibility.Visible;
                }
            }
        }


        private void StudentsButton_Click(object sender, RoutedEventArgs e)
        {
            var students = GetAllStudents();
            UpdateListBox(students.Select(s => $"{s.Name} {s.Surname}").ToList());
            HeaderText.Text = "Students";
            SetButtonVisibility(showStudentButtons: true, showTeacherButtons: false);
            ManageGroupsButton.Visibility = Visibility.Collapsed;
        }

        private void TeachersButton_Click(object sender, RoutedEventArgs e)
        {
            var teachers = GetAllTeachers();
            UpdateListBox(teachers.Select(t => $"{t.Name} {t.Surname}").ToList());
            HeaderText.Text = "Teachers";
            SetButtonVisibility(showStudentButtons: false, showTeacherButtons: true);
            ManageGroupsButton.Visibility = Visibility.Collapsed;
        }

        private void CoursesButton_Click(object sender, RoutedEventArgs e)
        {
            var courses = GetAllCourses();
            UpdateListBox(courses.Select(c => c.Name).ToList());
            HeaderText.Text = "Courses";
            SetButtonVisibility(showStudentButtons: false, showTeacherButtons: false);
            ManageGroupsButton.Visibility = Visibility.Collapsed;
        }

        private void GroupsButton_Click(object sender, RoutedEventArgs e)
        {
            var groups = GetAllGroups();
            UpdateListBox(groups.Select(g => g.Name).ToList());
            HeaderText.Text = "Groups";
            SetButtonVisibility(showStudentButtons: false, showTeacherButtons: false);
            ManageGroupsButton.Visibility = Visibility.Visible;
        }


        private void AddStudentButton_Click(object sender, RoutedEventArgs e)
        {
            EditStudentWindow addStudentWindow = new EditStudentWindow();
            addStudentWindow.ShowDialog();
            if (HeaderText.Text == "Students")
            {
                var students = GetAllStudents();
                UpdateListBox(students.Select(s => $"{s.Name} {s.Surname}").ToList());
            }
        }

        private void EditStudentButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataListBox.SelectedItem != null)
            {
                var selectedStudentName = (string)DataListBox.SelectedItem;
                var selectedStudent = GetAllStudents().FirstOrDefault(s => $"{s.Name} {s.Surname}" == selectedStudentName);

                if (selectedStudent != null)
                {
                    EditStudentWindow editStudentWindow = new EditStudentWindow(selectedStudent.Id);
                    editStudentWindow.ShowDialog();
                    LoadStudents(selectedStudent.GroupId);
                }
                else
                {
                    MessageBox.Show("No student selected for editing.");
                }
            }
        }

        private void DeleteStudentButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataListBox.SelectedItem != null)
            {
                var selectedStudentName = (string)DataListBox.SelectedItem;
                var selectedStudent = GetAllStudents().FirstOrDefault(s => $"{s.Name} {s.Surname}" == selectedStudentName);

                if (selectedStudent != null)
                {
                    var result = MessageBox.Show($"Are you sure you want to delete {selectedStudent.Name} {selectedStudent.Surname}?", "Confirm Delete", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        DeleteStudent(selectedStudent);
                        MessageBox.Show($"Student {selectedStudent.Name} {selectedStudent.Surname} deleted.");
                        LoadStudents(selectedStudent.GroupId);
                    }
                }
                else
                {
                    MessageBox.Show("No student selected for deletion.");
                }
            }
        }

        private void AddTeacherButton_Click(object sender, RoutedEventArgs e)
        {
            EditTeacherWindow addTeacherWindow = new EditTeacherWindow();
            addTeacherWindow.ShowDialog();
            if (HeaderText.Text == "Teachers")
            {
                var teachers = GetAllTeachers();
                UpdateListBox(teachers.Select(t => $"{t.Name} {t.Surname}").ToList());
            }
        }

        private void EditTeacherButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataListBox.SelectedItem != null)
            {
                var selectedTeacherName = (string)DataListBox.SelectedItem;
                var selectedTeacher = GetAllTeachers().FirstOrDefault(t => $"{t.Name} {t.Surname}" == selectedTeacherName);

                if (selectedTeacher != null)
                {
                    EditTeacherWindow editTeacherWindow = new EditTeacherWindow(selectedTeacher.Id);
                    editTeacherWindow.ShowDialog();
                    LoadTeachers();
                }
                else
                {
                    MessageBox.Show("No teacher selected for editing.");
                }
            }
        }

        private void DeleteTeacherButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataListBox.SelectedItem != null)
            {
                var selectedTeacherName = (string)DataListBox.SelectedItem;
                var selectedTeacher = GetAllTeachers().FirstOrDefault(t => $"{t.Name} {t.Surname}" == selectedTeacherName);

                if (selectedTeacher != null)
                {
                    var result = MessageBox.Show($"Are you sure you want to delete {selectedTeacher.Name} {selectedTeacher.Surname}?", "Confirm Delete", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        DeleteTeacher(selectedTeacher);
                        MessageBox.Show($"Teacher {selectedTeacher.Name} {selectedTeacher.Surname} deleted.");
                        LoadTeachers();
                    }
                }
                else
                {
                    MessageBox.Show("No teacher selected for deletion.");
                }
            }
        }

        private List<Course> GetAllCourses() => _context.Courses.ToList();
        private List<Group> GetAllGroups() => _context.Groups.ToList();
        private List<Student> GetAllStudents() => _context.Students.ToList();
        private List<Teacher> GetAllTeachers() => _context.Teachers.ToList();

        private void DeleteStudent(Student student)
        {
            _context.Students.Remove(student);
            _context.SaveChanges();
        }

        private void DeleteTeacher(Teacher teacher)
        {
            _context.Teachers.Remove(teacher);
            _context.SaveChanges();
        }

        private void LoadCourses()
        {
            var courses = GetAllCourses();
            UpdateListBox(courses.Select(c => c.Name).ToList());
            HeaderText.Text = "Courses";
        }

        private void LoadGroups(int courseId)
        {
            var groups = _context.Groups.Where(g => g.CourseId == courseId).ToList();
            UpdateListBox(groups.Select(g => g.Name).ToList());
            HeaderText.Text = "Groups";
        }

        private void LoadTeachers()
        {
            var teachers = GetAllTeachers();
            UpdateListBox(teachers.Select(t => $"{t.Name} {t.Surname}").ToList());
        }

        private void LoadStudents(int groupId)
        {
            var students = GetAllStudents().Where(s => s.GroupId == groupId).ToList();
            UpdateListBox(students.Select(s => $"{s.Name} {s.Surname}").ToList());
            HeaderText.Text = "Students";
        }

        private void UpdateListBox(List<string> items)
        {
            DataListBox.ItemsSource = null;
            DataListBox.ItemsSource = items;
        }

        private void SetButtonVisibility(bool showStudentButtons, bool showTeacherButtons)
        {
            AddStudentButton.Visibility = showStudentButtons ? Visibility.Visible : Visibility.Collapsed;
            EditStudentButton.Visibility = showStudentButtons ? Visibility.Visible : Visibility.Collapsed;
            DeleteStudentButton.Visibility = showStudentButtons ? Visibility.Visible : Visibility.Collapsed;

            AddTeacherButton.Visibility = showTeacherButtons ? Visibility.Visible : Visibility.Collapsed;
            EditTeacherButton.Visibility = showTeacherButtons ? Visibility.Visible : Visibility.Collapsed;
            DeleteTeacherButton.Visibility = showTeacherButtons ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}

