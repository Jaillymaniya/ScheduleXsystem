namespace ScheduleX.Web.DTOs
{
   
        public class OverviewDto
        {
            public int TotalFaculty { get; set; }

            public int AvailableFaculty { get; set; }

            public int TotalSubjects { get; set; }

            public int TotalRooms { get; set; }

            /* NEW */

            public int TotalDivisions { get; set; }

            public int TotalTemplates { get; set; }

            public int GeneratedCount { get; set; }

            public bool HasScheduleConfig { get; set; }
        }
    
}
