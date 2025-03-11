using Application;
using Infrastructure.Services;
using Kelist.API.Extensions;
using Kelist.API.Middlewares;

namespace Kelist.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddPresentation()
                            .AddInfrastructure(builder.Configuration)
                            .AddApplication();                            

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/openapi/v1.json", "v1");
                });
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