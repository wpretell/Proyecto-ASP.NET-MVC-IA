using Microsoft.EntityFrameworkCore;
using MiTiendaVirtual.Models;

namespace MiTiendaVirtual.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Marca> Marcas => Set<Marca>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Tarjeta> Tarjetas => Set<Tarjeta>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<PedidoDetalle> PedidoDetalles => Set<PedidoDetalle>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        // Nombres de tabla en singular, como en el diagrama.
        mb.Entity<Marca>().ToTable("Marca");
        mb.Entity<Categoria>().ToTable("Categoria");
        mb.Entity<Usuario>().ToTable("Usuario");
        mb.Entity<Cliente>().ToTable("Cliente");
        mb.Entity<Tarjeta>().ToTable("Tarjeta");

        mb.Entity<Producto>(e =>
        {
            e.ToTable("Producto");
            e.Property(p => p.Precio).HasPrecision(18, 2);
            e.HasOne(p => p.Categoria).WithMany(c => c.Productos)
                .HasForeignKey(p => p.IdCategoria).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(p => p.Marca).WithMany(m => m.Productos)
                .HasForeignKey(p => p.IdMarca).OnDelete(DeleteBehavior.Restrict);
        });

        mb.Entity<Pedido>(e =>
        {
            e.ToTable("Pedido");
            e.Property(p => p.Estado).HasMaxLength(20);
            e.Property(p => p.Total).HasPrecision(18, 2);
            e.HasOne(p => p.Cliente).WithMany(c => c.Pedidos)
                .HasForeignKey(p => p.IdCliente).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(p => p.Tarjeta).WithMany(t => t.Pedidos)
                .HasForeignKey(p => p.IdTarjeta).OnDelete(DeleteBehavior.Restrict);
        });

        mb.Entity<PedidoDetalle>(e =>
        {
            e.ToTable("PedidoDetalle");
            e.Property(d => d.PrecioUnitario).HasPrecision(18, 2);
            e.Property(d => d.SubTotal).HasPrecision(18, 2);
            e.HasOne(d => d.Pedido).WithMany(p => p.Detalles)
                .HasForeignKey(d => d.IdPedido).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(d => d.Producto).WithMany(p => p.PedidoDetalles)
                .HasForeignKey(d => d.IdProducto).OnDelete(DeleteBehavior.Restrict);
        });
    }
}
