using Azure.AI.FormRecognizer.DocumentAnalysis;
using Azure;
using AzDocumentReadOpenAIAPI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder
        .WithOrigins("http://localhost:3000", "https://polite-flower-05321ea0f.4.azurestaticapps.net")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});

// Add configuration to read environment variables
builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddSingleton(sp =>
{
    string endpoint = builder.Configuration["AIServiceEndPoint"];
    string apiKey = builder.Configuration["AIServiceKey"];
    if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(apiKey))
    {
        throw new InvalidOperationException("OpenAIServiceEndPoint and OpenAIServiceKey must be configured.");
    }
    var credential = new AzureKeyCredential(apiKey);
    return new DocumentAnalysisClient(new Uri(endpoint), credential);
});

// Register AzOpenAI as a Singleton
builder.Services.AddSingleton<AzOpenAI>(sp =>
{
    string openAIEndPoint = builder.Configuration["openAIEndPoint"];
    string openAIDeploymentName = builder.Configuration["openAIDeploymentName"];
    string openAIKey = builder.Configuration["openAIKey"];

    if (string.IsNullOrEmpty(openAIEndPoint) || string.IsNullOrEmpty(openAIDeploymentName) || string.IsNullOrEmpty(openAIKey))
    {
        throw new Exception("One or more required Azure OpenAI environment variables are missing.");
    }

    var logger = sp.GetRequiredService<ILogger<AzOpenAI>>();
    return new AzOpenAI(openAIEndPoint, openAIKey, openAIDeploymentName, logger);
});

builder.Services.AddTransient<AzAIForm>();

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

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigin");

app.UseAuthorization();

app.MapControllers();

app.Run();
