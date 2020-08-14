using MyCourse.Classes;
using MyCourse.Windows;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Windows;

namespace MyCourse
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Constants and Configurations
        public static readonly string DefaultDataLocation =  System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
        #endregion

        #region Data
        public ApplicationData Data;
        public string DataFolderPath;
        public string DataPath;
        #endregion

        #region Event Handling
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Load Application Data or create one
            if (System.IO.File.Exists(DefaultDataLocation))
                Data = LoadApplicationData(DefaultDataLocation);
            else
                Data = CreateApplicationData(DefaultDataLocation);

            // Assign Bookkeeping
            DataPath = DefaultDataLocation;
            DataFolderPath = System.IO.Path.GetDirectoryName(DataPath);
        }

        protected override void OnSessionEnding(SessionEndingCancelEventArgs e)
        {
            base.OnSessionEnding(e);
            SaveApplicationData();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            SaveApplicationData();
        }
        #endregion

        #region Helpers
        private static ApplicationData CreateApplicationData(string dataLocation)
        {
            // Create
            ApplicationData data = new ApplicationData();
            // Return
            return data;
        }

        private static ApplicationData LoadApplicationData(string dataLocation)
        {
            // Load serialized data
            Stream fileStream = File.OpenRead(dataLocation);
            BinaryFormatter deserializer = new BinaryFormatter();
            ApplicationData data = (ApplicationData)deserializer.Deserialize(fileStream);
            fileStream.Close();
            return data;
        }

        public void SaveApplicationData()
        {
            // Save Application Data
            Stream fileStream = File.Create(DefaultDataLocation);
            BinaryFormatter serializer = new BinaryFormatter();
            serializer.Serialize(fileStream, Data);
            fileStream.Close();
        }


        internal void CleanApplicationData()
        {
            Data.ObservableCourses.Clear();
            Data.GetNewestCources();
        }

        public void ExportApplicationData()
        {
            // Generate path
            string targetLocation = System.IO.Path.Combine((App.Current as App).DataFolderPath, "data" + ApplicationDataExportV1.Suffix);

            // Save Application Data using newest version
            Stream fileStream = File.Create(targetLocation);
            BinaryFormatter serializer = new BinaryFormatter();
            serializer.Serialize(fileStream, new ApplicationDataExportV1(Data.GetNewestCources()));
            fileStream.Close();
        }


        internal string ExportFromCSV(string sourceCSV)
        {
            // Generate path
            string folder = System.IO.Path.GetDirectoryName(sourceCSV);
            string fileName = System.IO.Path.GetFileNameWithoutExtension(sourceCSV);
            string targetLocation = System.IO.Path.Combine(folder, fileName + ApplicationDataExportV1.Suffix);

            // Parse and generate temp course informations
            List<Course> tempGenerated = ParseCSV(sourceCSV);

            // Save data using newest available export
            Stream fileStream = File.Create(targetLocation);
            BinaryFormatter serializer = new BinaryFormatter();
            serializer.Serialize(fileStream, new ApplicationDataExportV1(tempGenerated));
            fileStream.Close();

            return targetLocation;
        }

        public void ImportApplicationData(string sourceLocation, out int add, out int merge)
        {
            add = 0;
            merge = 0;
            // Determmine File Type
            switch (System.IO.Path.GetExtension(sourceLocation))
            {
                case ApplicationDataExportV1.Suffix:
                    // Load export data
                    Stream fileStream = File.OpenRead(sourceLocation);
                    BinaryFormatter deserializer = new BinaryFormatter();
                    ApplicationDataExportV1 exportData = (ApplicationDataExportV1)deserializer.Deserialize(fileStream);
                    fileStream.Close();
                    Data.Import(exportData, out add, out merge);
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Helper
        private List<Course> ParseCSV(string fileLocation)
        {
            List<Course> tempCourses = new List<Course>();
            foreach (string line in System.IO.File.ReadLines(fileLocation))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                // NAME, SECTION, START DATE,  DAY, START, FINISH, LOCATION, PROFESSOR(S), SCHEDULING NOTES
                // Each line represent one time slot of a course; Double quotes inside quotes shoube be escaped by using "&quot;"

                // Parse 
                string[] parameters = line.Split(new string[] { "\",\"" }, StringSplitOptions.None).Select(item => item.Trim(new char[] { '"'})).ToArray();
                if (parameters.Length != 9) continue;
                // Get parameters
                string courseCode = parameters[0];
                string sectionCode = parameters[1];
                string startDate = parameters[2];   // Not used
                string day = parameters[3];
                string startTime = parameters[4];
                string finishTime = parameters[5];
                string location = parameters[6];
                string profs = parameters[7];
                string notes = parameters[8];
                #region Further Processing
                // Further processing: section
                string sectionTypeString, sectionCodeValue;
                TimeSlot.BreakSectionCode(sectionCode, out sectionTypeString, out sectionCodeValue);
                TimeslotType sectionType = TimeslotType.Lecture;
                switch (sectionTypeString.ToUpper())
                {
                    case "LEC":
                        sectionType = TimeslotType.Lecture;
                        break;
                    case "TUT":
                        sectionType = TimeslotType.Tutorial;
                        break;
                    case "PRA":
                        sectionType = TimeslotType.Practice;
                        break;
                    default:
                        sectionType = TimeslotType.Lecture;
                        break;
                }
                // Further processing: day
                Day dayEnum = Day.Monday;
                switch (day.ToUpper())
                {
                    case "MON":
                        dayEnum = Day.Monday;
                        break;
                    case "TUE":
                        dayEnum = Day.Tuesday;
                        break;
                    case "WED":
                        dayEnum = Day.Wednesday;
                        break;
                    case "THU":
                        dayEnum = Day.Thursday;
                        break;
                    case "FRI":
                        dayEnum = Day.Friday;
                        break;
                    default:
                        break;
                }
                // Further processing: time; Ignore minute section
                int startTimeValue = int.Parse(startTime.Substring(0, startTime.IndexOf(':')));
                int finishTimeValue = int.Parse(finishTime.Substring(0, finishTime.IndexOf(':')));
                // Further processing: availability
                char lastChar = courseCode.ToUpper().Last();
                Availability availability = (lastChar == 'S' || lastChar == 'W') ? Availability.Winter : Availability.Fall;
                #endregion

                // Find existing: if it exists then it would be the last per design so we don't need to search
                if(tempCourses.Count == 0 || tempCourses.Last().CourseCode != courseCode)
                {
                    // Add new
                    tempCourses.Add(new Course(courseCode, "", 0, new CEABAU()));
                }
                // Merge
                tempCourses.Last().TimeSlots.Add(new TimeSlot(dayEnum, startTimeValue, finishTimeValue - startTimeValue,
                    sectionType, availability, sectionCodeValue, location, profs, notes));
            }
            return tempCourses;
        }
        #endregion

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception is System.Net.WebException) e.Handled = true;
            else
            {
                CrashScreen crashScreen = new CrashScreen(e.Exception);
                crashScreen.Show();
                App.Current.MainWindow.Close();
                e.Handled = true;
            }
        }

    }
}
