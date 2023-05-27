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

            builder.Services.Configure<List<AdobePdfServiceSetting>>(configuration.GetSection("AdobePdfServiceSettingList"));

            // NOTE appsettings.json ����ݒ���擾����ꍇ
            // �����̃N���f���V�������珈�����邱�Ƃ�z�肵�A�Ӑ}�I�ɐ؂�ւ����s���\���Ƃ��Ă��܂��B
            builder.Services.AddTransient<IExecutionContextFactory>(provider => new ExecutionContextFactory(provider.GetRequiredService<IOptions<List<AdobePdfServiceSetting>>>()
                .Value.Random()));

            // NOTE pdfservices-api-credentials.json �� private.key ����ݒ���擾����ꍇ
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
