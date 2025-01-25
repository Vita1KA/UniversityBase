using System.Windows;

namespace Task8
{
    public partial class EditTeacherWindow : Window
    {
        private readonly ApplicationDbContext _context;
        private readonly int? _teacherId;

        public EditTeacherWindow(int? teacherId = null)
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
            _teacherId = teacherId;

            LoadGroups();

            if (_teacherId.HasValue)
            {
                LoadTeacherDetails();
            }
        }

        public void LoadGroups()
        {
            var groups = _context.Groups.ToList();
            GroupComboBox.ItemsSource = groups.Select(g => new { g.Id, g.Name }).ToList();
            GroupComboBox.DisplayMemberPath = "Name";
            GroupComboBox.SelectedValuePath = "GroupId";
        }

        public void LoadTeacherDetails()
        {
            var teacher = _context.Teachers.FirstOrDefault(t => t.Id == _teacherId);
            if (teacher != null)
            {
                TeacherNameTextBox.Text = teacher.Name;
                TeacherSurnameTextBox.Text = teacher.Surname;
            }
            else
            {
                MessageBox.Show("Teacher not found.");
            }
        }

        public void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_teacherId.HasValue)
            {
                var teacher = _context.Teachers.FirstOrDefault(t => t.Id == _teacherId);
                if (teacher != null)
                {
                    teacher.Name = TeacherNameTextBox.Text;
                    teacher.Surname = TeacherSurnameTextBox.Text;
                    _context.SaveChanges();

                    MessageBox.Show($"Teacher {teacher.Name} {teacher.Surname} updated successfully.");
                }
                else
                {
                    MessageBox.Show("Teacher not found.");
                }
            }
            else
            {
                var newTeacher = new Teacher
                {
                    Name = TeacherNameTextBox.Text,
                    Surname = TeacherSurnameTextBox.Text
                };
                _context.Teachers.Add(newTeacher);
                _context.SaveChanges();
                MessageBox.Show($"Teacher {newTeacher.Name} {newTeacher.Surname} created successfully.");
            }
            Close();
        }
    }
}
