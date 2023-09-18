using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using LibraryBackend.Data;
using System.Text.Json.Serialization;
using LibraryBackend.Models;

var builder = WebApplication.CreateBuilder(args);

// Add database context and connection string

  var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");



builder.Services.AddDbContext<MyLibraryContext>(options =>
    options.UseNpgsql(connectionString));

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
  options.AddPolicy(name: MyAllowSpecificOrigins,
                     policy =>
                     {
                       policy.WithOrigins(
                         "https://booklibrary-backend-20f7a19cecb2.herokuapp.com/")
                       .AllowAnyHeader()
                       .AllowAnyMethod()
                       .SetIsOriginAllowed((host) => true)
                     .AllowCredentials();
                     });
});

// Add services to the container.
builder.Services.AddScoped(typeof(IRepository<>),typeof(Repository<>));
builder.Services.AddControllers().AddJsonOptions(x => 
x.JsonSerializerOptions.ReferenceHandler= ReferenceHandler.Preserve);
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
