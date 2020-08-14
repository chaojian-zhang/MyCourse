using MyCourse.Classes.ExportVersionControl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Classes
{
    [Serializable]
    public enum CourseType
    {
        CS,
        HSS,
        None,
        Engineering,
        ChemicalEngineering,
        MechanicalEngineering,
        ECEArea1Photonics,
        ECEArea2Energy,
        ECEArea3Electronics,
        ECEArea4Signal,
        ECEArea5Computer,
        ECEArea6Software,
        ArtAndScience
    }

    [Serializable]
    public class Course: INotifyPropertyChanged
    {
        #region Construction
        public Course()
        {
            // Defaults
            CourseCode = "Course000H1";
            SubjectName = "";
            CreditWeight = 0F;
            Description = "";
            Type = CourseType.None;
            AU = new CEABAU();
            IsFinished = false;
            IsPinned = false;
            IsInterested = false;
        }
        public Course(string courseCode, string title,  int creditWeight, CEABAU au, CourseType type = CourseType.None, string description = "", string[] prerequisites = null)
        {
            // Initialize
            CourseCode = courseCode;
            SubjectName = title;
            CreditWeight = creditWeight;
            AU = au;
            Description = description;
            IsFinished = false;
            IsPinned = false;
            IsInterested = false;

            // Notify
            NotifyPropertyChanged("Name");
            NotifyPropertyChanged("AreaCode");
            NotifyPropertyChanged("CourseCode");
        }
        #endregion
        #region Import Support
        public Course(CourseExportV1 export)
        {
            // Load Data
            this.CourseCode = export.CourseCode;
            this.SubjectName = export.Title;
            this.Description = export.Description;
            this.CreditWeight = export.CreditWeight;
            this.Type = export.Type;
            this.AU = export.AU;
            this.Corequisites = export.Corequisites;
            this.Prerequisites = export.Prerequisites;
            this.Exclusions = export.Exclusions;
            this.TimeSlots = export.TimeSlots;
            // Other Initialization
            IsFinished = false;
            IsPinned = false;
            IsInterested = false;
        }
        internal void Merge(CourseExportV1 export)
        {
            // Merge if we are empty
            if(string.IsNullOrWhiteSpace(this.SubjectName)) this.SubjectName = export.Title;
            if (string.IsNullOrWhiteSpace(this.Description)) this.Description = export.Description;
            if (CreditWeight == 0) this.CreditWeight = export.CreditWeight;
            if (this.Type == CourseType.None) this.Type = export.Type;
            if(this.AU.Math == 0 && this.AU.CS == 0 && this.AU.ED == 0 && this.AU.ES == 0 && this.AU.NS == 0) this.AU = export.AU;
            if (this.Corequisites == null || this.Corequisites.Count == 0) this.Corequisites = export.Corequisites;
            if (this.Prerequisites == null || this.Prerequisites.Count == 0) this.Prerequisites = export.Prerequisites;
            if (this.Exclusions == null || this.Exclusions.Count == 0) this.Exclusions = export.Exclusions;
            if(this.TimeSlots == null || this.TimeSlots.Count == 0) this.TimeSlots = export.TimeSlots;
            // Ignore IsFinished, IsPinned and IsInterested
        }
        #endregion

        #region Interface
        public void AddTimeSlot(TimeSlot slot)
        {
            TimeSlots.Add(slot);
            NotifyPropertyChanged("Avaliability");
            NotifyTimeSlotsChanged();
        }
        #endregion

        #region Data
        private string _CourseCode;
        private string _SubjectName;
        private string _Description;
        private float _CreditWeight;
        private bool _IsFinished;    // Whether we have taken this course before
        private bool _IsInterested;    // Whether we are interested in taking this course (for auto-generation)
        private bool _IsPinned;
        private CourseType _Type = CourseType.None;
        private CEABAU _AU;
        private ObservableCollection<string> _Corequisites = new ObservableCollection<string>();
        private ObservableCollection<string> _Prerequisites = new ObservableCollection<string>();
        private ObservableCollection<string> _Exclusions = new ObservableCollection<string>();
        private ObservableCollection<TimeSlot> _TimeSlots = new ObservableCollection<TimeSlot>();
        #endregion

        #region Data Binding
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public string CourseCode
        {
            get { return this._CourseCode; }
            set
            {
                if (value != this._CourseCode)
                {
                    _CourseCode = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string SubjectName
        {
            get { return this._SubjectName; }
            set
            {
                if (value != this._SubjectName)
                {
                    _SubjectName = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string Description
        {
            get { return this._Description; }
            set
            {
                if (value != this._Description)
                {
                    _Description = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public float CreditWeight
        {
            get { return this._CreditWeight; }
            set
            {
                if (value != this._CreditWeight)
                {
                    _CreditWeight = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public bool IsFinished
        {
            get { return this._IsFinished; }
            set
            {
                if (value != this._IsFinished)
                {
                    _IsFinished = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public bool IsInterested
        {
            get { return this._IsInterested; }
            set
            {
                if (value != this._IsInterested)
                {
                    _IsInterested = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public bool IsPinned
        {
            get { return this._IsPinned; }
            set
            {
                if (value != this._IsPinned)
                {
                    _IsPinned = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public CourseType Type
        {
            get { return this._Type; }
            set
            {
                if (value != this._Type)
                {
                    _Type = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public CEABAU AU
        {
            get { return this._AU; }
            set
            {
                if (value != this._AU)
                {
                    _AU = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public ObservableCollection<string> Prerequisites
        {
            get { return this._Prerequisites; }
            set
            {
                if (value != this._Prerequisites)
                {
                    _Prerequisites = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public ObservableCollection<string> Corequisites
        {
            get { return this._Corequisites; }
            set
            {
                if (value != this._Corequisites)
                {
                    _Corequisites = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public ObservableCollection<string> Exclusions
        {
            get { return this._Exclusions; }
            set
            {
                if (value != this._Exclusions)
                {
                    _Exclusions = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public ObservableCollection<TimeSlot> TimeSlots
        {
            get { return this._TimeSlots; }
            set
            {
                if (value != this._TimeSlots)
                {
                    _TimeSlots = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string PrerequisitesText
        {
            get { return string.Join(" ", this._Prerequisites); }
            set
            {
                if (value != string.Join(" ", this._Prerequisites))
                {
                    Prerequisites = new ObservableCollection<string>(value.ToUpper().Split(new char[] { ' '}));
                    NotifyPropertyChanged();
                }
            }
        }
        public string CorequisitesText
        {
            get { return string.Join(" ", this.Corequisites); }
            set
            {
                if (value != string.Join(" ", this.Corequisites))
                {
                    Corequisites = new ObservableCollection<string>(value.ToUpper().Split(new char[] { ' ' }));
                    NotifyPropertyChanged();
                }
            }
        }
        public string ExclusionsText
        {
            get { return string.Join(" ", this.Exclusions); }
            set
            {
                if (value != string.Join(" ", this.Exclusions))
                {
                    Exclusions = new ObservableCollection<string>(value.ToUpper().Split(new char[] { ' ' }));
                    NotifyPropertyChanged();
                }
            }
        }

        // Accessors
        public string CourseName
        {
            get { return string.Format("{0} {1}", CourseCode, SubjectName); }
        }
        public string Avaliability
        {
            get
            {
                bool bFall = false;
                bool bWinter = false;
                foreach (TimeSlot slot in TimeSlots)
                {
                    if (slot.Availability == Availability.Fall) bFall = true;
                    if (slot.Availability == Availability.Winter) bWinter = true;
                }
                if (bFall && bWinter) return "Available in both Fall and Winter terms.";
                else if (bFall) return "Available in Fall term";
                else if (bWinter) return "Available in Winter term";
                else return "Not available.";
            }
        }
        public string CEABAUDescription
        {
            get
            {
                return string.Format("Math: {0}, NS: {1}, CS: {2}, ES: {3}, ED: {4}"
                    , AU.Math, AU.NS, AU.CS, AU.ES, AU.ED);
            }
        }
        public void NotifyTimeSlotsChanged()
        {
            NotifyPropertyChanged("FallLecturesCount");
            NotifyPropertyChanged("FallTutorialsCount");
            NotifyPropertyChanged("FallLabsCount");
            NotifyPropertyChanged("WinterLecturesCount");
            NotifyPropertyChanged("WinterTutorialsCount");
            NotifyPropertyChanged("WinterLabsCount");
        }
        public int FallLecturesCount
        {
            get
            {
                return TimeSlots.Where(item => item.Type == TimeslotType.Lecture && item.Availability == Availability.Fall).Count();
            }
        }
        public int FallTutorialsCount
        {
            get
            {
                return TimeSlots.Where(item => item.Type == TimeslotType.Tutorial && item.Availability == Availability.Fall).Count();
            }
        }
        public int FallLabsCount
        {
            get
            {
                return TimeSlots.Where(item => item.Type == TimeslotType.Practice && item.Availability == Availability.Fall).Count();
            }
        }
        public int WinterLecturesCount
        {
            get
            {
                return TimeSlots.Where(item => item.Type == TimeslotType.Lecture && item.Availability == Availability.Winter).Count();
            }
        }
        public int WinterTutorialsCount
        {
            get
            {
                return TimeSlots.Where(item => item.Type == TimeslotType.Tutorial && item.Availability == Availability.Winter).Count();
            }
        }
        public int WinterLabsCount
        {
            get
            {
                return TimeSlots.Where(item => item.Type == TimeslotType.Practice && item.Availability == Availability.Winter).Count();
            }
        }
        #endregion
    }
}
