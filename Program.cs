using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsProduction()) {
    var keyVaultEndpoint = new Uri(Environment.GetEnvironmentVariable("VaultUri"));
    builder.Configuration.AddAzureKeyVault(keyVaultEndpoint, new DefaultAzureCredential());
}

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSwaggerDocument();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseOpenApi();
    app.UseSwaggerUi3();
}

//app.MapGet("/", () => "Hello World!");
app.MapControllers();

app.Run();
