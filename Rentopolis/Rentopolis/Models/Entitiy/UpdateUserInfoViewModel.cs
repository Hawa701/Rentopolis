using System.ComponentModel.DataAnnotations;

namespace Rentopolis.Models.Entitiy
{
    public class UpdateUserInfoViewModel
    {
        [Required]
        public string Id { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Profile Picture")]
        public IFormFile? ProfilePic { get; set; }
        public string? ProfilePicUrl { get; set; }
    }
}
