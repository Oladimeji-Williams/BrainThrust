using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Serilog;
using System.Text.Json.Serialization;
using BrainThrust.src.Models;
using BrainThrust.src.Services.Interfaces;
using BrainThrust.src.Services.Classes;
using BrainThrust.src.Repositories.Classes;
using BrainThrust.src.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);
DotNetEnv.Env.Load();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/app.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    builder.Services.AddSingleton<EmailSettingsProvider>();
    builder.Services.AddScoped<IEmailService, EmailService>();
    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });

    var serviceProvider = builder.Services.BuildServiceProvider();
    var emailSettingsProvider = serviceProvider.GetRequiredService<EmailSettingsProvider>();
    var emailSettings = emailSettingsProvider.LoadEmailSettings();

    var jwtSecret = ValidateEnvironmentVariable("JWT_SECRET", minLength: 32);
    var connectionString = ValidateEnvironmentVariable("SSMS_CONNECTION");

    builder.Services.AddHttpContextAccessor();
    
    builder.Services.AddScoped<IQuestionService, QuestionService>();
    builder.Services.AddScoped<UserService>();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<ITopicRepository, TopicRepository>();
    builder.Services.AddScoped<IQuizRepository, QuizRepository>(); 
    builder.Services.AddScoped<IQuizService, QuizService>();
    builder.Services.AddScoped<ILearningProgressService, LearningProgressService>();
    builder.Services.AddScoped<ILearningProgressRepository, LearningProgressRepository>();
    builder.Services.AddScoped<ILogService, LogService>();
    builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
    builder.Services.AddScoped<IDashboardService, DashboardService>();
    
    builder.Services.AddMemoryCache(); 
    
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));

    builder.Services.Configure<EmailSettings>(options =>
    {
        options.SmtpServer = emailSettings.SmtpServer;
        options.Port = emailSettings.Port;
        options.Username = emailSettings.Username;
        options.Password = emailSettings.Password;
        options.FromEmail = emailSettings.FromEmail;
        options.EnableSsl = emailSettings.EnableSsl;
    });

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                ValidateIssuer = true,
                ValidIssuer = "your-api",
                ValidateAudience = true,
                ValidAudience = "your-client",
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

    builder.Services.AddAuthorization();

    ConfigureSwagger(builder.Services);

    var app = builder.Build();


    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();

    app.Use(async (context, next) =>
    {
        if (context.User.Identity is { IsAuthenticated: false })
        {
            Log.Warning("JWT Authentication Failed!");
        }
        await next();
    });

    app.MapControllers();

    Log.Information("Application started successfully.");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application startup failed.");
}
finally
{
    Log.CloseAndFlush();
}

string ValidateEnvironmentVariable(string key, int minLength = 1)
{
    var value = Environment.GetEnvironmentVariable(key);
    if (string.IsNullOrEmpty(value) || value.Length < minLength)
    {
        Log.Fatal($"🚨 {key} is missing or does not meet the required length.");
        throw new InvalidOperationException($"{key} is missing or does not meet the required length.");
    }
    return value;
}

void ConfigureSwagger(IServiceCollection services)
{
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "BrainThrust API", Version = "v1" });
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter 'Bearer YOUR_ACCESS_TOKEN' below",
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new List<string>()
            }
        });
    });
}
