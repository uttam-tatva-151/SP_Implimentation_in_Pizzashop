namespace PMSCore.DTOs;

public class UpdateOrderDTO
{
    public int OrderId { get; set; }
    public int Modifiedby { get; set; }             // For Modifiedby
    public string OrderStatus { get; set; } = null!;
    public decimal Subtotal { get; set; }
    public decimal Total { get; set; }
    public string PaymentMethod { get; set; } = null!;
    public string PaymentStatus { get; set; } = null!;
    public string? OrderInstruction { get; set; } // For ExtraComments
}
