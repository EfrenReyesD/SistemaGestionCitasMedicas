using Microsoft.OpenApi.Models;
using System.Reflection;
using Serilog;
using FluentValidation;
using FluentValidation.AspNetCore;
using SistemaGestionCitasMedicas.Validators;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/sistema-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Iniciando Sistema de Gestión de Citas Médicas");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    // Add services to the container.
    builder.Services.AddControllers();
    
    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddValidatorsFromAssemblyContaining<PacienteValidator>();

    builder.Services.AddMemoryCache();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Sistema de Gestión de Citas Médicas",
            Version = "v1.0",
            Description = "API REST para gestionar citas médicas aplicando principios de POO",
            Contact = new OpenApiContact
            {
                Name = "Universidad IUV Maestría",
                Email = "contacto@ejemplo.com"
            }
        });

        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
        options.IncludeXmlComments(xmlPath);
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Sistema de Gestión de Citas Médicas v1.0");
            options.DocumentTitle = "API Citas Médicas - Swagger UI";
        });
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    Log.Information("Sistema iniciado correctamente en {Environment}", app.Environment.EnvironmentName);

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "El sistema falló al iniciar");
}
finally
{
    Log.CloseAndFlush();
}
