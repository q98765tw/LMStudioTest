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
                // �]�w API ���_�]�p�G�ݭn�^
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                // �o�e GET �ШD�� LM Studio
                var response = await _httpClient.GetAsync(baseUrl);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseBody);

                    // ��^ JSON ���e��
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
            // API �򥻳]�w
            string baseUrl = "http://localhost:1234/v1/chat/completions";
            string apiKey = "lm-studio";

            try
            {
                // �]�w API ���_
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                // �إ߽ШD�����e
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

                // �N����ǦC�Ƭ� JSON
                var jsonContent = JsonSerializer.Serialize(requestBody);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // �o�e POST �ШD
                var response = await _httpClient.PostAsync(baseUrl, httpContent);

                // �B�z�^��
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseBody);

                    return Ok(jsonResponse); // ��^���㪺 JSON ���եΤ�
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
                // ����ҥ~�ê�^
                return StatusCode(500, new
                {
                    error = "An unexpected error occurred",
                    details = ex.Message
                });
            }
        }


    }
}
