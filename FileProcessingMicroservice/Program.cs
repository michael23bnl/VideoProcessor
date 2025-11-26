using Amazon.S3;
using FileProcessingMicroservice.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = long.MaxValue;
});

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddScoped<IServerFileService, ServerFileService>();
builder.Services.AddScoped<IS3FileService, S3FileService>();

builder.Services.AddSingleton<IAmazonS3>(_ =>
{
    var config = new AmazonS3Config
    {
        ServiceURL = "http://localhost:9000", // эндпоинт MinIO
        ForcePathStyle = true,                
        UseHttp = true
    };

    var accessKey = "minioadmin";
    var secretKey = "minioadmin";

    return new AmazonS3Client(accessKey, secretKey, config);
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