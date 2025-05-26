namespace PMSCore.ViewModel
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string? LastName { get; set; } 
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string Status { get; set; } = null!;
        public int editorId { get; set; } = 0;
        public byte[]? imgData { get; set; }
    
    }
}
