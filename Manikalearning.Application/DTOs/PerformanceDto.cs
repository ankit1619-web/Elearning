using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.DTOs
{
    public class AttemptPointDto
    {
        public Guid AttemptId { get; set; }
        public DateTime SubmittedAt { get; set; }
        public int TotalQuestions { get; set; }
        public decimal Score { get; set; }
        public decimal TotalMarks { get; set; }
        public decimal Accuracy { get; set; }
    }

    public class DayPointDto
    {
        public DateTime Date { get; set; }
        public int Questions { get; set; }
    }

    public class PerformanceDto
    {
        public int Days { get; set; }
        public int Attempts { get; set; }
        public int TotalQuestions { get; set; }
        public int Attempted { get; set; }
        public int Correct { get; set; }
        public int Wrong { get; set; }
        public int Skipped { get; set; }
        public decimal Accuracy { get; set; }          // correct / attempted
        public decimal AvgScorePercent { get; set; }   // score / total marks
        public int TimeSpentSeconds { get; set; }
        public int StreakDays { get; set; }
        public List<AttemptPointDto> Recent { get; set; } = new();   // newest first
        public List<DayPointDto> Daily { get; set; } = new();        // last 7 days, oldest first
    }

}
