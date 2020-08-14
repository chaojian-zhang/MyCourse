using MyCourse.Classes.ExportVersionControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Classes
{
    /// <summary>
    /// Collaboration Support
    /// </summary>
    [Serializable]
    class ApplicationDataExportV1
    {
        public ApplicationDataExportV1(List<Course> source)
        {
            Courses = new List<CourseExportV1>();
            foreach (Course course in source)
            {
                Courses.Add(new CourseExportV1(course));
            }
        }

        // The only data is the course library
        public List<CourseExportV1> Courses { get; set; }
        // Configuration
        public const string Suffix = ".export_version1";
    }
}
