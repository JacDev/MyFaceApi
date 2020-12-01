using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MyFaceApi.IdentityServer.Models
{
	public class RegisterViewModel
	{
        [Required(ErrorMessage = "Login is required.")]
        [MaxLength(20, ErrorMessage = "Login must be max 20 characters.")]
        public string Login { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(20, ErrorMessage = "Name must be max 20 characters.")]
        [DisplayName("First name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(20, ErrorMessage = "Lastname must be max 20 characters.")]
        [DisplayName("Last name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Enter your password.")]
        //[StringLength(20, MinimumLength = 8, ErrorMessage = "Passwords must be at least 8 characters.")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Confirm yout password")]
        [Compare("Password", ErrorMessage = "Passwords must match.")]
        [DisplayName("Confirm Password")]
        public string ConfirmPassword { get; set; }
        public string ReturnUrl { get; set; }
	}
}
