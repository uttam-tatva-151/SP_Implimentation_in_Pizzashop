using NpgsqlTypes;

namespace PMSCore.DTOs;


[PgName("item_details")]
public class ItemDetailsDTO
{
    [PgName("categoryid")]
    public int CategoryId { get; set; }
    [PgName("id")]
    public int Id { get; set; }
    [PgName("itemname")]
    public string ItemName { get; set; } = null!;
    [PgName("itemtype")]
    public string ItemType { get; set; } = null!;
    [PgName("unitprice")]
    public decimal UnitPrice { get; set; }
    [PgName("description")]
    public string Description { get; set; } = string.Empty;
    [PgName("quantity")]
    public int Quantity { get; set; }

    [PgName("unittype")]
    public string UnitType { get; set; } = null!;
    [PgName("isfavorite")]
    public bool IsFavorite { get; set; }
    [PgName("photo")]
    public string Photo { get; set; } = string.Empty;// base64-encoded image string

}
