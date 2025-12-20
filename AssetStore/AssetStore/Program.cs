using Stripe;
using Microsoft.OpenApi;

public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        ConfigureServices(builder.Services);

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: "default", policy =>
            {
                policy.AllowAnyOrigin()
                      .WithOrigins("http://localhost:3000")
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        var app = builder.Build();
        Configure(app, app.Environment);

        app.Run();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        services.AddOpenApi();

        // Add Swagger/OpenAPI generation
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "AssetStore API",
                Version = "v1"
            });
        });
    }

    private static void Configure(WebApplication app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseDeveloperExceptionPage();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Serve Swagger UI at the root ("/")
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "AssetStore API v1");
                options.RoutePrefix = string.Empty; // Serve at root
            });
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        ConfigureStripe(app, env);
    }

    private static void ConfigureStripe(WebApplication app, IWebHostEnvironment env)
    {
        // This test secret API key is a placeholder. Don't include personal details in requests with this key.
        // To see your test secret API key embedded in code samples, sign in to your Stripe account.
        // You can also find your test secret API key at https://dashboard.stripe.com/test/apikeys.
        StripeConfiguration.ApiKey = "";
    }
}