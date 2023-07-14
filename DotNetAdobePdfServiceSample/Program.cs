using DotNetAdobePdfServiceSample.Lib.Interfaces;
using System.Reflection;
using DotNetAdobePdfServiceSample.Lib;
using Microsoft.Extensions.Options;

namespace DotNetAdobePdfServiceSample
{
    /// <summary>
    /// Program
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            ConfigurationManager configuration = builder.Configuration;

            builder.Services.Configure<AdobePdfServiceSetting>(configuration.GetSection("AdobePdfServiceSetting"));

            // NOTE appsettings.json Ç©ÇÁê›íËÇéÊìæÇ∑ÇÈèÍçá
            builder.Services.AddTransient<IExecutionContextFactory>(provider => new ExecutionContextFactory(provider.GetRequiredService<IOptions<AdobePdfServiceSetting>>().Value));

            // NOTE pdfservices-api-credentials.json Ç∆ private.key Ç©ÇÁê›íËÇéÊìæÇ∑ÇÈèÍçá
            //builder.Services.AddTransient<IExecutionContextFactory>(provider => new ExecutionContextFactory(provider.GetRequiredService<IHostEnvironment>().ContentRootPath));

            builder.Services.AddTransient(provider => provider.GetRequiredService<IExecutionContextFactory>().Create());
            builder.Services.AddTransient<IAdobePdfService, AdobePdfService>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                string xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });

            WebApplication app = builder.Build();

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
