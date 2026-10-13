using static Township_API.Models.commonTypes;

namespace Township_API.Services
{
    public interface IGeneralService
    {
        Task<(bool Success, string Message)> SoftDeleteModuleAsync(
            int id,
            ModuleTypes moduleType
        );
    }
}
