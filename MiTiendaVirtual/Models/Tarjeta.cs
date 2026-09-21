using System.ComponentModel.DataAnnotations;

namespace MiTiendaVirtual.Models;

public class Tarjeta
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Seleccione la marca de la tarjeta.")]
    [StringLength(30)]
    public string Marca { get; set; } = string.Empty;

    [Display(Name = "Número")]
    [Required(ErrorMessage = "El número es obligatorio.")]
    [RegularExpression(@"^\d{13,19}$", ErrorMessage = "Ingrese entre 13 y 19 dígitos, sin espacios.")]
    public string Numero { get; set; } = string.Empty;

    public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
