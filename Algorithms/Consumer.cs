using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static IzukaBus.HubClient;

namespace Algorithms
{
	public class Consumer
	{
		public event Func<Notification, EventArgs, Task> NotificationReceived;
	}
}
