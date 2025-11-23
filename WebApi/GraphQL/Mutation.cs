using WebApi.Data;
using WebApi.GraphQL.Inputs;
using WebApi.Models.Empleados;
using WebApi.GraphQL.Payloads;
using Microsoft.EntityFrameworkCore; // ¡Importante para AnyAsync!
using HotChocolate;
using HotChocolate.Types;

namespace WebApi.GraphQL
{ 
    // Esta clase es la RAÍZ.
    // GraphQL prohíbe que esté vacía, así que le ponemos un campo "Ping".
    public class Mutation
    {
        public string Ping() => "Pong";
    }
}