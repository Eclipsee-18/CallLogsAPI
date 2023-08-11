namespace CallLogs.Model
{
	public class Calllog
	{
		public int CallLogId { get; set; }
		public bool Outgoing { get; set; }
		public bool Incoming { get; set; }
		public bool Missed { get; set; }
		public int CallerId { get; set; }

		
		public int UserId { get; set; }
		public User User { get; set; }
	}
}
