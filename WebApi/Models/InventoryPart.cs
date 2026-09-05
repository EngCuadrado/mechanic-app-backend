using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models;

public class InventoryPart
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("inventory_part_id")]
    public int inventoryPartId { get; set; }

    [Column("image_url")]
    public string? imageUrl { get; set; }

    [Column("name")]
    public string name { get; set; }

    [Column("stock_quantity")]
    public int stockQuantity {  get; set; }

    [Column("unit_cost")]
    public decimal unitCost { get; set; }

    [Column("tax_cost")]
    public decimal taxCost { get; set; }

    [Column("total_unit_cost")]
    public decimal totalUnitCost { get; set; }

    [Column("base_price")]
    public decimal basePrice { get; set; }

    [Column("currency")]
    public string currency {  get; set; }

    [Column("min_stock_alert")]
    public int minStockAlert { get; set; }
}


/*

CREATE TABLE [inventory_parts] (
  [inventory_part_id] INT IDENTITY(1,1) PRIMARY KEY,
  -- [sku] nvarchar(255) UNIQUE,
  [image_url] nvarchar(255) DEFAULT NULL,
  [name] nvarchar(255),
  [stock_quantity] int,
  [unit_cost] decimal(10,2),
  [tax_cost] decimal(10,2) DEFAULT (0.00),
  [total_unit_cost] decimal(10,2),
  [base_price] decimal(10,2),
  [currency] nvarchar(255) DEFAULT 'USD',
  [min_stock_alert] int
)
GO
*/