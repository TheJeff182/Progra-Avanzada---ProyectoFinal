using ProyectoFinal.Models;
using Microsoft.EntityFrameworkCore;

namespace ProyectoFinal.Data;

public class PixelTicoContext : DbContext
{
    public PixelTicoContext(DbContextOptions<PixelTicoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Categoria> Categorias { get; set; }
    public virtual DbSet<Producto> Productos { get; set; }
    public virtual DbSet<Cliente> Clientes { get; set; }
    public virtual DbSet<Usuario> Usuarios { get; set; }
    public virtual DbSet<Pedido> Pedidos { get; set; }
    public virtual DbSet<PedidoDetalle> PedidoDetalles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Categoria
        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.ToTable("Categoria");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre)
                .IsRequired()
                .HasMaxLength(100);
            entity.HasIndex(e => e.Nombre).IsUnique();
        });

        // Producto
        modelBuilder.Entity<Producto>(entity =>
        {
            entity.ToTable("Producto");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre)
                .IsRequired()
                .HasMaxLength(150);
            entity.Property(e => e.Precio)
                .HasColumnType("decimal(10,2)");
            entity.Property(e => e.ImpuestoPorc)
                .HasColumnType("decimal(5,2)");
            entity.Property(e => e.ImagenUrl)
                .HasMaxLength(500);
            entity.Property(e => e.Activo)
                .HasDefaultValue(true);

            entity.HasOne(e => e.Categoria)
                .WithMany(c => c.Productos)
                .HasForeignKey(e => e.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasCheckConstraint("CHK_Producto_Precio", "Precio >= 0");
            entity.HasCheckConstraint("CHK_Producto_Stock", "Stock >= 0");
        });

        // Cliente
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.ToTable("Cliente");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.Cedula)
                .IsRequired()
                .HasMaxLength(20);
            entity.Property(e => e.Correo)
                .HasMaxLength(100);
            entity.Property(e => e.Telefono)
                .HasMaxLength(20);
            entity.Property(e => e.Direccion)
                .HasMaxLength(500);

            entity.HasIndex(e => e.Cedula).IsUnique();
        });

        // Usuario
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuario");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.Rol)
                .IsRequired()
                .HasMaxLength(30);

            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Pedido
        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.ToTable("Pedido");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.Subtotal)
                .HasColumnType("decimal(10,2)");
            entity.Property(e => e.Impuestos)
                .HasColumnType("decimal(10,2)");
            entity.Property(e => e.Total)
                .HasColumnType("decimal(10,2)");
            entity.Property(e => e.Estado)
                .HasMaxLength(30);

            entity.HasOne(e => e.Cliente)
                .WithMany(c => c.Pedidos)
                .HasForeignKey(e => e.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Usuario)
                .WithMany(u => u.Pedidos)
                .HasForeignKey(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // PedidoDetalle
        modelBuilder.Entity<PedidoDetalle>(entity =>
        {
            entity.ToTable("PedidoDetalle");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PrecioUnit)
                .HasColumnType("decimal(10,2)");
            entity.Property(e => e.Descuento)
                .HasColumnType("decimal(10,2)");
            entity.Property(e => e.ImpuestoPorc)
                .HasColumnType("decimal(5,2)");
            entity.Property(e => e.TotalLinea)
                .HasColumnType("decimal(10,2)");

            entity.HasOne(e => e.Pedido)
                .WithMany(p => p.PedidoDetalles)
                .HasForeignKey(e => e.PedidoId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Producto)
                .WithMany(p => p.PedidoDetalles)
                .HasForeignKey(e => e.ProductoId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasCheckConstraint("CHK_Detalle_Cantidad", "Cantidad > 0");
            entity.HasCheckConstraint("CHK_Detalle_Descuento", "Descuento >= 0");
        });
    }
}
