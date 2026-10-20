using HeadToHead_Latelier.Common.Interfaces.Services;
using HeadToHead_Latelier.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Enregistrer les services HeadToHead
builder.Services.AddScoped<IHeadToHeadService, HeadToHeadService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
