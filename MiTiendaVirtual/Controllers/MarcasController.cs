using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiTiendaVirtual.Data;
using MiTiendaVirtual.Models;

namespace MiTiendaVirtual.Controllers;

public class MarcasController : Controller
{
    private readonly AppDbContext _db;

    public MarcasController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index() =>
        View(await _db.Marcas.AsNoTracking().OrderBy(x => x.Nombre).ToListAsync());

    public IActionResult Create() => View(new Marca());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Nombre")] Marca model)
    {
        if (!ModelState.IsValid) return View(model);

        _db.Add(model);
        await _db.SaveChangesAsync();
        TempData["Ok"] = "Marca creada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var model = await _db.Marcas.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return model == null ? NotFound() : View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre")] Marca model)
    {
        if (id != model.Id) return NotFound();
        if (!ModelState.IsValid) return View(model);
        if (!await _db.Marcas.AnyAsync(x => x.Id == id)) return NotFound();

        _db.Update(model);
        await _db.SaveChangesAsync();
        TempData["Ok"] = "Marca actualizada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var model = await _db.Marcas.FindAsync(id);
        if (model == null) return NotFound();

        try
        {
            _db.Remove(model);
            await _db.SaveChangesAsync();
            TempData["Ok"] = "Marca eliminada correctamente.";
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "No se puede eliminar: hay registros que dependen de este.";
        }

        return RedirectToAction(nameof(Index));
    }
}
