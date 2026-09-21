using System.ComponentModel.DataAnnotations;

namespace MiTiendaVirtual.Models;

public class Usuario
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Los nombres son obligatorios.")]
    [StringLength(100)]
    public string Nombres { get; set; } = string.Empty;

    [Required(ErrorMessage = "Los apellidos son obligatorios.")]
    [StringLength(100)]
    public string Apellidos { get; set; } = string.Empty;

    [Required(ErrorMessage = "El DNI es obligatorio.")]
    [StringLength(15)]
    public string Dni { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "Correo no válido.")]
    [StringLength(150)]
    public string Correo { get; set; } = string.Empty;

    // Se guarda el hash PBKDF2 (nunca la contraseña en texto plano).
    [Display(Name = "Contraseña")]
    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [StringLength(200, MinimumLength = 6, ErrorMessage = "Mínimo 6 caracteres.")]
    public string Contrasena { get; set; } = string.Empty;

    public bool Activo { get; set; } = true;
}
