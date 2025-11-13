using Microsoft.EntityFrameworkCore;
using VideoMicroservice.VideoMicroservice.Application.Services;
using VideoMicroservice.VideoMicroservice.Core.Abstractions;
using VideoMicroservice.VideoMicroservice.Persistence;
using VideoMicroservice.VideoMicroservice.Persistence.Repositories;

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
