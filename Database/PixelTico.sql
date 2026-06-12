-- Crear la base de datos
Create DATABASE PixelTico;
GO
USE PixelTico;
GO

-- Crear Tabla Categoria
CREATE TABLE Categoria (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL UNIQUE
);

-- Crear Tabla Producto
CREATE TABLE Producto (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(150) NOT NULL, 
    CategoriaId INT NOT NULL,
    Precio DECIMAL(10,2) NOT NULL CONSTRAINT CHK_Producto_Precio CHECK (Precio >= 0), 
    ImpuestoPorc DECIMAL(5,2) NOT NULL DEFAULT 13.00, 
    Stock INT NOT NULL CONSTRAINT CHK_Producto_Stock CHECK (Stock >= 0),
    ImagenUrl VARCHAR(500) NULL,
    Activo BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Producto_Categoria FOREIGN KEY (CategoriaId) REFERENCES Categoria(Id)
);

-- Crear Tabla Cliente
CREATE TABLE Cliente (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL, 
    Cedula VARCHAR(20) NOT NULL UNIQUE, 
    Correo VARCHAR(100) NULL, 
    Telefono VARCHAR(20) NULL, 
    Direccion VARCHAR(500) NULL 
);

-- Crear Tabla Usuario
CREATE TABLE Usuario (
    Id INT IDENTITY(1,1) PRIMARY KEY, 
    Nombre VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE, 
    PasswordHash VARCHAR(MAX) NULL,
    Rol VARCHAR(30) NOT NULL 
);

-- Crear Tabla Pedido
CREATE TABLE Pedido (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ClienteId INT NOT NULL,
    UsuarioId INT NOT NULL, 
    Fecha DATETIME NOT NULL DEFAULT GETDATE(),
    Subtotal DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    Impuestos DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    Total DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    Estado VARCHAR(30) NOT NULL DEFAULT 'Pendiente', 
    CONSTRAINT FK_Pedido_Cliente FOREIGN KEY (ClienteId) REFERENCES Cliente(Id),
    CONSTRAINT FK_Pedido_Usuario FOREIGN KEY (UsuarioId) REFERENCES Usuario(Id)
);

-- Crear Tabla PedidoDetalle
CREATE TABLE PedidoDetalle (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    PedidoId INT NOT NULL,
    ProductoId INT NOT NULL,
    Cantidad INT NOT NULL CONSTRAINT CHK_Detalle_Cantidad CHECK (Cantidad > 0),
    PrecioUnit DECIMAL(10,2) NOT NULL,
    Descuento DECIMAL(10,2) NOT NULL DEFAULT 0.00 CONSTRAINT CHK_Detalle_Descuento CHECK (Descuento >= 0),
    ImpuestoPorc DECIMAL(5,2) NOT NULL,
    TotalLinea DECIMAL(10,2) NOT NULL,
    CONSTRAINT FK_PedidoDetalle_Pedido FOREIGN KEY (PedidoId) REFERENCES Pedido(Id) ON DELETE CASCADE,
    CONSTRAINT FK_PedidoDetalle_Producto FOREIGN KEY (ProductoId) REFERENCES Producto(Id)
);
GO


-- DATOS INICIALES

-- 1. INSERTAR CATEGORIAS
INSERT INTO Categoria (Nombre) VALUES
('Consolas'),
('Videojuegos'),
('Accesorios'),
('Funko Pop'),
('Audifonos'),
('Volantes'),
('Tarjetas Memoria'),
('Ropa Gamer');

-- 2. INSERTAR PRODUCTOS
INSERT INTO Producto (Nombre, CategoriaId, Precio, ImpuestoPorc, Stock, ImagenUrl, Activo) VALUES

-- CONSOLAS
('PlayStation 5 Estandar', 1, 499.99, 13.00, 15, NULL, 1),
('PlayStation 5 Digital', 1, 399.99, 13.00, 10, NULL, 1),
('Xbox Series X', 1, 499.99, 13.00, 12, NULL, 1),
('Xbox Series S', 1, 299.99, 13.00, 20, NULL, 1),
('Nintendo Switch OLED', 1, 349.99, 13.00, 18, NULL, 1),
('Nintendo Switch Lite', 1, 199.99, 13.00, 25, NULL, 1),

