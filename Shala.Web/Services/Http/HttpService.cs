using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Shala.Shared.Common;
using Shala.Web.Services.State;

namespace Shala.Web.Services.Http
{
    public class HttpService : IHttpService
    {
        private readonly HttpClient _httpClient;
        private readonly ApiSession _session;
        private readonly JsonSerializerOptions _jsonOptions;

        public HttpService(HttpClient httpClient, ApiSession session)
        {
            _httpClient = httpClient;
            _session = session;

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        // ===================== GET =====================

        public async Task<ServerResponseHelper<TResponse>> GetAsync<TResponse>(
            string url,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await PrepareRequestAsync();

                var response = await _httpClient.GetAsync(url, cancellationToken);
                return await BuildResponseAsync<TResponse>(response, cancellationToken);
            }
            catch (Exception ex)
            {
                return CreateExceptionResponse<TResponse>(ex);
            }
        }

        // ===================== POST =====================

        public async Task<ServerResponseHelper<TResponse>> PostAsync<TRequest, TResponse>(
            string url,
            TRequest data,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await PrepareRequestAsync();

                var response = await _httpClient.PostAsJsonAsync(url, data, cancellationToken);
                return await BuildResponseAsync<TResponse>(response, cancellationToken);
            }
            catch (Exception ex)
            {
                return CreateExceptionResponse<TResponse>(ex);
            }
        }

        // ===================== PUT =====================

        public async Task<ServerResponseHelper<TResponse>> PutAsync<TRequest, TResponse>(
            string url,
            TRequest data,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await PrepareRequestAsync();

                var response = await _httpClient.PutAsJsonAsync(url, data, cancellationToken);
                return await BuildResponseAsync<TResponse>(response, cancellationToken);
            }
            catch (Exception ex)
            {
                return CreateExceptionResponse<TResponse>(ex);
            }
        }

        // ===================== DELETE =====================

        public async Task<ServerResponseHelper<object>> DeleteAsync(
            string url,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await PrepareRequestAsync();

                var response = await _httpClient.DeleteAsync(url, cancellationToken);
                return await BuildResponseAsync<object>(response, cancellationToken);
            }
            catch (Exception ex)
            {
                return CreateExceptionResponse<object>(ex);
            }
        }

        // ===================== COMMON =====================

        private async Task PrepareRequestAsync()
        {
            if (!_session.IsAuthenticated)
                await _session.InitializeAsync();

            _httpClient.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrWhiteSpace(_session.Token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _session.Token);
            }
        }

        private async Task<ServerResponseHelper<TResponse>> BuildResponseAsync<TResponse>(
            HttpResponseMessage response,
            CancellationToken cancellationToken)
        {
            if (response.Content == null || response.StatusCode == HttpStatusCode.NoContent)
            {
                return new ServerResponseHelper<TResponse>(
                    response.IsSuccessStatusCode,
                    default,
                    response,
                    string.Empty);
            }

            var raw = await response.Content.ReadAsStringAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(raw))
            {
                return new ServerResponseHelper<TResponse>(
                    response.IsSuccessStatusCode,
                    default,
                    response,
                    string.Empty);
            }

            var contentType = response.Content.Headers.ContentType?.MediaType;

            // ✅ JSON response
            if (!string.IsNullOrWhiteSpace(contentType) &&
                contentType.Contains("json", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var wrapped = JsonSerializer.Deserialize<ApiResponse<TResponse>>(raw, _jsonOptions);

                    if (wrapped is not null)
                    {
                        return new ServerResponseHelper<TResponse>(
                            wrapped.Success,
                            wrapped.Data,
                            response,
                            wrapped.Message);
                    }
                }
                catch
                {
                    // fallback to direct
                }

                try
                {
                    var direct = JsonSerializer.Deserialize<TResponse>(raw, _jsonOptions);

                    return new ServerResponseHelper<TResponse>(
                        response.IsSuccessStatusCode,
                        direct,
                        response,
                        string.Empty);
                }
                catch
                {
                    return new ServerResponseHelper<TResponse>(
                        false,
                        default,
                        response,
                        raw);
                }
            }

            // ❌ Non JSON (HTML, text, error page)
            return new ServerResponseHelper<TResponse>(
                false,
                default,
                response,
                raw);
        }

        private ServerResponseHelper<TResponse> CreateExceptionResponse<TResponse>(Exception ex)
        {
            var response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(ex.Message)
            };

            return new ServerResponseHelper<TResponse>(
                false,
                default,
                response,
                ex.Message);
        }
    }
}