package org.example.demo3.resource;

import org.example.demo3.dto.RegistroRequest;
import org.example.demo3.entity.Usuario;
import org.example.demo3.service.UsuarioService;

import jakarta.inject.Inject;
import jakarta.validation.Valid;
import jakarta.ws.rs.*;
import jakarta.ws.rs.core.Context;
import jakarta.ws.rs.core.MediaType;
import jakarta.ws.rs.core.Response;
import jakarta.ws.rs.core.SecurityContext;

@Path("/usuarios")
@Produces(MediaType.APPLICATION_JSON)
@Consumes(MediaType.APPLICATION_JSON)
public class UsuarioResource {

    @Inject
    private UsuarioService usuarioService;

    @POST
    @Path("/register")
    public Response registrar(@Valid RegistroRequest request) {
        Usuario usuario = usuarioService.registrarUsuario(request);
        return Response.status(Response.Status.CREATED).entity(usuario).build();
    }

    @GET
    @Path("/health")
    public Response health() {
        return Response.ok("{\"status\":\"UP\"}").build();
    }

    @GET
    @Path("/me")
    public Response obtenerPerfil(@Context SecurityContext securityContext) {
        // El firebaseUid viene del token JWT validado por el API Gateway
        // El Gateway debe pasar el UID en un header custom
        
        // Opción 1: Si el Gateway pasa el UID en header
        // String firebaseUid = httpHeaders.getHeaderString("X-User-Id");
        
        // Opción 2: Si quieres extraerlo del JWT en este microservicio
        String firebaseUid = securityContext.getUserPrincipal().getName();
        
        Usuario usuario = usuarioService.obtenerPorFirebaseUid(firebaseUid);
        return Response.ok(usuario).build();
    }

    @PUT
    @Path("/me")
    public Response actualizarPerfil(
            @Context SecurityContext securityContext,
            Usuario datosActualizados) {
        
        String firebaseUid = securityContext.getUserPrincipal().getName();
        Usuario usuario = usuarioService.actualizarPerfil(firebaseUid, datosActualizados);
        
        return Response.ok(usuario).build();
    }
}
