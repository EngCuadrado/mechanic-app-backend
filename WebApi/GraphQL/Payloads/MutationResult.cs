namespace WebApi.GraphQL.Payloads
{
    // Este es un objeto de respuesta genérico
    // que podemos reusar para otras mutaciones.
    public record MutationResult(
        bool Result,
        string? Message
    );
}
