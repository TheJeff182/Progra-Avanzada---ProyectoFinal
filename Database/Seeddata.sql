-- =====================================================================
-- Datos de prueba (solo INSERT, sin crear tablas).
-- Ejecutar DESPUES de correr "Update-Database" (que crea el esquema
-- vacio via migraciones de EF). No usar PixelTico.sql completo, porque
-- ese script intenta crear las tablas de nuevo y va a fallar.
-- =====================================================================

USE PixelTico;
GO

-- 1. CATEGORIAS
INSERT INTO Categoria (Nombre) VALUES
('Consolas'),
('Videojuegos'),
('Accesorios'),
('Funko Pop'),
('Audifonos'),
('Volantes'),
('Tarjetas Memoria'),
('Ropa Gamer');

-- 2. PRODUCTOS
INSERT INTO Producto (Nombre, CategoriaId, Precio, ImpuestoPorc, Stock, ImagenUrl, Activo) VALUES

-- CONSOLAS
('PlayStation 5 Estandar', 1, 499.99, 13.00, 15, 'https://placehold.co/300x300?text=PlayStation%205%20Estandar', 1),
('PlayStation 5 Digital', 1, 399.99, 13.00, 10, 'https://placehold.co/300x300?text=PlayStation%205%20Digital', 1),
('Xbox Series X', 1, 499.99, 13.00, 12, 'https://placehold.co/300x300?text=Xbox%20Series%20X', 1),
('Xbox Series S', 1, 299.99, 13.00, 20, 'https://placehold.co/300x300?text=Xbox%20Series%20S', 1),
('Nintendo Switch OLED', 1, 349.99, 13.00, 18, 'https://placehold.co/300x300?text=Nintendo%20Switch%20OLED', 1),
('Nintendo Switch Lite', 1, 199.99, 13.00, 25, 'https://placehold.co/300x300?text=Nintendo%20Switch%20Lite', 1),

-- VIDEOJUEGOS
('Elden Ring', 2, 69.99, 13.00, 8, 'https://placehold.co/300x300?text=Elden%20Ring', 1),
('Final Fantasy XVI', 2, 69.99, 13.00, 10, 'https://placehold.co/300x300?text=Final%20Fantasy%20XVI', 1),
('God of War Ragnarok', 2, 69.99, 13.00, 7, 'https://placehold.co/300x300?text=God%20of%20War%20Ragnarok', 1),
('Spider-Man 2', 2, 69.99, 13.00, 12, 'https://placehold.co/300x300?text=Spider-Man%202', 1),
('Starfield', 2, 69.99, 13.00, 9, 'https://placehold.co/300x300?text=Starfield', 1),
('Halo Infinite', 2, 59.99, 13.00, 14, 'https://placehold.co/300x300?text=Halo%20Infinite', 1),
('Forza Motorsport', 2, 69.99, 13.00, 11, 'https://placehold.co/300x300?text=Forza%20Motorsport', 1),
('Zelda Tears of the Kingdom', 2, 69.99, 13.00, 6, 'https://placehold.co/300x300?text=Zelda%20Tears%20of%20the%20Kingdom', 1),
('Mario Kart 8 Deluxe', 2, 59.99, 13.00, 20, 'https://placehold.co/300x300?text=Mario%20Kart%208%20Deluxe', 1),
('Animal Crossing', 2, 59.99, 13.00, 15, 'https://placehold.co/300x300?text=Animal%20Crossing', 1),

-- ACCESORIOS
('Control DualSense PS5', 3, 74.99, 13.00, 25, 'https://placehold.co/300x300?text=Control%20DualSense%20PS5', 1),
('Control Xbox Series X', 3, 59.99, 13.00, 30, 'https://placehold.co/300x300?text=Control%20Xbox%20Series%20X', 1),
('Joy-Con Nintendo Switch Rojo', 3, 79.99, 13.00, 18, 'https://placehold.co/300x300?text=Joy-Con%20Nintendo%20Switch%20Rojo', 1),
('Cable HDMI 2.1', 3, 19.99, 13.00, 50, 'https://placehold.co/300x300?text=Cable%20HDMI%202.1', 1),
('Adaptador corriente USB-C', 3, 24.99, 13.00, 40, 'https://placehold.co/300x300?text=Adaptador%20corriente%20USB-C', 1),
('Soporte para Celular Gaming', 3, 12.99, 13.00, 60, 'https://placehold.co/300x300?text=Soporte%20para%20Celular%20Gaming', 1),

