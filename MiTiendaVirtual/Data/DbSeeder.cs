using MiTiendaVirtual.Helpers;
using MiTiendaVirtual.Models;

namespace MiTiendaVirtual.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext db)
    {
        // Garantiza que exista al menos un usuario para poder iniciar sesión.
        if (!db.Usuarios.Any())
        {
            db.Usuarios.Add(new Usuario
            {
                Nombres = "Administrador",
                Apellidos = "Sistema",
                Dni = "00000000",
                Correo = "admin@example.com",
                Contrasena = PasswordHelper.Hash("Admin123*"),
                Activo = true
            });
            db.SaveChanges();
        }

        // El resto de datos de ejemplo se carga solo si la base está vacía.
        if (db.Marcas.Any() || db.Productos.Any() || db.Clientes.Any()) return;

        var marcas = new[]
        {
            new Marca { Nombre = "Samsung" },
            new Marca { Nombre = "Apple" },
            new Marca { Nombre = "Logitech" }
        };
        var categorias = new[]
        {
            new Categoria { Nombre = "Celulares", Activo = true },
            new Categoria { Nombre = "Laptops", Activo = true },
            new Categoria { Nombre = "Accesorios", Activo = true }
        };
        db.Marcas.AddRange(marcas);
        db.Categorias.AddRange(categorias);
        db.SaveChanges();

        var productos = new[]
        {
            new Producto { IdCategoria = categorias[0].Id, IdMarca = marcas[0].Id, Nombre = "Galaxy S24", Descripcion = "Smartphone 256 GB", Precio = 3299.90m, Destacado = true, Activo = true },
            new Producto { IdCategoria = categorias[0].Id, IdMarca = marcas[1].Id, Nombre = "iPhone 15", Descripcion = "Smartphone 128 GB", Precio = 3899.00m, Destacado = true, Activo = true },
            new Producto { IdCategoria = categorias[1].Id, IdMarca = marcas[1].Id, Nombre = "MacBook Air 13", Descripcion = "Chip M2, 8 GB RAM", Precio = 5299.00m, Activo = true },
            new Producto { IdCategoria = categorias[2].Id, IdMarca = marcas[2].Id, Nombre = "Mouse MX Master 3S", Descripcion = "Mouse inalámbrico", Precio = 449.90m, Activo = true },
            new Producto { IdCategoria = categorias[2].Id, IdMarca = marcas[2].Id, Nombre = "Teclado MX Keys", Descripcion = "Teclado inalámbrico", Precio = 499.90m, Activo = true }
        };
        db.Productos.AddRange(productos);

        var clientes = new[]
        {
            new Cliente { Nombres = "María", Apellidos = "Quispe Rojas", Dni = "45871236", Telefono = "987654321", Correo = "maria.quispe@example.com", Direccion = "Av. Arequipa 1234" },
            new Cliente { Nombres = "Luis", Apellidos = "Fernández Soto", Dni = "40123987", Telefono = "955112233", Correo = "luis.fernandez@example.com", Direccion = "Jr. Cusco 567" }
        };
        db.Clientes.AddRange(clientes);

        // Números de prueba públicos de Visa y Mastercard.
        var tarjetas = new[]
        {
            new Tarjeta { Marca = "Visa", Numero = "4111111111111111" },
            new Tarjeta { Marca = "Mastercard", Numero = "5555555555554444" }
        };
        db.Tarjetas.AddRange(tarjetas);

        db.SaveChanges();

        var pedido = new Pedido
        {
            IdCliente = clientes[0].Id,
            IdTarjeta = tarjetas[0].Id,
            FechaHora = DateTime.Now,
            Estado = "Pagado"
        };
        pedido.Detalles.Add(new PedidoDetalle { IdProducto = productos[0].Id, Cantidad = 1, PrecioUnitario = productos[0].Precio, SubTotal = productos[0].Precio });
        pedido.Detalles.Add(new PedidoDetalle { IdProducto = productos[3].Id, Cantidad = 2, PrecioUnitario = productos[3].Precio, SubTotal = productos[3].Precio * 2 });
        pedido.Total = pedido.Detalles.Sum(d => d.SubTotal);

        db.Pedidos.Add(pedido);
        db.SaveChanges();
    }
}
