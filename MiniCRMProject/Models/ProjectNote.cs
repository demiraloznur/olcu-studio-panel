using System.ComponentModel.DataAnnotations;

namespace MiniCRMProject.Models
{
    // Projeye eklenen tarihli ilerleme notu ("Konsept sunumu yapıldı" gibi).
    public class ProjectNote
    {
        public int NoteId { get; set; }

        public int ProjectId { get; set; }

        [Required(ErrorMessage = "Not boş olamaz.")]
        [StringLength(1000, ErrorMessage = "Not en fazla 1000 karakter olabilir.")]
        public string Body { get; set; } = "";

        public DateTime CreatedAt { get; set; }
    }
}
