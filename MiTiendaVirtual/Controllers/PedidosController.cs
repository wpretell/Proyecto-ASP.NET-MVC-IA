using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MiTiendaVirtual.Data;
using MiTiendaVirtual.Helpers;
using MiTiendaVirtual.Models;
using MiTiendaVirtual.ViewModels;

namespace MiTiendaVirtual.Controllers;

// Maestro (Pedido) - Detalle (PedidoDetalle)
public class PedidosController : Controller
{
    private static readonly string[] Estados = { "Pendiente", "Pagado", "Enviado", "Entregado", "Cancelado" };

    private readonly AppDbContext _db;

    public PedidosController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index() =>
        View(await _db.Pedidos.AsNoTracking()
            .Include(p => p.Cliente).Include(p => p.Tarjeta)
            .OrderByDescending(p => p.FechaHora).ToListAsync());

    public async Task<IActionResult> Details(int id)
    {
        var pedido = await _db.Pedidos.AsNoTracking()
            .Include(p => p.Cliente).Include(p => p.Tarjeta)
            .Include(p => p.Detalles).ThenInclude(d => d.Producto)
            .FirstOrDefaultAsync(p => p.Id == id);

        return pedido == null ? NotFound() : View(pedido);
    }

    public async Task<IActionResult> Create()
    {
        var ahora = DateTime.Now;
        var vm = new PedidoViewModel
        {
            FechaHora = new DateTime(ahora.Year, ahora.Month, ahora.Day, ahora.Hour, ahora.Minute, 0)
        };
        await CargarListasAsync(vm);
        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PedidoViewModel vm)
    {
        await ValidarDetalleAsync(vm);
        if (!ModelState.IsValid)
        {
            await CargarListasAsync(vm);
            return View(vm);
        }

        var pedido = new Pedido();
        await AplicarAsync(pedido, vm);
        _db.Pedidos.Add(pedido);
        await _db.SaveChangesAsync();

        TempData["Ok"] = $"Pedido #{pedido.Id} creado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var pedido = await _db.Pedidos.AsNoTracking()
            .Include(p => p.Detalles)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (pedido == null) return NotFound();

        var vm = new PedidoViewModel
        {
            Id = pedido.Id,
            IdCliente = pedido.IdCliente,
            IdTarjeta = pedido.IdTarjeta,
            FechaHora = pedido.FechaHora,
            Estado = pedido.Estado,
            Detalles = pedido.Detalles.Select(d => new PedidoDetalleViewModel
            {
                IdProducto = d.IdProducto,
                Cantidad = d.Cantidad,
                PrecioUnitario = d.PrecioUnitario,
                SubTotal = d.SubTotal
            }).ToList()
        };
        await CargarListasAsync(vm);
        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PedidoViewModel vm)
    {
        if (id != vm.Id) return NotFound();

        await ValidarDetalleAsync(vm);
        if (!ModelState.IsValid)
        {
            await CargarListasAsync(vm);
            return View(vm);
        }

        var pedido = await _db.Pedidos.Include(p => p.Detalles).FirstOrDefaultAsync(p => p.Id == id);
        if (pedido == null) return NotFound();

        await AplicarAsync(pedido, vm);
        await _db.SaveChangesAsync();

        TempData["Ok"] = $"Pedido #{pedido.Id} actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var pedido = await _db.Pedidos.Include(p => p.Detalles).FirstOrDefaultAsync(p => p.Id == id);
        if (pedido == null) return NotFound();

        try
        {
            _db.Remove(pedido); // elimina también su detalle
            await _db.SaveChangesAsync();
            TempData["Ok"] = $"Pedido #{id} eliminado correctamente.";
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "No se pudo eliminar el pedido.";
        }

        return RedirectToAction(nameof(Index));
    }

    // ---------------- Auxiliares ----------------

    private async Task ValidarDetalleAsync(PedidoViewModel vm)
    {
        if (ModelState.Any(kv => kv.Key.StartsWith("Detalles[", StringComparison.Ordinal)
                                 && kv.Value != null && kv.Value.Errors.Count > 0))
        {
            ModelState.AddModelError(string.Empty,
                "Revise el detalle: cada fila necesita un producto, una cantidad mayor a cero y un precio válido.");
        }

        if (vm.Detalles.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "Agregue al menos un producto al pedido.");
            return;
        }

        var ids = vm.Detalles.Select(d => d.IdProducto).Distinct().ToList();
        var existentes = await _db.Productos.CountAsync(p => ids.Contains(p.Id));
        if (existentes != ids.Count)
            ModelState.AddModelError(string.Empty, "El detalle contiene productos que no existen.");
    }

    // Copia el ViewModel al pedido y recalcula subtotales y total en el servidor.
    private async Task AplicarAsync(Pedido pedido, PedidoViewModel vm)
    {
        pedido.IdCliente = vm.IdCliente;
        pedido.IdTarjeta = vm.IdTarjeta;
        pedido.FechaHora = vm.FechaHora;
        pedido.Estado = vm.Estado;

        var ids = vm.Detalles.Select(d => d.IdProducto).Distinct().ToList();
        var precios = await _db.Productos
            .Where(p => ids.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, p => p.Precio);

        // Reemplaza el detalle completo.
        _db.PedidoDetalles.RemoveRange(pedido.Detalles.ToList());

        var nuevos = vm.Detalles.Select(d =>
        {
            var precio = d.PrecioUnitario > 0 ? d.PrecioUnitario : precios[d.IdProducto];
            return new PedidoDetalle
            {
                IdProducto = d.IdProducto,
                Cantidad = d.Cantidad,
                PrecioUnitario = precio,
                SubTotal = Math.Round(d.Cantidad * precio, 2)
            };
        }).ToList();

        foreach (var detalle in nuevos) pedido.Detalles.Add(detalle);
        pedido.Total = nuevos.Sum(d => d.SubTotal);
    }

    private async Task CargarListasAsync(PedidoViewModel vm)
    {
        var clientes = await _db.Clientes.AsNoTracking()
            .OrderBy(c => c.Apellidos).ThenBy(c => c.Nombres).ToListAsync();
        ViewBag.Clientes = clientes
            .Select(c => new SelectListItem($"{c.Nombres} {c.Apellidos} ({c.Dni})", c.Id.ToString()))
            .ToList();

        var tarjetas = await _db.Tarjetas.AsNoTracking().OrderBy(t => t.Marca).ToListAsync();
        ViewBag.Tarjetas = tarjetas
            .Select(t => new SelectListItem($"{t.Marca} {Formato.Ocultar(t.Numero)}", t.Id.ToString()))
            .ToList();

        ViewBag.Estados = Estados.Select(e => new SelectListItem(e, e)).ToList();

        // Productos activos + los que ya usa este pedido (aunque estén desactivados).
        var usados = vm.Detalles.Select(d => d.IdProducto).ToList();
        vm.Productos = await _db.Productos.AsNoTracking()
            .Where(p => p.Activo || usados.Contains(p.Id))
            .OrderBy(p => p.Nombre)
            .Select(p => new ProductoOpcion { Id = p.Id, Nombre = p.Nombre, Precio = p.Precio })
            .ToListAsync();
    }
}
