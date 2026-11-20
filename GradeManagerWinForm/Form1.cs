using System;
using System.Linq;
using System.Windows.Forms;

namespace GradeManagerWinForm
{
    public partial class Form1 : Form
    {
        private readonly GradeRepository _repo = new GradeRepository();
        private int? _selectedGradeId = null;

        public Form1()
        {
            InitializeComponent();
            dvgGrades.SelectionChanged += dvgGrades_SelectionChanged;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cmbCourse.Items.Clear();
            cmbCourse.Items.AddRange(new object[]
            {
                "Riyaziyyat",
                "Proqramlaşdırma",
                "Verilənlər Bazası",
                "Şəbəkələr"
            });
            if (cmbCourse.Items.Count > 0)
                cmbCourse.SelectedIndex = 0;

            LoadStudents();
            RefreshGradesAndAverage();
        }

        private void LoadStudents()
        {
            var students = _repo.GetStudents();
            cmbStudents.DataSource = null;
            cmbStudents.DisplayMember = "FullName";
            cmbStudents.ValueMember = "Id";
            cmbStudents.DataSource = students;
        }

        private void RefreshGradesAndAverage()
        {
            var grades = _repo.GetGrades();
            dvgGrades.DataSource = null;
            dvgGrades.DataSource = grades;

            if (dvgGrades.Columns["GradeId"] != null)
                dvgGrades.Columns["GradeId"].Visible = false;
            if (dvgGrades.Columns["StudentId"] != null)
                dvgGrades.Columns["StudentId"].Visible = false;

            if (grades.Count > 0)
            {
                double avg = grades.Average(g => g.Grade);
                lblAverage.Text = $"Orta bal: {avg:0.00}";
            }
            else
            {
                lblAverage.Text = "Orta bal: -";
            }
        }

        private bool TryReadFormInputs(out int studentId, out string subject, out int score)
        {
            studentId = 0;
            subject = cmbCourse.Text.Trim();
            score = 0;

            if (!(cmbStudents.SelectedValue is int selectedId) || selectedId <= 0)
            {
                MessageBox.Show("Zəhmət olmasa tələbə seç.", "Xəbərdarlıq", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(subject))
            {
                MessageBox.Show("Zəhmət olmasa fənn seç və ya yaz.", "Xəbərdarlıq", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!int.TryParse(txtScore.Text.Trim(), out score) || score < 0 || score > 100)
            {
                MessageBox.Show("Bal 0 ilə 100 arasında tam ədəd olmalıdır.", "Xəbərdarlıq", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            studentId = selectedId;
            return true;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!TryReadFormInputs(out int studentId, out string subject, out int score))
                return;

            _repo.AddGrade(studentId, subject, score);
            txtScore.Clear();
            _selectedGradeId = null;
            RefreshGradesAndAverage();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!_selectedGradeId.HasValue)
            {
                MessageBox.Show("Yeniləmək üçün cədvəldən sətr seç.", "Xəbərdarlıq", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!TryReadFormInputs(out int studentId, out string subject, out int score))
                return;

            _repo.UpdateGrade(_selectedGradeId.Value, studentId, subject, score);
            RefreshGradesAndAverage();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!_selectedGradeId.HasValue)
            {
                MessageBox.Show("Silmək üçün cədvəldən sətr seç.", "Xəbərdarlıq", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show("Seçilmiş qiyməti silmək istəyirsən?", "Təsdiq", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
                return;

            _repo.DeleteGrade(_selectedGradeId.Value);
            txtScore.Clear();
            _selectedGradeId = null;
            RefreshGradesAndAverage();
        }

        private void dvgGrades_SelectionChanged(object sender, EventArgs e)
        {
            if (dvgGrades.CurrentRow == null)
                return;

            var row = dvgGrades.CurrentRow.DataBoundItem as GradeRow;
            if (row == null)
                return;

            _selectedGradeId = row.GradeId;
            cmbStudents.SelectedValue = row.StudentId;
            cmbCourse.Text = row.Subject;
            txtScore.Text = row.Grade.ToString();
        }

        private void cmbStudents_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void btnAdd_Click_1(object sender, EventArgs e)
        {
            btnAdd_Click(sender, e);
        }

        private void cmbCourse_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void btnUpdate_Click_1(object sender, EventArgs e)
        {
            btnUpdate_Click(sender, e);
        }

        private void txtScore_TextChanged(object sender, EventArgs e)
        {
        }

        private void btnDelete_Click_1(object sender, EventArgs e)
        {
            btnDelete_Click(sender, e);
        }

        private void lblAverage_Click(object sender, EventArgs e)
        {
            RefreshGradesAndAverage();
        }

        private void dvgGrades_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            dvgGrades_SelectionChanged(sender, e);
        }
    }
}