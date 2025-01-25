using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Xceed.Words.NET;


namespace Task8
{
    public partial class EditGroupWindow : Window
    {
        private readonly ApplicationDbContext _context;
        private ServiceGroup _serviceGroup;

        public EditGroupWindow()
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
            _serviceGroup = new ServiceGroup(_context);
            LoadGroups();
            LoadTeachers();
            LoadCourses();
        }

        private void LoadCourses()
        {
            CourseComboBox.Items.Clear();
            var courses = _context.Courses.ToList();
            CourseComboBox.ItemsSource = courses;
            CourseComboBox.DisplayMemberPath = "Name";
            CourseComboBox.SelectedValuePath = "CourseId";
        }

        private void LoadGroups()
        {
            var groups = _context.Groups
                .Include(g => g.Teacher)
                .ToList();

            GroupListBox.ItemsSource = groups.Select((g, index) => new
            {
                g.Id,
                DisplayText = $"{index + 1}. GroupId = {g.Id}, Name = {g.Name}, Teacher = {(g.Teacher != null ? $"{g.Teacher.Name} {g.Teacher.Surname}" : "None")}"
            }).ToList();

            GroupListBox.DisplayMemberPath = "DisplayText";
            GroupListBox.SelectedValuePath = "GroupId";
        }

        private void LoadTeachers()
        {
            TeacherComboBox.Items.Clear();
            var teachers = _context.Teachers.ToList();
            TeacherComboBox.ItemsSource = teachers
                .Select(t => new { t.Id, Name = $"{t.Name} {t.Surname}" })
                .ToList();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var groupName = GroupNameTextBox.Text;
            var selectedTeacher = TeacherComboBox.SelectedItem as dynamic;
            var selectedCourse = CourseComboBox.SelectedItem as dynamic;

            if (string.IsNullOrEmpty(groupName))
            {
                MessageBox.Show("Please enter a group name.");
                return;
            }

            if (selectedCourse is null)
            {
                MessageBox.Show("Please select a course.");
                return;
            }

            var existingGroup = _context.Groups.FirstOrDefault(g => g.Name == groupName);

            if (existingGroup != null)
            {
                MessageBox.Show("A group with this name already exists. Please choose a different name.");
                return;
            }

            var newGroup = new Group
            {
                Name = groupName,
                TeacherId = selectedTeacher?.TeacherId,
                CourseId = selectedCourse.CourseId
            };

            _context.Groups.Add(newGroup);

            try
            {
                _context.SaveChanges();
                LoadGroups();
                MessageBox.Show("Group created successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving group: {ex.Message}");
            }

            Close();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedGroup = GroupListBox.SelectedItem as dynamic;

            if (selectedGroup is null)
            {
                MessageBox.Show("No group selected.");
                return;
            }

            var groupId = (int)selectedGroup.GroupId;
            var group = _context.Groups
            .Include(g => g.Students)
            .FirstOrDefault(g => g.Id == groupId);


            if (group is null)
            {
                MessageBox.Show("Group not found.");
                return;
            }

            if (group.Students.Count > 0)
            {
                MessageBox.Show("Cannot delete group with students.");
                return;
            }

            _context.Groups.Remove(group);
            _context.SaveChanges();
            LoadGroups();
            MessageBox.Show("Group deleted successfully.");
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedGroup = GroupListBox.SelectedItem as dynamic;

            if (selectedGroup is null)
            {
                MessageBox.Show("No group selected.");
                return;
            }

            var groupId = (int)selectedGroup.GroupId;
            var group = _context.Groups
            .Include(g => g.Students)
            .FirstOrDefault(g => g.Id == groupId);

            if (group is null)
            {
                MessageBox.Show("Group not found.");
                return;
            }

            var newGroupName = GroupNameTextBox.Text;
            var existingGroup = _context.Groups.FirstOrDefault(g => g.Name == newGroupName && g.Id != groupId);

            if (existingGroup != null)
            {
                MessageBox.Show("A group with this name already exists.");
                return;
            }

            group.Name = newGroupName;
            var selectedTeacher = TeacherComboBox.SelectedItem as dynamic;
            group.TeacherId = selectedTeacher != null ? selectedTeacher.TeacherId : (int?)null;

            _context.Groups.Update(group);
            _context.SaveChanges();
            LoadGroups();
            MessageBox.Show("Group updated successfully.");
        }

        private void ExportCsvButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedGroup = GroupListBox.SelectedItem as dynamic;

            if (selectedGroup is null)
            {
                MessageBox.Show("No group selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int groupId = (int)selectedGroup.Id;

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                Title = "Save CSV File"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;

                try
                {
                    _serviceGroup.ExportStudentsToCsv(groupId, filePath);
                    MessageBox.Show("Export successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (FileNotFoundException ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ImportCsvButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedGroup = GroupListBox.SelectedItem as dynamic;

            if (selectedGroup is null)
            {
                MessageBox.Show("No group selected.");
                return;
            }

            var groupId = (int)selectedGroup.Id;

            var openFileDialog = new OpenFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                Title = "Open CSV File"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                try
                {
                    _serviceGroup.ImportStudentsFromCsv(groupId, filePath);
                    LoadGroups();
                    MessageBox.Show("Students imported successfully.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }
        }

        private void GroupListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedGroup = GroupListBox.SelectedItem as dynamic;

            if (selectedGroup != null)
            {
                var groupId = (int)selectedGroup.GroupId;
                var group = _context.Groups
                    .Include(g => g.Teacher)
                    .FirstOrDefault(g => g.Id == groupId);

                if (group != null)
                {
                    GroupNameTextBox.Text = group.Name;
                    var teachers = TeacherComboBox.ItemsSource.Cast<dynamic>().ToList();
                    TeacherComboBox.SelectedItem = teachers.FirstOrDefault(t => t.TeacherId == group.TeacherId);
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void GenerateDocButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedGroup = GroupListBox.SelectedItem as dynamic;

            if (selectedGroup == null)
            {
                MessageBox.Show("No group selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int groupId = (int)selectedGroup.Id;

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Word Document (*.docx)|*.docx|All files (*.*)|*.*",
                Title = "Save Document"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;

                try
                {
                    _serviceGroup.GenerateDoc(groupId, filePath);
                    MessageBox.Show("Document generated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (FileNotFoundException ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void GeneratePdfButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedGroup = GroupListBox.SelectedItem as dynamic;

            if (selectedGroup == null)
            {
                MessageBox.Show("No group selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int groupId = (int)selectedGroup.Id;

            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*",
                Title = "Save PDF Document"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;

                try
                {
                    _serviceGroup.GeneratePdf(groupId, filePath);
                    MessageBox.Show($"PDF document generated successfully and saved to {filePath}.",
                        "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (FileNotFoundException ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving PDF document: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}    