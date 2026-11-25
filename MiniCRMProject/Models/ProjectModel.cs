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

        public ProjectStage Stage { get; set; } = ProjectStage.OlcuAlindi;

        public ProjectCategory Category { get; set; } = ProjectCategory.Konut;

        [StringLength(50, ErrorMessage = "İlçe en fazla 50 karakter olabilir.")]
        public string? District { get; set; }

        [Display(Name = "Müşteri")]
        [Range(1, int.MaxValue, ErrorMessage = "Bir müşteri seçin.")]
        public int CustomerId { get; set; }

        public DateTime CreatedAt { get; set; }
        [Display(Name = "Tahmini bitiş")]
        public DateTime? EndDate { get; set; }
        public string? CustomerFullName { get; set; }
    }
}
