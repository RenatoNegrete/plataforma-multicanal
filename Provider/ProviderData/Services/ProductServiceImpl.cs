using System;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using ProviderData.Entities;
using ProviderData.Protos;

namespace ProviderData.Services;

public class ProductServiceImpl : ProductService.ProductServiceBase
{
    private readonly ProviderDbContext _context;

    public ProductServiceImpl(ProviderDbContext context)
    {
        _context = context;
    }

    public override async Task<ProductList> GetAll(Empty request, ServerCallContext context)
    {
        var products = await _context.Products.ToListAsync();
        var response = new ProductList();
        response.Products.AddRange(products.Select(p => new ProductResponse
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            Quantity = p.Quantity,
            PriceType = p.PriceType,
            QuantityType = p.QuantityType
        }));
        return response;
    }

    public override async Task<ProductResponse> GetById(ProductIdRequest request, ServerCallContext context)
    {
        var p = await _context.Products.FindAsync(request.Id);
        if (p == null) throw new RpcException(new Status(StatusCode.NotFound, "Product not found"));
        return new ProductResponse
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            Quantity = p.Quantity,
            PriceType = p.PriceType,
            QuantityType = p.QuantityType
        };
    }

    public override async Task<ProductResponse> Create(ProductRequest request, ServerCallContext context)
    {
        var entity = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Quantity = request.Quantity,
            PriceType = request.PriceType,
            QuantityType = request.QuantityType
        };
        _context.Products.Add(entity);
        await _context.SaveChangesAsync();
        request.Id = entity.Id;
        return new ProductResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Price = entity.Price,
            Quantity = entity.Quantity,
            PriceType = entity.PriceType,
            QuantityType = entity.QuantityType
        };
    }

    public override async Task<ProductResponse> Update(ProductRequest request, ServerCallContext context)
    {
        var entity = await _context.Products.FindAsync(request.Id);
        if (entity == null) throw new RpcException(new Status(StatusCode.NotFound, "Product not found"));
        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.Price = request.Price;
        entity.Quantity = request.Quantity;
        entity.PriceType = request.PriceType;
        entity.QuantityType = request.QuantityType;
        await _context.SaveChangesAsync();
        return new ProductResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Price = entity.Price,
            Quantity = entity.Quantity,
            PriceType = entity.PriceType,
            QuantityType = entity.QuantityType
        };
    }

    public override async Task<Empty> Delete(ProductIdRequest request, ServerCallContext context)
    {
        var entity = await _context.Products.FindAsync(request.Id);
        if (entity == null) throw new RpcException(new Status(StatusCode.NotFound, "Product not found"));
        _context.Products.Remove(entity);
        await _context.SaveChangesAsync();
        return new Empty();
    }

    public override async Task<OrderReply> CreateOrder(OrderRequest request, ServerCallContext context)
    {
        var order = new Order
        {
            ProductId = request.ProductId,
            BuyerEmail = request.BuyerEmail,
            Quantity = request.Quantity,
            Status = "pending",
            OrderDate = DateTime.Now
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return new OrderReply
        {
            OrderId = order.Id,
            Status = order.Status
        };
    }
}
