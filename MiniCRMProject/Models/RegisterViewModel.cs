using System.ComponentModel.DataAnnotations;

namespace MiniCRMProject.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Ad zorunlu.")]
        [StringLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir.")]
        public string FirstName { get; set; } = "";

        [Required(ErrorMessage = "Soyad zorunlu.")]
        [StringLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir.")]
        public string LastName { get; set; } = "";

        [Required(ErrorMessage = "E-posta zorunlu.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
        [StringLength(100, ErrorMessage = "E-posta en fazla 100 karakter olabilir.")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Şifre zorunlu.")]
        [MinLength(8, ErrorMessage = "Şifre en az 8 karakter olmalı.")]
        [StringLength(100, ErrorMessage = "Şifre en fazla 100 karakter olabilir.")]
        public string Password { get; set; } = "";
    }
}