-- VIDEOJUEGOS
('Elden Ring', 2, 69.99, 13.00, 8, NULL, 1),
('Final Fantasy XVI', 2, 69.99, 13.00, 10, NULL, 1),
('God of War Ragnarok', 2, 69.99, 13.00, 7, NULL, 1),
('Spider-Man 2', 2, 69.99, 13.00, 12, NULL, 1),
('Starfield', 2, 69.99, 13.00, 9, NULL, 1),
('Halo Infinite', 2, 59.99, 13.00, 14, NULL, 1),
('Forza Motorsport', 2, 69.99, 13.00, 11, NULL, 1),
('Zelda Tears of the Kingdom', 2, 69.99, 13.00, 6, NULL, 1),
('Mario Kart 8 Deluxe', 2, 59.99, 13.00, 20, NULL, 1),
('Animal Crossing', 2, 59.99, 13.00, 15, NULL, 1),

-- ACCESORIOS
('Control DualSense PS5', 3, 74.99, 13.00, 25, NULL, 1),
('Control Xbox Series X', 3, 59.99, 13.00, 30, NULL, 1),
('Joy-Con Nintendo Switch Rojo', 3, 79.99, 13.00, 18, NULL, 1),
('Cable HDMI 2.1', 3, 19.99, 13.00, 50, NULL, 1),
('Adaptador corriente USB-C', 3, 24.99, 13.00, 40, NULL, 1),
('Soporte para Celular Gaming', 3, 12.99, 13.00, 60, NULL, 1),

-- FUNKO POP
('Funko Pop Mario', 4, 14.99, 13.00, 35, NULL, 1),
('Funko Pop Link Zelda', 4, 14.99, 13.00, 28, NULL, 1),
('Funko Pop Sonic', 4, 14.99, 13.00, 32, NULL, 1),
('Funko Pop Pikachu', 4, 16.99, 13.00, 45, NULL, 1),

-- AUDIFONOS
('HyperX Cloud Stinger 2', 5, 99.99, 13.00, 16, NULL, 1),
('SteelSeries Arctis Nova 1', 5, 129.99, 13.00, 14, NULL, 1),
('Corsair HS80 RGB', 5, 149.99, 13.00, 11, NULL, 1),
('Sony WH-CH720N', 5, 79.99, 13.00, 20, NULL, 1),

-- VOLANTES
('Logitech G920', 6, 249.99, 13.00, 8, NULL, 1),
('Thrustmaster T300RS', 6, 299.99, 13.00, 6, NULL, 1),

-- TARJETAS MEMORIA
('Tarjeta microSD SanDisk 256GB', 7, 34.99, 13.00, 40, NULL, 1),
('USB Samsung 3.1 128GB', 7, 29.99, 13.00, 50, NULL, 1),
('SSD Externo Samsung T5 1TB', 7, 119.99, 13.00, 12, NULL, 1),

-- ROPA GAMER
('Playera Gamer Negra Talla M', 8, 24.99, 13.00, 55, NULL, 1),
('Gorra PixelTico Logo', 8, 19.99, 13.00, 40, NULL, 1),
('Sudadera Gamer Hoodie', 8, 54.99, 13.00, 25, NULL, 1),
('Mochila Gamer Backpack', 8, 44.99, 13.00, 30, NULL, 1);


-- 3. INSERTAR CLIENTES
INSERT INTO Cliente (Nombre, Cedula, Correo, Telefono, Direccion) VALUES
('Jeffry Elizondo', '305456789', 'jeffry.elizondo@email.com', '5551234567', 'Sabanilla, San Jose'),
('Tony Stark', '206789012', 'tony.stark@email.com', '5552345678', 'Santa Ana, San Jose'),
('Bruce Wayne', '107890123', 'bruce.wayne@email.com', '5553456789', 'Poas, Alajuela'),
('Peter Parker', '408901234', 'peter.parker@email.com', '5554567890', 'Turrialba, Cartago'),
('Frodo Baggins', '509012345', 'frodo.baggins@email.com', '5555678901', 'San Rafael, Heredia'),
('Ron Weasley', '610123456', 'ron.weasley@email.com', '5556789012', 'Tibas, San Jose'),
('Elon Musk', '711234567', 'elon.musk@email.com', '5557890123', 'Grecia, Alajuela'),
('Grogu', '812345678', 'grogu@email.com', '5558901234', 'Paraiso, Cartago'),
('Ash Ketchum', '913456789', 'ash.ketchum@email.com', '5559012345', 'Santo Domingo, Heredia'),
('Lara Croft', '104567890', 'lara.croft@email.com', '5550123456', 'Escazu, San Jose');