using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IzukaBus.Model
{
	public class Topic : BaseModel
	{
		public string Name { get; set; }
		public ICollection<TopicSubscriber> ? Subscribers { get; set; }
		public bool IsActive { get; set; }
	}
}
