using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EntityDataApi.ViewModels
{
    public class LoginModel
    {
        [Required(ErrorMessage = "User Name is required")]
        [MaxLength(50)]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MaxLength(100)]
        public string? Password { get; set; }
    }
}