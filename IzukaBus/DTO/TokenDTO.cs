using System.ComponentModel.DataAnnotations;

namespace IzukaBus.DTO
{
    public class TokenDTO
    {
        [Required]
        public string SecretKey { get; set; }
        [Required]
        public string ClientId { get; set; }
    }
}
