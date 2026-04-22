using Shala.Shared.Responses.Registration;

namespace Shala.Web.Repositories.Registration
{
    public interface IRegistrationFeeHeadLookupWebRepository
    {
        Task<List<RegistrationFeeHeadLookupResponse>> GetAsync(
            CancellationToken cancellationToken = default);
    }
}