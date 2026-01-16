using Microsoft.EntityFrameworkCore;
using VideoDataMicroservice.Application.Services;
using VideoDataMicroservice.Core.Abstractions;
using VideoDataMicroservice.Infrastructure;
using VideoDataMicroservice.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<VideoMicroserviceDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

builder.Services.AddScoped<IVideoDataService, VideoDataService>();
builder.Services.AddScoped<IVideoDataRepository, VideoDataRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
