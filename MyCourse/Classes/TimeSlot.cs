using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Classes
{
    [Serializable]
    public enum Day
    {
        Monday = 1,
        Tuesday,
        Wednesday,
        Thursday,
        Friday
    }

    [Serializable]
    public enum TimeslotType
    {
        Lecture,
        Tutorial,
        Practice,
    }

    [Serializable]
    public enum Availability
    {
        Fall,
        Winter
    }

    [Serializable]
    public class TimeSlot
    {
        public TimeSlot()
        {
            Type = TimeslotType.Lecture;
            Availability = Availability.Fall;
            _StartTime = 8;
            TimeSpan = 1;
            Day = Day.Monday;
            SectionCode = "0101";
            Location = "UT";
            Professors = "";
            ExtraNotes = "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startTime">In 24 hours</param>
        /// <param name="timeSpan">In hours</param>
        /// <param name="type"></param>
        /// <param name="sectionCode"></param>
        public TimeSlot(Day day, int startTime, int timeSpan, TimeslotType type, Availability availability, string sectionCode = "0000", string location = "UT", string professors = "", string notes = "")
        {
            Day = day;
            _StartTime = startTime;
            TimeSpan = timeSpan;
            Type = type;
            Availability = availability;
            SectionCode = sectionCode;
            Location = location;
            Professors = professors;
            ExtraNotes = notes;
        }

        public Day Day { get; set; }
        public int _StartTime;
        public int TimeSpan { get; set; }
        public TimeslotType Type { get; set; }
        public Availability Availability { get; set; }
        public string SectionCode { get; set; }
        public string Location { get; set; }
        public string Professors { get; set; }
        public string ExtraNotes { get; set; }

        public int StartTime { get { return _StartTime; } set { if (value != _StartTime) _StartTime = value; } }
        public string StartTimeText
        {
            get
            {
                return _StartTime < 12 ? _StartTime + " a.m." : _StartTime - 12 + " p.m.";
            }
            set
            {
                bool bAfternoon = false;
                if(value.Contains("m"))
                {
                    if (value.Contains("a.m.") || value.Contains("am")) value = value.Substring(0, value.IndexOf('a'));
                    if (value.Contains("p.m.") || value.Contains("pm")) { value = value.Substring(0, value.IndexOf('p')); bAfternoon = true; }
                }
                if (int.TryParse(value, out _StartTime) == false) _StartTime = 8;
                if (bAfternoon) _StartTime += 12;
                if (_StartTime < 8 || _StartTime > 21) _StartTime = 8;
            }
        }
        public string TimeSpanText
        {
            get
            {
                return TimeSpan == 1? TimeSpan + " hour" : TimeSpan + " hours";
            }
        }
        public string SectionNameText
        {
            get
            {
                return Type.ToString() + " " + SectionCode;
            }
        }

        #region Helper
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sectionCodeName">e.g. LEC0101</param>
        /// <param name="sectionTypeString">e.g. LEC</param>
        /// <param name="sectionCode">e.g. 0101</param>
        public static void BreakSectionCode(string sectionCodeName, out string sectionTypeString, out string sectionCode)
        {
            int numberIndex = sectionCodeName.IndexOfAny(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });
            sectionTypeString = sectionCodeName.Substring(0, numberIndex);
            sectionCode = sectionCodeName.Substring(numberIndex);
        }
        #endregion
    }
}
