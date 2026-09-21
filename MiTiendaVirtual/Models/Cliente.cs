using System.ComponentModel.DataAnnotations;

namespace MiTiendaVirtual.Models;

public class Cliente
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

    [Display(Name = "Teléfono")]
    [StringLength(20)]
    public string? Telefono { get; set; }

    [EmailAddress(ErrorMessage = "Correo no válido.")]
    [StringLength(150)]
    public string? Correo { get; set; }

    [Display(Name = "Dirección")]
    [StringLength(250)]
    public string? Direccion { get; set; }

    public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
