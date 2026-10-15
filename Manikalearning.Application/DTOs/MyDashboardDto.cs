namespace Manikalearning.Application.DTOs
{
    public class MyDashboardDto
    {
        public string FullName { get; set; } = "";
        public string FirstName => string.IsNullOrWhiteSpace(FullName) ? "there" : FullName.Trim().Split(' ')[0];
         
        public string? TargetExamName { get; set; }
        public int? DaysLeft { get; set; }                
        public int? DailyGoal { get; set; } = 30;
         
        public int SolvedToday { get; set; }
        public int SolvedWeek { get; set; }
        public int SolvedTotal { get; set; }
        public decimal Accuracy { get; set; }
        public int AccuracyTarget { get; set; } = 80;
        public int StreakDays { get; set; }
        public List<DayPointDto> Week { get; set; } = new();  
        public int Mistakes { get; set; }
        public int Saved { get; set; }
        public int OpenDoubts { get; set; }
        public int AnsweredDoubts { get; set; }
         
        public string? PlanName { get; set; }
        public DateTime? PlanEnds { get; set; }
        public int PlanDaysLeft { get; set; }
    }

    public class HeaderStatsDto
    {
        public string FullName { get; set; } = "";
        public string? ProfileImage { get; set; }
        public int StreakDays { get; set; }
        public HashSet<string> Features { get; set; } = new(StringComparer.OrdinalIgnoreCase);
        public bool Has(string feature) => Features.Contains(feature);
    }
}
