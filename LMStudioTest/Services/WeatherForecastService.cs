using LMStudioTest.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Text;
using System.Text.Json;

namespace LMStudioTest.Services
{
    public class WeatherForecastService
    {
        private readonly ILogger<WeatherForecastService> _logger;
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _memoryCache;  //cache 儲存對話歷史
        private readonly string _apiKey;
        private readonly string _base;
        private readonly IConfiguration _configuration;
        public WeatherForecastService(
             ILogger<WeatherForecastService> logger,
             IHttpClientFactory httpClientFactory,
             IMemoryCache memoryCache,
             IConfiguration configuration)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
            _memoryCache = memoryCache;
            _apiKey = configuration["LMStudio:ApiKey"];
            _base = configuration["LMStudio:BaseUrl"];
            _configuration = configuration;
        }

        public async Task<(bool IsSuccess, JsonElement? Data, string? ErrorMessage)> GetModelsAsync()
        {
            string baseUrl = _base + _configuration["LMStudio:ModelList"];

            try
            {
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

                var response = await _httpClient.GetAsync(baseUrl);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseBody);
                    return (true, jsonResponse, null);
                }
                else
                {
                    return (false, null, response.ReasonPhrase);
                }
            }
            catch (HttpRequestException ex)
            {
                return (false, null, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, JsonElement? Data, string? ErrorMessage)> PostTestAsync(string input)
        {
            string baseUrl = _base + _configuration["LMStudio:Chat"];
            string conversationHistoryKey = "conversationHistoryKey"; // 每個用戶應該有不同的鍵

            if (string.IsNullOrEmpty(input)) 
            { 
                return (false, null, "input 不能null");
            }
            
            var conversationHistory = new List<object>();
            // 確保從緩存中讀取對話歷史
            if (!_memoryCache.TryGetValue(conversationHistoryKey, out conversationHistory))
            {
                conversationHistory = new List<object>();
            }

            try
            {
                // 設定 API 金鑰
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

                // 將 user 訊息加入對話歷史 string 插入
                var userMessage = new { role = "user", content = input };
                conversationHistory.Add(userMessage);
                
                // 將歷史對話作為請求的一部分
                var requestBody = new
                {
                    model = _configuration["LMStudio:Model"],  // 使用正確的模型識別碼
                    messages = conversationHistory,  // 將整個對話歷史傳遞給模型
                    temperature = 0.7
                };

                // 將物件序列化為 JSON
                var jsonContent = JsonSerializer.Serialize(requestBody);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // 發送 POST 請求
                var response = await _httpClient.PostAsync(baseUrl, httpContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseBody);
                    string resContent = "";
                    if (jsonResponse.TryGetProperty("choices", out var choices))
                    {
                        resContent = choices[0].GetProperty("message").GetProperty("content").GetString();
                    }
                    
                    // 儲存模型回應並記錄到對話中
                    var assistantMessage = new { role = "assistant", content = resContent };
                    conversationHistory.Add(assistantMessage);  // 記錄助手的回應

                    // 將更新後的對話歷史寫回記憶體緩存
                    _memoryCache.Set(conversationHistoryKey, conversationHistory, TimeSpan.FromMinutes(5));
                    return (true, jsonResponse, null);
                }
                else
                {
                    return (false, null, response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                return (false, null, ex.Message);
            }
        }
        public bool checkfile(IFormFile file)
        {
            // 檢查是否有檔案上傳
            if (file == null || file.Length == 0)
            {
                return false;
            }

            // 檢查檔案副檔名是否為 .txt 或 .md
            var allowedExtensions = new[] { ".txt", ".md" };
            var fileExtension = Path.GetExtension(file.FileName)?.ToLower();
            if (!allowedExtensions.Contains(fileExtension))
            {
                return false;
            }
            return true;
        }

        public async Task<int> changeSetting(IFormFile file)
        {
            string conversationHistoryKey = "conversationHistoryKey"; // 每個用戶應該有不同的鍵

            if (!checkfile(file))
            {
                return 0;
            }
            // 讀取檔案內容
            string systemContent;
            try
            {
                using (var stream = file.OpenReadStream())
                using (var reader = new StreamReader(stream))
                {
                    systemContent = await reader.ReadToEndAsync();
                }
            }
            catch 
            {
                return 0;
            }
            var conversationHistory = new List<object>();
            // 確保從緩存中讀取對話歷史
            if (!_memoryCache.TryGetValue(conversationHistoryKey, out conversationHistory))
            {
                conversationHistory = new();
            }
            if (conversationHistory.Any()) 
            {
                conversationHistory.RemoveAt(0);
            }
            // 生成系統訊息（如果有需要）處理file
            var systemMessage = new { role = "system", content = systemContent };
            conversationHistory.Insert(0, systemMessage);  // 頭部插入，保證系統訊息在前

            // 將更新後的對話歷史寫回記憶體緩存
            _memoryCache.Set(conversationHistoryKey, conversationHistory, TimeSpan.FromMinutes(5));
            return conversationHistory.Count();
        }
    }
}
