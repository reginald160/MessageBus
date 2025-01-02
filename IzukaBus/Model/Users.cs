using System;

namespace IzukaBus.Model
{
	public class Users
	{
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool	IsActive { get; set; }
	}
}
