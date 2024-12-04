using LMStudioTest.Services;

var builder = WebApplication.CreateBuilder(args);

// 設置 Configuration
var configuration = builder.Configuration;
// Add services to the container.
builder.Services.AddScoped<WeatherForecastService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// 確保添加 HttpClient 支援
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();
// 如果有其他依賴於 IConfiguration 的服務
builder.Services.AddSingleton<IConfiguration>(configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
