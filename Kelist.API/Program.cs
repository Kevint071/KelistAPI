using System.Text;
using Application;
using Infrastructure.Services;
using Kelist.API.Extensions;
using Kelist.API.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp;
using DotNetEnv;

namespace Kelist.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Env.Load();

            var builder = WebApplication.CreateBuilder(args);

            var vaultToken = Environment.GetEnvironmentVariable("VAULT_TOKEN");
            if (string.IsNullOrEmpty(vaultToken)) throw new Exception("El token de Vault no está configurado en las variables de entorno.");

            var authMethod = new TokenAuthMethodInfo(vaultToken);
            var vaultClientSettings = new VaultClientSettings("http://localhost:8200", authMethod);
            var vaultClient = new VaultClient(vaultClientSettings);

            string jwtKey;
            try
            {
                var secret = await vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(path: "jwt-signing-key", mountPoint: "secret");
                jwtKey = secret.Data.Data["key"]?.ToString() ?? throw new InvalidOperationException("La clave JWT no se encontró en el secreto de Vault.");
            }
            catch (VaultSharp.Core.VaultApiException ex)
            {
                Console.WriteLine("Error al comunicarse con Vault");
                Console.WriteLine($"Status Code: {ex.StatusCode}");
                Console.WriteLine($"Message: {ex.Message}");
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error inesperado al leer el secreto de Vault");
                Console.WriteLine($"Message: {ex.Message}");
                return;
            }

            builder.Configuration["Jwt:Key"] = jwtKey;
            builder.Services.AddPresentation()
                            .AddInfrastructure(builder.Configuration)
                            .AddApplication();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        ValidateLifetime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                        ValidateIssuerSigningKey = true
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            Console.WriteLine("Authentication failed: " + context.Exception.Message);
                            return Task.CompletedTask;
                        }
                    };
                });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowLocalhost", policy =>
                {
                    policy.WithOrigins("http://localhost:3000")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            var app = builder.Build();
            if (app.Environment.IsDevelopment())
            {
                app.UseCors("AllowLocalhost");
                app.MapOpenApi();
                app.MapScalarApiReference();
                app.ApplyMigrations();
            }

            app.UseExceptionHandler("/error");
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
            app.MapControllers();

            app.Run();
        }
    }
}
