package com.example.api_gateway.api_gateway.routes;

import org.springframework.cloud.gateway.server.mvc.handler.GatewayRouterFunctions;
import org.springframework.cloud.gateway.server.mvc.handler.HandlerFunctions;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.web.servlet.function.RequestPredicates;
import org.springframework.web.servlet.function.RouterFunction;
import org.springframework.web.servlet.function.ServerResponse;

import com.example.api_gateway.api_gateway.configuration.ServiceUrlProperties;

@Configuration
public class Routes {

    private final ServiceUrlProperties serviceUrls;

    public Routes(ServiceUrlProperties serviceUrls) {
        this.serviceUrls = serviceUrls;
    }

    @Bean
    public RouterFunction<ServerResponse> allRoutes() {
        return GatewayRouterFunctions.route("usuarios-service")
                .route(RequestPredicates.path("/usuarios-service/api/usuarios/**"), 
                       HandlerFunctions.http(serviceUrls.getUsuarios()))
                .build()
                
            .and(GatewayRouterFunctions.route("catalogo-service")
                .route(RequestPredicates.path("/api/catalogo/**"), 
                       HandlerFunctions.http(serviceUrls.getCatalogo()))
                .build())
                
            .and(GatewayRouterFunctions.route("ordenes-service")
                .route(RequestPredicates.path("/api/ordenes/**"), 
                       HandlerFunctions.http(serviceUrls.getOrdenes()))
                .build())
                
            .and(GatewayRouterFunctions.route("pagos-service")
                .route(RequestPredicates.path("/api/pagos/**"), 
                       HandlerFunctions.http(serviceUrls.getPagos()))
                .build())
                
            .and(GatewayRouterFunctions.route("proveedores-service")
                .route(RequestPredicates.path("/api/proveedores/**"), 
                       HandlerFunctions.http(serviceUrls.getProveedores()))
                .build())
                
            .and(GatewayRouterFunctions.route("notificaciones-service")
                .route(RequestPredicates.path("/api/notificaciones/**"), 
                       HandlerFunctions.http(serviceUrls.getNotificaciones()))
                .build())
                
            .and(GatewayRouterFunctions.route("reportes-service")
                .route(RequestPredicates.path("/api/reportes/**"), 
                       HandlerFunctions.http(serviceUrls.getReportes()))
                .build())
                
            .and(GatewayRouterFunctions.route("recomendaciones-service")
                .route(RequestPredicates.path("/api/recomendaciones/**"), 
                       HandlerFunctions.http(serviceUrls.getRecomendaciones()))
                .build())
                
            .and(GatewayRouterFunctions.route("logistica-service")
                .route(RequestPredicates.path("/api/logistica/**"), 
                       HandlerFunctions.http(serviceUrls.getLogistica()))
                .build());
    }
}
