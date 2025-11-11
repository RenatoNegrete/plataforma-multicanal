using Grpc.Net.Client;
using ProviderAPI.Services;
using ProviderData.Protos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSingleton(provider =>
{
    // Dirección del servidor gRPC (ProviderData)
    var channel = GrpcChannel.ForAddress("http://provider-data:8080");
    return new ProductService.ProductServiceClient(channel);
});

builder.Services.AddSingleton<KafkaProducerService>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var topicInitializer = new KafkaTopicInitializer();
await topicInitializer.EnsureTopicExistsAsync("order-confirmations");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
