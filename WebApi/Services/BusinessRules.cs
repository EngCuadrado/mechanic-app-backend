using System.Globalization;
using HotChocolate;

namespace WebApi.Services;

// Reglas de negocio puras (sin BD) para poder probarlas aisladas.
public static class WorkOrderRules
{
    public static readonly string[] Types = { "Preventivo", "Correctivo", "Choque", "Entrega", "Devolucion" };
    public static readonly string[] Statuses = { "Recibido", "En proceso", "Listo", "Entregado", "Cancelado" };

    private static readonly Dictionary<string, string[]> Transitions = new()
    {
        ["Recibido"] = new[] { "En proceso", "Cancelado" },
        ["En proceso"] = new[] { "Listo", "Cancelado" },
        ["Listo"] = new[] { "Entregado", "Cancelado" },
    };

    public static bool IsClosed(string status) => status is "Entregado" or "Cancelado";

    public static bool CanTransition(string from, string to) =>
        Transitions.TryGetValue(from, out var next) && next.Contains(to);

    public static void RequireValid(string value, string[] allowed, string label)
    {
        if (!allowed.Contains(value))
            throw new GraphQLException($"{label} inválido. Valores permitidos: {string.Join(", ", allowed)}.");
    }

    public static (decimal Cost, decimal Billed) Totals(
        IEnumerable<(int Quantity, decimal UnitCost, decimal UnitPrice)> parts,
        IEnumerable<(decimal InternalCost, decimal Price)> services)
    {
        var p = parts.ToList();
        var s = services.ToList();
        return (p.Sum(x => x.Quantity * x.UnitCost) + s.Sum(x => x.InternalCost),
                p.Sum(x => x.Quantity * x.UnitPrice) + s.Sum(x => x.Price));
    }
}

public static class MovementRules
{
    // Uso y Devolucion los genera el trigger de work_order_parts: no se aceptan desde el cliente.
    public static readonly string[] ClientTypes = { "Compra", "Ajuste", "Inicial" };

    public static void Validate(string type, int quantity, decimal? unitCost)
    {
        if (!ClientTypes.Contains(type))
            throw new GraphQLException("Tipo de movimiento inválido. Solo se permite Compra, Ajuste o Inicial (Uso y Devolucion los genera el sistema).");
        if (type == "Ajuste" ? quantity == 0 : quantity <= 0)
            throw new GraphQLException(type == "Ajuste"
                ? "La cantidad de un Ajuste no puede ser 0 (use positivo para sumar, negativo para restar)."
                : $"La cantidad de un movimiento {type} debe ser mayor que 0.");
        if (unitCost.HasValue && type != "Compra")
            throw new GraphQLException("El costo unitario solo aplica a movimientos de tipo Compra.");
        if (unitCost < 0)
            throw new GraphQLException("El costo unitario no puede ser negativo.");
    }
}

public static class InspectionRules
{
    public static readonly string[] InputTypes =
        { "text", "textarea", "number", "checkbox", "date", "select", "radio" };

    public static bool UsesOptions(string inputType) => inputType is "select" or "radio";

    // Regla BD: options obligatorio (no vacío) para select/radio y NULL para el resto.
    public static List<string>? ValidateOptions(string inputType, List<string>? options)
    {
        if (!InputTypes.Contains(inputType))
            throw new GraphQLException($"Tipo de entrada inválido. Valores permitidos: {string.Join(", ", InputTypes)}.");

        if (!UsesOptions(inputType))
        {
            if (options != null)
                throw new GraphQLException($"Las opciones solo aplican a los tipos select y radio (tipo actual: {inputType}).");
            return null;
        }

        var clean = options?.Select(o => o?.Trim() ?? "").ToList();
        if (clean == null || clean.Count == 0 || clean.Any(o => o.Length == 0))
            throw new GraphQLException($"El tipo {inputType} requiere una lista de opciones no vacía, sin valores en blanco.");
        if (clean.Distinct().Count() != clean.Count)
            throw new GraphQLException("Las opciones no pueden repetirse.");
        return clean;
    }

    // Devuelve el mensaje de error o null si el valor es válido. Vacío = sin respuesta (válido aquí).
    public static string? ValidateValue(string inputType, List<string>? options, string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        return inputType switch
        {
            "checkbox" => value is "true" or "false" ? null : "debe ser \"true\" o \"false\"",
            "number" => decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out _) ? null : "debe ser numérico",
            "date" => DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out _) ? null : "debe ser una fecha válida (yyyy-MM-dd)",
            "select" or "radio" => options != null && options.Contains(value) ? null : "debe ser una de las opciones del ítem",
            _ => null
        };
    }
}
