using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using System.Net.Http;

namespace LMStudioTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly HttpClient _httpClient;
        public WeatherForecastController(
            ILogger<WeatherForecastController> logger,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
        [HttpGet("get-models")]
        public async Task<IActionResult> GetModels()
        {
            string baseUrl = "http://localhost:1234/v1/models";
            string apiKey = "lm-studio";

            try
            {
                // 設定 API 金鑰（如果需要）
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                // 發送 GET 請求到 LM Studio
                var response = await _httpClient.GetAsync(baseUrl);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseBody);

                    // 返回 JSON 給前端
                    return Ok(jsonResponse);
                }
                else
                {
                    return StatusCode((int)response.StatusCode, new
                    {
                        error = response.ReasonPhrase
                    });
                }
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, new
                {
                    error = "Error connecting to LM Studio",
                    details = ex.Message
                });
            }
        }
        [HttpPost("PostTest")]
        public async Task<IActionResult> PostTest(IFormFile file)
        {
            // API 基本設定
            string baseUrl = "http://localhost:1234/v1/chat/completions";
            string apiKey = "lm-studio";

            try
            {
                // 設定 API 金鑰
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                // 建立請求的內容
                var requestBody = new
                {
                    model = "model-identifier",
                    messages = new[]
                    {
                        new { role = "system", content = "Use Traditional Chinese." },
                        new { role = "user", content = "Introduce yourself." }
                    },
                    temperature = 0.7
                };

                // 將物件序列化為 JSON
                var jsonContent = JsonSerializer.Serialize(requestBody);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // 發送 POST 請求
                var response = await _httpClient.PostAsync(baseUrl, httpContent);

                // 處理回應
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseBody);

                    return Ok(jsonResponse); // 返回完整的 JSON 給調用方
                }
                else
                {
                    return StatusCode((int)response.StatusCode, new
                    {
                        error = "Failed to fetch data from LM Studio",
                        details = response.ReasonPhrase
                    });
                }
            }
            catch (Exception ex)
            {
                // 捕獲例外並返回
                return StatusCode(500, new
                {
                    error = "An unexpected error occurred",
                    details = ex.Message
                });
            }
        }


    }
}
