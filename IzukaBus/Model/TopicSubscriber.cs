using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace IzukaBus.Model
{
	public class TopicSubscriber : BaseModel
	{
		
		public string UserId { get; set; }
		public Guid TopicId { get; set; }
		[ForeignKey(nameof(TopicId))]
		public virtual Topic Topic { get; set; }
		
		public DateTime Date { get; set; }
	}
}
