package org.example.demo3.Controller;

import jakarta.inject.Inject;
import jakarta.ws.rs.*;
import jakarta.ws.rs.core.MediaType;
import jakarta.ws.rs.core.Response;
import org.example.demo3.Modelo.Mensaje;
import org.example.demo3.Modelo.Usuario;
import org.example.demo3.servicio.UsuarioServicio;

@Path("/usuarios")
@Consumes(MediaType.APPLICATION_JSON)
@Produces(MediaType.APPLICATION_JSON)
public class UsuarioController {


    @Inject
    private UsuarioServicio service;

    // POST -> agregar usuario
    @POST
    public Response crearUsuario(Usuario usuario) {
        service.crearUsuario(usuario);
        return Response.status(Response.Status.CREATED)
                .entity(usuario)
                .build();
    }

    // GET -> buscar usuario por ID
    @GET
    @Path("/{id}")
    public Response buscarUsuario(@PathParam("id") Long id) {
        Usuario usuario = service.buscarUsuarioPorId(id);

        if (usuario == null) {
            return Response.status(Response.Status.NOT_FOUND)
                    .entity("Usuario no encontrado")
                    .build();
        }

        return Response.ok(usuario).build();
    }

    @PUT
    @Path("/editar/{id}")
    public Response editarUsuario(@PathParam("id") Long id, Usuario usuario) {
        Usuario usuario1 = service.buscarUsuarioPorId(id);

        if (usuario1 == null) {
            return Response.status(Response.Status.NOT_FOUND)
                    .entity("Usuario no encontrado")
                    .build();

        }
        service.actualizarUsuario(usuario);
        Mensaje mensaje = new Mensaje("Usuario actualizado exitosamente");
        return Response.ok(mensaje).build();

    }

    @DELETE
    @Path("/{id}")
    public Response eliminarUsuario(@PathParam("id") Long id) {
        Usuario usuario = service.buscarUsuarioPorId(id);
        if (usuario == null) {
            return Response.status(Response.Status.NOT_FOUND)
                    .entity("Usuario no encontrado")
                    .build();
        }
        service.eliminarUsuario(id);
        Mensaje mensaje = new Mensaje("Usuario eliminado exitosamente");
        return Response.ok(mensaje).build();
    }
}
