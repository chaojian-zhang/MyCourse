using MyCourse.Classes.ExportVersionControl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Classes
{
    [Serializable]
    public class ApplicationData: ISerializable, INotifyPropertyChanged
    {
        #region ApplicationData Initialization
        public ApplicationData()
        {
            Courses = new List<Course>();
        }
        #endregion

        #region Data
        // Course Lirbaries
        private List<Course> Courses;
        [NonSerialized] // Because it can't be
        private ObservableCollection<Course> _ObservableCourses = null;
        // Timetable state
        // ...
        // Extra Accessor
        public List<Course> GetNewestCources()
        {
            // Do a conversion
            if (_ObservableCourses != null) Courses = new List<Course>(_ObservableCourses);
            return Courses;
        }
        #endregion

        #region Data Binding Support
        [field:NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ObservableCollection<Course> ObservableCourses
        {
            get
            {
                if (_ObservableCourses != null) return _ObservableCourses;
                else { _ObservableCourses = new ObservableCollection<Course>(Courses); return _ObservableCourses; }
            }
            set
            {
                if(value != _ObservableCourses)
                {
                    _ObservableCourses = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        #region Custom Serialization
        public ApplicationData(SerializationInfo info, StreamingContext ctxt)
        {
            // Load properties
            Courses = (List<Course>)info.GetValue("Courses", typeof(List<Course>));
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Do a conversion
            if (_ObservableCourses != null) Courses = new List<Course>(_ObservableCourses);

            // Save Properties
            info.AddValue("Courses", Courses);
        }

        internal void Import(ApplicationDataExportV1 exportData, out int add, out int merge)
        {
            add = 0;
            merge = 0;
            foreach (CourseExportV1 newCourse in exportData.Courses)
            {
                bool bMerged = false;
                // Find existing
                foreach (Course existingCourse in ObservableCourses)
                {
                    if(existingCourse.CourseCode == newCourse.CourseCode)
                    {
                        merge++;
                        // Merge into existing
                        existingCourse.Merge(newCourse);
                        bMerged = true;
                        break;
                    }
                }
                // If non-existing then create one
                if (bMerged == false)
                {
                    add++;
                    ObservableCourses.Add(new Course(newCourse));
                }
            }
        }
        #endregion
    }
}
