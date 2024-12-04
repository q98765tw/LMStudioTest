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
            // �I�s�A�ȼh�� GetModels ��k
            var result = await _weatherForecastService.GetModelsAsync();

            if (result.IsSuccess)
            {
                // ��^���\���G
                return Ok(result.Data);
            }
            else
            {
                // ��^���~���G
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
                return Ok(result.Data); // ��^���\�� JSON ���
            }
            else
            {
                return StatusCode(500, new
                {
                    error = "Failed to fetch data from LM Studio",
                    details = result.ErrorMessage
                }); // ��^���~�T��
            }
        }
        [HttpPost("changeSetting")]
        public async Task<IActionResult> changeSetting(IFormFile file)
        {

            int result = await _weatherForecastService.changeSetting(file);

            if (result > 0)
            {
                return Ok(result); // ��^���\�� JSON ���
            }
            else
            {
                return StatusCode(500, new
                {
                    error = "Failed to fetch data from LM Studio",
                }); // ��^���~�T��
            }
        }

    }
}
