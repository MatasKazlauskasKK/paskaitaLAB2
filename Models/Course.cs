using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace paskaita.Models
{
    public class Course
    {

        private int courseId;
        private string coursenumber;
        private string title;
        private string teachername;

        public int CourseId
        {
            get
            {
                return courseId;
            }

            set
            {
                courseId = value;
            }
        }

        public string Coursenumber
        {
            get
            {
                return coursenumber;
            }

            set
            {
                coursenumber = value;
            }
        }

        public string Title
        {
            get
            {
                return title;
            }

            set
            {
                title = value;
            }
        }

        public string Teachername
        {
            get
            {
                return teachername;
            }

            set
            {
                teachername = value;
            }
        }


        public Course(int courseId, string coursenumber, string title, string teachername)
        {
            this.courseId = courseId;
            this.coursenumber = coursenumber;
            this.title = title;
            this.teachername = teachername;
        }

        public class ClassRoom
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public int class_id { get; set; }
        }
        public class ClassRoom1
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public int class_id { get; set; }
        }
        public class Theme
        {
            public int Class_id { get; set; }
            public int id { get; set; }
            public string Title { get; set; }

            public List<ThemeBlock> ThemeBlocks { get; set; }
    }

        public class ThemeBlock
        {
            public int Theme_id { get; set; }
            public int id { get; set; }
            public string Title { get; set; }
            public string Text { get; set; }
        }
        public class ClassStudents
        {
            public int id { get; set; }
            public int class_id { get; set; }
            public int account_id { get; set; }
            public string account_name { get; set; }
            public string account_surname { get; set; }
            public string student_grade { get; set; }
            public string Title { get; set; }
        }
        public class StudentGrades
        {
            public int id { get; set; }
            public int class_id { get; set; }
            public int account_id { get; set; }
            public string UserName { get; set; }
            public string Name { get; set; }
            public string Surname { get; set; }
            public string student_grade { get; set; }
        }
    }
}