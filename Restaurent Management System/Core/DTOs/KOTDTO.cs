namespace PMSCore.DTOs;

public class KOTDTO
{
    public int OrderId { get; set; }
    public string OrderStatus { get; set; } = string.Empty;
    public DateTime OrderAt { get; set; } = DateTime.MinValue;
    public string ExtraComments { get; set; } = string.Empty;
    public string GroupListId { get; set; }= string.Empty;
    public int ItemId { get; set; } 
    public string ItemName { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int ItemQuantity { get; set; }
    public int PreparedItems { get; set; }
    public string SpecialInstructions { get; set; } = string.Empty;
    public int? ModifierId { get; set; }
    public string ModifierName { get; set; } = string.Empty;
    public string TableNames { get; set; } = string.Empty;
    public string SectionName { get; set; } = string.Empty;
}
