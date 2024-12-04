using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using System.Net.Http;
using LMStudioTest.Services;

namespace LMStudioTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly HttpClient _httpClient;

        private readonly WeatherForecastService _weatherForecastService;
        public WeatherForecastController(
            ILogger<WeatherForecastController> logger,
            IHttpClientFactory httpClientFactory,
            WeatherForecastService weatherForecastService)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
            _weatherForecastService = weatherForecastService;
        }

        [HttpGet("get-models")]
        public async Task<IActionResult> GetModels()
        {
            // 呼叫服務層的 GetModels 方法
            var result = await _weatherForecastService.GetModelsAsync();

            if (result.IsSuccess)
            {
                // 返回成功結果
                return Ok(result.Data);
            }
            else
            {
                // 返回錯誤結果
                return StatusCode(500, new
                {
                    error = "Error connecting to LM Studio",
                    details = result.ErrorMessage
                });
            }
        }

        [HttpPost("userRequest")]
        public async Task<IActionResult> userRequest(string userInput)
        {
            var result = await _weatherForecastService.PostTestAsync(userInput);

            if (result.IsSuccess)
            {
                return Ok(result.Data); // 返回成功的 JSON 資料
            }
            else
            {
                return StatusCode(500, new
                {
                    error = "Failed to fetch data from LM Studio",
                    details = result.ErrorMessage
                }); // 返回錯誤訊息
            }
        }
        [HttpPost("changeSetting")]
        public async Task<IActionResult> changeSetting(IFormFile file)
        {

            int result = await _weatherForecastService.changeSetting(file);

            if (result > 0)
            {
                return Ok(result); // 返回成功的 JSON 資料
            }
            else
            {
                return StatusCode(500, new
                {
                    error = "Failed to fetch data from LM Studio",
                }); // 返回錯誤訊息
            }
        }

    }
}
