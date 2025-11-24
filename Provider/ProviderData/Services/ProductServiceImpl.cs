using System;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using ProviderData.Models;
using ProviderData.Protos;

namespace ProviderData.Services;

public class ProductServiceImpl : ProductService.ProductServiceBase
{
    private readonly ProviderdbContext _context;

    public ProductServiceImpl(ProviderdbContext context)
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
            Nombre = p.Nombre,
            Precio = p.Precio,
            Imagen = p.Imagen,
            Descripcion = p.Descripcion,
            Categoria = p.Categoria,
            Stock = p.Stock
        }));

        return response;
    }

    public override async Task<ProductResponse> GetById(ProductIdRequest request, ServerCallContext context)
    {
        var p = await _context.Products.FindAsync(request.Id);

        if (p == null)
            throw new RpcException(new Status(StatusCode.NotFound, "Product not found"));

        return new ProductResponse
        {
            Id = p.Id,
            Nombre = p.Nombre,
            Precio = p.Precio,
            Imagen = p.Imagen,
            Descripcion = p.Descripcion,
            Categoria = p.Categoria,
            Stock = p.Stock
        };
    }

    public override async Task<ProductResponse> Create(ProductRequest request, ServerCallContext context)
    {
        var entity = new Product
        {
            Nombre = request.Nombre,
            Precio = request.Precio,
            Imagen = request.Imagen,
            Descripcion = request.Descripcion,
            Categoria = request.Categoria,
            Stock = request.Stock
        };

        _context.Products.Add(entity);
        await _context.SaveChangesAsync();

        return new ProductResponse
        {
            Id = entity.Id,
            Nombre = entity.Nombre,
            Precio = entity.Precio,
            Imagen = entity.Imagen,
            Descripcion = entity.Descripcion,
            Categoria = entity.Categoria,
            Stock = entity.Stock
        };
    }

    public override async Task<ProductResponse> Update(ProductRequest request, ServerCallContext context)
    {
        var entity = await _context.Products.FindAsync(request.Id);

        if (entity == null)
            throw new RpcException(new Status(StatusCode.NotFound, "Product not found"));

        entity.Nombre = request.Nombre;
        entity.Precio = request.Precio;
        entity.Imagen = request.Imagen;
        entity.Descripcion = request.Descripcion;
        entity.Categoria = request.Categoria;
        entity.Stock = request.Stock;

        await _context.SaveChangesAsync();

        return new ProductResponse
        {
            Id = entity.Id,
            Nombre = entity.Nombre,
            Precio = entity.Precio,
            Imagen = entity.Imagen,
            Descripcion = entity.Descripcion,
            Categoria = entity.Categoria,
            Stock = entity.Stock
        };
    }

    public override async Task<Empty> Delete(ProductIdRequest request, ServerCallContext context)
    {
        var entity = await _context.Products.FindAsync(request.Id);

        if (entity == null)
            throw new RpcException(new Status(StatusCode.NotFound, "Product not found"));

        _context.Products.Remove(entity);
        await _context.SaveChangesAsync();

        return new Empty();
    }

    public override async Task<OrderResponse> CreateOrder(OrderRequest request, ServerCallContext context)
    {
        // validar producto
        var product = await _context.Products.FindAsync(request.ProductId);
        if (product == null)
            throw new RpcException(new Status(StatusCode.NotFound, "Product not found"));

        var order = new Order
        {
            ProductId = request.ProductId,
            BuyerEmail = request.BuyerEmail,
            Quantity = request.Quantity,
            Status = "pending",
            OrderDate = DateTime.UtcNow
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return new OrderResponse
        {
            Id = order.Id,
            ProductId = order.ProductId,
            BuyerEmail = order.BuyerEmail,
            Quantity = order.Quantity,
            Status = order.Status,
            OrderDate = Timestamp.FromDateTime(order.OrderDate.Value.ToUniversalTime())
        };
    }

    public override async Task<OrderResponse> GetOrderById(OrderIdRequest request, ServerCallContext context)
    {
        var o = await _context.Orders.FindAsync(request.Id);

        if (o == null)
            throw new RpcException(new Status(StatusCode.NotFound, "Order not found"));

        return new OrderResponse
        {
            Id = o.Id,
            ProductId = o.ProductId,
            BuyerEmail = o.BuyerEmail,
            Quantity = o.Quantity,
            Status = o.Status,
            OrderDate = Timestamp.FromDateTime(o.OrderDate.Value.ToUniversalTime())
        };
    }
}
