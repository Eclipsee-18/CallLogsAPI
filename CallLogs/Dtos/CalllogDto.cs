namespace Dtos
{
	public class CalllogDto
	{
		public bool Outgoing { get; set; }
		public bool Incoming { get; set; }
		public bool Missed { get; set; }
		public int  CallerId { get; set; }
	}
}
