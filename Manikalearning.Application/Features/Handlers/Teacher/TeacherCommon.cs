using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Teacher
{
    public record TeacherActionResult(bool IsSuccess, string Message, Guid? Id = null)
    {
        public static TeacherActionResult Ok(string message, Guid? id = null) => new(true, message, id);
        public static TeacherActionResult Fail(string message) => new(false, message);
    }

    public record LookupItem(Guid Id, string Name);

    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }
        public int TotalPages => Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));
    }
     
    public static class TeacherRepo
    {
        public static async Task AddRow<T>(this IRepository<T> r, T entity) where T : class
        { await r.AddAsync(entity); await r.SaveChangesAsync(); }

        public static async Task UpdateRow<T>(this IRepository<T> r, T entity) where T : class
        { r.Update(entity); await r.SaveChangesAsync(); }

        public static async Task DeleteRow<T>(this IRepository<T> r, T entity) where T : class
        { r.Remove(entity); await r.SaveChangesAsync(); }

        public static async Task AddRows<T>(this IRepository<T> r, IEnumerable<T> rows) where T : class
        { foreach (var e in rows) await r.AddAsync(e);  await r.SaveChangesAsync(); }

        public static async Task DeleteRows<T>(this IRepository<T> r, IEnumerable<T> rows) where T : class
        { foreach (var e in rows)   r.Remove(e); await r.SaveChangesAsync(); }
    }
     
    public static class TeacherGuards
    {
        public static Task<bool> OwnsCourse(this IRepository<TCourse> courses, Guid courseId, Guid teacherId, CancellationToken ct)
            => courses.Entity().AsNoTracking().AnyAsync(c => c.CourseId == courseId && c.CreatedBy == teacherId, ct);
    }
     
    public static class Conv
    {
        public static T Cast<T>(int value, T like)
        {
            var t = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
            object boxed = t.IsEnum ? Enum.ToObject(t, value) : Convert.ChangeType(value, t);
            return (T)boxed;
        }
    }
     
    public record GetTeacherCourseOptionsQuery(Guid TeacherId) : IRequest<List<LookupItem>>;

    public class GetTeacherCourseOptionsQueryHandler : IRequestHandler<GetTeacherCourseOptionsQuery, List<LookupItem>>
    {
        private readonly IRepository<TCourse> _courses;
        public GetTeacherCourseOptionsQueryHandler(IRepository<TCourse> courses) { _courses = courses; }

        public async Task<List<LookupItem>> Handle(GetTeacherCourseOptionsQuery r, CancellationToken ct)
            => await _courses.Entity().AsNoTracking()
                .Where(c => c.CreatedBy == r.TeacherId)
                .OrderBy(c => c.CourseName)
                .Select(c => new LookupItem(c.CourseId, c.CourseName))
                .ToListAsync(ct);
    }
}
