CREATE DATABASE IF NOT EXISTS providerdb;
USE providerdb;

CREATE TABLE IF NOT EXISTS products (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    description TEXT NOT NULL,
    price INT NOT NULL,
    quantity INT NOT NULL,
    price_type INT NOT NULL,
    quantity_type INT NOT NULL
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

INSERT INTO products (name, description, price, quantity, price_type, quantity_type)
VALUES 
('Laptop', 'Computadora portatil', 2500, 10, 1, 1),
('Mouse', 'Mouse inalambrico', 100, 50, 1, 1);