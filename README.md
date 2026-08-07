# PixelTico - Sistema de Gestión de Tienda

Guia de despliegue

### Clonar repo
```bash
git clone https://github.com/TheJeff182/Progra-Avanzada---ProyectoFinal.git
cd ProyectoFinal
```

### Restaurar BD
Abrir Package Manager Console:
```powershell
Update-Database
```

### Correr la app
Navegar desde un browser a https://localhost:7117/

## Usuarios de prueba

**Admin**
- Email: `admin@pixeltico.local`
- Password: `Admin123!`

Crear nuevos usuarios con rol Ventas en el registro. Se asigna automáticamente.

## Roles

- **Admin**: Acceso a todo (Productos, Clientes, Ventas, Reportes, Gestión de Usuarios)
- **Ventas**: Solo Productos, Clientes, Ventas e Historial