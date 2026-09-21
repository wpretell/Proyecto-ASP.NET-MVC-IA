using System.ComponentModel.DataAnnotations;

namespace MiTiendaVirtual.Models;

public class Producto
{
    public int Id { get; set; }

    [Display(Name = "Categoría")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione una categoría.")]
    public int IdCategoria { get; set; }

    [Display(Name = "Marca")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione una marca.")]
    public int IdMarca { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(150)]
    public string Nombre { get; set; } = string.Empty;

    [Display(Name = "Descripción")]
    [StringLength(500)]
    public string? Descripcion { get; set; }

    [Range(0.01, 999999.99, ErrorMessage = "Ingrese un precio válido.")]
    public decimal Precio { get; set; }

    [Display(Name = "Imagen (URL)")]
    [StringLength(500)]
    public string? Url { get; set; }

    public bool Destacado { get; set; }

    public bool Activo { get; set; } = true;

    public Categoria? Categoria { get; set; }
    public Marca? Marca { get; set; }
    public ICollection<PedidoDetalle> PedidoDetalles { get; set; } = new List<PedidoDetalle>();
}
