
using Amazon.S3;
using VideoProcessingMicroservice.Application.Abstractions;
using VideoProcessingMicroservice.Application.Handlers;
using VideoProcessingMicroservice.Application.Services;
using VideoProcessingMicroservice.Application.Services.BackgroundServices;
using VideoProcessingMicroservice.Core.Abstractions;
using VideoProcessingMicroservice.Infrastructure.Ffmpeg;
using VideoProcessingMicroservice.Infrastructure.Http;
using VideoProcessingMicroservice.Infrastructure.RabbitMq;
using VideoProcessingMicroservice.Infrastructure.RabbitMq.Connection;
using VideoProcessingMicroservice.Infrastructure.S3;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = long.MaxValue;
});

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddScoped<IVideoStorage, S3VideoStorage>();
builder.Services.AddScoped<IVideoProcessingService, VideoProcessingService>();
builder.Services.AddScoped<IVideoUploadedHandler, VideoUploadedHandler>();
builder.Services.AddScoped<IVideoProcessor, FfmpegVideoProcessor>();
builder.Services.AddScoped<IVideoUploadService, VideoUploadService>();

builder.Services.AddSingleton<IMessageSubscriber, RabbitMqSubscriber>();
builder.Services.AddSingleton<IRabbitMqConnection>(await RabbitMqConnection.InitializeConnectionAsync());
builder.Services.AddSingleton<IMessageProducer>(sp =>
{
    var connection = sp.GetRequiredService<IRabbitMqConnection>();

    return RabbitMqProducer
        .CreateAsync(connection, CancellationToken.None)
        .GetAwaiter()
        .GetResult();
});
builder.Services.AddHostedService<RabbitMqBackgroundService>();

builder.Services.AddSingleton<IAmazonS3>(_ =>
{
    var minioConfig = builder.Configuration.GetSection("Minio");
    var config = new AmazonS3Config
    {
        ServiceURL = minioConfig["EndpointUrl"], // эндпоинт MinIO
        ForcePathStyle = true,                
        UseHttp = true
    };
    var accessKey = minioConfig["AccessKey"];
    var secretKey = minioConfig["SecretKey"];

    return new AmazonS3Client(accessKey, secretKey, config);
});

builder.Services.AddHttpClient<IVideoMetadataClient, VideoMetadataClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["VideoMetadataMicroserviceUrl"]);
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.MapControllers();

app.UseHttpsRedirection();

app.UseCors();

app.Run();