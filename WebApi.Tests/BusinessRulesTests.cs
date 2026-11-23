using HotChocolate;
using Microsoft.Data.SqlClient;
using WebApi.GraphQL;
using WebApi.Services;
using Xunit;

public class WorkOrderRulesTests
{
    [Theory]
    [InlineData("Recibido", "En proceso")]
    [InlineData("En proceso", "Listo")]
    [InlineData("Listo", "Entregado")]
    [InlineData("Recibido", "Cancelado")]
    [InlineData("En proceso", "Cancelado")]
    [InlineData("Listo", "Cancelado")]
    public void Valid_transitions(string from, string to) => Assert.True(WorkOrderRules.CanTransition(from, to));

    [Theory]
    [InlineData("Recibido", "Listo")]
    [InlineData("Recibido", "Entregado")]
    [InlineData("Listo", "En proceso")]
    [InlineData("Entregado", "Cancelado")]
    [InlineData("Entregado", "Recibido")]
    [InlineData("Cancelado", "Recibido")]
    public void Invalid_transitions(string from, string to) => Assert.False(WorkOrderRules.CanTransition(from, to));

    [Fact]
    public void Totals_sum_parts_and_services()
    {
        var (cost, billed) = WorkOrderRules.Totals(
            new[] { (2, 10.5m, 15m), (1, 3m, 5m) },
            new[] { (20m, 50m) });
        Assert.Equal(44m, cost);   // 2*10.5 + 3 + 20
        Assert.Equal(85m, billed); // 2*15 + 5 + 50
    }

    [Fact]
    public void Totals_empty_is_zero() => Assert.Equal((0m, 0m), WorkOrderRules.Totals(new (int, decimal, decimal)[0], new (decimal, decimal)[0]));

    private static readonly OrderFacts Complete = new(true, true, true, true, true);

    [Theory]
    [InlineData("En proceso", "RECEPTION_INCOMPLETE")]
    [InlineData("Listo", "NO_WORK")]
    [InlineData("Entregado", "DELIVERY_INCOMPLETE")]
    public void Nothing_done_blocks_each_step(string to, string code) =>
        Assert.Equal(code, WorkOrderRules.TransitionBlocker(to, new OrderFacts())?.Code);

    [Theory]
    [InlineData("En proceso")]
    [InlineData("Listo")]
    [InlineData("Entregado")]
    [InlineData("Cancelado")]
    public void Complete_order_is_never_blocked(string to) => Assert.Null(WorkOrderRules.TransitionBlocker(to, Complete));

    [Fact]
    public void Cancel_is_never_blocked() => Assert.Null(WorkOrderRules.TransitionBlocker("Cancelado", new OrderFacts()));

    [Fact]
    public void Reception_needs_inspection_and_signature()
    {
        Assert.NotNull(WorkOrderRules.TransitionBlocker("En proceso", Complete with { DropoffSigned = false }));
        Assert.NotNull(WorkOrderRules.TransitionBlocker("En proceso", Complete with { EntryInspection = false }));
    }

    [Fact]
    public void Delivery_needs_inspection_and_signature()
    {
        Assert.NotNull(WorkOrderRules.TransitionBlocker("Entregado", Complete with { PickupSigned = false }));
        Assert.NotNull(WorkOrderRules.TransitionBlocker("Entregado", Complete with { ExitInspection = false }));
    }

    [Theory]
    [InlineData(3, WebApi.Models.VehicleStatuses.ALQUILADO)]
    [InlineData(null, WebApi.Models.VehicleStatuses.DISPONIBLE)]
    public void Vehicle_leaves_shop_to_company_or_free(int? companyId, WebApi.Models.VehicleStatuses expected) =>
        Assert.Equal(expected, WorkOrderRules.StatusAfterShop(companyId));

