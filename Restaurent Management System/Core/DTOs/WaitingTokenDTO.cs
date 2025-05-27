using NpgsqlTypes;

namespace PMSCore.DTOs;

[PgName("waiting_token")]
public class WaitingTokenDTO
{
    [PgName("token_id")]
    public int TokenId { get; set; }
    [PgName("customer_id")]
    public int CustomerId { get; set; }
    [PgName("createat")]
    public DateTime Createat { get; set; } = DateTime.Now;
    [PgName("modifyat")]
    public DateTime? Modifyat { get; set; }
    [PgName("no_of_person")]
    public int NoOfPerson { get; set; }
    [PgName("cust_name")]
    public string CustName { get; set; } = null!;
    [PgName("email_id")]
    public string EmailId { get; set; } = null!;
    [PgName("phone_number")]
    public string PhoneNumber { get; set; } = null!;
    public List<int> TableIds { get; set; } = new();
    [PgName("section_id")]
    public int SectionId { get; set; }
    [PgName("createby")]
    public int Createby { get; set; } = 0;
    [PgName("modifyby")]
    public int Modifyby { get; set; } = 0;
    [PgName("isactive")]
    public bool IsActive { get; set; } = true;
}
