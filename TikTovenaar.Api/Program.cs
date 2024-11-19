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
                c.RouteTemplate = "swagger/{documentName}/swagger.json";
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TikTovenaarAPI");
                c.RoutePrefix = "";
            });
            app.MapControllers();
            app.Run();
        }
    }
}
