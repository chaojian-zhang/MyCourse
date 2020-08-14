using MyCourse.Classes;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyCourse.Components
{
    /// <summary>
    /// Interaction logic for CourseSlot.xaml
    /// </summary>
    public partial class CourseSlot : UserControl
    {
        public CourseSlot(Course course, TimeSlot slot, Color surfaceColor, Timetable owner)
        {
            InitializeComponent();
            CourseStack.DataContext = course;
            TimeStack.DataContext = slot;
            CourseStackTooltip.DataContext = course;
            TimeStackTooltip.DataContext = slot;
            // surfaceColor.A = (byte)128;
            OuterBorder.Background = new SolidColorBrush(surfaceColor);

            // Bookeeping
            OwnerTable = owner;
            Course = course;
            Slot = slot;
        }

        #region Interaction
        public Course Course { get; set; }
        public TimeSlot Slot { get; set; }
        public Timetable OwnerTable { get; set; }
        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            OwnerTable.ShowOnlyCurrentSlot(Course, Slot);
        }
        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            OwnerTable.UnShowOnlyCurrentSlot();
        }
        #endregion
    }
}
