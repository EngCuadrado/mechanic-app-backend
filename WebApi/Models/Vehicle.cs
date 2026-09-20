using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models;

public class Vehicle
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("vehicle_id")]
    public int vehicleId { get; set; }

    [Column("company_id")]
    public int? companyId { get; set; }

    [Column("model_id")]
    public int modelId { get; set; }

    [Column("plate")]
    public string plate { get; set; }

    [Column("vin")]
    public string vin { get; set; }

    [Column("chassis_number")]
    public string chassisNumber { get; set; }

    [Column("year")]
    public int year { get; set; }

    [Column("current_mileage")]
    public int currentMilage { get; set; }

    [Column("next_maintenance_date")]
    public DateOnly nextMaintenanceDate { get; set; }

    [Column("next_maintenance_mileage")]
    public int nextMaintenanceMilage { get; set; }

    [Column("insurance_expiration_date")]
    public DateOnly insuranceExpirationDate { get; set; }

    [Column("mechanical_inspection_expiration")]
    public DateOnly mechanicalInspectionExpiration { get; set; }

    [Column("emissions_inspection_expiration")]
    public DateOnly emissionsInspectionExpiration { get; set; }

    [Column("status")]
    public VehicleStatuses status { get; set; } = VehicleStatuses.DISPONIBLE;

    [ForeignKey("companyId")]
    public Company? Company { get; set; }

    [ForeignKey("modelId")]
    public Model? Model { get; set; }
}


/*
CREATE TABLE [vehicles] (
  [vehicle_id] INT IDENTITY(1,1) PRIMARY KEY,
  [company_id] int, -- Empresa que actualmente tiene el vehículo. Null = Disponible
  [model_id] int NOT NULL, -- Relación con el modelo (y por ende, con la marca)
  [plate] nvarchar(255) UNIQUE, -- Placa
  [vin] nvarchar(255) UNIQUE, -- Número de serie del vehículo
  [chassis_number] nvarchar(255) UNIQUE, -- Número de chasis
  [year] int, -- Año de fabricación
  [current_mileage] int, -- Kilometraje actual
  [next_maintenance_date] date, -- Fecha del proximo mantenimiento
  [next_maintenance_mileage] int, -- Kilometraje del proximo mantenimiento
  [insurance_expiration_date] date, -- Fecha de vencimiento del seguro
  [mechanical_inspection_expiration] date, -- Fecha de vencimiento de la inspección mecánica
  [emissions_inspection_expiration] date, -- Fecha de vencimiento de la inspección de emisiones
  [status] nvarchar(255), -- Estado del vehículo [en mantenimiento, disponible, alquilado, etc.]
  
  FOREIGN KEY ([model_id]) REFERENCES [models]([model_id])
)
GO
*/