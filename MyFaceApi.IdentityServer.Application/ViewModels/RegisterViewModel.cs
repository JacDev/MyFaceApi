using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MyFaceApi.IdentityServer.Application.ViewModels
{
	public class RegisterViewModel
	{
		[Required(ErrorMessage = "Login jest wymagany!")]
		[MaxLength(20, ErrorMessage = "Maks. 20 znaków!")]
		public string Login { get; set; }

		[Required(ErrorMessage = "Imie jest wymagane!")]
		[StringLength(20, ErrorMessage = "Maks. 20 znaków!")]
		[DisplayName("Imię")]
		public string FirstName { get; set; }

		[Required(ErrorMessage = "Nazwisko jest wymagane!")]
		[StringLength(20, ErrorMessage = "Maks. 20 znaków!")]
		[DisplayName("Nazwisko")]
		public string LastName { get; set; }

		//[Required(ErrorMessage = "Email jest wymagany!")]
		[EmailAddress]
		public string Email { get; set; }

		[DataType(DataType.Password)]
		[Required(ErrorMessage = "Hasło jest wymagane!")]
		[DisplayName("Hasło")]
		[StringLength(20, ErrorMessage = "Maks. 20 znaków!")]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Required(ErrorMessage = "Potwierdź hasło!")]
		[Compare("Password", ErrorMessage = "Hasła muszą być take same!")]
		[DisplayName("Potwierdź hasło")]
		public string ConfirmPassword { get; set; }
		
		[DisplayName("Miejscowość")]
		public string City { get; set; }
		
		[DisplayName("Data urodzenia")]
		public string DateOfBirth { get; set; }
		public string ReturnUrl { get; set; }
		public bool AlreadySeenNotification { get; set; }
		public string ErrorMessage { get; set; }
	}
}
