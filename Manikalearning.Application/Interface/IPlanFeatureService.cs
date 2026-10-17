namespace Manikalearning.Application.Interface
{
    public interface IPlanFeatureService
    {
        Task<bool> HasAsync(string featureCode);
        Task<HashSet<string>> AllAsync();
    }

}
