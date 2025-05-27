namespace PMSCore.DTOs;

public class CustomerDTO
{
    public int CustId { get; set; }
    public string CustName { get; set; } = null!;
    public string EmailId { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public int TotalOrders { get; set; } = 1;
    public DateTime Createat { get; set; } = DateTime.Now;
    public DateTime? Modifyat { get; set; }
    public bool Iscontinued { get; set; } = true;
}
