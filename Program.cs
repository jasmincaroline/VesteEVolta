using Microsoft.EntityFrameworkCore;
using VesteEVolta.Models;
using VesteEVolta.Repositories;
using VesteEVolta.Repositories.Interfaces;
using VesteEVolta.Services;
using VesteEVolta.Services.Interfaces;


var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext
builder.Services.AddDbContext<VesteEVoltaContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
    .UseSnakeCaseNamingConvention()
);

// Dependency Injection
builder.Services.AddDbContext<VesteEVoltaContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
