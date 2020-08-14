using MyCourse.Windows;
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
using MyCourse.Classes;
using Microsoft.Win32;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;

namespace MyCourse
{
    /// <summary>
    /// Details of all courses, can be pinned for overlay check, not directly editable
    /// </summary>
    public partial class TimetableWindow : Window
    {
        #region Initialization and Event Handling
        public TimetableWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Translate self a bit
            this.Top = this.Top / 2;
            // Create Other Windows
            LibraryWindow = new CourseLibrary();
            LibraryWindow.Show();
            LibraryWindow.Left = this.Left - LibraryWindow.ActualWidth;
            LibraryWindow.Top = this.Top;
            LibraryWindow.Height = this.Height;
            LibraryWindow.Owner = this;
            CEABWindow = new UTEngineeringCEAB();
            CEABWindow.Show();
            CEABWindow.Left = this.Left;
            CEABWindow.Top = this.Top + this.ActualHeight;
            CEABWindow.Width = this.ActualWidth;
            CEABWindow.Owner = this;
            InformationWindow = new CourseDetail();
            InformationWindow.Show();
            InformationWindow.Left = this.Left + this.ActualWidth;
            InformationWindow.Top = this.Top;
            InformationWindow.Owner = this;
            InformationWindow.Height = this.ActualHeight;
            TipsWindow = new Tips();
            TipsWindow.Show();
            TipsWindow.Left = LibraryWindow.Left;
            TipsWindow.Top = LibraryWindow.Top + LibraryWindow.ActualHeight;
            TipsWindow.Width = LibraryWindow.Width;
            TipsWindow.Owner = this;
            TimeslotsWindow = new Timeslots();
            TimeslotsWindow.Show();
            TimeslotsWindow.Left = this.Left + this.ActualWidth;
            TimeslotsWindow.Top = InformationWindow.Top + InformationWindow.ActualHeight;
            TimeslotsWindow.Width = InformationWindow.Width;
            TimeslotsWindow.Owner = this;

            // Status Window
            Status = new StatusPromt(this);
            Status.Update(StatusEnum.Welcome, "Welcome to MyCourse, add a new course to begin planning!", ActionsEnum.None, PlacementEnum.UpperRight);

            // Show pinned courses
            List<Course> pinnedCourses = (App.Current as App).Data.GetNewestCources().Where(item => item.IsPinned).ToList();
            FallTimeTable.PinCourses(pinnedCourses);
            WinterTimeTable.PinCourses(pinnedCourses);
        }
        #endregion

        #region Windows
        private CourseLibrary LibraryWindow;
        private UTEngineeringCEAB CEABWindow;
        private CourseDetail InformationWindow;
        private Tips TipsWindow;
        private Timeslots TimeslotsWindow;
        private StatusPromt Status;
        #endregion

        #region Interaction
        internal void PinCourse(Course course)
        {
            FallTimeTable.PinCourse(course);
            WinterTimeTable.PinCourse(course);
            course.IsPinned = true;
        }
        internal void UnpinCourse(Course course)
        {
            FallTimeTable.UnpinCourse(course);
            WinterTimeTable.UnpinCourse(course);
            course.IsPinned = false;
        }
        internal void ChangeSelection(Course course)
        {
            if(course != null)
            {
                // Update Information Window
                InformationWindow.UpdateCourse(course);
                // Update Timeslots Window
                TimeslotsWindow.UpdateCourse(course);
                // Update Timetable Preview Display
                UpdateTimeTable(course);
            }
        }

        internal void UpdateTimeTable(Course course)
        {
            FallTimeTable.UpdateCourse(course);
            WinterTimeTable.UpdateCourse(course);
            course.NotifyTimeSlotsChanged();
        }
        #endregion

        #region Timetable Generation
        private bool bBackgroundGenerationRunning = false;

