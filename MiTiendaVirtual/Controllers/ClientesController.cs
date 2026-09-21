using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiTiendaVirtual.Data;
using MiTiendaVirtual.Models;

namespace MiTiendaVirtual.Controllers;

public class ClientesController : Controller
{
    private readonly AppDbContext _db;

    public ClientesController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index() =>
        View(await _db.Clientes.AsNoTracking().OrderBy(x => x.Apellidos).ToListAsync());

    public IActionResult Create() => View(new Cliente());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Nombres,Apellidos,Dni,Telefono,Correo,Direccion")] Cliente model)
    {
        if (!ModelState.IsValid) return View(model);

        _db.Add(model);
        await _db.SaveChangesAsync();
        TempData["Ok"] = "Cliente creado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var model = await _db.Clientes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return model == null ? NotFound() : View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Nombres,Apellidos,Dni,Telefono,Correo,Direccion")] Cliente model)
    {
        if (id != model.Id) return NotFound();
        if (!ModelState.IsValid) return View(model);
        if (!await _db.Clientes.AnyAsync(x => x.Id == id)) return NotFound();

        _db.Update(model);
        await _db.SaveChangesAsync();
        TempData["Ok"] = "Cliente actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var model = await _db.Clientes.FindAsync(id);
        if (model == null) return NotFound();

        try
        {
            _db.Remove(model);
            await _db.SaveChangesAsync();
            TempData["Ok"] = "Cliente eliminado correctamente.";
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "No se puede eliminar: hay registros que dependen de este.";
        }

        return RedirectToAction(nameof(Index));
    }
}
