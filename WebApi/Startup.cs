﻿using Infrastructure;

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
        services.AddInfrastructureDependencies(Configuration);
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

    public void Configure(IApplicationBuilder app, IWebHostEnvironment _)
    {
        app.UseRouting();
        app.UseCors("policy");
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}