using System.ComponentModel.DataAnnotations;

namespace PMSCore.Beans.ENUM
{
    public enum UnitType
    {
        [Display(Name = "Pieces")]
        PCS,    // Key: Pcs, Value: Pieces

        [Display(Name = "Gram")]
        GRAM,  

        [Display(Name = "Kilo Gram")]
        KG,    

        [Display(Name = "Liters")]
        LITRE, 

        [Display(Name = "Cm")]
        CM   

    }
}
