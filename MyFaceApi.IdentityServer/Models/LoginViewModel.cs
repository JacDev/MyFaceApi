using System.ComponentModel.DataAnnotations;

namespace MyFaceApi.IdentityServer.Models
{
	public class LoginViewModel
	{
        [Required(ErrorMessage = "Login is required.")]
        public string Login { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Enter your password.")]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
    }
}