        private async Task<string> GenerateTimetableAsync(int courseCount, List<Course> interestedCourses)
        {
            return await Task.Run(() => GenerateTimeTable(courseCount, interestedCourses));
        }
        // Return path to folder
        private string GenerateTimeTable(int courseCount, List<Course> interestedCourses)
        {
            // Check folder existence
            string optimizedFolderpath = System.IO.Path.Combine((App.Current as App).DataFolderPath, "GeneratedTimeTableFor" + courseCount);
            if (System.IO.Directory.Exists(optimizedFolderpath) == false) System.IO.Directory.CreateDirectory(optimizedFolderpath);
            string conflictFolderpath = System.IO.Path.Combine((App.Current as App).DataFolderPath, "GeneratedTimeTableFor" + courseCount + "_WithConflicts");
            if (System.IO.Directory.Exists(conflictFolderpath) == false) System.IO.Directory.CreateDirectory(conflictFolderpath);

            // Approach 1: Iterative Routine for Generating an Optimized Timetable: <Needs proficiency improvement with such arithmetic algorithms>
            // 1. Get all combinations: n is total, k is selected, then n!/(k!(n-k)!) possibilities; E.g. for (50, 5) it's 211,8760 possibilities
            // 2. For each combination, get all its timeslot combinations: a course with z lecture sections x tutorial sections and c practice sections will have z*x*c possibilitiess, thus k courses will have (z*x*c)^~5 combinations
            // 3. Record each timeslot combination's conflict count
            // 4. Among all timeslot combinations, sort by conflict count
            // 5. Timeslot combinations without any conflict is the optimized ones; Otherwise we return least conflicted course


            // Pack return results into ApplicationData then output to folder
            // ...
        }
        #endregion

        #region Menus
        private void ToolHeader_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            // Initial
            GenerateFourMenu.IsEnabled = false;
            GenerateFiveMenu.IsEnabled = false;
            GenerateSixMenu.IsEnabled = false;
            GenerateSevenMenu.IsEnabled = false;

