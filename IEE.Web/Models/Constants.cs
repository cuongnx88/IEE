using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IEE.Web.Models
{
    public static class CommonConstants
    {
        //file type
        public const int Image = 1;
        public const int File = 2;

        //course name
        public const string IELTS_Foundation = "IELTS Foundation";
        public const string IELTS_55 = "IELTS 5.5";
        public const string IELTS_65 = "IELTS 6.5";
        public const string IELTS_75 = "IELTS >7.5";
        public const string SAT_Beginner = "SAT Beginner";
        public const string SAT_Intermediate = "SAT Intermediate";
        public const string SAT_Advanced = "SAT Advanced";
        public const string SAT_Super_Advanced = "SAT Super Advanced";

        //course type
        public const string IELTS = "IELTS";
        public const string SAT = "SAT";

        public enum EnumCourseLevel
        {
            IELTS_Foundation = 1,
            IELTS_55 = 2,
            IELTS_65 = 3,
            IELTS_75 = 4,
            SAT_Beginner = 5,
            SAT_Intermediate = 6,
            SAT_Advanced = 7,
            SAT_Super_Advanced = 8
        }

        public static SelectList GetListLevel()
        {
            var listLevel = new List<KeyValuePair<int, string>>() {
                new KeyValuePair<int, string>((int)EnumCourseLevel.IELTS_Foundation,IELTS_Foundation),
                new KeyValuePair<int, string>((int)EnumCourseLevel.IELTS_55,IELTS_55),
                new KeyValuePair<int, string>((int)EnumCourseLevel.IELTS_65,IELTS_65),
                new KeyValuePair<int, string>((int)EnumCourseLevel.IELTS_75,IELTS_75),
                new KeyValuePair<int, string>((int)EnumCourseLevel.SAT_Beginner,SAT_Beginner),
                new KeyValuePair<int, string>((int)EnumCourseLevel.SAT_Intermediate,SAT_Intermediate),
                new KeyValuePair<int, string>((int)EnumCourseLevel.SAT_Advanced,SAT_Advanced),
                new KeyValuePair<int, string>((int)EnumCourseLevel.SAT_Super_Advanced,SAT_Super_Advanced)
            };

            return new SelectList(listLevel, "Key", "Value");

        }

        public enum EnumSection
        {
            Reading = 1,
            Writing = 2,
            Math_NoCalc = 3,
            Math_Calc = 4
        }

        public enum EnumSectionDuration
        {
            Reading = 65,
            Writing = 55,
            Math_NoCalc = 25,
            Math_Calc = 35
        }
    }
}