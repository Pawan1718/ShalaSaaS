using System.Net.Http;

namespace Shala.Web.Services.Http
{
    public class ServerResponseHelper<T>
    {
        public ServerResponseHelper(
            bool isSuccess,
            T? serverResponse,
            HttpResponseMessage responseMessage,
            string? message = null)
        {
            IsSuccess = isSuccess;
            ServerResponse = serverResponse;
            ResponseMessage = responseMessage;
            Message = message;
        }

        public bool IsSuccess { get; }
        public T? ServerResponse { get; }
        public HttpResponseMessage ResponseMessage { get; }
        public string? Message { get; }

        public async Task<string> GetBodyPartAsync()
        {
            if (ResponseMessage.Content is null)
                return string.Empty;

            return await ResponseMessage.Content.ReadAsStringAsync();
        }

        public async Task<string> GetRawBodyAsync()
        {
            return await GetBodyPartAsync();
        }
    }
}