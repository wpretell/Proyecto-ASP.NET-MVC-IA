using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiTiendaVirtual.Data;
using MiTiendaVirtual.Models;

namespace MiTiendaVirtual.Controllers;

public class TarjetasController : Controller
{
    private readonly AppDbContext _db;

    public TarjetasController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index() =>
        View(await _db.Tarjetas.AsNoTracking().OrderBy(x => x.Marca).ToListAsync());

    public IActionResult Create() => View(new Tarjeta());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Marca,Numero")] Tarjeta model)
    {
        if (!ModelState.IsValid) return View(model);

        _db.Add(model);
        await _db.SaveChangesAsync();
        TempData["Ok"] = "Tarjeta creada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var model = await _db.Tarjetas.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return model == null ? NotFound() : View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Marca,Numero")] Tarjeta model)
    {
        if (id != model.Id) return NotFound();
        if (!ModelState.IsValid) return View(model);
        if (!await _db.Tarjetas.AnyAsync(x => x.Id == id)) return NotFound();

        _db.Update(model);
        await _db.SaveChangesAsync();
        TempData["Ok"] = "Tarjeta actualizada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var model = await _db.Tarjetas.FindAsync(id);
        if (model == null) return NotFound();

        try
        {
            _db.Remove(model);
            await _db.SaveChangesAsync();
            TempData["Ok"] = "Tarjeta eliminada correctamente.";
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "No se puede eliminar: hay registros que dependen de este.";
        }

        return RedirectToAction(nameof(Index));
    }
}
