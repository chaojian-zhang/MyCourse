using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Classes.ExportVersionControl
{
    [Serializable]
    public class CourseExportV1
    {
        public CourseExportV1(Course course)
        {
            this.CourseCode = course.CourseCode;
            this.Title = course.SubjectName;
            this.Description = course.Description;
            this.CreditWeight = course.CreditWeight;
            this.Type = course.Type;
            this.AU = course.AU;
            this.Corequisites = course.Corequisites;
            this.Prerequisites = course.Prerequisites;
            this.Exclusions = course.Exclusions;
            this.TimeSlots = course.TimeSlots;
        }

        // Data
        public string CourseCode { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public float CreditWeight { get; set; }
        public CourseType Type { get; set; }
        public CEABAU AU { get; set; }
        // Just to save some trouble doing conversion and since ObsevableCollection is serializable anyway, we just use it instead of List<>
        public ObservableCollection<string> Corequisites { get; set; }
        public ObservableCollection<string> Prerequisites { get; set; }
        public ObservableCollection<string> Exclusions { get; set; }
        public ObservableCollection<TimeSlot> TimeSlots { get; set; }
    }
}
