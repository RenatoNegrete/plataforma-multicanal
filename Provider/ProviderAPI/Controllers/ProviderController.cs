using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using ProviderAPI.DTOs;
using ProviderData.Protos;

namespace ProviderAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProviderController : ControllerBase
    {
        private readonly ProductService.ProductServiceClient _grpcClient;

        public ProviderController(ProductService.ProductServiceClient grpcClient)
        {
            _grpcClient = grpcClient;
        }

        // GET: api/provider
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var reply = await _grpcClient.GetAllAsync(new Empty());
            return Ok(reply.Products);
        }


        // GET: api/provider/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var reply = await _grpcClient.GetByIdAsync(new ProductIdRequest { Id = id });
            return Ok(reply);
        }


        // POST: api/provider/create
        [HttpPost("create")]
        public async Task<IActionResult> Create(ProductRequest product)
        {
            var created = await _grpcClient.CreateAsync(product);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }


        // POST: api/provider/receive-order
        [HttpPost("receive-order")]
        public async Task<IActionResult> ReceiveOrder([FromBody] OrderBatchRequest batch)
        {
            if (batch == null || batch.Items == null || !batch.Items.Any())
                return BadRequest("La orden debe contener al menos un ítem");

            var total = 0;
            var detailedItems = new List<object>();

            foreach (var item in batch.Items)
            {
                var product = await _grpcClient.GetByIdAsync(
                    new ProductIdRequest { Id = item.ProductId }
                );

                total += product.Precio * item.Quantity;

                await _grpcClient.CreateOrderAsync(new OrderRequest
                {
                    ProductId = item.ProductId,
                    BuyerEmail = batch.CustomerMail,
                    Quantity = item.Quantity
                });

                detailedItems.Add(new
                {
                    ProductId = item.ProductId,
                    ProductName = product.Nombre,
                    Quantity = item.Quantity,
                    UnitPrice = product.Precio,
                    SubTotal = product.Precio * item.Quantity
                });
            }

            return Ok(new
            {
                message = "Orden recibida correctamente",
                customer = batch.CustomerMail,
                total,
                items = detailedItems
            });
        }


        // PUT: api/provider/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ProductRequest product)
        {
            product.Id = id;
            await _grpcClient.UpdateAsync(product);
            return NoContent();
        }


        // DELETE: api/provider/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _grpcClient.DeleteAsync(new ProductIdRequest { Id = id });
            return NoContent();
        }
    }
}
