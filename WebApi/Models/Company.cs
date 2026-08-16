using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models;

public class Company
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("company_id")]
    public int companyId { get; set; }

    [Column("name")]
    public string name { get; set; }

    [Column("tax_id")]
    public string taxId { get; set; }

    [Column("billing_address")]
    public string billingAddress { get; set; }

    [Column("contact_email")]
    public string contactEmail { get; set; }

    [Column("contact_phone")]
    public string contactPhone { get; set; }

    [Column("default_currency")]
    public string defaultCurrency { get; set; }

    [Column("logo_url")]
    public string logoUrl { get; set; }

    [Column("is_active")]
    public bool isActive { get; set; }
}


/*
CREATE TABLE [companies] (
  [company_id] INT IDENTITY(1,1) PRIMARY KEY,
  [name] nvarchar(255),
  [tax_id] nvarchar(255),
  [billing_address] text,
  [contact_email] nvarchar(255),
  [contact_phone] nvarchar(255),
  [default_currency] nvarchar(255) DEFAULT 'NIO',
  [logo_url] nvarchar(255),
  [is_active] BIT DEFAULT 1
)
GO
 */
