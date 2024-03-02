using System.ComponentModel.DataAnnotations;

namespace EntityDataApi.ViewModels
{
    public class RegisterModel
    {
        [Required]
        [MaxLength(50)]
        public string? Username { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Password { get; set; }

        [MaxLength(100)]
        public string? FullName { get; set; }
    }
}