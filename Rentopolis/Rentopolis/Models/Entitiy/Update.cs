using System.ComponentModel.DataAnnotations;

namespace Rentopolis.Models.Entitiy
{
    public class Update
    {
        [Required]
        public string Id { get; set; } 
        
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
    }
}
