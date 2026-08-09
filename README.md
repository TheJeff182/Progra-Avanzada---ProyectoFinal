# PixelTico - Sistema de Gestión de Tienda

Guia de despliegue

### Clonar repo
```bash
git clone https://github.com/TheJeff182/Progra-Avanzada---ProyectoFinal.git
cd ProyectoFinal
```


### Restaurar BD (crea las tablas, vacías)
Abrir Package Manager Console:
```powershell
Update-Database
```
Si el comando no se reconoce, falta el paquete NuGet `Microsoft.EntityFrameworkCore.Tools`
en el proyecto — instálalo (misma versión que los demás paquetes de EF Core) y reinicia la consola.

### Cargar datos de prueba (paso obligatorio)
`Update-Database` solo crea las tablas — **sin esto la tienda queda vacía**. Con SQL Server
Management Studio (o el SQL Server Object Explorer de Visual Studio), conéctate a la base
`PixelTico` y ejecuta el script `Database/SeedData.sql`. Carga 8 categorías, 39 productos y
10 clientes de prueba.

### Correr la app
Navegar desde un browser a https://localhost:7117/

## Usuarios de prueba

**Admin**
- Email: `admin@pixeltico.local`
- Password: `Admin123!`

Se crea automáticamente la primera vez que arranca la aplicación (ver seeding en `Program.cs`).

Crear nuevos usuarios con rol Ventas en el registro (`/Account/Register`). Se asigna
automáticamente — no hay forma de auto-registrarse como Admin u Operaciones por seguridad.

## Roles

- **Admin**: Acceso a todo (Productos, Clientes, Ventas, Pedidos, Reportes, Gestión de Usuarios). Único rol que puede eliminar productos.
- **Ventas**: Productos (sin eliminar), Clientes, Ventas (crear pedidos) e Historial.
- **Operaciones**: Productos (gestión de inventario/stock), solo lectura del historial de Pedidos. No puede facturar ventas.