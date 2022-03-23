var builder = WebApplication.CreateBuilder(args);

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
