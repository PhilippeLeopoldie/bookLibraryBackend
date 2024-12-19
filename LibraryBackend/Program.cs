using Microsoft.EntityFrameworkCore;
using LibraryBackend.Services;
using LibraryBackend.Repositories;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);


if(builder.Environment.IsProduction())
{
  var productionConnectionString = Environment.GetEnvironmentVariable("DefaultConnection");
  builder.Services.AddDbContext<MyLibraryContext>(options =>
  options.UseNpgsql(productionConnectionString));
}
else
{
  builder.Configuration.AddUserSecrets<Program>();
  var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
  builder.Services.AddDbContext<MyLibraryContext>(options =>
  options.UseNpgsql(connectionString));
}


var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
  options.AddPolicy(name: MyAllowSpecificOrigins,
                     policy =>
                     {
                       policy.WithOrigins(
                         "https://booklibrary-backend-20f7a19cecb2.herokuapp.com")
                       .AllowAnyHeader()
                       .AllowAnyMethod()
                       .SetIsOriginAllowed((host) => true)
                     .AllowCredentials();
                     });
});

// Add services to the container.
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IOpinionService, OpinionService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddControllers().AddJsonOptions(options =>
options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

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

app.UseAuthorization();

app.UseCors(MyAllowSpecificOrigins);

app.MapControllers();

// **Test Code Snippet**
string rawString = """
This is a raw string literal.
""";
Console.WriteLine(rawString);

app.Run();
