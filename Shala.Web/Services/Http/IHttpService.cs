using System.Threading;

namespace Shala.Web.Services.Http
{
    public interface IHttpService
    {
        Task<ServerResponseHelper<TResponse>> GetAsync<TResponse>(
            string url,
            CancellationToken cancellationToken = default);

        Task<ServerResponseHelper<TResponse>> PostAsync<TRequest, TResponse>(
            string url,
            TRequest data,
            CancellationToken cancellationToken = default);

        Task<ServerResponseHelper<TResponse>> PutAsync<TRequest, TResponse>(
            string url,
            TRequest data,
            CancellationToken cancellationToken = default);

        Task<ServerResponseHelper<object>> DeleteAsync(
            string url,
            CancellationToken cancellationToken = default);
    }
}