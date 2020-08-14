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

namespace MyCourse
{
    /// <summary>
    /// Basic intro, prerequisites, availability, automatically update main timetable window;
    /// Reference see: https://magellan.ece.toronto.edu/course_information.php
    /// </summary>
    public partial class CourseDetail : Window, INotifyPropertyChanged
    {
        #region Construction and Event Handling
        public CourseDetail()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            e.Cancel = true;
        }

        public void UpdateCourse(Course newCourse)
        {
            Course = newCourse;
            UnpinLabel.Visibility = newCourse.IsPinned ? Visibility.Visible : Visibility.Collapsed;
        }
        #endregion

        #region Interaction
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Course == null) return;
            if(Course.IsPinned == false)
            {
                (Owner as TimetableWindow).PinCourse(Course);
                UnpinLabel.Visibility = Visibility.Visible;
            }
            else
            {
                (Owner as TimetableWindow).UnpinCourse(Course);
                UnpinLabel.Visibility = Visibility.Collapsed;
            }
        }

        //  Just so things commit, especially textbox
        //private void Window_MouseLeave(object sender, MouseEventArgs e)
        //{
        //    Keyboard.FocusedElement.;
        //}
        #endregion

        #region Data
        private Course _Course;
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
    }
}
