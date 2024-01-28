using System.ComponentModel.DataAnnotations;

namespace Rentopolis.Models.Entitiy
{
    public class PasswordViewModel
    {
        [Required]
        public string Id { get; set; }
        
        [Required]
        [Display(Name = "Old Password")]
        public string OldPassword { get; set; }

        [Required]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [Required]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword")]
        public string ConfirmNewPassword { get; set; }
    }
}
