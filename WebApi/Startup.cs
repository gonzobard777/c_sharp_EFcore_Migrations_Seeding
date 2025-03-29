using Infrastructure;
using WebApi.Helpers;

namespace WebApi;

public class Startup
{
    private IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddInfrastructureDependencies(Configuration.GetConnectionString(ConnectionStrings.DBConnectionString));
        services.AddControllers();
        services.AddCors(options => options.AddPolicy("CorsPolicy", policyBuilder =>
        {
            policyBuilder
                .AllowAnyOrigin() // https://developer.mozilla.org/ru/docs/Web/HTTP/Headers/Access-Control-Allow-Origin
                .AllowAnyMethod()
                .AllowAnyHeader();
            // .AllowCredentials()
            // .WithExposedHeaders("*") // https://developer.mozilla.org/ru/docs/Web/HTTP/Headers/Access-Control-Allow-Headers
        }));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Миграции перед каждым стартом приложения.
        using var scope = app.ApplicationServices.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.MigrateAndAction();
        
        app.UseRouting();
        app.UseCors("policy");
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}