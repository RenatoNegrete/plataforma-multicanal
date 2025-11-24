CREATE DATABASE IF NOT EXISTS providerdb;
USE providerdb;

CREATE TABLE IF NOT EXISTS products (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    precio INT NOT NULL,
    imagen TEXT NOT NULL,
    descripcion TEXT NOT NULL,
    categoria TEXT NOT NULL,
    stock INT NOT NULL
);

CREATE TABLE IF NOT EXISTS orders (
    id INT AUTO_INCREMENT PRIMARY KEY,
    product_id INT NOT NULL,
    buyer_email VARCHAR(255) NOT NULL,
    quantity INT NOT NULL,
    status VARCHAR(50) DEFAULT 'pending',
    order_date DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (product_id) REFERENCES products(id)
);

INSERT INTO products (nombre, precio, imagen, descripcion, categoria, stock)
VALUES 
('Laptop', 2500, 'url-laptop', 'Computadora portátil', 'fisico', 10),
('Mouse', 100, 'url-mouse', 'Mouse inalámbrico', 'fisico', 50);