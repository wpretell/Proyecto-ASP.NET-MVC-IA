using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MiTiendaVirtual.ViewModels;

public class PedidoViewModel
{
    public int Id { get; set; }

    [Display(Name = "Cliente")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un cliente.")]
    public int IdCliente { get; set; }

    [Display(Name = "Tarjeta")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione una tarjeta.")]
    public int IdTarjeta { get; set; }

    [Display(Name = "Fecha y hora")]
    [Required(ErrorMessage = "Ingrese la fecha y hora.")]
    public DateTime FechaHora { get; set; } = DateTime.Now;

    [Required(ErrorMessage = "Seleccione el estado.")]
    [StringLength(20)]
    public string Estado { get; set; } = "Pendiente";

    public List<PedidoDetalleViewModel> Detalles { get; set; } = new();

    // Solo para pintar el formulario; nunca se recibe del navegador.
    [BindNever, ValidateNever]
    public List<ProductoOpcion> Productos { get; set; } = new();
}

public class PedidoDetalleViewModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un producto.")]
    public int IdProducto { get; set; }

    [Range(1, 100000, ErrorMessage = "La cantidad debe ser mayor a cero.")]
    public int Cantidad { get; set; } = 1;

    [Range(0, 999999.99, ErrorMessage = "Precio no válido.")]
    public decimal PrecioUnitario { get; set; }

    public decimal SubTotal { get; set; }
}

public class ProductoOpcion
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
}

public record ModuloInicio(string Controlador, string Titulo, string Descripcion, string Icono, int Total);
