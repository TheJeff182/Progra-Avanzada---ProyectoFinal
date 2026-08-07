## Control de Acceso por Roles (RBAC) - Guía de Implementación

### ✅ YA CONFIGURADO:
Los siguientes controllers están protegidos:
- **ProductosController** → `[Authorize(Roles = "Admin,Ventas")]`
- **ClientesController** → `[Authorize(Roles = "Admin,Ventas")]`
- **PedidosController** → `[Authorize(Roles = "Admin,Ventas")]`
- **VentasController** → `[Authorize(Roles = "Admin,Ventas")]`

### 📋 ESTRUCTURA DE ROLES:

**ADMIN**
- ✅ Acceso a: Productos, Clientes, Ventas, Pedidos, Dashboard, Reportes, Usuarios
- ✅ Puede crear, editar, eliminar todo
- ✅ Ve menú completo

**VENTAS**
- ✅ Acceso a: Productos, Clientes, Ventas, Pedidos (solo lectura)
- ✅ Puede crear/editar clientes y productos
- ❌ NO ve: Reportes, Dashboard Admin, Gestión de Usuarios
- ✅ Menú filtrado

### 🔐 PROTEGER UN NUEVO CONTROLLER:

Para proteger un controller completo:

```csharp
using Microsoft.AspNetCore.Authorization;

[Authorize(Roles = "Admin")] // Solo Admin
public class ReportesController : Controller
{
	// Acciones aquí
}
```

Para proteger solo una acción:

```csharp
[Authorize(Roles = "Admin")]
public IActionResult ReporteVentas()
{
	return View();
}
```

### 📝 PROTEGER ACTIONS ESPECÍFICAS:

#### Solo Admin
```csharp
[Authorize(Roles = "Admin")]
public IActionResult EliminarUsuario(int id)
{
	// Solo admin puede eliminar
}
```

#### Admin Y Ventas
```csharp
[Authorize(Roles = "Admin,Ventas")]
public IActionResult CrearPedido()
{
	// Admin y Ventas pueden crear
}
```

#### Público (sin autenticación)
```csharp
public IActionResult Index()
{
	// Sin protección
}
```

### 🎨 MOSTRAR/OCULTAR EN VISTAS (Razor):

En cualquier `.cshtml`:

```html
@if (User.IsInRole("Admin"))
{
	<a href="/Reportes">Reportes (Solo Admin)</a>
}

@if (User.IsInRole("Admin") || User.IsInRole("Ventas"))
{
	<a href="/Productos">Productos</a>
}

@if (User.Identity.IsAuthenticated)
{
	<p>Bienvenido, @User.Identity.Name</p>
}
```

### 🚀 CREAR USUARIO CON DIFERENTES ROLES:

En **AccountController.Register**:

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Register(RegisterViewModel model, string? role = null)
{
	if (ModelState.IsValid)
	{
		var user = new IdentityUser { UserName = model.Email, Email = model.Email };
		var result = await _userManager.CreateAsync(user, model.Password);

		if (result.Succeeded)
		{
			// Asignar rol (default "Ventas" si no se especifica)
			var roleToAssign = role ?? "Ventas";
			if (await _roleManager.RoleExistsAsync(roleToAssign))
			{
				await _userManager.AddToRoleAsync(user, roleToAssign);
			}

			await _signInManager.SignInAsync(user, isPersistent: false);
			return RedirectToAction("Index", "Home");
		}
	}

	return View(model);
}
```

### 🔍 VERIFICAR ROLES EN CONTROLADOR:

```csharp
public IActionResult SomeAction()
{
	if (User.IsInRole("Admin"))
	{
		// Mostrar datos de admin
	}
	else if (User.IsInRole("Ventas"))
	{
		// Mostrar datos de ventas
	}

	return View();
}
```

### ⚠️ NOTAS IMPORTANTES:

1. **[Authorize]** sin parámetros requiere autenticación (cualquier rol)
2. **[Authorize(Roles = "Admin")]** requiere rol específico
3. Si el usuario no tiene permiso → Redirección a `/Account/AccessDenied`
4. Siempre validar en el BACK-END (no confiar solo en el frontend)
5. El navegador oculta opciones pero no impide acceso directo a URLs

### 📌 PRÓXIMOS PASOS:

1. **Proteger ReportesController** → `[Authorize(Roles = "Admin")]`
2. **Proteger PrototiposController** → `[Authorize(Roles = "Admin")]` (solo Dashboard, Reportes, Usuarios)
3. **Crear vista de Admin Dashboard** con widgets de reportes
4. **Agregar filtros en búsquedas** según rol (p.ej., Ventas solo ve sus propias ventas)
