using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models;

public class Model
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("model_id")]
    public int modelId { get; set; }

    [Column("brand_id")]
    public int brandId { get; set; }

    [Column("model_name")]
    public string modelName { get; set; }

    [ForeignKey("brandId")]
    public Brand Brand { get; set; }
}


/*
 CREATE TABLE [brands] (
  [brand_id] INT IDENTITY(1,1) PRIMARY KEY,
  [brand_name] nvarchar(255) NOT NULL UNIQUE
)
GO

-- 2. Tabla de Modelos por Marca
CREATE TABLE [models] (
  [model_id] INT IDENTITY(1,1) PRIMARY KEY,
  [brand_id] INT NOT NULL,
  [model_name] nvarchar(255) NOT NULL,
  FOREIGN KEY ([brand_id]) REFERENCES [brands]([brand_id])
)
GO 
*/
