using LMStudioTest.Services;

var builder = WebApplication.CreateBuilder(args);

// �]�m Configuration
var configuration = builder.Configuration;
// Add services to the container.
builder.Services.AddScoped<WeatherForecastService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// �T�O�K�[ HttpClient �䴩
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();
// �p�G����L�̿�� IConfiguration ���A��
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
