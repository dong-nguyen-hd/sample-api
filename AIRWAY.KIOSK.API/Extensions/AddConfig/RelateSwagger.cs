using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

namespace AIRWAY.KIOSK.API.Extensions.AddConfig;

public static class RelateSwagger
{
    public static void AddCustomizeSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            // Add comment for DTO
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            
            c.CustomSchemaIds(type => type.ToString());
            c.EnableAnnotations();
            c.SwaggerDoc($"v1", new OpenApiInfo { Title = SystemInformation.ApplicationName, Version = $"v{SystemInformation.Version}" });

            var securityScheme = new OpenApiSecurityScheme
            {
                Name = SystemInformation.ApplicationName,
                Description = "Enter JWT Bearer token **_only_**",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer", // Must be lower case
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };
            c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {securityScheme, new string[] { }}
            });
        });
    }
}
