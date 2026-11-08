using HotChocolate;
using Microsoft.Data.SqlClient;

namespace WebApi.GraphQL;

// Traduce errores de SQL Server (constraints, triggers) a mensajes de API sin exponer SQL crudo.
// Solo actúa sobre errores con una SqlException; los GraphQLException controlados pasan intactos.
public class DbErrorFilter : IErrorFilter
{
    public IError OnError(IError error)
    {
        for (var ex = error.Exception; ex != null; ex = ex.InnerException)
        {
            if (ex is not SqlException sql) continue;
            var (code, message) = Map(sql);
            return error.WithMessage(message).WithCode(code).WithException(null);
        }
        return error;
    }

    internal static (string Code, string Message) Map(SqlException e)
    {
        var m = e.Message;
        switch (e.Number)
        {
            case 50001:
            case 50002:
                return ("ORDER_CLOSED", "La orden está cerrada y no admite cambios en repuestos o servicios.");
            case 50003:
                return ("MOVEMENT_IMMUTABLE", "Los movimientos de inventario son inmutables; corrija con un movimiento de Ajuste.");
            case 2601:
            case 2627:
                if (m.Contains("UX_WO_OneOpenPerVehicle")) return ("OPEN_ORDER_EXISTS", "El vehículo ya tiene una orden abierta.");
                if (m.Contains("UQ_Inspections_WO_Type")) return ("INSPECTION_EXISTS", "La orden ya tiene una inspección de ese tipo.");
                if (m.Contains("UQ_Results_Inspection_Item")) return ("RESULT_EXISTS", "Ya existe un resultado para ese ítem en la inspección.");
                return ("DUPLICATE", "Ya existe un registro con esos datos únicos.");
            case 547:
                if (m.Contains("CK_inventory_parts_stock")) return ("INSUFFICIENT_STOCK", "Stock insuficiente para completar la operación.");
                if (m.Contains("DELETE statement")) return ("IN_USE", "No se puede eliminar: el registro está en uso por otros datos.");
                if (m.Contains("FOREIGN KEY")) return ("INVALID_REFERENCE", "Referencia inválida: un registro relacionado no existe.");
                return ("CONSTRAINT_VIOLATION", "Un valor no cumple las reglas de validación de datos.");
            default:
                return ("DATABASE_ERROR", "Error de base de datos al procesar la solicitud.");
        }
    }
}
