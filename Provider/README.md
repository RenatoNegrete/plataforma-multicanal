# Instrucciones para ejecutar el proyecto

---

## En esta misma carpeta ejecutar los siguientes comandos

---

### 1. Construir la imagen de la aplicación

```bash
docker compose build
```

### 2. Levantar los contenedores

```bash
docker compose up -d
```

### 3. Verificar que los contenedores esten corriendo

```bash
docker ps
```

Deberia aparecer algo similar a:

```bash
api             Up  ...   0.0.0.0:8080->8080/tcp  
mysql-provider  Up  ...   0.0.0.0:3307->3306/tcp  
```

### 4. Probar la API

Opción 1: Usando Swagger

En un navegador, acceder a:

```bash
http://localhost:8080/swagger
```

Usando Postman o cURL

Todos los endpoints CRUD están implementados, pero se muestran los principales ejemplos:

Obtener todos los proveedores:

```bash
GET http://localhost:8080/api/Provider
```

Enviar una orden:

```bash
POST http://localhost:8080/api/Provider/receive-order
```

Body (JSON de ejemplo):

```bash
{
  "orderId": "1",
  "customerMail": "mail@gmail.com",
  "customerName": "nombre",
  "items": [
    {
      "productId": 1,
      "quantity": 1
    }
  ]
}
```

### Verificacion al del envio del correo

Si deseas comprobar que el correo realmente se envía, coloca tu dirección de correo electrónico en el campo customerMail.
Deberías recibir un mensaje confirmando la recepción de la orden.