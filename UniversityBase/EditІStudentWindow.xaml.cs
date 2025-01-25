using System.Windows;

namespace Task8
{
    public partial class EditStudentWindow : Window
    {
        private readonly ApplicationDbContext _context;
        private readonly int? _studentId;

        public EditStudentWindow(int? studentId = null)
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
            _studentId = studentId;

            LoadGroups();

            if (_studentId.HasValue)
            {
                LoadStudentDetails();
            }
        }

        private void LoadGroups()
        {
            var groups = _context.Groups.ToList();
            GroupComboBox.ItemsSource = groups.Select(g => new { g.Id, g.Name }).ToList();
            GroupComboBox.DisplayMemberPath = "Name";
            GroupComboBox.SelectedValuePath = "GroupId";
        }

        private void LoadStudentDetails()
        {
            var student = _context.Students.FirstOrDefault(s => s.Id == _studentId);
            if (student != null)
            {
                StudentNameTextBox.Text = student.Name;
                StudentSurnameTextBox.Text = student.Surname;
                GroupComboBox.SelectedValue = student.GroupId;
            }
            else
            {
                MessageBox.Show("Student not found.");
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_studentId.HasValue)
            {
                var student = _context.Students.FirstOrDefault(s => s.Id == _studentId);
                if (student != null)
                {
                    student.Name = StudentNameTextBox.Text;
                    student.Surname = StudentSurnameTextBox.Text;
                    student.GroupId = (int)GroupComboBox.SelectedValue;
                    _context.SaveChanges();

                    MessageBox.Show($"Student {student.Name} {student.Surname} successfully updated.");
                }
                else
                {
                    MessageBox.Show("Student not found.");
                }
            }
            else
            {
                var newStudent = new Student
                {
                    Name = StudentNameTextBox.Text,
                    Surname = StudentSurnameTextBox.Text,
                    GroupId = (int)GroupComboBox.SelectedValue
                };
                _context.Students.Add(newStudent);
                _context.SaveChanges();
                MessageBox.Show($"Student {newStudent.Name} {newStudent.Surname} successfully created.");
            }
            Close();
        }
    }
}
