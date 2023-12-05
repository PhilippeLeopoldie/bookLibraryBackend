using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using LibraryBackend.Data;
using System.Text.Json.Serialization;

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
builder.Services.AddControllers().AddJsonOptions(x =>
x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);

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

app.Run();
