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
            return context.Employees;
        }

        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<EmployeeRoles> GetEmployeeRoles(
        [Service] AppDbContext context)
        {
            return context.EmployeeRoles;
        }

        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<EmployeeStatuses> GetEmployeeStatus(
            [Service] AppDbContext context)
        {
            return context.EmployeeStatuses;
        }

        [UsePaging]
        [UseProjection] // Lee las relaciones.
        [UseFiltering]  // (Opcional) Permite filtrar (ej: where name = "...")
        [UseSorting]    // (Opcional) Permite ordenar (ej: order by names)    
        public IQueryable<Brands> GetBrands([Service] AppDbContext context)
        {
            return context.Brands;
        }

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
