using System.ComponentModel.DataAnnotations;

namespace EntityDataApi.ViewModels
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "User Name is required")]
        [MaxLength(50)]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [MaxLength(100)]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MaxLength(100)]
        public string? Password { get; set; }
    }
}