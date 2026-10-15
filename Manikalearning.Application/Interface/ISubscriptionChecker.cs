using Manikalearning.Application.DTOs;

namespace Manikalearning.Application.Interface
{
    
    public interface ISubscriptionChecker
    {
        Task<bool> HasActiveAsync(Guid userId, CancellationToken ct = default);
        Task<CourseAccessDto> CheckCourseAsync(Guid userId, Guid courseId, CancellationToken ct = default);
        Task<CourseAccessDto> CheckLectureAsync(Guid userId, Guid lectureId, CancellationToken ct = default);
        Task<CourseAccessDto> CheckMediaAsync(Guid userId, string fileName, CancellationToken ct = default);
        Task<HashSet<string>> GetFeaturesAsync(Guid userId, CancellationToken ct = default);
        Task<bool> HasFeatureAsync(Guid userId, string feature, CancellationToken ct = default);
        Task<HashSet<Guid>> AccessibleCourseIdsAsync(Guid userId, CancellationToken ct = default);
    }
}
