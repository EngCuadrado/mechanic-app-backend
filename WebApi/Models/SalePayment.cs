using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models;

public class SalePayment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("sale_payment_id")]
    public int SalePaymentId { get; set; }

    [Required]
    [Column("sale_id")]
    public int SaleId { get; set; }

    [Required]
    [Column("payment_method_id")]
    public int PaymentMethodId { get; set; }

    [Required]
    [Column("amount", TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    // Al no tener NOT NULL en SQL, no lleva [Required] en C#.
    [MaxLength(100)]
    [Column("transaction_reference")]
    public string TransactionReference { get; set; } // Código del voucher del POS o número de transferencia

    // --- Propiedades de Navegación (Claves Foráneas) ---

    [ForeignKey(nameof(SaleId))]
    public virtual Sale Sale { get; set; }

    [ForeignKey(nameof(PaymentMethodId))]
    public virtual PaymentMethod PaymentMethod { get; set; }
}
