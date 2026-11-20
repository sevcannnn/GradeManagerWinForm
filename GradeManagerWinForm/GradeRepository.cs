using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

namespace GradeManagerWinForm
{
    public class GradeRepository
    {
        public List<StudentItem> GetStudents()
        {
            var list = new List<StudentItem>();
            using (var conn = new SqlConnection(Db.ConnStr))
            using (var cmd = new SqlCommand(
                "SELECT StudentId, FirstName + ' ' + LastName AS FullName FROM dbo.Students ORDER BY FirstName, LastName",
                conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new StudentItem
                        {
                            Id = reader.GetInt32(0),
                            FullName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1)
                        });
                    }
                }
            }
            return list;
        }

        public List<GradeRow> GetGrades()
        {
            var list = new List<GradeRow>();
            using (var conn = new SqlConnection(Db.ConnStr))
            using (var cmd = new SqlCommand(@"
                SELECT g.GradeId,
                       g.StudentId,
                       s.FirstName + ' ' + s.LastName AS StudentName,
                       g.Subject,
                       g.Grade
                FROM dbo.Grades g
                INNER JOIN dbo.Students s ON g.StudentId = s.StudentId
                ORDER BY g.GradeId DESC", conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new GradeRow
                        {
                            GradeId = reader.GetInt32(0),
                            StudentId = reader.GetInt32(1),
                            StudentName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                            Subject = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                            Grade = reader.IsDBNull(4) ? 0 : reader.GetInt32(4)
                        });
                    }
                }
            }
            return list;
        }

        public void AddGrade(int studentId, string subject, int grade)
        {
            using (var conn = new SqlConnection(Db.ConnStr))
            using (var cmd = new SqlCommand(
                "INSERT INTO dbo.Grades (StudentId, Subject, Grade) VALUES (@StudentId, @Subject, @Grade)",
                conn))
            {
                cmd.Parameters.Add("@StudentId", SqlDbType.Int).Value = studentId;
                cmd.Parameters.Add("@Subject", SqlDbType.NVarChar, 50).Value = subject;
                cmd.Parameters.Add("@Grade", SqlDbType.Int).Value = grade;

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateGrade(int gradeId, int studentId, string subject, int grade)
        {
            using (var conn = new SqlConnection(Db.ConnStr))
            using (var cmd = new SqlCommand(
                "UPDATE dbo.Grades SET StudentId = @StudentId, Subject = @Subject, Grade = @Grade WHERE GradeId = @GradeId",
                conn))
            {
                cmd.Parameters.Add("@GradeId", SqlDbType.Int).Value = gradeId;
                cmd.Parameters.Add("@StudentId", SqlDbType.Int).Value = studentId;
                cmd.Parameters.Add("@Subject", SqlDbType.NVarChar, 50).Value = subject;
                cmd.Parameters.Add("@Grade", SqlDbType.Int).Value = grade;

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteGrade(int gradeId)
        {
            using (var conn = new SqlConnection(Db.ConnStr))
            using (var cmd = new SqlCommand(
                "DELETE FROM dbo.Grades WHERE GradeId = @GradeId",
                conn))
            {
                cmd.Parameters.Add("@GradeId", SqlDbType.Int).Value = gradeId;

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}