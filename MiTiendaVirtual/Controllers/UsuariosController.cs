using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiTiendaVirtual.Data;
using MiTiendaVirtual.Helpers;
using MiTiendaVirtual.Models;

namespace MiTiendaVirtual.Controllers;

public class UsuariosController : Controller
{
    private const string Campos = "Nombres,Apellidos,Dni,Correo,Contrasena,Activo";

    private readonly AppDbContext _db;

    public UsuariosController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index() =>
        View(await _db.Usuarios.AsNoTracking().OrderBy(u => u.Apellidos).ToListAsync());

    public IActionResult Create() => View(new Usuario());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind(Campos)] Usuario model)
    {
        if (!ModelState.IsValid) return View(model);

        model.Contrasena = PasswordHelper.Hash(model.Contrasena);
        _db.Add(model);
        await _db.SaveChangesAsync();
        TempData["Ok"] = "Usuario creado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var model = await _db.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        if (model == null) return NotFound();

        model.Contrasena = string.Empty; // el hash nunca sale al navegador
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id," + Campos)] Usuario model)
    {
        if (id != model.Id) return NotFound();

        // Contraseña en blanco = conservar la actual.
        var cambiaClave = !string.IsNullOrWhiteSpace(model.Contrasena);
        if (!cambiaClave) ModelState.Remove(nameof(Usuario.Contrasena));
        if (!ModelState.IsValid) return View(model);

        var actual = await _db.Usuarios.FindAsync(id);
        if (actual == null) return NotFound();

        actual.Nombres = model.Nombres;
        actual.Apellidos = model.Apellidos;
        actual.Dni = model.Dni;
        actual.Correo = model.Correo;
        actual.Activo = model.Activo;
        if (cambiaClave) actual.Contrasena = PasswordHelper.Hash(model.Contrasena);

        await _db.SaveChangesAsync();
        TempData["Ok"] = "Usuario actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var model = await _db.Usuarios.FindAsync(id);
        if (model == null) return NotFound();

        try
        {
            _db.Remove(model);
            await _db.SaveChangesAsync();
            TempData["Ok"] = "Usuario eliminado correctamente.";
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "No se puede eliminar: hay registros que dependen de este usuario.";
        }

        return RedirectToAction(nameof(Index));
    }
}
