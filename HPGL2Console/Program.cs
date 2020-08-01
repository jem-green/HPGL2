using System;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading;
using System.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using HPGL2Library;

namespace HPGL2Console
{
    class Program
    {
        static void Main(string[] args)
        {
            // create service collection
            var services = new ServiceCollection();
            ConfigureServices(services);

            // create service provider
            var serviceProvider = services.BuildServiceProvider();

            // entry to run app
            serviceProvider.GetService<App>().Run(args);

        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // configure logger options

            FileLoggerOptions fileLogger = new FileLoggerOptions();
            fileLogger.FileName = "HPGL2";
            fileLogger.Extension = ".log";

            // configure logging
            services.AddLogging(builder =>
                builder
                    .AddDebug()
                    //.AddConsole()
                    .SetMinimumLevel(LogLevel.Debug)
                    .AddProvider(new FileLoggerProvider(fileLogger))
                    .AddProvider(new CustomLoggerProvider())
                    );

            services.AddTransient<App>();
        }
    }
}
