using Microsoft.AspNetCore.Localization;
using Microsoft.OpenApi.Models;
using QuestPDF.Infrastructure;
using RecomeceAPI.DI;
using RecomeceAPI.Middlewares;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Registra injeção de dependência
builder.Services.AddApplicationServices();
builder.Services.AddTableServices();
builder.Services.AddAsaasServices();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(c =>
{
  c.SwaggerDoc("v1", new OpenApiInfo { Title = "Recomece API", Version = "v1" });

  c.AddSecurityDefinition("Basic", new OpenApiSecurityScheme
  {
    Description = "Entre com o token no formato: Basic {seu_token}",
    Name = "Authorization",
    In = ParameterLocation.Header,
    Type = SecuritySchemeType.ApiKey // Alterado de Http para ApiKey
  });

  c.AddSecurityRequirement(new OpenApiSecurityRequirement
  {
      {
          new OpenApiSecurityScheme
          {
              Reference = new OpenApiReference
              {
                  Type = ReferenceType.SecurityScheme,
                  Id = "Basic"
              }
          },
          new string[] {}
      }
  });
});

// Configuração CORS
builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowAll", builder =>
  {
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
  });
});

builder.Logging.AddConsole();

var app = builder.Build();

QuestPDF.Settings.License = LicenseType.Community;

//------------------------------------------------------------------------------------------------------------------------------------
// Forçar o idioma sempre em inglês para evitar problemas de moeda e data
var supportedCultures = new[] { new CultureInfo("en-US") };
app.UseRequestLocalization(new RequestLocalizationOptions
{
  DefaultRequestCulture = new RequestCulture("en-US"),
  SupportedCultures = supportedCultures,
  FallBackToParentCultures = false
});
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
//------------------------------------------------------------------------------------------------------------------------------------
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseCors("AllowAll");

// Lista de rotas que não requerem token
var excludedRoutes = new List<string>
        {
            "/api/public",
            "/api/Login",
            "/api/Asaas",
        };

// Registra o middleware de autenticação, passando as rotas excluídas
app.UseMiddleware<AuthMiddleware>(excludedRoutes);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
