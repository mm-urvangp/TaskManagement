using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace TaskManagement.Web.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor accessor)
        {
            _httpClient = httpClientFactory.CreateClient();
            var token = accessor.HttpContext?.Session.GetString("JWToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<T> GetAsync<T>(string url)
        {
            var res = await _httpClient.GetAsync(url);
            if (res.IsSuccessStatusCode)
            {
                var json = await res.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(json);
            }
            return default;
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string url, T data)
        {
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            return await _httpClient.PostAsync(url, content);
        }

        public async Task<HttpResponseMessage> PutAsync<T>(string url, T data)
        {
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            return await _httpClient.PutAsync(url, content);
        }
    }
}
