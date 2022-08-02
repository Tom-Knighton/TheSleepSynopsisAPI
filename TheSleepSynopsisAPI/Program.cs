using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TheSleepSynopsisAPI.Data;
using TheSleepSynopsisAPI.Domain.Services;
using TheSleepSynopsisAPI.Implementations;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

AddServices(builder);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "The Sleep Synopsis API");
    });
}

static void AddServices(WebApplicationBuilder builder)
{
    builder.Services
        .AddControllers()
        .AddNewtonsoftJson(option => {

            option.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            option.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
        }); ;
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddDbContext<DataContext>();

    builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("TheSleepSynopsis"));

    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<ITokenService, TokenService>();
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<ISleepService, SleepService>();

    builder.Services.AddAuthentication(o =>
    {
        o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(o =>
    {
        o.SaveToken = true;
        o.RequireHttpsMetadata = true;
        o.TokenValidationParameters = new()
        {
            ValidIssuer = builder.Configuration["TheSleepSynopsis:Issuer"],
            ValidAudience = builder.Configuration["TheSleepSynopsis:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["TheSleepSynopsis:SecretKey"])),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

