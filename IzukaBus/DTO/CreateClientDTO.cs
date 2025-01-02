using System.ComponentModel.DataAnnotations;

namespace IzukaBus.DTO
{
	public class CreateClientDTO
	{
		[Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
