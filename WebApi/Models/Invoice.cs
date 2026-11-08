using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models;

public class Invoice
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int id { get; set; }

    [Column("company_id")]
    public int companyId { get; set; }

    [Column("invoice_number")]
    public string? invoiceNumber { get; set; }

    [Column("status")]
    public string? status { get; set; }

    [Column("issue_date")]
    public DateTime? issueDate { get; set; }

    [Column("due_date")]
    public DateTime? dueDate { get; set; }

    [Column("subtotal")]
    public decimal? subtotal { get; set; }

    [Column("tax_rate")]
    public decimal? taxRate { get; set; }

    [Column("tax_amount")]
    public decimal? taxAmount { get; set; }

    [Column("total_amount")]
    public decimal? totalAmount { get; set; }

    [Column("currency")]
    public string? currency { get; set; }

    [Column("exchange_rate")]
    public decimal? exchangeRate { get; set; }

    [Column("notes")]
    public string? notes { get; set; }

    [Column("created_at")]
    public DateTime? createdAt { get; set; }

    [ForeignKey("companyId")]
    public Company? Company { get; set; }
}
