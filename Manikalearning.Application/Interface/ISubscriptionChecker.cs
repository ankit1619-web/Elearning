namespace Manikalearning.Application.Interface
{
    public interface ISubscriptionChecker
    {
        Task<bool> HasActiveAsync(Guid userId, CancellationToken ct = default);
        Task<bool> CanAccessCourseAsync(Guid userId, Guid courseId, CancellationToken ct = default);
        Task<HashSet<Guid>> AccessibleCourseIdsAsync(Guid userId, CancellationToken ct = default);
    }
}
