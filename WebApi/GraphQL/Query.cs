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
        
        [UsePaging]
        [UseProjection] // Lee las relaciones.
        [UseFiltering]  // (Opcional) Permite filtrar (ej: where name = "...")
        [UseSorting]    // (Opcional) Permite ordenar (ej: order by names)    
        public IQueryable<Category> GetCategories([Service] AppDbContext context)
        {
            return context.Categories;
        }
        
        [UsePaging]
        [UseProjection] // Lee las relaciones.
        [UseFiltering]  // (Opcional) Permite filtrar (ej: where name = "...")
        [UseSorting]    // (Opcional) Permite ordenar (ej: order by names)    
        public IQueryable<ActiveIngredient> GetActiveIngredients([Service] AppDbContext context)
        {
            return context.ActiveIngredients;
        }
        
        [UsePaging]
        [UseProjection] // Lee las relaciones.
        [UseFiltering]  // (Opcional) Permite filtrar (ej: where name = "...")
        [UseSorting]    // (Opcional) Permite ordenar (ej: order by names)    
        public IQueryable<AdministrationRoute> GetAdministrationRoutes([Service] AppDbContext context)
        {
            return context.AdministrationRoutes;
        }
        
        [UsePaging]
        [UseProjection] // Lee las relaciones.
        [UseFiltering]  // (Opcional) Permite filtrar (ej: where name = "...")
        [UseSorting]    // (Opcional) Permite ordenar (ej: order by names)    
        public IQueryable<UnitOfMeasure> GetUnitOfMeasures([Service] AppDbContext context)
        {
            return context.UnitOfMeasures;
        }
        
        [UsePaging]
        [UseProjection] // Lee las relaciones.
        [UseFiltering]  // (Opcional) Permite filtrar (ej: where name = "...")
        [UseSorting]    // (Opcional) Permite ordenar (ej: order by names)    
        public IQueryable<DoseUnit> GetDoseUnits([Service] AppDbContext context)
        {
            return context.DoseUnits;
        }
        
        [UsePaging]
        [UseProjection] // Lee las relaciones.
        [UseFiltering]  // (Opcional) Permite filtrar (ej: where name = "...")
        [UseSorting]    // (Opcional) Permite ordenar (ej: order by names)    
        public IQueryable<SupplierType> GetSupplierTypes([Service] AppDbContext context)
        {
            return context.SupplierTypes;
        }
        
        [UsePaging]
        [UseProjection] // Lee las relaciones.
        [UseFiltering]  // (Opcional) Permite filtrar (ej: where name = "...")
        [UseSorting]    // (Opcional) Permite ordenar (ej: order by names)    
        public IQueryable<Supplier> GetSuppliers([Service] AppDbContext context)
        {
            return context.Suppliers;
        }

        
        // -------------------------------------------------------------------
        [UsePaging]
        [UseProjection] // Lee las relaciones.
        [UseFiltering]  // (Opcional) Permite filtrar (ej: where name = "...")
        [UseSorting]    // (Opcional) Permite ordenar (ej: order by names)    
        public IQueryable<Product> GetProducts([Service] AppDbContext context)
        {
            return context.Products;
        }
        
        [UsePaging]
        [UseProjection] 
        [UseFiltering]
        [UseSorting]       
        public IQueryable<Medicine> GetMedicines([Service] AppDbContext context)
        {
            return context.Medicines;
        }
        
        [UsePaging]
        [UseProjection] // Lee las relaciones.
        [UseFiltering]  // (Opcional) Permite filtrar (ej: where name = "...")
        [UseSorting]    // (Opcional) Permite ordenar (ej: order by names)    
        public IQueryable<MedicineActiveIngredient> GetMedicineActiveIngredients([Service] AppDbContext context)
        {
            return context.MedicineActiveIngredients;
        }

        [UsePaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Batch> GetBatches([Service] AppDbContext context)
        {
            return context.Batches; 
        }

        [UsePaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Presentation> GetPresentations([Service] AppDbContext context)
        {
            return context.Presentations;
        }


        [UsePaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Manufacturer> GetManufacturers([Service] AppDbContext context)
        {
            return context.Manufacturers;
        }

        [UsePaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<PaymentMethod> GetActivePaymentMethods([Service] AppDbContext context)
        {
            return context.PaymentMethods.Where(p => p.IsActive);
        }

        [UsePaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Promotion> GetActivePromotions([Service] AppDbContext context)
        {
            return context.Promotions.Where(p => p.IsActive);
        }


        [UsePaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Customer> GetCustomers([Service] AppDbContext context)
        {
            return context.Customers;
        }

        [UsePaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<PaymentMethod> GetPaymentMethods([Service] AppDbContext context)
        {
            return context.PaymentMethods;
        }

        [UsePaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<MechanicSpecialties> GetMechanicSpecialties([Service] AppDbContext context)
        {
            return context.mechanicSpecialties;
        }

        [UsePaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Mechanics> GetMechanics([Service] AppDbContext context)
        {
            return context.Mechanics;
        }

        [UsePaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Company> GetCompanies([Service] AppDbContext context)
        {
            return context.Companies;
        }

        [UsePaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<InventoryPart> GetInventoryParts([Service] AppDbContext context)
        {
            return context.InventoryParts;
        }

    }
}
