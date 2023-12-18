using Microsoft.EntityFrameworkCore;
using LibraryBackend.Services;
using LibraryBackend.Repositories;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// in production environment variable set on heruko
if (builder.Environment.IsProduction())
{
  var productionConnectionString = Environment.GetEnvironmentVariable("DefaultConnection");
  builder.Services.AddDbContext<MyLibraryContext>(options =>
  options.UseNpgsql(productionConnectionString));
}
else // in development environment variable set using user secrets
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
builder.Services.AddControllers().AddJsonOptions(x =>
x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Authentication
builder.Services
  .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(options =>
  {
    var validIssuer = "";
    var validAudience = "";
    var issuerSigningKey = "";

    // in production environment variable set on heruko
    if (builder.Environment.IsProduction())
    {
      validIssuer = Environment.GetEnvironmentVariable("validIssuer");
      validAudience = Environment.GetEnvironmentVariable("validAudience");
      issuerSigningKey = Environment.GetEnvironmentVariable("issuerSigningkey");

    }
    else // in development
    {
      // Read configurtion values from appsettings.Development.json
      var userConfig = builder.Configuration.GetSection("JwtConfiguration");
      validIssuer = userConfig["validIssuer"];
      validAudience = userConfig["validAudience"];

      // Use user secrets
      builder.Configuration.AddUserSecrets<Program>();
      issuerSigningKey = builder.Configuration["JwtConfiguration:issuerSigningKey"];
      
    }

    // Check issuerSigningKey nullability before using it
    byte[] issuerSigningKeyBytes = issuerSigningKey is not null 
    ? Encoding.UTF8.GetBytes(issuerSigningKey)
    : Array.Empty<byte>();
    options.TokenValidationParameters = new TokenValidationParameters()
    {
      ClockSkew = TimeSpan.Zero,
      ValidateIssuer = true,
      ValidateAudience = true,
      ValidateLifetime = true,
      ValidateIssuerSigningKey = true,
      ValidIssuer = validIssuer,
      ValidAudience = validAudience,
      IssuerSigningKey = new SymmetricSecurityKey(issuerSigningKeyBytes),
    };
  });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseCors(MyAllowSpecificOrigins);

app.MapControllers();

app.Run();
