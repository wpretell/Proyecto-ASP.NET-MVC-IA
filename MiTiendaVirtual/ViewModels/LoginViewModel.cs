using System.ComponentModel.DataAnnotations;

namespace MiTiendaVirtual.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Ingrese su correo.")]
    [EmailAddress(ErrorMessage = "Correo no válido.")]
    public string Correo { get; set; } = string.Empty;

    [Display(Name = "Contraseña")]
    [Required(ErrorMessage = "Ingrese su contraseña.")]
    public string Contrasena { get; set; } = string.Empty;

    [Display(Name = "Mantener la sesión iniciada")]
    public bool Recordarme { get; set; }

    public string? ReturnUrl { get; set; }
}
