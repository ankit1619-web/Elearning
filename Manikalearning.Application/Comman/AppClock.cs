using Manikalearning.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Comman
{
    public static class AppClock
    {
        private static readonly TimeZoneInfo Ist = Find();

        private static TimeZoneInfo Find()
        {
            foreach (var id in new[] { "Asia/Kolkata", "India Standard Time" })    
            {
                try { return TimeZoneInfo.FindSystemTimeZoneById(id); }
                catch (TimeZoneNotFoundException) { }
                catch (InvalidTimeZoneException) { }
            }
            return TimeZoneInfo.CreateCustomTimeZone("IST", TimeSpan.FromHours(5.5), "IST", "IST");
        }

        public static DateTime ToIst(DateTime utc) => TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), Ist);
        public static DateTime TodayIst => ToIst(DateTime.UtcNow).Date;
         
        public static DateTime IstDateStartUtc(DateTime istDate) =>
            TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(istDate.Date, DateTimeKind.Unspecified), Ist);
    }

    public static class StreakCalculator
    {
         public static int Compute(IEnumerable<DateTime> submittedAtUtc)
        {
            var days = submittedAtUtc.Select(d => AppClock.ToIst(d).Date).ToHashSet();
            var day = days.Contains(AppClock.TodayIst) ? AppClock.TodayIst : AppClock.TodayIst.AddDays(-1);
            var streak = 0;
            while (days.Contains(day)) { streak++; day = day.AddDays(-1); }
            return streak;
        }
    }

    public static class AttemptRules
    {
       public static IQueryable<TestAttempt> Finished(IQueryable<TestAttempt> q) =>
            q.Where(a => a.SubmittedAt != null && (a.Status == "Submitted" || a.Status == "Completed"));
    }
}
