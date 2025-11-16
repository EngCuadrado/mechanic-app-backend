namespace WebApi.GraphQL
{
    // DbContext
    using WebApi.Data; 
    
    // Modelos
    using WebApi.Models.Empleados;
    using WebApi.Models;
    
    using HotChocolate.Types;
    using HotChocolate.Data;
    using HotChocolate;

    // Esta clase es el "mostrador" para el cliente.
    // Define QUÉ se puede pedir.
    public class Query
    {
        // Este método expone tu tabla de Empleados.
        // Usamos IQueryable para máxima eficiencia.
        [UsePaging]
        [UseProjection] // Lee las relaciones.
        [UseFiltering]  // (Opcional) Permite filtrar (ej: where name = "...")
        [UseSorting]    // (Opcional) Permite ordenar (ej: order by names)
        public IQueryable<Employee> GetEmployees(
            [Service] AppDbContext context) // Inyecta tu DbContext
        {
            // NO HAGAS .ToList()
            // Solo devuelve la consulta. Hot Chocolate y EF Core
            // se encargan de optimizar el SQL.
            return context.Employees;
        }

        // Hacemos lo mismo para Productos
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Product> GetProducts(
            [Service] AppDbContext context)
        {
            return context.Products;
        }
    }
}
