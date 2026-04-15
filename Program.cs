using System;
using TaskManagement.Api.Data;
using Microsoft.EntityFrameworkCore; // Add this using directive for 'UseSqlite'
using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Api.Services.Interfaces;
using TaskManagement.Api.Services.Implementations;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IProjectInterface, ProjectService>();
builder.Services.AddScoped<IFeatureInterface, FeatureService>();
builder.Services.AddScoped<IBacklogInterface, BacklogService>();
builder.Services.AddScoped<ITaskInterface, TaskService>();
builder.Services.AddControllers();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
