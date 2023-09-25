using Flatlinq.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Flatlinq.Hubs;
using Flatlinq.Services;

var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetValue<string>("DatabaseConnection")!;
MySqlServerVersion version = new(new Version(8, 0, 34));
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddDbContext<CoreDbContext>(options =>
{
    options
        .UseMySql(connectionString, version)
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors();
});
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();
builder.Services
    .AddScoped<IMemoryCache>()
    .AddScoped<IAuthServices, AuthServices>()
    .AddScoped<IHouseServices, HouseServices>()
    .AddSingleton<IJwtServices, JwtServices>()
    .AddScoped<ILandlordServices, LandlordServices>()
    .AddScoped<ISwipeServices, SwipeServices>()
    .AddScoped<ITenantServices, TenantServices>();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services
    .AddIdentityCore<User>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<CoreDbContext>();
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateAudience  = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAlgorithms = new string[]{"HS256"},
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            )
        };
    });
builder.Services.AddAuthorization();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors(options =>
{
    options
        .AllowAnyHeader()
        .AllowAnyOrigin()
        .AllowCredentials();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chatHub");

app.Run();
