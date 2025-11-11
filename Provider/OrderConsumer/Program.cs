using OrderConsumer.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<EmailService>();
builder.Services.AddSingleton<OrderConsumerService>();
builder.Services.AddSingleton<KafkaConsumerService>();
builder.Services.AddHostedService<KafkaConsumerHostedService>();

builder.Services.AddLogging();

var app = builder.Build();
app.Run();
