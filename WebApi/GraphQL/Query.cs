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
    using Microsoft.EntityFrameworkCore;
    using WebApi.GraphQL.Mutations;
    using WebApi.GraphQL.Payloads;
    using WebApi.Services;

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


        [UsePaging(MaxPageSize = 1000)]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Brand> GetVehicleBrands([Service] AppDbContext context)
        {
            return context.Brands;
        }

        [UsePaging(MaxPageSize = 1000)]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Model> GetVehicleModels([Service] AppDbContext context)
        {
            return context.Models;
        }

        [UsePaging(MaxPageSize = 1000)]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Vehicle> GetVehicles([Service] AppDbContext context)
        {
            return context.Vehicles;
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

        [UsePaging(MaxPageSize = 1000)]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<InspectionCategory> GetInspectionCategories([Service] AppDbContext context)
        {
            return context.InspectionCategories.OrderBy(c => c.sortOrder).ThenBy(c => c.name);
        }

        [UsePaging(MaxPageSize = 1000)]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<InspectionChecklistItem> GetInspectionChecklistItems([Service] AppDbContext context)
        {
            return context.InspectionChecklistItems
                .OrderBy(i => i.Category!.sortOrder).ThenBy(i => i.itemId);
        }

        [UseFirstOrDefault]
        [UseProjection]
        public IQueryable<InspectionChecklistItem> GetInspectionChecklistItem(
            [Service] AppDbContext context, int itemId)
        {
            return context.InspectionChecklistItems.Where(i => i.itemId == itemId);
        }

        // ---- Inventario: libro de movimientos ----
        [UsePaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<InventoryMovement> GetInventoryMovements([Service] AppDbContext context, int partId)
        {
            return context.InventoryMovements.Where(m => m.partId == partId)
                .OrderByDescending(m => m.createdAt).ThenByDescending(m => m.movementId);
        }

        // Lee la vista inventory_stock_check: debe estar vacía; si tiene filas el saldo no coincide con el libro.
        public async Task<StockCheck> GetInventoryStockCheck([Service] AppDbContext context)
        {
            var rows = new List<StockCheckRow>();
            await context.Database.OpenConnectionAsync();
            try
            {
                using var cmd = context.Database.GetDbConnection().CreateCommand();
                cmd.CommandText = "SELECT * FROM inventory_stock_check";
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                    rows.Add(new StockCheckRow(Enumerable.Range(0, reader.FieldCount)
                        .Select(i => new StockCheckCell(reader.GetName(i), reader.IsDBNull(i) ? null : Convert.ToString(reader.GetValue(i), System.Globalization.CultureInfo.InvariantCulture)))
                        .ToList()));
            }
            finally { await context.Database.CloseConnectionAsync(); }
            return new StockCheck(rows.Count == 0, rows);
        }

        // ---- Alquileres y facturas ----
        [UsePaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<VehicleRental> GetVehicleRentals([Service] AppDbContext context) =>
            context.VehicleRentals.OrderByDescending(r => r.startDate).ThenByDescending(r => r.id);

        [UseFirstOrDefault]
        [UseProjection]
        public IQueryable<VehicleRental> GetVehicleRental([Service] AppDbContext context, int id) =>
            context.VehicleRentals.Where(r => r.id == id);

        [UsePaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Invoice> GetInvoices([Service] AppDbContext context) =>
            context.Invoices.OrderByDescending(i => i.createdAt).ThenByDescending(i => i.id);

        [UseFirstOrDefault]
        [UseProjection]
        public IQueryable<Invoice> GetInvoice([Service] AppDbContext context, int id) =>
            context.Invoices.Where(i => i.id == id);

        // ---- Órdenes de trabajo ----
        // Filtros vía where (currentStatus, type, vehicleId, companyId, entryTimestamp gte/lte); más recientes primero.
        [UsePaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<WorkOrder> GetWorkOrders([Service] AppDbContext context) =>
            context.WorkOrders.OrderByDescending(o => o.entryTimestamp).ThenByDescending(o => o.workOrderId);

        // Una fila por estado (incluye los de 0), calculado con GROUP BY en la BD.
        public async Task<List<WorkOrderStatusCount>> GetWorkOrderStatusCounts(
            [Service] AppDbContext context, DateTime? entryFrom, DateTime? entryTo, int? vehicleId)
        {
            var q = context.WorkOrders.AsNoTracking().AsQueryable();
            if (entryFrom.HasValue) q = q.Where(o => o.entryTimestamp >= entryFrom.Value);
            if (entryTo.HasValue) q = q.Where(o => o.entryTimestamp <= entryTo.Value);
            if (vehicleId.HasValue) q = q.Where(o => o.vehicleId == vehicleId.Value);

            var counts = await q.GroupBy(o => o.currentStatus)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Status, x => x.Count);
            return WorkOrderRules.Statuses
                .Select(s => new WorkOrderStatusCount(s, counts.GetValueOrDefault(s))).ToList();
        }

        // Cifras del dashboard. "Hoy" = día calendario en WorkOrderRules.TimeZoneId.
        // stalled: Recibido/En proceso con más de stalledHours desde statusChangedAt (entryTimestamp si es null).
        public async Task<WorkOrderDashboard> GetWorkOrderDashboard(
            [Service] AppDbContext context, int stalledHours = 24)
        {
            if (stalledHours <= 0) throw new GraphQLException("stalledHours debe ser mayor que 0.");
            var now = WorkOrderRules.Now();
            var today = now.Date;
            var tomorrow = today.AddDays(1);
            var limit = now.AddHours(-stalledHours);
            var orders = context.WorkOrders.AsNoTracking();
            return new WorkOrderDashboard(
                await orders.CountAsync(o => o.entryTimestamp >= today && o.entryTimestamp < tomorrow),
                await orders.CountAsync(o => o.currentStatus == "Listo"),
                await orders.CountAsync(o => (o.currentStatus == "Recibido" || o.currentStatus == "En proceso")
                                             && (o.statusChangedAt ?? o.entryTimestamp) < limit));
        }

        [UseFirstOrDefault]
        [UseProjection]
        public IQueryable<WorkOrder> GetWorkOrder([Service] AppDbContext context, int workOrderId) =>
            context.WorkOrders.Where(o => o.workOrderId == workOrderId);

        // ---- Inspecciones ----
        [UseFirstOrDefault]
        [UseProjection]
        public IQueryable<VehicleInspection> GetVehicleInspection([Service] AppDbContext context, int vehicleInspectionId) =>
            context.VehicleInspections.Where(i => i.vehicleInspectionId == vehicleInspectionId);

        // Checklist agrupado por categoría (sort_order) con los valores guardados; sin id devuelve el formulario vacío.
        public Task<InspectionForm> GetInspectionForm(
            [Service] AppDbContext context, int? vehicleInspectionId, string? inspectionType) =>
            WorkOrderSupport.BuildFormAsync(context, vehicleInspectionId, inspectionType);

        // ---- Fotos ----
        public async Task<List<PhotoStageGroup>> GetWorkOrderPhotos(
            [Service] AppDbContext context, int workOrderId, int? vehicleInspectionId)
        {
            var q = context.WorkOrderPhotos.AsNoTracking().Where(p => p.workOrderId == workOrderId);
            if (vehicleInspectionId.HasValue) q = q.Where(p => p.vehicleInspectionId == vehicleInspectionId.Value);
            var photos = await q.OrderBy(p => p.uploadedAt).ToListAsync();
            return new[] { "Antes", "Despues", "Otro" }
                .Select(s => new PhotoStageGroup(s, photos.Where(p => p.stage == s).ToList()))
                .Where(g => g.Photos.Count > 0).ToList();
        }

    }
}
