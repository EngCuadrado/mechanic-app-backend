using GreenDonut;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Models;

namespace WebApi.GraphQL;

// Campos calculados de WorkOrder (HotChocolate los resuelve en lote, sin N+1).
[ExtendObjectType(typeof(WorkOrder))]
public class WorkOrderFields
{
    // Mecánicos distintos asignados a los servicios de la orden.
    public async Task<IEnumerable<Mechanics>> GetAssignedMechanics(
        [Parent] WorkOrder order, AssignedMechanicsDataLoader loader, CancellationToken ct) =>
        await loader.LoadAsync(order.workOrderId, ct) ?? Array.Empty<Mechanics>();
}

public class AssignedMechanicsDataLoader : BatchDataLoader<int, Mechanics[]>
{
    private readonly AppDbContext _context;

    public AssignedMechanicsDataLoader(AppDbContext context, IBatchScheduler scheduler, DataLoaderOptions options)
        : base(scheduler, options) => _context = context;

    protected override async Task<IReadOnlyDictionary<int, Mechanics[]>> LoadBatchAsync(
        IReadOnlyList<int> keys, CancellationToken ct)
    {
        var rows = await _context.WorkOrderServices.AsNoTracking()
            .Where(s => keys.Contains(s.workOrderId) && s.mechanicId != null)
            .Select(s => new { s.workOrderId, Mechanic = s.Mechanic! })
            .ToListAsync(ct);
        return rows.GroupBy(r => r.workOrderId).ToDictionary(
            g => g.Key,
            g => g.Select(r => r.Mechanic).DistinctBy(m => m.MechanicId).ToArray());
    }
}
