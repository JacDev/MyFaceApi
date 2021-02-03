using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MyFaceApi.IdentityServer.Application.ViewModels
{
	public class LoginViewModel
	{
        [Required(ErrorMessage = "Login jest wymagany!")]
        public string Login { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Hasło jest wymagane!")]
        [DisplayName("Hasło")]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
        public bool AlreadySeenNotificaton { get; set; }
    }
}
