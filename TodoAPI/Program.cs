using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TodoAPI.AppDataContext;
using TodoAPI.Interface;
using TodoAPI.Middleware;
using TodoAPI.Models;
using TodoAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Adding of automapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


// Adding of the database context

builder.Services.Configure<DbSettings>(builder.Configuration.GetSection("DbSettings"));

builder.Services.AddDbContext<TodoDbContext>((serviceProvider, options) =>
{
    var dbSettings = serviceProvider.GetRequiredService<IOptions<DbSettings>>().Value;
    options.UseSqlServer(dbSettings.ConnectionString);
});

builder.Services.AddScoped<TodoDbContext>();



// Adding service 

builder.Services.AddScoped<ITodoServices, TodoServices>();


// Adding of Exception Handler 
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddProblemDetails();


var app = builder.Build();


// ensure the database exit 

{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider;
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
