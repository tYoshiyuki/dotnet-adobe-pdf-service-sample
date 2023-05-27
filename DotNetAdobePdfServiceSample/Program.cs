using DotNetAdobePdfServiceSample.Lib.Interfaces;
using System.Reflection;
using DotNetAdobePdfServiceSample.Lib;
using Microsoft.Extensions.Options;

namespace DotNetAdobePdfServiceSample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;

            builder.Services.Configure<AdobePdfServiceSettings>(configuration.GetSection("AdobePdfServiceSettings"));

            // NOTE appsettings.json Ç©ÇÁê›íËÇéÊìæÇ∑ÇÈèÍçá
            builder.Services.AddTransient<IExecutionContextFactory>(provider => new ExecutionContextFactory(provider.GetRequiredService<IOptions<AdobePdfServiceSettings>>().Value));

            // NOTE pdfservices-api-credentials.json Ç∆ private.key Ç©ÇÁê›íËÇéÊìæÇ∑ÇÈèÍçá
            //builder.Services.AddTransient<IExecutionContextFactory>(provider => new ExecutionContextFactory(provider.GetRequiredService<IHostEnvironment>().ContentRootPath));

            builder.Services.AddSingleton(provider => provider.GetRequiredService<IExecutionContextFactory>().Create());
            builder.Services.AddTransient<IAdobePdfService, AdobePdfService>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                string xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
