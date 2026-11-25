using System.ComponentModel.DataAnnotations;

namespace MiniCRMProject.Models
{
    public class ProjectModel
    {
        public int ProjectId { get; set; }

        [Required(ErrorMessage = "Proje adı zorunlu.")]
        [StringLength(150, ErrorMessage = "Proje adı en fazla 150 karakter olabilir.")]
        public string ProjectName { get; set; } = "";

        [StringLength(2000, ErrorMessage = "Açıklama en fazla 2000 karakter olabilir.")]
        public string? Description { get; set; }

        public string? Status { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Bir müşteri seçin.")]
        public int CustomerId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? EndDate { get; set; }
        public string? CustomerFullName { get; set; }
    }
}
