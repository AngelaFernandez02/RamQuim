using Microsoft.EntityFrameworkCore;
using RamQuim.Models;
using RamQuim.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddScoped<EmailService>();
// CORS
builder.Services.AddCors(options =>
{
options.AddPolicy("AllowAll", policy =>
    {
       
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// DB
var connectionString =
    builder.Configuration.GetConnectionString("cadenaSQL");

builder.Services.AddDbContext<RamQuimContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseCors("AllowAll");   // SOLO UNA POLÍTICA

app.UseAuthorization();

app.MapControllers();

app.Run();
