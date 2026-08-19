using Entities.ConfigurationModels;
using Entities.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Repositories.Contracts;
using Repositories.EFCore;
using Services;
using Services.Contracts;
using Services.Mapping;
using System.Text;
using WebApi.Extensions;
using WebApi.Data;
using StackExchange.Redis;
using Services.Configuration;
using Services.Messaging;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RepositoryContext>(options =>  options.UseSqlServer( //Bir sınıf RepositoryContext isterse, ASP.NET Core bunu oluşturup versin.
        builder.Configuration.GetConnectionString("sqlConnection")));

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

builder.Services
    .AddIdentity<User, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<RepositoryContext>()
    .AddDefaultTokenProviders();

var jwtSettings = builder.Configuration
    .GetSection("JwtSettings")
    .Get<JwtSettings>();

if (jwtSettings is null)
{
    throw new InvalidOperationException(
        "JwtSettings yapılandırması bulunamadı.");
}



builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =   //Kullanıcının kimliğini doğrularken varsayılan olarak JWT Bearer kullan
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =  //Kimlik doğrulanamazsa hangi sistemin cevap vereceğini belirler.
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,     //Token’ı üreten sistem doğru mu?
                ValidateAudience = true,   //Token’ı kullanan sistem doğru mu?
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            jwtSettings.SecretKey)),

                ClockSkew = TimeSpan.Zero
            };
    });

builder.Services.AddScoped<IAuthenticationService,AuthenticationService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>(); //Bir sınıf IProductRepository isterse ona ProductRepository nesnesi ver.
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();
builder.Services.AddScoped<ICacheService, RedisCacheService>();

builder.Services.AddAutoMapper(config =>
{
    config.AddProfile<MappingProfile>();
});

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IServiceManager, ServiceManager>();
// Add services to the container.

builder.Services                            // Controller’ları Presentation projesinin assembly’sinde de ara
    .AddControllers()                           
    .AddApplicationPart(
        typeof(Presentation.Controllers.ProductsController)
            .Assembly);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "bearer",
        new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description =
                "JWT access token değerini girin."
        });

    options.AddSecurityRequirement(
        document => new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(
                "bearer",
                document)] = []
        });
});

builder.Services.AddOpenApi();

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = builder.Configuration
        .GetConnectionString("redisConnection");

    return ConnectionMultiplexer.Connect(configuration!);
});
var rabbitMqSettings = new RabbitMqSettings
{
    Host = builder.Configuration["RabbitMq:Host"]!,
    UserName = builder.Configuration["RabbitMq:UserName"]!,
    Password = builder.Configuration["RabbitMq:Password"]!
};

builder.Services.AddSingleton(rabbitMqSettings);
builder.Services.AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>();

var app = builder.Build();

app.ConfigureExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider
        .GetRequiredService<RoleManager<IdentityRole>>();

    var userManager = scope.ServiceProvider
        .GetRequiredService<UserManager<User>>();

    await IdentityDataSeeder.SeedRolesAsync(
        roleManager);

    await IdentityDataSeeder.SeedAdminAsync(
        userManager);
}



app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program
{
}