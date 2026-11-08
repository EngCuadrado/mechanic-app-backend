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
}
