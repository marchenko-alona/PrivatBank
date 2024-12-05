using PrivatBank.Applicaion.Middlewares;
using PrivatBank.HostedServices;
using QueueUtils.QueueServices;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Configuration.AddEnvironmentVariables();

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddSingleton<IQueueServiceSender, QueueServiceSender>();
        builder.Services.AddHostedService<QueueInitializer>();
        builder.Services.Configure<QueueSettings>(options =>
        {
            options.Host = builder.Configuration["RABBITMQ_HOST"];
            options.UserName = builder.Configuration["RABBITMQ_USER"];
            options.Password = builder.Configuration["RABBITMQ_PASS"];
            options.Queues = builder.Configuration["RABBITMQ_QUEUES"]?.Split(',').ToList();
            options.SendMessageQueueName = builder.Configuration["RPCQueueName"];
        });

        var app = builder.Build();

        app.UseDefaultFiles();
        app.UseStaticFiles();
        app.UseMiddleware<LoggingMiddleware>();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.MapFallbackToFile("/index.html");

        app.Run();
    }
}