-- FUNKO POP
('Funko Pop Mario', 4, 14.99, 13.00, 35, 'https://placehold.co/300x300?text=Funko%20Pop%20Mario', 1),
('Funko Pop Link Zelda', 4, 14.99, 13.00, 28, 'https://placehold.co/300x300?text=Funko%20Pop%20Link%20Zelda', 1),
('Funko Pop Sonic', 4, 14.99, 13.00, 32, 'https://placehold.co/300x300?text=Funko%20Pop%20Sonic', 1),
('Funko Pop Pikachu', 4, 16.99, 13.00, 45, 'https://placehold.co/300x300?text=Funko%20Pop%20Pikachu', 1),

-- AUDIFONOS
('HyperX Cloud Stinger 2', 5, 99.99, 13.00, 16, 'https://placehold.co/300x300?text=HyperX%20Cloud%20Stinger%202', 1),
('SteelSeries Arctis Nova 1', 5, 129.99, 13.00, 14, 'https://placehold.co/300x300?text=SteelSeries%20Arctis%20Nova%201', 1),
('Corsair HS80 RGB', 5, 149.99, 13.00, 11, 'https://placehold.co/300x300?text=Corsair%20HS80%20RGB', 1),
('Sony WH-CH720N', 5, 79.99, 13.00, 20, 'https://placehold.co/300x300?text=Sony%20WH-CH720N', 1),

-- VOLANTES
('Logitech G920', 6, 249.99, 13.00, 8, 'https://placehold.co/300x300?text=Logitech%20G920', 1),
('Thrustmaster T300RS', 6, 299.99, 13.00, 6, 'https://placehold.co/300x300?text=Thrustmaster%20T300RS', 1),

-- TARJETAS MEMORIA
('Tarjeta microSD SanDisk 256GB', 7, 34.99, 13.00, 40, 'https://placehold.co/300x300?text=Tarjeta%20microSD%20SanDisk%20256GB', 1),
('USB Samsung 3.1 128GB', 7, 29.99, 13.00, 50, 'https://placehold.co/300x300?text=USB%20Samsung%203.1%20128GB', 1),
('SSD Externo Samsung T5 1TB', 7, 119.99, 13.00, 12, 'https://placehold.co/300x300?text=SSD%20Externo%20Samsung%20T5%201TB', 1),

-- ROPA GAMER
('Playera Gamer Negra Talla M', 8, 24.99, 13.00, 55, 'https://placehold.co/300x300?text=Playera%20Gamer%20Negra%20Talla%20M', 1),
('Gorra PixelTico Logo', 8, 19.99, 13.00, 40, 'https://placehold.co/300x300?text=Gorra%20PixelTico%20Logo', 1),
('Sudadera Gamer Hoodie', 8, 54.99, 13.00, 25, 'https://placehold.co/300x300?text=Sudadera%20Gamer%20Hoodie', 1),
('Mochila Gamer Backpack', 8, 44.99, 13.00, 30, 'https://placehold.co/300x300?text=Mochila%20Gamer%20Backpack', 1);

-- 3. CLIENTES
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

-- 4. USUARIO "del sistema" (tabla legacy Usuario, distinta de AspNetUsers de Identity;
--    todavia se usa como referencia en Pedido.UsuarioId -- ver nota en la revision de abajo)
INSERT INTO Usuario (Nombre, Email, PasswordHash, Rol) VALUES
('Administrador', 'admin@pixeltico.com', NULL, 'Admin');

PRINT 'Datos de prueba cargados correctamente.';