            // If currently running, then disable buttons
            if(!bBackgroundGenerationRunning)
            {
                int interestedCount = (App.Current as App).Data.ObservableCourses.Where(item => item.IsInterested == true).Count();
                if(interestedCount >= 4) GenerateFourMenu.IsEnabled = true;
                if (interestedCount >= 5) GenerateFiveMenu.IsEnabled = true;
                if (interestedCount >= 6) GenerateSixMenu.IsEnabled = true;
                if (interestedCount >= 7) GenerateSevenMenu.IsEnabled = true;
            }
        }

        private void WindowHeader_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            CourseLibraryMenu.IsChecked = LibraryWindow.Visibility == Visibility.Visible;
            CourseDetailMenu.IsChecked = InformationWindow.Visibility == Visibility.Visible;
            ReferenceMenu.IsChecked = TipsWindow.Visibility == Visibility.Visible;
            CEABMenu.IsChecked = CEABWindow.Visibility == Visibility.Visible;
            TimeSlotMenu.IsChecked = TimeslotsWindow.Visibility == Visibility.Visible;
        }

        private async Task GenerateTimetableSetup(int courseCount)
        {
            List<Course> interested = (App.Current as App).Data.GetNewestCources().Where(item => item.IsInterested == true).ToList();   // User GetNewestCourse instead of ObservableCourses so user can keep editing
            if (interested.Count() >= courseCount)
            {
                Status.Update(StatusEnum.Notice, "Generating timetable in background.", ActionsEnum.None, PlacementEnum.UpperRight);
                bBackgroundGenerationRunning = true;
                string path = await GenerateTimetableAsync(courseCount, interested);
                Status.Update(string.Format("Timetable generated at: {0}", path));
                bBackgroundGenerationRunning = false;
            }
            else
                Status.Update(StatusEnum.Notice, "Not enough interested courses; Select more than four to generate a timetable.", ActionsEnum.None, PlacementEnum.UpperRight);
        }
        private async void Generate4_Click(object sender, RoutedEventArgs e)
        {
            await GenerateTimetableSetup(4);
        }

        private async void Generate5_Click(object sender, RoutedEventArgs e)
        {
            await GenerateTimetableSetup(5);
        }

        private async void Generate6_Click(object sender, RoutedEventArgs e)
        {
            await GenerateTimetableSetup(6);
        }

        private async void Generate7_Click(object sender, RoutedEventArgs e)
        {
            await GenerateTimetableSetup(7);
        }

        // REf: https://stackoverflow.com/questions/307688/how-to-download-a-file-from-a-url-in-c
        private void Download_Click(object sender, RoutedEventArgs e)
        {
            using (var client = new WebClient())
            {
                try
                {
                    string path = System.IO.Path.Combine((App.Current as App).DataFolderPath, "data_latestest.export_version1");
                    client.DownloadFile("http://www.totalimagine.com/Service/MyCourse/Database/data_latestest.export_version1", path);
                    Status.Update(string.Format("File downloaded to: {0}", path));
                    // Don't automatically import into current ApplicationData because user might not want to
                }
                catch (Exception)
                {
                    Status.Update("Download failed.");
                    return;
                }
            }
        }

        private void Parse_Click(object sender, RoutedEventArgs e)
        {
            // Select
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Eliminated CSV files (*.eliminated_csv)|*.eliminated_csv|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = (App.Current as App).DataFolderPath;
            if (openFileDialog.ShowDialog() == true)
            {
                // Update Status
                Status.Update("Begin parsing....");
                // Do job
                string filePath = (App.Current as App).ExportFromCSV(openFileDialog.FileName);
                // Update Status
                Status.Update("Parsing done, file output: " + filePath);
            }
        }

        private void ToggleLibrary_Click(object sender, RoutedEventArgs e)
        {
            if (LibraryWindow.Visibility == Visibility.Hidden) LibraryWindow.Visibility = Visibility.Visible;
            else LibraryWindow.Visibility = Visibility.Hidden;
        }

        private void ToggleDetail_Click(object sender, RoutedEventArgs e)
        {
            if (InformationWindow.Visibility == Visibility.Hidden) InformationWindow.Visibility = Visibility.Visible;
            else InformationWindow.Visibility = Visibility.Hidden;
        }

        private void ToggleReference_Click(object sender, RoutedEventArgs e)
        {
            if (TipsWindow.Visibility == Visibility.Hidden) TipsWindow.Visibility = Visibility.Visible;
            else TipsWindow.Visibility = Visibility.Hidden;
        }

        private void ToggleCEAB_Click(object sender, RoutedEventArgs e)
        {
            if (CEABWindow.Visibility == Visibility.Hidden) CEABWindow.Visibility = Visibility.Visible;
            else CEABWindow.Visibility = Visibility.Hidden;
        }

        private void ToggleTimeSlots_Click(object sender, RoutedEventArgs e)
        {
            if (TimeslotsWindow.Visibility == Visibility.Hidden) TimeslotsWindow.Visibility = Visibility.Visible;
            else TimeslotsWindow.Visibility = Visibility.Hidden;
        }

        private void OpenDataFolder_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start((App.Current as App).DataFolderPath);
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Export version 1 files (*.export_version1)|*.export_version1|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = (App.Current as App).DataFolderPath;
            if (openFileDialog.ShowDialog() == true)
            {
                // Update Status
                Status.Update("Importing...");
                // Do Action
                int add = 0, merge = 0;
                (App.Current as App).ImportApplicationData(openFileDialog.FileName, out add, out merge);
                // Update Status
                Status.Update(string.Format("Import finished with {0} courses added and {1} courses merged.", add, merge));
            }
            // Update CourseLibrary
            LibraryWindow.UpdateSource();
        }

        private void Clean_Click(object sender, RoutedEventArgs e)
        {
            (App.Current as App).CleanApplicationData();
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            (App.Current as App).ExportApplicationData();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            AboutScreen screen = new AboutScreen(this);
            screen.Show();
        }

        private void CloseApp_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion
    }
}
