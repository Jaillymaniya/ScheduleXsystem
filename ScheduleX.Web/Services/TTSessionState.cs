//namespace ScheduleX.Web.Services
//{
//    public class TTSessionState
//    {
//        public event Action? OnChange;

//        private int? _academicYearId;
//        public int? AcademicYearId
//        {
//            get => _academicYearId;
//            set
//            {
//                _academicYearId = value;
//                NotifyStateChanged();
//            }
//        }

//        private string? _academicYearName;
//        public string? AcademicYearName
//        {
//            get => _academicYearName;
//            set
//            {
//                _academicYearName = value;
//                NotifyStateChanged();
//            }
//        }

//        private int? _courseId;
//        public int? CourseId
//        {
//            get => _courseId;
//            set
//            {
//                _courseId = value;
//                NotifyStateChanged();
//            }
//        }

//        private string? _courseName;
//        public string? CourseName
//        {
//            get => _courseName;
//            set
//            {
//                _courseName = value;
//                NotifyStateChanged();
//            }
//        }

//        private void NotifyStateChanged()
//        {
//            OnChange?.Invoke();
//        }
//    }
//}

namespace ScheduleX.Web.Services
{
    public class TTSessionState
    {
        public event Action? OnChange;

        private int? _academicYearId;

        public int? AcademicYearId
        {
            get => _academicYearId;
            set
            {
                _academicYearId = value;
                NotifyStateChanged();
            }
        }

        private string? _academicYearName;

        public string? AcademicYearName
        {
            get => _academicYearName;
            set
            {
                _academicYearName = value;
                NotifyStateChanged();
            }
        }

        private int? _courseId;

        public int? CourseId
        {
            get => _courseId;
            set
            {
                _courseId = value;
                NotifyStateChanged();
            }
        }

        private string? _courseName;

        public string? CourseName
        {
            get => _courseName;
            set
            {
                _courseName = value;
                NotifyStateChanged();
            }
        }
        // NEW TERM
        private int? _academicTermId;
        public int? AcademicTermId
        {
            get => _academicTermId;
            set
            {
                _academicTermId = value;
                NotifyStateChanged();
            }
        }

        private string? _termName;
        public string? TermName
        {
            get => _termName;
            set
            {
                _termName = value;
                NotifyStateChanged();
            }
        }
        private void NotifyStateChanged()
        {
            OnChange?.Invoke();
        }
    }
}