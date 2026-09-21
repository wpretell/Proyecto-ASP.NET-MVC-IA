namespace MiTiendaVirtual.Helpers;

public static class Formato
{
    /// <summary>Muestra solo los últimos 4 dígitos de una tarjeta.</summary>
    public static string Ocultar(string? numero)
    {
        if (string.IsNullOrWhiteSpace(numero)) return string.Empty;
        var digitos = new string(numero.Where(char.IsDigit).ToArray());
        return digitos.Length <= 4 ? digitos : "•••• " + digitos[^4..];
    }

    /// <summary>Clase de Bootstrap para la etiqueta de estado del pedido.</summary>
    public static string ClaseEstado(string? estado) => estado switch
    {
        "Pendiente" => "text-bg-warning",
        "Pagado" => "text-bg-info",
        "Enviado" => "text-bg-primary",
        "Entregado" => "text-bg-success",
        "Cancelado" => "text-bg-secondary",
        _ => "text-bg-light"
    };
}
