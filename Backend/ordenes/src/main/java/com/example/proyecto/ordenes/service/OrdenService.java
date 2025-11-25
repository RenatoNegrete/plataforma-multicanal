package com.example.proyecto.ordenes.service;

import com.example.proyecto.ordenes.dto.CotizacionEnvioRequest;
import com.example.proyecto.ordenes.dto.IntencionPagoRequest;
import com.example.proyecto.ordenes.dto.ItemOrden;
import com.example.proyecto.ordenes.dto.OrdenRequest;
import com.example.proyecto.ordenes.dto.ProductoDto;
import com.example.proyecto.ordenes.model.Orden;
import com.example.proyecto.ordenes.repository.OrdenRepository;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.time.LocalDate;
import java.util.List;

@Service
public class OrdenService {

    private final OrdenRepository repository;
    private final CatalogoClient catalogoClient;
    private final PagosClient pagosClient;
    private final EnviosClient enviosClient;

    public OrdenService(OrdenRepository repository, CatalogoClient catalogoClient, PagosClient pagosClient, EnviosClient enviosClient) {
        this.repository = repository;
        this.catalogoClient = catalogoClient;
        this.pagosClient = pagosClient;
        this.enviosClient = enviosClient;
    }

    @Transactional
    public Orden crearOrden(OrdenRequest request) {
        Double totalOrden = 0.0;

        for (ItemOrden item : request.getProductos()) {
            String productoId = item.getProductoId();
            Integer cantidad = item.getCantidad();
            
            if (!catalogoClient.verificarDisponibilidad(productoId, cantidad)) {
                throw new RuntimeException(
                    "Producto no disponible o stock insuficiente: " + productoId);
            }
            
            ProductoDto producto = catalogoClient.obtenerProducto(productoId);
            totalOrden += producto.getPrecio() * cantidad;
            
            catalogoClient.actualizarStock(productoId, cantidad, producto);
        }

        Long totalFinal = Math.round(totalOrden);
        IntencionPagoRequest pagoRequest = new IntencionPagoRequest();
        pagoRequest.setAmount(totalFinal);
        pagoRequest.setCurrency("USD");
        pagoRequest.setDescription("Pago por orden de cliente ID: " + request.getClienteId());
        pagoRequest.setClientMail(request.getClienteMail());

        try {
            pagosClient.crearIntencionPago(pagoRequest);
        } catch (Exception e) {
            revertirStock(request.getProductos());
            throw new RuntimeException("Error al crear intención de pago", e);
        }

        CotizacionEnvioRequest envioRequest = new CotizacionEnvioRequest();
        envioRequest.setEmail(request.getClienteMail());

        try {
            enviosClient.cotizarEnvio(envioRequest);
        } catch (Exception e) {
            revertirStock(request.getProductos());
            throw new RuntimeException("Error al cotizar envío", e);
        }

        Orden orden = new Orden();
        orden.setClienteId(request.getClienteId());
        orden.setProductos(request.getProductos());
        orden.setFecha(LocalDate.now());
        orden.setTotal(totalOrden);
        
        return repository.save(orden);
    }

    public List<Orden> listarOrdenes() {
        return repository.findAll();
    }

    public Orden obtenerPorId(String id) {
        return repository.findById(id).orElse(null);
    }

    public List<Orden> listarPorCliente(Long clienteId) {
        return repository.findByClienteId(clienteId);
    }

    private void revertirStock(List<ItemOrden> productos) {
        for (ItemOrden item : productos) {
            try {
                ProductoDto producto = catalogoClient.obtenerProducto(item.getProductoId());
                catalogoClient.actualizarStock(
                    item.getProductoId(), 
                    -item.getCantidad(),
                    producto
                );
            } catch (Exception e) {
                System.err.println("Error al revertir stock para producto: " + 
                    item.getProductoId() + " - " + e.getMessage());
            }
        }
    }
}
