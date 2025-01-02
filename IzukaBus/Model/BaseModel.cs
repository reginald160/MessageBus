using System.ComponentModel.DataAnnotations;
using System;

namespace IzukaBus.Model
{
	public class BaseModel
	{
		[Key]
		public Guid Id { get; set; }
		public string ClientId { get; set; } = "";

		public string Username { get; set; } = "";
		public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    }
}
