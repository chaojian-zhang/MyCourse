using MyCourse.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyCourse.Components
{
    /// <summary>
    /// Interaction logic for Timetable.xaml
    /// </summary>
    public partial class Timetable : UserControl, INotifyPropertyChanged
    {
        public Timetable()
        {
            InitializeComponent();

            // Initialization
            PinnedCourses = new ObservableCollection<Course>();
        }
        private Random rnd = new Random();

        #region Interface
        // No need to explicitly remove previous shown course
        public void UpdateCourse(Course course)
        {
            DisplayingCourse = course;
            UpdateDisplay();
        }
        public void PinCourse(Course course)
        {
            if (PinnedCourses.Contains(course) == false) PinnedCourses.Add(course);
            UpdateDisplay();
        }

        public void UnpinCourse(Course course)
        {
            if (PinnedCourses.Contains(course) == true) PinnedCourses.Remove(course);
            UpdateDisplay();
        }

        // One time update
        internal void PinCourses(List<Course> pinnedCourses)
        {
            PinnedCourses = new ObservableCollection<Course>(pinnedCourses);
            UpdateDisplay();
        }

        internal void ShowOnlyCurrentSlot(Course course, TimeSlot slot)
        {
            foreach (UIElement element in TimetableGrid.Children)
            {
                CourseSlot courseSlot = element as CourseSlot;
                if(courseSlot != null && courseSlot.Course == course && 
                    (courseSlot.Slot.Type == slot.Type && courseSlot.Slot.SectionCode == slot.SectionCode))
                    courseSlot.Visibility = Visibility.Visible;
                else courseSlot.Visibility = Visibility.Collapsed;
            }
        }

        internal void UnShowOnlyCurrentSlot()
        {
            foreach (UIElement element in TimetableGrid.Children)
            {
                element.Visibility = Visibility.Visible;
            }
        }

        #endregion

        #region Helper
        private void UpdateDisplay()
        {
            // Clear
            TimetableGrid.Children.Clear();
            // Show pineed courses
            foreach (Course course in PinnedCourses) AddCourseToDisplay(course);
            // Show displaying course if not already shown
            if (DisplayingCourse != null && PinnedCourses.Contains(DisplayingCourse) == false) AddCourseToDisplay(DisplayingCourse);
        }
        private void AddCourseToDisplay(Course course)
        {
            // Generate random color
            byte r = (byte)rnd.Next(64, 192 + 1);
            byte g = (byte)rnd.Next(64, 192 + 1);
            byte b = (byte)rnd.Next(64, 192 + 1);
            Color courseColor = Color.FromRgb(r, g, b); // Not used because that's not distinct enough

            // Show relavent time slots
            List<TimeSlot> termSlots = course.TimeSlots.Where(item => item.Availability == _TermName).ToList();
            // Section info
            string currentSectionName = termSlots.Count > 0 ? termSlots[0].SectionNameText : string.Empty;
            Color currentSectionColor = Color.FromRgb((byte)rnd.Next(64, 192 + 1), (byte)rnd.Next(64, 192 + 1), (byte)rnd.Next(64, 192 + 1));
            foreach (TimeSlot slot in termSlots)
            {
                // Section compare info
                string sectionNameCompare = slot.SectionNameText;
                if(sectionNameCompare != currentSectionName)
                {
                    currentSectionColor = Color.FromRgb((byte)rnd.Next(64, 192 + 1), (byte)rnd.Next(64, 192 + 1), (byte)rnd.Next(64, 192 + 1));
                    currentSectionName = sectionNameCompare;
                }
                // Generate Slot
                CourseSlot newSlot = new CourseSlot(course, slot, currentSectionColor, this);
                newSlot.SetValue(Grid.RowProperty, slot.StartTime - 8);
                newSlot.SetValue(Grid.ColumnProperty, (int)slot.Day - 1);
                newSlot.SetValue(Grid.RowSpanProperty, slot.TimeSpan);
                TimetableGrid.Children.Add(newSlot);
            }
        }
        #endregion

        #region Data
        private Availability _TermName = Availability.Fall;
        public Course DisplayingCourse;
        public ObservableCollection<Course> _ShowingCourses;
        #endregion

        #region Data Binding
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public Availability TermName
        {
            get { return this._TermName; }
            set
            {
                if (value != this._TermName)
                {
                    this._TermName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ObservableCollection<Course> PinnedCourses
        {
            get { return this._ShowingCourses; }
            set
            {
                if (value != this._ShowingCourses)
                {
                    this._ShowingCourses = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

    }
}
