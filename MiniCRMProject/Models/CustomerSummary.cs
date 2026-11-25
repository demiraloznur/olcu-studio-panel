namespace MiniCRMProject.Models
{
    public class CustomerSummary
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public int ProjectCount { get; set; }
        public int ActiveProjectCount { get; set; }
    }
}
