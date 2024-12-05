﻿using QueueUtils.QueueServices;
using QueueUtils.QueueServices.Configs;
using RequestProcessor.HostedServices;

namespace RequestProcessor;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        //var messageHandler = host.Services.GetRequiredService<MessageHandler>();
        //await messageHandler.StartListening(CancellationToken.None);
        //Console.WriteLine("Listening to RabbitMQ...");

        await host.RunAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                config.AddEnvironmentVariables();
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.Configure<QueueSettings>(options =>
                {
                    options.Host = hostContext.Configuration["RABBITMQ_HOST"];
                    options.UserName = hostContext.Configuration["RABBITMQ_USER"];
                    options.Password = hostContext.Configuration["RABBITMQ_PASS"];
                    options.SendMessageQueueName = hostContext.Configuration["RPCQueueName"];
                });

                services.Configure<DatabaseSettings>(options =>
                {
                    options.DatabaseType = hostContext.Configuration["Database_Type"] == "PostgreSql" ? DatabaseType.PostgreSql : DatabaseType.MsSql;

                    if (options.DatabaseType == DatabaseType.PostgreSql)
                    {
                        options.ConnectionString = hostContext.Configuration["Postgres_Connection_String"];
                    }
                    else
                    {
                        options.ConnectionString = hostContext.Configuration["MSSQL_ConnectionString"];
                    }
                });

                services.AddScoped<IQueueServiceConsumer, QueueServiceConsumer>();
                services.AddHostedService<MessageHandlerHostedService>();
            });
}