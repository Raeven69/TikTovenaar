using Microsoft.OpenApi.Models;

namespace TikTovenaar.Api
{
    public class Program
    {
        public static Database Db { get; } = new();

        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            WebApplication app = builder.Build();
            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((swagger, httpreq) =>
                {
                    swagger.Servers = [new OpenApiServer { Url = "/api" }];
                });
            });
            app.UseSwaggerUI();
            app.MapControllers();
            app.Run();
        }
    }
}
