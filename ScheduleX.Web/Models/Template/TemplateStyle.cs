//namespace ScheduleX.Web.Models.Template
//{
//    public class TemplateStyle
//    {
//        public string headerBg { get; set; }
//        public string headerText { get; set; }
//        public string bodyBg { get; set; }
//        public string bodyText { get; set; }
//        public string borderColor { get; set; }
//        public string cellPadding { get; set; }
//        public string fontSize { get; set; }

//        public bool showRoom { get; set; }
//        public bool showFaculty { get; set; }
//        public bool showSubjectCode { get; set; }
//        public string titleAlign { get; set; }
//    }
//}
namespace ScheduleX.Web.Models.Template
{
    public class TemplateStyle
    {
        public string HeaderBg { get; set; } = "#1e293b";

        public string HeaderText { get; set; } = "#ffffff";

        public string BodyBg { get; set; } = "#ffffff";

        public string BodyText { get; set; } = "#111827";

        public string BorderColor { get; set; } = "#cbd5e1";

        public bool ShowRoom { get; set; } = true;

        public bool ShowFaculty { get; set; } = true;

        public bool ShowSubjectCode { get; set; } = false;

        public string CellPadding { get; set; } = "8px";

        public string FontSize { get; set; } = "14px";

        public string TitleAlign { get; set; } = "center";
    }
}