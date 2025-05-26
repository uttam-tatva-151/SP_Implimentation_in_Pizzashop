namespace PMSCore.DTOs
{
    public class UserDto
    {
        public string Email { get; set; } =null!;
        public string UserName { get; set; } =null!;
        public int UserId { get; set; } = 0;
        public string AccessToken { get; set; } = string.Empty;
    }
}
