using MyCourse.Classes;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace MyCourse.Windows
{
    // Important Note: CanUserSortColumns needs to be false otherwise when user does the following execeptions occuring during "TimeSlotsList.ItemsSource = groupedCollection;" due to unable to sort during add/edit item:
    // 1. Double click to create a new course in Course Library 2. Do not enter "Enter" but just click header to sort courses 3. Create a new course and don't click enter 4. Use that method to add time slots to course A and sort it then select course B

    /// <summary>
    /// Interaction logic for Timeslots.xaml
    /// </summary>
    public partial class Timeslots : Window, INotifyPropertyChanged
    {
        public Timeslots()
        {
            InitializeComponent();
        }

        public void UpdateCourse(Course course)
        {
            Course = course;

            // Data Grid Setup
            if (Course != null)
            {
                ListCollectionView groupedCollection = new ListCollectionView(Course.TimeSlots);
                groupedCollection.GroupDescriptions.Add(new PropertyGroupDescription("Availability"));
                TimeSlotsList.ItemsSource = groupedCollection;
            }
            else
                TimeSlotsList.ItemsSource = null;
        }

        private Course _Course;

        #region Data Binding
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public Course Course
        {
            get { return this._Course; }
            set
            {
                if (value != this._Course)
                {
                    this._Course = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        #region Interactions
        private bool bCellChanged = false;
        private void TimeSlotsList_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            bCellChanged = true;
        }

        private void TimeSlotsList_CurrentCellChanged(object sender, EventArgs e)
        {
            if(bCellChanged) (Owner as TimetableWindow).UpdateTimeTable(Course);
            bCellChanged = false;
        }

        private bool bAddingNewRow = false;
        private void TimeSlotsList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            // Intelligent creation
            if(bAddingNewRow && PrevSlots.Count == 1)
            {
                TimeSlot slot = e.Row.DataContext as TimeSlot;
                if(NewSlots.Count == 1 && NewSlots[0] == slot)
                {
                    TimeSlot prevSlot = PrevSlots.Last();
                    // Copy Data
                    slot.Availability = prevSlot.Availability;
                    slot.Day = prevSlot.Day;
                    slot.Location = prevSlot.Location;
                    slot.SectionCode = prevSlot.SectionCode;
                    slot.StartTime = prevSlot.StartTime;
                    slot.TimeSpan = prevSlot.TimeSpan;
                    slot.Type = prevSlot.Type;
                }
            }
            bAddingNewRow = false;
        }

        private void TimeSlotsList_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            bAddingNewRow = true;
        }

        private List<TimeSlot> PrevSlots;
        private List<TimeSlot> NewSlots;
        private void CourseListGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                PrevSlots = new List<TimeSlot>(e.RemovedItems.Cast<TimeSlot>());
                NewSlots = new List<TimeSlot>(e.AddedItems.Cast<TimeSlot>());
            }
            catch (Exception)
            {
                PrevSlots = new List<TimeSlot>();
                NewSlots = new List<TimeSlot>();
            }
        }
        #endregion
    }
}
