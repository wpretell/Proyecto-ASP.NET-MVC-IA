namespace MiTiendaVirtual.Models;

public class Pedido
{
    public int Id { get; set; }
    public int IdCliente { get; set; }
    public int IdTarjeta { get; set; }
    public DateTime FechaHora { get; set; }
    public string Estado { get; set; } = "Pendiente";
    public decimal Total { get; set; }

    public Cliente? Cliente { get; set; }
    public Tarjeta? Tarjeta { get; set; }
    public ICollection<PedidoDetalle> Detalles { get; set; } = new List<PedidoDetalle>();
}
