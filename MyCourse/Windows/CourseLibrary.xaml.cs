using MyCourse.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
    /// A grid view of all currently added courses, grouped by type; Editable details are provided
    /// </summary>
    public partial class CourseLibrary : Window
    {
        public CourseLibrary()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateSource();
        }

        internal void UpdateSource()
        {
            // Data Grid Setup
            ListCollectionView groupedCollection = new ListCollectionView((App.Current as App).Data.ObservableCourses);
            groupedCollection.GroupDescriptions.Add(new PropertyGroupDescription("Type"));
            CourseListGrid.ItemsSource = groupedCollection;
            CourseListGrid.SelectedItem = null;
        }

        private void CourseListGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(Owner != null) (Owner as TimetableWindow).ChangeSelection(CourseListGrid.SelectedItem as Course);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            e.Cancel = true;
        }

        #region UI Improvement - Single Click Editing
        // Well actually no need to do that because allowing selection makes deleting rows more intuitive
        // Ref: https://stackoverflow.com/questions/3426765/single-click-edit-in-wpf-datagrids
        // https://stackoverflow.com/questions/10302173/single-click-edit-on-wpf-datagrid-combobox-template-column
        // https://stackoverflow.com/questions/3426765/single-click-edit-in-wpf-datagrid
        #endregion

        #region Library Management - Avoid Repeated Names
        private bool bCellChanged = false;
        private Course editingCourse = null;
        private void CourseListGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            bCellChanged = true;
            if((string)e.Column.Header == "Course Code")
            {
                editingCourse = CourseListGrid.SelectedItem as Course;
            }
        }

        private void CourseListGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            // Repeatition Check
            if(bCellChanged && editingCourse != null)
            {
                // <Performance> Cautious performance
                int repeatCount = (App.Current as App).Data.GetNewestCources().Where(item => item.CourseCode.IndexOf(editingCourse.CourseCode) == 0).Count();
                if (repeatCount > 0) editingCourse.CourseCode = editingCourse.CourseCode + " " + repeatCount;
            }
            bCellChanged = false;
        }
        #endregion
    }
}