    [Theory]
    // 02:20 UTC del 23 = 20:20 del 22 en Managua (UTC-6): "hoy" empezó el 22 a las 06:00 UTC.
    [InlineData("2026-11-23T02:20:00", "2026-11-22T06:00:00")]
    // 07:00 UTC del 23 = 01:00 del 23 en Managua: "hoy" empezó el 23 a las 06:00 UTC.
    [InlineData("2026-11-23T07:00:00", "2026-11-23T06:00:00")]
    public void Today_starts_at_shop_midnight_in_utc(string utcNow, string expected) =>
        Assert.Equal(DateTime.Parse(expected), WorkOrderRules.TodayStartUtc(DateTime.Parse(utcNow)));

    [Fact]
    public void Now_is_utc() => Assert.Equal(DateTimeKind.Utc, WorkOrderRules.Now().Kind);
}

public class MovementRulesTests
{
    [Theory]
    [InlineData("Compra", 5, 2.5)]
    [InlineData("Inicial", 1, null)]
    [InlineData("Ajuste", -3, null)]
    [InlineData("Ajuste", 3, null)]
    public void Valid(string type, int qty, double? cost) =>
        MovementRules.Validate(type, qty, cost.HasValue ? (decimal)cost : null);

    [Theory]
    [InlineData("Uso", -1, null)]
    [InlineData("Devolucion", 1, null)]
    [InlineData("Compra", -1, null)]
    [InlineData("Inicial", 0, null)]
    [InlineData("Ajuste", 0, null)]
    [InlineData("Inicial", 1, 2.0)]
    public void Invalid(string type, int qty, double? cost) =>
        Assert.Throws<GraphQLException>(() => MovementRules.Validate(type, qty, cost.HasValue ? (decimal)cost : null));
}

public class InspectionRulesTests
{
    [Fact] public void Select_requires_options() =>
        Assert.Throws<GraphQLException>(() => InspectionRules.ValidateOptions("select", null));
    [Fact] public void Radio_rejects_empty_list() =>
        Assert.Throws<GraphQLException>(() => InspectionRules.ValidateOptions("radio", new List<string>()));
    [Fact] public void Select_rejects_blank_option() =>
        Assert.Throws<GraphQLException>(() => InspectionRules.ValidateOptions("select", new List<string> { "a", " " }));
    [Fact] public void Text_rejects_options() =>
        Assert.Throws<GraphQLException>(() => InspectionRules.ValidateOptions("text", new List<string> { "a" }));
    [Fact] public void Text_without_options_is_null() => Assert.Null(InspectionRules.ValidateOptions("text", null));
    [Fact] public void Select_trims_options() =>
        Assert.Equal(new[] { "a", "b" }, InspectionRules.ValidateOptions("select", new List<string> { " a", "b " }));
    [Fact] public void Unknown_type_rejected() =>
        Assert.Throws<GraphQLException>(() => InspectionRules.ValidateOptions("bogus", null));

    [Theory]
    [InlineData("checkbox", "true", true)]
    [InlineData("checkbox", "false", true)]
    [InlineData("checkbox", "yes", false)]
    [InlineData("number", "12.5", true)]
    [InlineData("number", "abc", false)]
    [InlineData("date", "2026-11-07", true)]
    [InlineData("date", "no-fecha", false)]
    [InlineData("text", "lo que sea", true)]
    [InlineData("number", "", true)]
    public void Value_validation(string type, string value, bool ok) =>
        Assert.Equal(ok, InspectionRules.ValidateValue(type, null, value) == null);

    [Fact] public void Select_value_must_be_in_options()
    {
        var opts = new List<string> { "Limpio", "Sucio" };
        Assert.Null(InspectionRules.ValidateValue("select", opts, "Limpio"));
        Assert.NotNull(InspectionRules.ValidateValue("radio", opts, "Otro"));
    }

    [Theory]
    [InlineData("Ambas", "Entrada", true)]
    [InlineData("Ambas", "Salida", true)]
    [InlineData("Entrada", "Entrada", true)]
    [InlineData("Entrada", "Salida", false)]
    [InlineData("Salida", "Entrada", false)]
    public void AppliesTo_FiltersByInspectionType(string appliesTo, string type, bool expected) =>
        Assert.Equal(expected, InspectionRules.Applies(appliesTo, type));
}
