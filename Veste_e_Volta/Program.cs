using Microsoft.EntityFrameworkCore;
using VesteEVolta.Models;

var builder = WebApplication.CreateBuilder(args);    

builder.Services.AddControllers();

builder.Services.AddDbContext<PostgresContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();      

app.MapControllers();   
app.Run();      