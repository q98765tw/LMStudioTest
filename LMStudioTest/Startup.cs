//using Newtonsoft.Json;

//namespace LMStudioTest
//{
//    public class Startup
//    {
//        public IConfiguration Configuration { get; }
//        public Startup(IConfiguration configuration)
//        {
//            Configuration = configuration;
//        }
//        public void ConfigureServices(IServiceCollection services)
//        {
//            //services.AddAuthService(Configuration);

//            services.AddHttpContextAccessor();
//            services.AddHttpClient();

//            services.AddControllers()
//                .AddNewtonsoftJson(option =>
//                {
//                    option.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
//                    option.SerializerSettings.Converters.Add(new UnixTimestampDateTimeConvert());
//                })
//                .ConfigureApiBehaviorOptions(option =>
//                {
//                    // 這裡設定當 ModelState 驗證失敗時, 回傳自訂的錯誤訊息
//                    option.SuppressModelStateInvalidFilter = false;
//                    option.SuppressMapClientErrors = true;
//                    option.InvalidModelStateResponseFactory = actionContext =>
//                    {
//                        string msg = null;

//                        if (actionContext.ModelState.ErrorCount > 0)
//                        {
//                            msg = actionContext.ModelState
//                                .FirstOrDefault(o => o.Value.ValidationState == ModelValidationState.Invalid).Key ?? string.Empty;
//                        }

//                        // 200 ok
//                        return new JsonResult(new { code = ResponseCode.BAD_PARAMS, msg });
//                    };
//                });

//            //// 添加 CORS 支援
//            //services.AddCors(options =>
//            //{
//            //    options.AddPolicy("AllowAll", builder =>
//            //    {
//            //        builder.WithOrigins(
//            //            "https://teligi-web.fiami.com.tw", // 前台
//            //            "https://teligi-cms.fiami.com.tw", // 後台
//            //        )
//            //       .AllowAnyMethod()
//            //       .AllowAnyHeader()
//            //       .AllowCredentials();
//            //    });
//            //});

//            services.AddCustomSwaggerGen();

//            services.AddDatabase(Configuration);

//            ConfigureService(services);

//            ConfigureBackgroundTask(services);

//        }
//    }
//}
