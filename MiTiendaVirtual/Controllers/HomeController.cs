using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiTiendaVirtual.Data;
using MiTiendaVirtual.ViewModels;

namespace MiTiendaVirtual.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _db;

    public HomeController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var modulos = new List<ModuloInicio>
        {
            new("Pedidos", "Pedidos", "Órdenes de los clientes con su detalle de productos.", "bi-receipt", await _db.Pedidos.CountAsync()),
            new("Productos", "Productos", "Catálogo con categoría, marca y precio.", "bi-box-seam", await _db.Productos.CountAsync()),
            new("Clientes", "Clientes", "Personas que realizan pedidos.", "bi-people", await _db.Clientes.CountAsync()),
            new("Tarjetas", "Tarjetas", "Medios de pago de los pedidos.", "bi-credit-card", await _db.Tarjetas.CountAsync()),
            new("Marcas", "Marcas", "Marcas de los productos.", "bi-tags", await _db.Marcas.CountAsync()),
            new("Categorias", "Categorías", "Clasificación del catálogo.", "bi-diagram-3", await _db.Categorias.CountAsync()),
            new("Usuarios", "Usuarios", "Quienes administran el sistema.", "bi-person-badge", await _db.Usuarios.CountAsync())
        };
        return View(modulos);
    }

    [AllowAnonymous]
    public IActionResult Error() => View();
}
