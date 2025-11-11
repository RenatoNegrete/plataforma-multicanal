using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProviderAPI.DTOs;
using ProviderAPI.Services;
using ProviderData.Protos;

namespace ProviderAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProviderController : ControllerBase
    {
        private readonly ProductService.ProductServiceClient _grpcClient;
        private readonly KafkaProducerService _kafkaProducer;

        public ProviderController(ProductService.ProductServiceClient grpcClient, KafkaProducerService kafkaProducer)
        {
            _grpcClient = grpcClient;
            _kafkaProducer = kafkaProducer;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var reply = await _grpcClient.GetAllAsync(new Empty());
            return Ok(reply.Products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var reply = await _grpcClient.GetByIdAsync(new ProductIdRequest { Id = id });
            return Ok(reply);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(ProductRequest product)
        {
            var created = await _grpcClient.CreateAsync(product);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPost("receive-order")]
        public async Task<IActionResult> ReceiveOrder([FromBody] OrderBatchRequest batch)
        {
            var total = 0;

            if (batch == null || batch.Items == null || !batch.Items.Any())
                return BadRequest("La order tiene que contener al menos un item");

            var detailedItems = new List<object>();

            foreach (var item in batch.Items)
            {
                var order = new OrderRequest
                {
                    ProductId = item.ProductId,
                    BuyerEmail = batch.CustomerMail,
                    Quantity = item.Quantity
                };
                var product = await _grpcClient.GetByIdAsync(new ProductIdRequest { Id = item.ProductId });
                total += product.Price * item.Quantity;
                await _grpcClient.CreateOrderAsync(order);
                detailedItems.Add(new
                {
                    ProductId = item.ProductId,
                    ProductName = product.Name,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price,
                    SubTotal = product.Price * item.Quantity
                });
            }

            var eventMessage = new
            {
                CustomerEmail = batch.CustomerMail,
                Total = total,
                ItemsCount = batch.Items.Count,
                Items = detailedItems,
                Timestamp = DateTime.UtcNow
            };

            await _kafkaProducer.PublishAsync(eventMessage);

            return Ok(new { message = "Orden recibida correctamente", items = batch.Items.Count, total });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ProductRequest product)
        {
            product.Id = id;
            await _grpcClient.UpdateAsync(product);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _grpcClient.DeleteAsync(new ProductIdRequest { Id = id });
            return NoContent();
        }
    }
}