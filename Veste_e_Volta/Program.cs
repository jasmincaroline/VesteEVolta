using Microsoft.EntityFrameworkCore;
using VesteEVolta.Models;
using VesteEVolta.Repositories;
using VesteEVolta.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddDbContext<PostgresContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ICategoryRepositories, CategoryRepositories>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Run();
