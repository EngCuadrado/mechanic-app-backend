using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApi.Models.Empleados;

namespace WebApi.Models;

public class Sale
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("sale_id")]
    public int SaleId { get; set; }

    [Column("customer_id")]
    public int? CustomerId { get; set; }

    [Required]
    [Column("employee_id")]
    public int EmployeeId { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("receipt_type")]
    public string ReceiptType { get; set; } // 'FACTURA', 'TICKET'

    [Required]
    [MaxLength(50)]
    [Column("receipt_number")]
    public string ReceiptNumber { get; set; }

    [Required]
    [Column("sale_date")]
    public DateTime SaleDate { get; set; } = DateTime.Now;

    [Required]
    [Column("subtotal", TypeName = "decimal(18,2)")]
    public decimal Subtotal { get; set; }

    [Required]
    [Column("total_discount", TypeName = "decimal(18,2)")]
    public decimal TotalDiscount { get; set; } = 0m;

    [Required]
    [Column("total_tax", TypeName = "decimal(18,2)")]
    public decimal TotalTax { get; set; }

    [Required]
    [Column("total_amount", TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [MaxLength(50)]
    [Column("status")]
    public string Status { get; set; } = "COMPLETED"; // Ej: 'COMPLETED', 'CANCELLED'

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // --- Propiedades de Navegación ---

    [ForeignKey(nameof(CustomerId))]
    public virtual Customer Customer { get; set; }

    [ForeignKey(nameof(EmployeeId))]
    public virtual Employee Employee { get; set; }

    public virtual ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();
    public virtual ICollection<SalePayment> SalePayments { get; set; } = new List<SalePayment>();
}
