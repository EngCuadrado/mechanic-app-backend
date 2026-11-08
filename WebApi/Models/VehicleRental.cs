using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models;

public class VehicleRental
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int id { get; set; }

    [Column("vehicle_id")]
    public int vehicleId { get; set; }

    [Column("company_id")]
    public int companyId { get; set; }

    [Column("start_date")]
    public DateTime? startDate { get; set; }

    // NULL = indefinido
    [Column("expected_end_date")]
    public DateTime? expectedEndDate { get; set; }

    [Column("actual_end_date")]
    public DateTime? actualEndDate { get; set; }

    // Diario, Mensual, Anual
    [Column("billing_period")]
    public string? billingPeriod { get; set; }

    [Column("rate_amount")]
    public decimal? rateAmount { get; set; }

    // Activo, Finalizado, Cancelado
    [Column("status")]
    public string? status { get; set; }

    [Column("contract_url")]
    public string? contractUrl { get; set; }

    [Column("created_at")]
    public DateTime? createdAt { get; set; }

    [ForeignKey("vehicleId")]
    public Vehicle? Vehicle { get; set; }

    [ForeignKey("companyId")]
    public Company? Company { get; set; }
}
