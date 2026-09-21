using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MiTiendaVirtual.Data;
using MiTiendaVirtual.Models;

namespace MiTiendaVirtual.Controllers;

public class ProductosController : Controller
{
    private const string Campos = "IdCategoria,IdMarca,Nombre,Descripcion,Precio,Url,Destacado,Activo";

    private readonly AppDbContext _db;

    public ProductosController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index() =>
        View(await _db.Productos.AsNoTracking()
            .Include(p => p.Categoria).Include(p => p.Marca)
            .OrderBy(p => p.Nombre).ToListAsync());

    public async Task<IActionResult> Create()
    {
        await CargarListasAsync();
        return View(new Producto());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind(Campos)] Producto model)
    {
        if (!ModelState.IsValid)
        {
            await CargarListasAsync();
            return View(model);
        }

        _db.Add(model);
        await _db.SaveChangesAsync();
        TempData["Ok"] = "Producto creado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var model = await _db.Productos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        if (model == null) return NotFound();

        await CargarListasAsync();
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id," + Campos)] Producto model)
    {
        if (id != model.Id) return NotFound();
        if (!ModelState.IsValid)
        {
            await CargarListasAsync();
            return View(model);
        }
        if (!await _db.Productos.AnyAsync(p => p.Id == id)) return NotFound();

        _db.Update(model);
        await _db.SaveChangesAsync();
        TempData["Ok"] = "Producto actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var model = await _db.Productos.FindAsync(id);
        if (model == null) return NotFound();

        try
        {
            _db.Remove(model);
            await _db.SaveChangesAsync();
            TempData["Ok"] = "Producto eliminado correctamente.";
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "No se puede eliminar: el producto figura en pedidos existentes. Puede desactivarlo en su lugar.";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task CargarListasAsync()
    {
        ViewBag.Categorias = new SelectList(
            await _db.Categorias.AsNoTracking().OrderBy(c => c.Nombre).ToListAsync(), "Id", "Nombre");
        ViewBag.Marcas = new SelectList(
            await _db.Marcas.AsNoTracking().OrderBy(m => m.Nombre).ToListAsync(), "Id", "Nombre");
    }
}
