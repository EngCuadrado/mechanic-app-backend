using System.ComponentModel.DataAnnotations.Schema;
using WebApi.Models.Empleados;

namespace WebApi.Models;

public class MedicineActiveIngredient
{
    public int medicine_id { get; set; }

    public int active_ingredient_id { get; set; }

    public decimal dose_value { get; set; }

    public int dose_unit_id { get; set; }

    // --- Propiedades de Navegación ---

    [ForeignKey("medicine_id")]
    public virtual Medicine medicine { get; set; }

    [ForeignKey("active_ingredient_id")]
    public virtual ActiveIngredient active_ingredient { get; set; }

    [ForeignKey("dose_unit_id")]
    public virtual DoseUnit dose_unit { get; set; }
}
