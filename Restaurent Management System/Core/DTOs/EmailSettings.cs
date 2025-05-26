namespace PMSCore.DTOs
{
    public class EmailSettings
    {
        public string SenderEmail { get; set; } = null!;
        public string SenderPassword { get; set; } = null!;
        public string SmtpServer { get; set; } = null!;
        public int SmtpPort { get; set; }
        public bool EnableSsl { get; set; }
    }
}
