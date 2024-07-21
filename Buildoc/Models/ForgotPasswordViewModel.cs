using System.ComponentModel.DataAnnotations;

namespace Buildoc.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
